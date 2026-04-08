using UnityEngine;

public class ParallaxTileRepeater : MonoBehaviour
{
    private float width;
    private Transform cam;

    void Start()
    {
        width = GetComponent<SpriteRenderer>().bounds.size.x;
        TryFindCamera();
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            TryFindCamera();
            return;
        }

        float distance = cam.position.x - transform.position.x;

        if (distance > width * 1.5f)
        {
            transform.position += new Vector3(width * 3f, 0f, 0f);
        }
        else if (distance < -width * 1.5f)
        {
            transform.position -= new Vector3(width * 3f, 0f, 0f);
        }
    }

    void TryFindCamera()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }
}