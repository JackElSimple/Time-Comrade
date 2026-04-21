using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public sealed class RewindVisualManager : MonoBehaviour
{
    private const string OverlayShaderResourcePath = "Rewind/RewindScreenGlitch";
    private const string RuntimeFeatureName = "Runtime Rewind Screen Pass";
    private const float ResolveInterval = 0.25f;
    private const float DefaultPixelsPerUnit = 16f;

    private static readonly int GlobalRewindIntensityId = Shader.PropertyToID("_RewindIntensity");
    private static readonly int IntensityId = Shader.PropertyToID("_Intensity");
    private static readonly int DesaturateId = Shader.PropertyToID("_Desaturate");
    private static readonly int TintColorId = Shader.PropertyToID("_TintColor");
    private static readonly int DistortionId = Shader.PropertyToID("_Distortion");
    private static readonly int RgbSplitId = Shader.PropertyToID("_RgbSplit");
    private static readonly int JitterId = Shader.PropertyToID("_Jitter");

    private static RewindVisualManager instance;

    [Header("Transitions")]
    [SerializeField] private float fadeInDuration = 0.22f;
    [SerializeField] private float fadeOutDuration = 0.3f;

    [Header("Ghost Trail")]
    [SerializeField] private float historyDuration = 1.6f;
    [SerializeField] private float historySampleInterval = 0.05f;
    [SerializeField] private float ghostSpawnInterval = 0.035f;
    [SerializeField] private float ghostLifetime = 0.45f;
    [SerializeField] private int ghostPoolSize = 32;
    [SerializeField] private float ghostMaxAlpha = 0.58f;
    [SerializeField] private Color ghostTint = new Color(0.42f, 0.84f, 1f, 1f);

    [Header("Screen Glitch")]
    [SerializeField] private Color screenTint = new Color(0.92f, 0.96f, 1.08f, 1f);
    [SerializeField, Range(0f, 1f)] private float desaturation = 0.28f;
    [SerializeField] private float distortionPixels = 1.4f;
    [SerializeField] private float rgbSplitPixels = 1f;
    [SerializeField] private float jitterPixels = 1.75f;

    [Header("Camera Feedback")]
    [SerializeField] private float shakePixels = 1f;
    [SerializeField] private float shakeSpeed = 19f;
    [SerializeField] private float zoomAmount = 0.08f;

    private SceneController currentSceneController;
    private OpitControllerRewind currentPlayer;
    private SpriteRenderer currentPlayerSprite;
    private Camera currentCamera;
    private Transform currentCameraTransform;

    private Material screenEffectMaterial;
    private Shader screenEffectShader;
    private ScriptableRendererData rendererData;
    private FullScreenPassRendererFeature fullScreenFeature;

    private GhostSnapshot[] historyBuffer;
    private int historyHead = -1;
    private int historyCount;
    private float historySampleTimer;
    private int rewindHistoryCursor;
    private float ghostSpawnTimer;
    private GhostInstance[] ghostPool;
    private int nextGhostIndex;

    private bool rewindingRequested;
    private float currentIntensity;
    private float nextResolveTime;
    private float noiseSeedX;
    private float noiseSeedY;
    private Vector3 baseCameraLocalPosition;
    private float baseCameraOrthoSize;
    private bool cameraStateCaptured;

    private struct GhostSnapshot
    {
        public Vector3 position;
        public Vector3 scale;
        public Quaternion rotation;
        public Sprite sprite;
        public Color color;
        public bool flipX;
        public bool flipY;
        public int sortingLayerId;
        public int sortingOrder;
        public Material sharedMaterial;
    }

    private sealed class GhostInstance
    {
        public GameObject GameObject;
        public Transform Transform;
        public SpriteRenderer Renderer;
        public Color BaseColor;
        public float Age;
        public bool Active;
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            return;
        }

        GameObject managerObject = new GameObject(nameof(RewindVisualManager));
        DontDestroyOnLoad(managerObject);
        managerObject.AddComponent<RewindVisualManager>();
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        noiseSeedX = Random.value * 100f;
        noiseSeedY = Random.value * 100f;

        BuildHistoryBuffer();
        InitializeGhostPool();
        ResetScreenEffectState();
    }

    private void OnEnable()
    {
        SceneController.RewindStarted += HandleRewindStarted;
        SceneController.RewindEnded += HandleRewindEnded;
        SceneManager.sceneLoaded += HandleSceneLoaded;
    }

    private void OnDisable()
    {
        SceneController.RewindStarted -= HandleRewindStarted;
        SceneController.RewindEnded -= HandleRewindEnded;
        SceneManager.sceneLoaded -= HandleSceneLoaded;
        ResetScreenEffectState();
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }

        RemoveRuntimeFullScreenPass();

        if (screenEffectMaterial != null)
        {
            Destroy(screenEffectMaterial);
        }
    }

    private void LateUpdate()
    {
        float deltaTime = Time.unscaledDeltaTime;

        TryEnsureFullScreenPass();
        TryResolveTargets();
        SyncWithControllerState();
        UpdateIntensity(deltaTime);
        UpdateHistory(deltaTime);
        UpdateGhostSpawning(deltaTime);
        UpdateGhostPool(deltaTime);
        UpdateScreenEffectMaterial();
        UpdateCameraFeedback();
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ClearSceneBindings();
        TryResolveTargets(force: true);
    }

    private void HandleRewindStarted()
    {
        if (rewindingRequested)
        {
            return;
        }

        rewindingRequested = true;
        rewindHistoryCursor = 0;
        ghostSpawnTimer = ghostSpawnInterval;
        TryResolveTargets(force: true);
    }

    private void HandleRewindEnded()
    {
        if (!rewindingRequested && currentIntensity <= 0f)
        {
            return;
        }

        rewindingRequested = false;
        ResetHistory();
    }

    private void TryEnsureFullScreenPass()
    {
        if (!EnsureScreenEffectMaterial())
        {
            return;
        }

        UniversalRenderPipelineAsset pipelineAsset =
            GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset ??
            QualitySettings.renderPipeline as UniversalRenderPipelineAsset;

        if (pipelineAsset == null || pipelineAsset.rendererDataList.Length == 0)
        {
            RemoveRuntimeFullScreenPass();
            return;
        }

        ScriptableRendererData targetRendererData = pipelineAsset.rendererDataList[0];
        if (targetRendererData == null)
        {
            RemoveRuntimeFullScreenPass();
            return;
        }

        if (rendererData != targetRendererData)
        {
            RemoveRuntimeFullScreenPass();
            rendererData = targetRendererData;
        }

        bool needsRendererRefresh = false;

        if (fullScreenFeature == null)
        {
            fullScreenFeature = ScriptableObject.CreateInstance<FullScreenPassRendererFeature>();
            fullScreenFeature.name = RuntimeFeatureName;
            fullScreenFeature.hideFlags = HideFlags.HideAndDontSave;
            rendererData.rendererFeatures.Add(fullScreenFeature);
            needsRendererRefresh = true;
        }
        else if (!rendererData.rendererFeatures.Contains(fullScreenFeature))
        {
            rendererData.rendererFeatures.Add(fullScreenFeature);
            needsRendererRefresh = true;
        }

        if (fullScreenFeature.injectionPoint != FullScreenPassRendererFeature.InjectionPoint.BeforeRenderingPostProcessing)
        {
            fullScreenFeature.injectionPoint = FullScreenPassRendererFeature.InjectionPoint.BeforeRenderingPostProcessing;
            needsRendererRefresh = true;
        }

        if (!fullScreenFeature.fetchColorBuffer)
        {
            fullScreenFeature.fetchColorBuffer = true;
            needsRendererRefresh = true;
        }

        if (fullScreenFeature.requirements != ScriptableRenderPassInput.None)
        {
            fullScreenFeature.requirements = ScriptableRenderPassInput.None;
            needsRendererRefresh = true;
        }

        if (fullScreenFeature.passMaterial != screenEffectMaterial)
        {
            fullScreenFeature.passMaterial = screenEffectMaterial;
            needsRendererRefresh = true;
        }

        if (fullScreenFeature.passIndex != 0)
        {
            fullScreenFeature.passIndex = 0;
            needsRendererRefresh = true;
        }

        if (fullScreenFeature.bindDepthStencilAttachment)
        {
            fullScreenFeature.bindDepthStencilAttachment = false;
            needsRendererRefresh = true;
        }

        if (!fullScreenFeature.isActive)
        {
            fullScreenFeature.SetActive(true);
            needsRendererRefresh = true;
        }

        if (needsRendererRefresh)
        {
            fullScreenFeature.Create();
            rendererData.SetDirty();
        }
    }

    private void RemoveRuntimeFullScreenPass()
    {
        if (rendererData != null && fullScreenFeature != null)
        {
            rendererData.rendererFeatures.Remove(fullScreenFeature);
            rendererData.SetDirty();
        }

        if (fullScreenFeature != null)
        {
            Destroy(fullScreenFeature);
            fullScreenFeature = null;
        }

        rendererData = null;
    }

    private bool EnsureScreenEffectMaterial()
    {
        if (screenEffectMaterial != null)
        {
            return true;
        }

        if (screenEffectShader == null)
        {
            screenEffectShader = Resources.Load<Shader>(OverlayShaderResourcePath);
            if (screenEffectShader == null)
            {
                screenEffectShader = Shader.Find("Custom/RewindScreenGlitch");
            }
        }

        if (screenEffectShader == null)
        {
            return false;
        }

        screenEffectMaterial = new Material(screenEffectShader)
        {
            name = "RewindScreenGlitch (Runtime)",
            hideFlags = HideFlags.HideAndDontSave
        };

        ResetScreenEffectState();
        return true;
    }

    private void TryResolveTargets(bool force = false)
    {
        if (!force && Time.unscaledTime < nextResolveTime)
        {
            return;
        }

        nextResolveTime = Time.unscaledTime + ResolveInterval;

        if (currentSceneController == null || !currentSceneController.isActiveAndEnabled)
        {
            currentSceneController = Object.FindAnyObjectByType<SceneController>();
        }

        if (currentPlayer == null || !currentPlayer.isActiveAndEnabled)
        {
            currentPlayer = Object.FindAnyObjectByType<OpitControllerRewind>();
            currentPlayerSprite = ResolvePlayerSprite(currentPlayer);
        }
        else if (currentPlayerSprite == null)
        {
            currentPlayerSprite = ResolvePlayerSprite(currentPlayer);
        }

        Camera mainCamera = Camera.main;
        if (mainCamera != currentCamera)
        {
            currentCamera = mainCamera;
            currentCameraTransform = currentCamera != null ? currentCamera.transform : null;
            cameraStateCaptured = false;
        }

        if (currentCamera != null && !cameraStateCaptured)
        {
            baseCameraLocalPosition = currentCameraTransform.localPosition;
            baseCameraOrthoSize = currentCamera.orthographicSize;
            cameraStateCaptured = true;
        }
    }

    private void SyncWithControllerState()
    {
        bool controllerRewinding = currentSceneController != null && currentSceneController.isRewinding;

        if (controllerRewinding && !rewindingRequested)
        {
            HandleRewindStarted();
        }
        else if (!controllerRewinding && rewindingRequested)
        {
            HandleRewindEnded();
        }
    }

    private void UpdateIntensity(float deltaTime)
    {
        float target = rewindingRequested ? 1f : 0f;
        float duration = rewindingRequested ? fadeInDuration : fadeOutDuration;
        float step = duration > 0f ? deltaTime / duration : 1f;

        currentIntensity = Mathf.MoveTowards(currentIntensity, target, step);
        Shader.SetGlobalFloat(GlobalRewindIntensityId, currentIntensity);
    }

    private void UpdateHistory(float deltaTime)
    {
        if (rewindingRequested || currentPlayerSprite == null || !currentPlayerSprite.enabled)
        {
            return;
        }

        historySampleTimer += deltaTime;
        while (historySampleTimer >= historySampleInterval)
        {
            historySampleTimer -= historySampleInterval;
            PushHistorySnapshot(CreateSnapshot(currentPlayerSprite));
        }
    }

    private void UpdateGhostSpawning(float deltaTime)
    {
        if (!rewindingRequested || historyCount == 0)
        {
            return;
        }

        ghostSpawnTimer += deltaTime;
        while (ghostSpawnTimer >= ghostSpawnInterval && rewindHistoryCursor < historyCount)
        {
            ghostSpawnTimer -= ghostSpawnInterval;
            SpawnGhost(GetHistoryFromNewest(rewindHistoryCursor));
            rewindHistoryCursor++;
        }
    }

    private void UpdateGhostPool(float deltaTime)
    {
        if (ghostPool == null)
        {
            return;
        }

        for (int i = 0; i < ghostPool.Length; i++)
        {
            GhostInstance ghost = ghostPool[i];
            if (!ghost.Active)
            {
                continue;
            }

            ghost.Age += deltaTime;
            float lifeT = ghostLifetime > 0f ? ghost.Age / ghostLifetime : 1f;
            if (lifeT >= 1f)
            {
                ghost.Active = false;
                ghost.GameObject.SetActive(false);
                continue;
            }

            float alpha = ghost.BaseColor.a * (1f - Mathf.SmoothStep(0f, 1f, lifeT));
            ghost.Renderer.color = new Color(ghost.BaseColor.r, ghost.BaseColor.g, ghost.BaseColor.b, alpha);
        }
    }

    private void UpdateScreenEffectMaterial()
    {
        if (screenEffectMaterial == null)
        {
            return;
        }

        screenEffectMaterial.SetFloat(IntensityId, currentIntensity);
        screenEffectMaterial.SetFloat(DesaturateId, desaturation);
        screenEffectMaterial.SetColor(TintColorId, screenTint);
        screenEffectMaterial.SetFloat(DistortionId, distortionPixels);
        screenEffectMaterial.SetFloat(RgbSplitId, rgbSplitPixels);
        screenEffectMaterial.SetFloat(JitterId, jitterPixels);
    }

    private void ResetScreenEffectState()
    {
        Shader.SetGlobalFloat(GlobalRewindIntensityId, 0f);

        if (screenEffectMaterial == null)
        {
            return;
        }

        screenEffectMaterial.SetFloat(IntensityId, 0f);
        screenEffectMaterial.SetFloat(DesaturateId, desaturation);
        screenEffectMaterial.SetColor(TintColorId, screenTint);
        screenEffectMaterial.SetFloat(DistortionId, distortionPixels);
        screenEffectMaterial.SetFloat(RgbSplitId, rgbSplitPixels);
        screenEffectMaterial.SetFloat(JitterId, jitterPixels);
    }

    private void UpdateCameraFeedback()
    {
        if (currentCamera == null || currentCameraTransform == null || !cameraStateCaptured)
        {
            return;
        }

        if (currentIntensity <= 0.001f)
        {
            currentCameraTransform.localPosition = baseCameraLocalPosition;
            currentCamera.orthographicSize = baseCameraOrthoSize;
            return;
        }

        float pixelSize = GetPixelWorldSize();
        float time = Time.unscaledTime * shakeSpeed;
        float offsetX = (Mathf.PerlinNoise(noiseSeedX, time) - 0.5f) * 2f;
        float offsetY = (Mathf.PerlinNoise(noiseSeedY, time + 11.7f) - 0.5f) * 2f;
        Vector3 jitterOffset = new Vector3(offsetX, offsetY, 0f) * shakePixels * pixelSize * currentIntensity;
        jitterOffset = SnapToPixel(jitterOffset, pixelSize);

        currentCameraTransform.localPosition = baseCameraLocalPosition + jitterOffset;
        currentCamera.orthographicSize = baseCameraOrthoSize - (zoomAmount * currentIntensity);
    }

    private void InitializeGhostPool()
    {
        ghostPool = new GhostInstance[Mathf.Max(ghostPoolSize, 1)];

        for (int i = 0; i < ghostPool.Length; i++)
        {
            GameObject ghostObject = new GameObject($"RewindGhost_{i}");
            ghostObject.transform.SetParent(transform, false);
            SpriteRenderer renderer = ghostObject.AddComponent<SpriteRenderer>();
            renderer.enabled = true;
            ghostObject.SetActive(false);

            ghostPool[i] = new GhostInstance
            {
                GameObject = ghostObject,
                Transform = ghostObject.transform,
                Renderer = renderer,
                BaseColor = ghostTint,
                Age = 0f,
                Active = false
            };
        }
    }

    private void SpawnGhost(GhostSnapshot snapshot)
    {
        if (ghostPool == null || ghostPool.Length == 0 || snapshot.sprite == null)
        {
            return;
        }

        GhostInstance ghost = ghostPool[nextGhostIndex];
        nextGhostIndex = (nextGhostIndex + 1) % ghostPool.Length;

        ghost.Active = true;
        ghost.Age = 0f;
        ghost.Transform.SetPositionAndRotation(SnapToPixel(snapshot.position, GetPixelWorldSize()), snapshot.rotation);
        ghost.Transform.localScale = snapshot.scale;
        ghost.Renderer.sprite = snapshot.sprite;
        ghost.Renderer.flipX = snapshot.flipX;
        ghost.Renderer.flipY = snapshot.flipY;
        ghost.Renderer.sortingLayerID = snapshot.sortingLayerId;
        ghost.Renderer.sortingOrder = snapshot.sortingOrder - 1;
        ghost.Renderer.sharedMaterial = snapshot.sharedMaterial;

        ghost.BaseColor = new Color(
            snapshot.color.r * ghostTint.r,
            snapshot.color.g * ghostTint.g,
            snapshot.color.b * ghostTint.b,
            snapshot.color.a * ghostMaxAlpha);

        ghost.Renderer.color = ghost.BaseColor;
        ghost.GameObject.SetActive(true);
    }

    private GhostSnapshot CreateSnapshot(SpriteRenderer renderer)
    {
        Transform spriteTransform = renderer.transform;
        return new GhostSnapshot
        {
            position = spriteTransform.position,
            scale = spriteTransform.lossyScale,
            rotation = spriteTransform.rotation,
            sprite = renderer.sprite,
            color = renderer.color,
            flipX = renderer.flipX,
            flipY = renderer.flipY,
            sortingLayerId = renderer.sortingLayerID,
            sortingOrder = renderer.sortingOrder,
            sharedMaterial = renderer.sharedMaterial
        };
    }

    private void PushHistorySnapshot(GhostSnapshot snapshot)
    {
        if (historyBuffer == null || historyBuffer.Length == 0)
        {
            return;
        }

        historyHead = (historyHead + 1) % historyBuffer.Length;
        historyBuffer[historyHead] = snapshot;
        historyCount = Mathf.Min(historyCount + 1, historyBuffer.Length);
    }

    private GhostSnapshot GetHistoryFromNewest(int offset)
    {
        int index = historyHead - offset;
        while (index < 0)
        {
            index += historyBuffer.Length;
        }

        return historyBuffer[index % historyBuffer.Length];
    }

    private void BuildHistoryBuffer()
    {
        int historyCapacity = Mathf.Max(4, Mathf.CeilToInt(historyDuration / historySampleInterval) + 2);
        historyBuffer = new GhostSnapshot[historyCapacity];
        ResetHistory();
    }

    private void ResetHistory()
    {
        historyHead = -1;
        historyCount = 0;
        rewindHistoryCursor = 0;
        historySampleTimer = 0f;
    }

    private void HideAllGhosts()
    {
        if (ghostPool == null)
        {
            return;
        }

        for (int i = 0; i < ghostPool.Length; i++)
        {
            ghostPool[i].Active = false;
            ghostPool[i].GameObject.SetActive(false);
        }
    }

    private void ClearSceneBindings()
    {
        rewindingRequested = false;
        currentIntensity = 0f;
        currentSceneController = null;
        currentPlayer = null;
        currentPlayerSprite = null;
        currentCamera = null;
        currentCameraTransform = null;
        cameraStateCaptured = false;
        nextResolveTime = 0f;

        ResetHistory();
        HideAllGhosts();
        ResetScreenEffectState();
    }

    private SpriteRenderer ResolvePlayerSprite(OpitControllerRewind player)
    {
        if (player == null)
        {
            return null;
        }

        SpriteRenderer[] renderers = player.GetComponentsInChildren<SpriteRenderer>(true);
        SpriteRenderer bestMatch = null;

        for (int i = 0; i < renderers.Length; i++)
        {
            SpriteRenderer renderer = renderers[i];
            if (renderer == null || renderer.sprite == null)
            {
                continue;
            }

            if (renderer.name.ToLowerInvariant().Contains("overlay"))
            {
                continue;
            }

            Camera parentCamera = renderer.GetComponentInParent<Camera>();
            if (parentCamera != null)
            {
                continue;
            }

            if (bestMatch == null || renderer.sortingOrder >= bestMatch.sortingOrder)
            {
                bestMatch = renderer;
            }
        }

        return bestMatch;
    }

    private float GetPixelWorldSize()
    {
        if (currentPlayerSprite != null && currentPlayerSprite.sprite != null && currentPlayerSprite.sprite.pixelsPerUnit > 0f)
        {
            return 1f / currentPlayerSprite.sprite.pixelsPerUnit;
        }

        return 1f / DefaultPixelsPerUnit;
    }

    private static Vector3 SnapToPixel(Vector3 position, float pixelWorldSize)
    {
        if (pixelWorldSize <= 0f)
        {
            return position;
        }

        position.x = Mathf.Round(position.x / pixelWorldSize) * pixelWorldSize;
        position.y = Mathf.Round(position.y / pixelWorldSize) * pixelWorldSize;
        return position;
    }
}
