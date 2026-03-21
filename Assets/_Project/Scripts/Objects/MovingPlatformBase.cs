using UnityEngine;

public abstract class MovingPlatformBase : MonoBehaviour
{
    [Header("Target Setup")]
    public Transform targetPoint;

    [Header("Movement Settings")]
    public float speed = 2f;

    protected Vector3 startPos;
    protected Vector3 currentTarget;

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

    protected void HandlePassenger(Transform passenger, bool onPlatform)
    {
        if (passenger == null) return;

        if (onPlatform)
            passenger.SetParent(transform);
        else
            passenger.SetParent(null);
    }
}