using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using static OpitControllerRewind;

public class SceneController : MonoBehaviour
{

    [Header("Cosas Rewind")]
    [SerializeField] private float recordingDuration = 5.0f; // Duracion maxima de la grabacion, se podria hacer publica para que segun el nivel dure más o menos

    [Header("Objetos en escena")]
    [SerializeField] private GameObject spawnPoint;
    [SerializeField] private GameObject personaje;
    [SerializeField] private GameObject sombra;
    [SerializeField] private GameObject[] inanimateObjects;
    [SerializeField] private GameObject[] enemies;

    private bool isRecording;
    private float recordingTime = 0;
    private GameObject opit;
    private GameObject clone;
    private Vector3[] positions = new Vector3[100];
    private Vector3[] velocitys = new Vector3[100];
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CreateOpit();
    }

    // Update is called once per frame
    void Update()
    {

		// Si el manager dice que estamos pausados, salimos del Update antes de leer nada
		if (PauseMenuHandler.Instance != null && PauseMenuHandler.Instance.isPaused)
			return;


        if (isRecording)
        {
            recordingTime += Time.deltaTime;
            if (recordingTime >= recordingDuration)
            {
                isRecording = false;
                LoadState();

            }
        }

        

    }
	public void GestionarHabilidad()
	{
		if (!isRecording)
		{
			recordingTime = 0; // antes recordingTime = Time.deltaTime;
			isRecording = true;
			SaveState();
		}
		else
		{
			isRecording = false;
			LoadState();
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

    private void SaveState()
    {
        opit.GetComponent<OpitControllerRewind>().StartRecording(); //opit
        Destroy(clone); //clone
        for (int i=0; i < inanimateObjects.Length; i++)
        {
            positions[i] = inanimateObjects[i].transform.position;
            //add the status that u want
        }
        for (int i = 0; i < enemies.Length; i++)
        {
             positions[i + inanimateObjects.Length] = enemies[i].transform.position;
             velocitys[i + inanimateObjects.Length] =enemies[i].GetComponent<Rigidbody2D>().linearVelocity;
            //add the status that u want
        }
    }

    private void LoadState()
    {
        opit.GetComponent<OpitControllerRewind>().FinishRecording(); //change because the OpitControllerRewind can change
        CreateClone();
        for (int i = 0; i < inanimateObjects.Length; i++)
        {
             inanimateObjects[i].transform.position = positions[i];
            //add the status that u want
        }

        for (int i = 0; i < enemies.Length; i++)
        {
            enemies[i].transform.position = positions[i + inanimateObjects.Length];
            enemies[i].GetComponent<Rigidbody2D>().linearVelocity = velocitys[i + inanimateObjects.Length];
            //add the status that u want
        }
    }

    public void KillPlayer()//and respwan it
    {

        Destroy(opit);
        Destroy(clone);
        isRecording = false;
        CreateOpit();
    }
    public void KillClone()
    {
        Destroy(clone);
    }

}
