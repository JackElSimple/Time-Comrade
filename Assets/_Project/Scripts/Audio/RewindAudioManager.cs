using UnityEngine;
using UnityEngine.SceneManagement;

public sealed class RewindAudioManager : MonoBehaviour
{
    private const string RewindClipResourcePath = "Rewind/RewindSoundEffect";
    private const float SilenceThreshold = 0.001f;

    private static readonly int GlobalRewindIntensityId = Shader.PropertyToID("_RewindIntensity");

    private static RewindAudioManager instance;
    private static AudioClip rewindClip;

    [Header("Volume")]
    [SerializeField, Range(0f, 1f)] private float maxVolume = 0.75f;

    [Header("Pitch")]
    [SerializeField] private float minPitch = 0.92f;
    [SerializeField] private float maxPitch = 1.08f;

    [Header("Low Pass Filter")]
    [SerializeField] private bool useLowPassFilter = true;
    [SerializeField] private float highCutoffFrequency = 22000f;
    [SerializeField] private float lowCutoffFrequency = 1800f;

    private AudioSource rewindAudioSource;
    private AudioLowPassFilter rewindLowPassFilter;
    private bool rewindRequested;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Bootstrap()
    {
        if (instance != null)
        {
            return;
        }

        GameObject managerObject = new GameObject(nameof(RewindAudioManager));
        DontDestroyOnLoad(managerObject);
        managerObject.AddComponent<RewindAudioManager>();
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

        EnsureAudioSetup();
        ApplyIntensity(0f);
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
    }

    private void OnDestroy()
    {
        if (instance == this)
        {
            instance = null;
        }
    }

    private void Update()
    {
        if (rewindAudioSource == null)
        {
            return;
        }

        float intensity = Mathf.Clamp01(Shader.GetGlobalFloat(GlobalRewindIntensityId));
        ApplyIntensity(intensity);

        if (!rewindRequested && rewindAudioSource.isPlaying && intensity <= SilenceThreshold)
        {
            rewindAudioSource.Stop();
        }
    }

    private void HandleRewindStarted()
    {
        rewindRequested = true;
        EnsureAudioSetup();

        if (rewindClip == null || rewindAudioSource == null || rewindAudioSource.isPlaying)
        {
            return;
        }

        rewindAudioSource.time = 0f;
        rewindAudioSource.Play();
    }

    private void HandleRewindEnded()
    {
        rewindRequested = false;
    }

    private void HandleSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        rewindRequested = false;

        if (rewindAudioSource != null && rewindAudioSource.isPlaying)
        {
            rewindAudioSource.Stop();
        }

        ApplyIntensity(0f);
    }

    private void EnsureAudioSetup()
    {
        if (rewindClip == null)
        {
            rewindClip = Resources.Load<AudioClip>(RewindClipResourcePath);
        }

        if (rewindAudioSource == null)
        {
            rewindAudioSource = gameObject.GetComponent<AudioSource>();
            if (rewindAudioSource == null)
            {
                rewindAudioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        rewindAudioSource.clip = rewindClip;
        rewindAudioSource.playOnAwake = false;
        rewindAudioSource.loop = true;
        rewindAudioSource.spatialBlend = 0f;
        rewindAudioSource.volume = 0f;
        rewindAudioSource.pitch = minPitch;
        rewindAudioSource.priority = 64;
        rewindAudioSource.ignoreListenerPause = true;

        if (useLowPassFilter)
        {
            if (rewindLowPassFilter == null)
            {
                rewindLowPassFilter = gameObject.GetComponent<AudioLowPassFilter>();
                if (rewindLowPassFilter == null)
                {
                    rewindLowPassFilter = gameObject.AddComponent<AudioLowPassFilter>();
                }
            }

            rewindLowPassFilter.enabled = true;
            rewindLowPassFilter.cutoffFrequency = highCutoffFrequency;
            rewindLowPassFilter.lowpassResonanceQ = 1f;
        }
        else if (rewindLowPassFilter != null)
        {
            rewindLowPassFilter.enabled = false;
        }
    }

    private void ApplyIntensity(float intensity)
    {
        if (rewindAudioSource == null)
        {
            return;
        }

        rewindAudioSource.volume = intensity * maxVolume;
        rewindAudioSource.pitch = Mathf.Lerp(minPitch, maxPitch, intensity);

        if (rewindLowPassFilter != null && rewindLowPassFilter.enabled)
        {
            rewindLowPassFilter.cutoffFrequency = Mathf.Lerp(highCutoffFrequency, lowCutoffFrequency, intensity);
        }
    }
}
