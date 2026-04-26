using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [Header("Parallax")]
    [Range(-1f, 1f)] public float parallaxX = 0.2f;
    [Range(0f, 1f)] public float parallaxY = 0f;

    private Transform cam;
    private Vector3 initialCamPosition;
    private Vector3 initialLayerPosition;
    private bool initialized = false;

    void Start()
    {
        initialLayerPosition = transform.position;
        TryFindCamera();
    }

    void LateUpdate()
    {
        if (cam == null)
        {
            TryFindCamera();
            return;
        }

        if (!initialized)
        {
            initialCamPosition = cam.position;
            initialized = true;
        }

        Vector3 delta = cam.position - initialCamPosition;

        transform.position = new Vector3(
            initialLayerPosition.x + delta.x * parallaxX,
            initialLayerPosition.y + delta.y * parallaxY,
            initialLayerPosition.z
        );
    }

    void TryFindCamera()
    {
        if (Camera.main != null)
        {
            cam = Camera.main.transform;
        }
    }
}