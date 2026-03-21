using UnityEngine;
using UnityEngine.Events;

public class PressurePlatePlatform : MovingPlatformBase
{
    [Header("Events")]
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private bool pressed = false;
    private bool doublePressed = false;  
    private bool playerOnPlate = false;

    protected override void Awake()
    {
        base.Awake();

        currentTarget = transform.position;
    }

    protected override void Update()
    {
        if (targetPoint == null) return;

        currentTarget = playerOnPlate ? targetPoint.position : startPos;

        Move();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        if (pressed)
            doublePressed = true;
        else
        {
            doublePressed = false;
            pressed = true;
            playerOnPlate = true;
            HandlePassenger(collision.transform, true); // parent player
            onPressed?.Invoke();
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (!collision.transform.CompareTag("Player")) return;

        if (doublePressed)
            doublePressed = false;
        else
        {
            pressed = false;
            playerOnPlate = false;
            HandlePassenger(collision.transform, false); // unparent player
            onReleased?.Invoke();
        }
    }
}