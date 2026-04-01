using UnityEngine;

public class PointInTime
{
    public Vector2 position;
	public Quaternion rotation;
	public Vector2? velocity;

	public PointInTime(Vector2 _position, Quaternion _rotation, Vector2? _velocity)
	{
		position = _position;
		rotation = _rotation;
		velocity = _velocity;
	}
}
