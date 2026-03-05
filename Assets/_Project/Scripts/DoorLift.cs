using UnityEngine;

public class DoorLift : MonoBehaviour
{
    Vector3 closedPos;

    public float liftHeight = 3f;
    public float speed = 2f;

    bool open = false;

    void Start()
    {
        closedPos = transform.position;
    }

    void Update()
    {
        Vector3 target = open
            ? closedPos + Vector3.up * liftHeight //vector3 is directional vector for y axis
            : closedPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            target,
            speed * Time.deltaTime
        );
    }

    public void OpenDoor()
    {
        open = true;
    }

    public void CloseDoor()
    {
        open = false;
    }
}