using UnityEngine;

public class MovingPlatformAuto : MovingPlatformBase
{
    private bool movingForward = true;

    protected override void Update()
    {
        base.Update(); 

        if (targetPoint == null) return;

        if (Vector3.Distance(transform.position, currentTarget) < 0.01f)
        {
            if (movingForward)
                MoveToStart();
            else
                MoveToTarget();
            movingForward = !movingForward;
        }
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