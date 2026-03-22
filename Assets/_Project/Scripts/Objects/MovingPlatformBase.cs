using UnityEngine;

public abstract class MovingPlatformBase : MonoBehaviour, SaveListener
{
    [Header("Target Setup")]
    public Transform targetPoint;

    [Header("Movement Settings")]
    public float speed = 2f;

    protected Vector3 startPos;
    protected Vector3 currentTarget;
    private Vector3 savedPosition;
    private Vector3 savedTarget;

    protected virtual void Awake()
    {
        startPos = transform.position;

        if (targetPoint == null)
        {
            Debug.LogError($"{name}: targetPoint is not assigned!");
            return;
        }


        currentTarget = targetPoint.position;
    }

    protected virtual void Update()
    {
        Move();
    }

    protected void Move()
{
    if (targetPoint == null) return;

    Vector3 newPos = Vector3.MoveTowards(
        transform.position,
        currentTarget,
        speed * Time.deltaTime
    );

    // Lock Z so it never changes
    newPos.z = startPos.z;

    transform.position = newPos;
}

    public void MoveToTarget()
    {
        if (targetPoint == null) return;
        currentTarget = targetPoint.position;
    }

    public void MoveToStart()
    {
        currentTarget = startPos;
    }
    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            HandlePassenger(collision.transform, true);
        }
    }

    protected virtual void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
        {
            HandlePassenger(collision.transform, false);
        }
    }

    protected void HandlePassenger(Transform passenger, bool onPlatform)
    {
        if (passenger == null) return;

        if (onPlatform)
            passenger.SetParent(transform);
        else
            passenger.SetParent(null);
    }
    public virtual void SaveState()
    {
        savedPosition = transform.position;
        savedTarget = currentTarget;
    }
    public virtual void LoadState()
    {
        transform.position = savedPosition;
        currentTarget = savedTarget;
    }
}