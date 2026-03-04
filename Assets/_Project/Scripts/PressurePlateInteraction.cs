using UnityEngine;

public class pressurePlateInteraction : MonoBehaviour
{
    Vector3 initialPos;
    float pressDepth = 0.1f;
    bool playerOnPlate = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPos = transform.position;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.transform.name=="Opit")
        {
            collision.transform.parent = transform; // if you don't group this the players will lag behind when the plate goes down
            playerOnPlate = true;
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.transform.name=="Opit")
        {
            if (transform.position.y > initialPos.y - pressDepth)
            {
                transform.Translate(0,-0.01f,0);
            }
        }
    }

     void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.transform.name=="Opit")
        {
            collision.transform.parent = null;
            playerOnPlate = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!playerOnPlate && transform.position.y < initialPos.y)
        {
            transform.Translate(0,0.01f,0);
        }
    }
}