using UnityEngine;

public class OneShotPlayer : MonoBehaviour
{
    GameObject controller;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
   
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnCollisionEnter2D(Collision2D collision)
        {
        controller = GameObject.FindWithTag("ControladorEscena");
            if (collision.gameObject.layer == LayerMask.NameToLayer("Opit"))
        {
            controller.GetComponent<SceneController>().KillPlayer();
        }
            if (collision.gameObject.layer == LayerMask.NameToLayer("Clone"))
            {
                controller.GetComponent<SceneController>().KillClone();
            }
        }
    }
