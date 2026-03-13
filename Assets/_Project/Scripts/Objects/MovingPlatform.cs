using UnityEngine;
using static UnityEditor.PlayerSettings;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{


public GameObject destination;
    public float speed = 4f;
    private Rigidbody2D _rb;
    private Vector3 _startPos;
    private int _direction = 1;
    void Start()
    {
        _startPos = transform.position;
        _rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        if (destination == null) return;
        Vector2 s = _startPos, e = destination.transform.position;
        float completed;
        if (_direction == 1)
        {
            completed = Vector2.Distance(s, transform.position);
        }
        else
        {
            completed = Vector2.Distance(e, transform.position);
        }
        if (completed / Vector2.Distance(s, e) >= 0.99f)
            _direction = -_direction;
        var dir = (e - s).normalized * speed;
        _rb.linearVelocity = dir * _direction;
    }

}