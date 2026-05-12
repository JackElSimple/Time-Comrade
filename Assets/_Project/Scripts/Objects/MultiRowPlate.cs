using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MultiRowPate : MonoBehaviour, SaveListener
{
    private sealed class SharedRootState
    {
        public SharedRootState(Transform root)
        {
            Root = root;
            TargetPosition = root.position;
        }

        public Transform Root;
        public Transform[] TrackedTransforms;
        public Vector3[] SavedLocalPositions;

        public int Counter;
        public bool IsMoving;
        public bool IsRewinding;
        public Vector3 TargetPosition;

        public int SavedCounter;
        public Vector3 SavedPosition;
        public Vector3 SavedTarget;
        public bool SavedIsMoving;
        public bool HasSavedState;
    }

    private static readonly Dictionary<int, SharedRootState> SharedStatesByRoot = new Dictionary<int, SharedRootState>();

	[Header("Movement")]
	[SerializeField] private Transform platformRoot;
	[SerializeField] private float yStep = 2f;
	[SerializeField] private int yDirection = 1;
	[SerializeField] private float moveSpeed = 10f;

	[Header("Limits")]
	[SerializeField] private int maxSteps = 1;

	[Header("Events")]
	public UnityEvent onActivated;
	public UnityEvent onMaxReached;

	[Header("Interaction")]
	[SerializeField] private string playerTag = "Player";
	[SerializeField] private string cloneTag = "Clone";

    [Header("Debug")]
    [SerializeField] private bool enableDebugLogs = true;

    private readonly HashSet<int> overlappingColliderIds = new HashSet<int>();
    private SharedRootState sharedState;
    private bool isAuthority;
    private bool activationArmed = true;
    private Coroutine delayedRestoreRoutine;

	private void Awake()
	{
		if (platformRoot == null)
			platformRoot = transform;

        sharedState = GetOrCreateSharedState(platformRoot);
        isAuthority = sharedState.Root == platformRoot && sharedState.TrackedTransforms == null;
        if (isAuthority)
        {
            CacheTrackedTransforms();
        }

        LogState($"Awake. platformRoot='{platformRoot.name}', initialRootPos={platformRoot.position}, yStep={yStep}, yDirection={yDirection}, maxSteps={maxSteps}, authority={isAuthority}");
	}

    private void OnEnable()
    {
        SceneController.RewindStarted += HandleGlobalRewindStarted;
        SceneController.RewindEnded += HandleGlobalRewindEnded;

        if (isAuthority && !SceneController.saveListeners.Contains(this))
        {
            SceneController.saveListeners.Add(this);
        }
    }

    private void OnDisable()
    {
        SceneController.RewindStarted -= HandleGlobalRewindStarted;
        SceneController.RewindEnded -= HandleGlobalRewindEnded;

        if (delayedRestoreRoutine != null)
        {
            StopCoroutine(delayedRestoreRoutine);
            delayedRestoreRoutine = null;
        }

        if (isAuthority)
        {
            SceneController.saveListeners.Remove(this);
        }
    }

	private void Update()
	{
        if (overlappingColliderIds.Count == 0 && !sharedState.IsMoving && !sharedState.IsRewinding)
        {
            activationArmed = true;
        }

        if (!isAuthority || sharedState.IsRewinding || !sharedState.IsMoving)
        {
            return;
        }

		platformRoot.position = Vector3.MoveTowards(
			platformRoot.position,
			sharedState.TargetPosition,
			moveSpeed * Time.deltaTime
		);

		if (Vector3.Distance(platformRoot.position, sharedState.TargetPosition) < 0.01f)
		{
			platformRoot.position = sharedState.TargetPosition;
			sharedState.IsMoving = false;
            LogState($"Movement completed at {platformRoot.position}. sharedCounter={sharedState.Counter}");
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (sharedState.IsRewinding) return;
		if (!IsValid(collision)) return;
        if (!overlappingColliderIds.Add(collision.collider.GetInstanceID())) return;
        if (overlappingColliderIds.Count > 1) return;
        if (!activationArmed)
        {
            LogState($"OnCollisionEnter2D by '{collision.transform.name}' ignored because activation is not armed yet.");
            return;
        }

        activationArmed = false;
        LogState($"OnCollisionEnter2D by '{collision.transform.name}' (tag='{collision.transform.tag}'). sharedCounter={sharedState.Counter}, rootPos={platformRoot.position}, targetPos={sharedState.TargetPosition}");
		Activate();
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (!IsValid(collision)) return;
        if (!overlappingColliderIds.Remove(collision.collider.GetInstanceID())) return;
		if (sharedState.IsRewinding) return;

        LogState($"OnCollisionExit2D by '{collision.transform.name}' (tag='{collision.transform.tag}'). sharedCounter={sharedState.Counter}, rootPos={platformRoot.position}, targetPos={sharedState.TargetPosition}, remainingContacts={overlappingColliderIds.Count}");
	}

	private bool IsValid(Collision2D collision)
	{
		return collision.transform.CompareTag(playerTag) ||
			   collision.transform.CompareTag(cloneTag);
	}

	private void Activate()
	{
        LogState($"Activate requested. sharedCounter={sharedState.Counter}/{maxSteps}, rootPos={platformRoot.position}, targetBefore={sharedState.TargetPosition}");

		if (sharedState.Counter >= maxSteps)
		{
            LogState("Max steps reached. Activation ignored.");
			onMaxReached?.Invoke();
			return;
		}

        sharedState.Counter++;
        sharedState.TargetPosition = platformRoot.position + new Vector3(0f, yStep * yDirection, 0f);
		sharedState.IsMoving = true;

        LogState($"Activation accepted. sharedCounter={sharedState.Counter}/{maxSteps}, targetAfter={sharedState.TargetPosition}, deltaY={yStep * yDirection}");

		onActivated?.Invoke();
	}

	// =========================
	// REWIND INTEGRATION
	// =========================

	public void StartRewind()
	{
		sharedState.IsRewinding = true;
        ClearLocalInteractionState();
        LogState($"StartRewind. sharedCounter={sharedState.Counter}, rootPos={platformRoot.position}, targetPos={sharedState.TargetPosition}, isMoving={sharedState.IsMoving}");
	}

	public void StopRewind()
	{
		sharedState.IsRewinding = false;
        ClearLocalInteractionState();
        LogState("StopRewind requested. Scheduling delayed RestoreState().");

        if (delayedRestoreRoutine != null)
        {
            StopCoroutine(delayedRestoreRoutine);
        }

        delayedRestoreRoutine = StartCoroutine(RestoreStateAfterListeners());
	}

	public void SaveState()
	{
		sharedState.SavedCounter = sharedState.Counter;
		sharedState.SavedPosition = platformRoot.position;
		sharedState.SavedTarget = sharedState.TargetPosition;
		sharedState.SavedIsMoving = sharedState.IsMoving;
        sharedState.HasSavedState = true;
        SaveTrackedLocalPositions();
        LogState($"SaveState. savedCounter={sharedState.SavedCounter}, savedRootPos={sharedState.SavedPosition}, savedTarget={sharedState.SavedTarget}, savedIsMoving={sharedState.SavedIsMoving}");
	}

	public void LoadState()
	{
        LogState("LoadState called. Entering rewind mode.");
		StartRewind();
	}

	public void CancelState()
	{
		sharedState.IsRewinding = false;
        ClearLocalInteractionState();
        LogState("CancelState called. Rewind flag cleared.");
	}

	public void OnRewindFinished()
	{
        LogState("OnRewindFinished called.");
		StopRewind();
	}

	public void RestoreState()
	{
        if (!sharedState.HasSavedState)
        {
            LogState("RestoreState skipped because there is no saved snapshot.");
            return;
        }

        Vector3 previousRootPosition = platformRoot.position;
        Vector3 previousTargetPosition = sharedState.TargetPosition;

		sharedState.Counter = sharedState.SavedCounter;

		platformRoot.position = sharedState.SavedPosition;
		sharedState.TargetPosition = sharedState.SavedTarget;

		sharedState.IsMoving = sharedState.SavedIsMoving;
        ClearLocalInteractionState();

        RestoreTrackedLocalPositions();
        LogState($"RestoreState applied. rootPos {previousRootPosition} -> {platformRoot.position}, targetPos {previousTargetPosition} -> {sharedState.TargetPosition}, sharedCounter={sharedState.Counter}, isMoving={sharedState.IsMoving}");
	}

    private SharedRootState GetOrCreateSharedState(Transform root)
    {
        int rootId = root.GetInstanceID();
        if (!SharedStatesByRoot.TryGetValue(rootId, out SharedRootState state))
        {
            state = new SharedRootState(root);
            SharedStatesByRoot[rootId] = state;
        }

        return state;
    }

    private void CacheTrackedTransforms()
    {
        sharedState.TrackedTransforms = platformRoot.GetComponentsInChildren<Transform>(true);
        sharedState.SavedLocalPositions = new Vector3[sharedState.TrackedTransforms.Length];
    }

    private void SaveTrackedLocalPositions()
    {
        if (sharedState.TrackedTransforms == null || sharedState.TrackedTransforms.Length == 0)
        {
            CacheTrackedTransforms();
        }

        for (int i = 0; i < sharedState.TrackedTransforms.Length; i++)
        {
            sharedState.SavedLocalPositions[i] = sharedState.TrackedTransforms[i].localPosition;
        }
    }

    private void RestoreTrackedLocalPositions()
    {
        if (sharedState.TrackedTransforms == null || sharedState.SavedLocalPositions == null)
        {
            return;
        }

        int count = Mathf.Min(sharedState.TrackedTransforms.Length, sharedState.SavedLocalPositions.Length);
        for (int i = 0; i < count; i++)
        {
            sharedState.TrackedTransforms[i].localPosition = sharedState.SavedLocalPositions[i];
        }
    }

    private IEnumerator RestoreStateAfterListeners()
    {
        yield return null;
        LogState("Delayed RestoreState() executing after one frame.");
        RestoreState();
        delayedRestoreRoutine = null;
    }

    private void HandleGlobalRewindStarted()
    {
        ClearLocalInteractionState();
    }

    private void HandleGlobalRewindEnded()
    {
        ClearLocalInteractionState();
    }

    private void ClearLocalInteractionState()
    {
        overlappingColliderIds.Clear();
        activationArmed = true;
    }

    private void LogState(string message)
    {
        if (!enableDebugLogs)
        {
            return;
        }

        string rootName = platformRoot != null ? platformRoot.name : "<null>";
        Debug.Log($"[MultiRowPlate:{name}] [root:{rootName}] {message}", this);
    }
}
