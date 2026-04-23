using UnityEngine;
using UnityEngine.Events;

public class LeverPlate: MovingPlatformBase
{
    [Header("Events")]
    public UnityEvent on;
    public UnityEvent off;

    private bool pressed = false;
    private bool doublePressed = false;
    private bool playerOnPlate = false;
    private bool active = false;
    protected override void Awake()
    {
        base.Awake();

        currentTarget = transform.position;
    }

    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (targetPoint == null) return;

        currentTarget = playerOnPlate ? targetPoint.position : startPos;

        Move();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!(collision.transform.CompareTag("Player") || collision.transform.CompareTag("Clone"))) return;

        if (pressed)
            doublePressed = true;
        else
        {
            SceneController sc = FindFirstObjectByType<SceneController>();
            sc.ReproducirPalanca();
            doublePressed = false;
            pressed = true;
            playerOnPlate = true;
            HandlePassenger(collision.transform, true); // parent player
            if (!active) {
                on?.Invoke();
            }
            else
            {
                off?.Invoke();
            }
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if (!(collision.transform.CompareTag("Player") || collision.transform.CompareTag("Clone"))) return;

        if (doublePressed)
            doublePressed = false;
        else
        {
            pressed = false;
            playerOnPlate = false;
            HandlePassenger(collision.transform, false); // unparent player
            active = !active;
        }
    }
}