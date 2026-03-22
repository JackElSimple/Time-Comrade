using UnityEngine;

public class MovingPlatformAuto : MovingPlatformBase
{
    private bool movingForward = true;
    private bool savedMovingForward;

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

    public override void SaveState()
    {
        base.SaveState();
        savedMovingForward = movingForward;
    }

    public override void LoadState()
    {
        base.LoadState();
        movingForward = savedMovingForward;
    }
}