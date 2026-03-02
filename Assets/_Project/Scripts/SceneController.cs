using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static OpitControllerRewind;

public class SceneController : MonoBehaviour
{

    [Header("Cosas Rewind")]
    [SerializeField] private float recordingDuration = 5.0f; // Duracion maxima de la grabacion, se podria hacer publica para que segun el nivel dure más o menos

    [Header("Cosas Patata")]
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject personaje;
    [SerializeField] private GameObject sombra;

    private bool isRecording;
    private float recordingTime = 0;
    private GameObject opit;
    private GameObject clone;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateOpit();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!isRecording)
            {
                recordingTime = Time.deltaTime;//cuenta un frame
                isRecording = true;
                opit.GetComponent<OpitControllerRewind>().StartRecording(); //change because the OpitControllerRewind can change
                Destroy(clone);
            }
            else
            {
                isRecording = false;
                opit.GetComponent<OpitControllerRewind>().FinishRecording(); //change because the OpitControllerRewind can change
                CreateClone();
            }
        }
        if (isRecording)
        {
            recordingTime += Time.deltaTime;
            if (recordingTime >= recordingDuration)
            {
                isRecording = false;
                opit.GetComponent<OpitControllerRewind>().FinishRecording(); //change because the OpitControllerRewind can change
                CreateClone();

            }
        }

        

    }
    private void CreateOpit()
    {
        opit = Instantiate<GameObject>(personaje);
        opit.transform.position = spawnPoint.transform.position;
    }
    private void CreateClone()
    {

        clone = Instantiate<GameObject>(sombra);

        clone.transform.position = opit.GetComponent<OpitControllerRewind>().getInitialPosition();
        clone.GetComponent<CloneController>().SetInitialPosition(clone.transform.position);

        clone.GetComponent<Rigidbody2D>().linearVelocity = opit.GetComponent<OpitControllerRewind>().getInitialVelocity();
        clone.GetComponent<CloneController>().SetInitialVelocity(clone.GetComponent<Rigidbody2D>().linearVelocity);
        List<PlayerInputFrame> listaInputs = opit.GetComponent<OpitControllerRewind>().getImputsList();

        clone.GetComponent<CloneController>().SetListaInputs(listaInputs);
    }

}
