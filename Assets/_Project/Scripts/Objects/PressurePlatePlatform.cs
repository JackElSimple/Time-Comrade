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

    protected override void OnUpdate()
    {
		base.OnUpdate();

		if (targetPoint == null) return;

        currentTarget = playerOnPlate ? targetPoint.position : startPos;

        Move();
    }

    protected override void OnCollisionEnter2D(Collision2D collision)
    {
        if (!(collision.transform.CompareTag("Player") || collision.transform.CompareTag("Clone") || collision.transform.CompareTag("ActivaBoton"))) return;

        if (pressed)
            doublePressed = true;
        else
        {
            SceneController sc = FindFirstObjectByType<SceneController>();
            sc.ReproducirPlateOn();
            doublePressed = false;
            pressed = true;
            playerOnPlate = true;
            HandlePassenger(collision.transform, true); // parent player
            onPressed?.Invoke();
        }
    }

    protected override void OnCollisionExit2D(Collision2D collision)
    {
        if (!(collision.transform.CompareTag("Player") || collision.transform.CompareTag("Clone") || collision.transform.CompareTag("ActivaBoton"))) return;

        if (doublePressed)
            doublePressed = false;
        else
        {
            SceneController sc = FindFirstObjectByType<SceneController>();
            sc.ReproducirPlateOff();
            pressed = false;
            playerOnPlate = false;
            HandlePassenger(collision.transform, false); // unparent player
            onReleased?.Invoke();
        }
    }
}