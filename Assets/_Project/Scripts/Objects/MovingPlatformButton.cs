using UnityEngine;

public class MovingPlatformButton : MovingPlatformBase
{
    protected override bool RequiresTargetPoint => false;

    protected override void Awake()
    {
        base.Awake();
        currentTarget = transform.position;
    }
}
