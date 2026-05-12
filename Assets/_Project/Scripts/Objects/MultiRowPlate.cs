using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MultiRowPate : MonoBehaviour, SaveListener
{
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

	private int counter = 0;
	private bool pressed = false;

	private bool isMoving = false;
	private bool isRewinding = false;

	private Vector3 targetPosition;
    private Transform[] trackedTransforms;
    private Vector3[] savedLocalPositions;
    private Coroutine delayedRestoreRoutine;

	// --- REWIND STATE ---
	private int savedCounter;
	private Vector3 savedPosition;
	private Vector3 savedTarget;
	private bool savedIsMoving;
	private bool savedPressed;

	private void Awake()
	{
		if (platformRoot == null)
			platformRoot = transform;

		targetPosition = platformRoot.position;
        CacheTrackedTransforms();
	}

    private void OnEnable()
    {
        if (!SceneController.saveListeners.Contains(this))
        {
            SceneController.saveListeners.Add(this);
        }
    }

    private void OnDisable()
    {
        if (delayedRestoreRoutine != null)
        {
            StopCoroutine(delayedRestoreRoutine);
            delayedRestoreRoutine = null;
        }

        SceneController.saveListeners.Remove(this);
    }

	private void Update()
	{
		if (isRewinding) return;

		if (!isMoving) return;

		platformRoot.position = Vector3.MoveTowards(
			platformRoot.position,
			targetPosition,
			moveSpeed * Time.deltaTime
		);

		if (Vector3.Distance(platformRoot.position, targetPosition) < 0.01f)
		{
			isMoving = false;
		}
	}

	private void OnCollisionEnter2D(Collision2D collision)
	{
		if (isRewinding) return;
		if (!IsValid(collision)) return;
		if (pressed || isMoving) return;

		pressed = true;
		Activate();
	}

	private void OnCollisionExit2D(Collision2D collision)
	{
		if (isRewinding) return;
		if (!IsValid(collision)) return;

		pressed = false;
	}

	private bool IsValid(Collision2D collision)
	{
		return collision.transform.CompareTag(playerTag) ||
			   collision.transform.CompareTag(cloneTag);
	}

	private void Activate()
	{
		if (counter >= maxSteps)
		{
			onMaxReached?.Invoke();
			return;
		}

		counter++;

		targetPosition += new Vector3(0f, yStep * yDirection, 0f);
		isMoving = true;

		onActivated?.Invoke();
	}

	// =========================
	// REWIND INTEGRATION
	// =========================

	public void StartRewind()
	{
		isRewinding = true;
	}

	public void StopRewind()
	{
		isRewinding = false;

        if (delayedRestoreRoutine != null)
        {
            StopCoroutine(delayedRestoreRoutine);
        }

        delayedRestoreRoutine = StartCoroutine(RestoreStateAfterListeners());
	}

	public void SaveState()
	{
		savedCounter = counter;
		savedPosition = platformRoot.position;
		savedTarget = targetPosition;
		savedIsMoving = isMoving;
		savedPressed = pressed;
        SaveTrackedLocalPositions();
	}

	public void LoadState()
	{
		StartRewind();
	}

	public void CancelState()
	{
		isRewinding = false;
	}

	public void OnRewindFinished()
	{
		StopRewind();
	}

	public void RestoreState()
	{
		counter = savedCounter;

		platformRoot.position = savedPosition;
		targetPosition = savedTarget;

		isMoving = savedIsMoving;
		pressed = savedPressed;

        RestoreTrackedLocalPositions();
	}

    private void CacheTrackedTransforms()
    {
        trackedTransforms = platformRoot.GetComponentsInChildren<Transform>(true);
        savedLocalPositions = new Vector3[trackedTransforms.Length];
    }

    private void SaveTrackedLocalPositions()
    {
        if (trackedTransforms == null || trackedTransforms.Length == 0)
        {
            CacheTrackedTransforms();
        }

        for (int i = 0; i < trackedTransforms.Length; i++)
        {
            savedLocalPositions[i] = trackedTransforms[i].localPosition;
        }
    }

    private void RestoreTrackedLocalPositions()
    {
        if (trackedTransforms == null || savedLocalPositions == null)
        {
            return;
        }

        int count = Mathf.Min(trackedTransforms.Length, savedLocalPositions.Length);
        for (int i = 0; i < count; i++)
        {
            trackedTransforms[i].localPosition = savedLocalPositions[i];
        }
    }

    private IEnumerator RestoreStateAfterListeners()
    {
        yield return null;
        RestoreState();
        delayedRestoreRoutine = null;
    }
}
