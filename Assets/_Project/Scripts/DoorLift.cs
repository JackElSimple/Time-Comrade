using UnityEngine;

public class MovingPlatformButton : MovingPlatformBase
{
    protected override void Awake()
    {
        base.Awake();
        currentTarget = transform.position;;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
            HandlePassenger(collision.transform, true);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.CompareTag("Player"))
            HandlePassenger(collision.transform, false);
    }
}