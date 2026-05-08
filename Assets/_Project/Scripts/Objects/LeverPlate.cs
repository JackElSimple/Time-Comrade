using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LeverPlate: MovingPlatformBase
{
    [Header("Events")]
    public UnityEvent on;
    public UnityEvent off;
    private GameObject led;
    private GameObject textOn;
    private GameObject textOff;

    private bool pressed = false;
    private bool doublePressed = false;
    private bool playerOnPlate = false;
    private bool active = false;
    private bool savedActive = false;
    protected override void Awake()
    {
        base.Awake();

        currentTarget = transform.position;
        if (transform.childCount == 1)
        {
            led = transform.GetChild(0).GameObject();
            led.GetComponent<SpriteRenderer>().color = Color.red;
            //textOn = transform.GetChild(1).GetChild(0).GameObject();
            //textOff = transform.GetChild(1).GetChild(1).GameObject();
            //textOn.SetActive(false);
            //textOff.SetActive(true);
        }
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
                if (led != null) {
                    led.GetComponent<SpriteRenderer>().color= Color.green;
                }
                //if (textOn != null) {
                //    textOn.SetActive(true);
                //    textOff.SetActive(false);
                //}
            }
            else
            {
                off?.Invoke();
                if (led != null)
                {
                    led.GetComponent<SpriteRenderer>().color = Color.red;
                }
                //if (textOn != null)
                //{
                //    textOn.SetActive(false);
                //    textOff.SetActive(true);
                //}
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

    public override void SaveState() //We will save the state of the pressure plate because it bugged when rewinding
    {
        base.SaveState();
        savedActive = active;
    }
    public override void OnRewindFinished()
    {
        base.OnRewindFinished();
        if (active != savedActive)
        {
            //if active is not the same of savedActive we toggle it
            if (!active)
            {
                on?.Invoke();
                if (led != null)
                {
                    led.GetComponent<SpriteRenderer>().color = Color.green;
                }
            }
            else
            {
                off?.Invoke();
                if (led != null)
                {
                    led.GetComponent<SpriteRenderer>().color = Color.red;
                }
            }
            if (!pressed) //if is not pressed, we simulate that someone left the pressure plate
            {
                active = savedActive; 
            }
        }

    }
}