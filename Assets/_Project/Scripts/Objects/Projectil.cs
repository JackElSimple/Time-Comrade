using UnityEngine;

public class Projectil : MonoBehaviour
{
    public Vector2 speed;
    void Start()
    {
    }
    void Update()
    {
        transform.Rotate(0.0f, 0.0f, -360.0f * Time.deltaTime);
        Vector2 off = speed * Time.deltaTime;
        transform.position += new Vector3(off.x, off.y);
    }
}