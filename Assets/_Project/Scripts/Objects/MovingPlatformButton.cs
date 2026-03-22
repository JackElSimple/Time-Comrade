using UnityEngine;

public class MovingPlatformButton : MovingPlatformBase
{
    protected override void Awake()
    {
        base.Awake();
        currentTarget = transform.position;
    }
}