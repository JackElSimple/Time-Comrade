using UnityEngine;

public class MovingPlatformAuto : MovingPlatformBase
{
	[SerializeField]
	private bool movingForward = true;
	[SerializeField]

	private bool savedMovingForward;

	protected override void OnUpdate()
	{
		base.OnUpdate();

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
	}

	public override void OnRewindFinished()
	{
		base.OnRewindFinished();
		movingForward = savedMovingForward;
		if (movingForward)
		{
			currentTarget = targetPoint.position;
		}
		else
		{
			currentTarget = startPos;
		}
	}
}