using UnityEngine;
using UnityEngine.Events;

public class PressurePlateInteraction : MonoBehaviour
{
    private float pressDepth = 0.1f;
    private float pressSpeed = 2f;

    private bool pressed = false;
    private bool doublePressed = false; //para que el clon no se quite
    public UnityEvent onPressed;
    public UnityEvent onReleased;

    private Vector3 initialPos;
    private bool playerOnPlate = false;

    void Start()
    {
        initialPos = transform.position;
    }

    void Update()
    {
        Vector3 targetPos = playerOnPlate 
            ? initialPos + Vector3.down * pressDepth 
            : initialPos;

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            pressSpeed * Time.deltaTime
        );
    }

    void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.transform.tag == "Player")
        {   
            if (pressed){
                doublePressed = true;
            }
            else { 
                doublePressed= false; //por si acaso
                pressed = true;
                collision.transform.SetParent(transform); 
                playerOnPlate = true;
                onPressed?.Invoke();
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
       
        if (collision.transform.tag == "Player")
        {
            if (doublePressed)
            {
                doublePressed = false;
            }
            else { 
                pressed = false;
                collision.transform.SetParent(null);
                playerOnPlate = false;
                onReleased?.Invoke();
            }
        }
    }
}