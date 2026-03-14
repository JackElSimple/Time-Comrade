using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;
using static OpitControllerRewind;

public class SceneController : MonoBehaviour
{

    [Header("Cosas Rewind")]
    [SerializeField] private float recordingDuration = 5.0f; // Duracion maxima de la grabacion, se podria hacer publica para que segun el nivel dure m�s o menos

    [Header("Objetos en escena")]
	[SerializeField] private Transform currentSpawnPoint;
	[SerializeField] private GameObject personaje;
    [SerializeField] private GameObject sombra;
    [SerializeField] private GameObject[] inanimateObjects;
    [SerializeField] private GameObject[] enemies;
    public static List<RecordSwitch> recordingListeners = new List<RecordSwitch>();

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
            notifyListenersStart();
            
		}
		else
		{
			isRecording = false;
			LoadState();
            notifyListenersStop();
		}
	}

    public void notifyListenersStart()
    {
        foreach (var obj in recordingListeners)
            obj.OnRecordingStart();
    }
    public void notifyListenersStop()
    {
        foreach (var obj in recordingListeners)
            obj.OnRecordingStop();
    }

	public void UpdateSpawnPoint(Transform newSpawn)
	{
		currentSpawnPoint = newSpawn;
	}
	private void CreateOpit()
    {
        opit = Instantiate<GameObject>(personaje);
        opit.transform.position = currentSpawnPoint.transform.position;
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
		for (int i = 0; i < inanimateObjects.Length; i++)
		{
			var movable = inanimateObjects[i].GetComponent<MovableObject>();
			if (movable != null)
				movable.RecordCurrentState();
			else
				positions[i] = inanimateObjects[i].transform.position; // Backup por si no tiene el script
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
			var movable = inanimateObjects[i].GetComponent<MovableObject>();
			if (movable != null)
				movable.RestoreState();
			else
				inanimateObjects[i].transform.position = positions[i];
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
	public void CompleteLevel(string sceneName)
	{
		//  detenemos la grabacion si estaba activa
		if (opit != null)
			opit.GetComponent<OpitControllerRewind>().FinishRecording();

		// Cargamos la siguiente escena
		Debug.Log("<color=green>[SCENE] Nivel Completado. Cargando: " + sceneName + "</color>");
		SceneManager.LoadScene(sceneName);
	}
	public void CancelarGrabacion()
	{
		if (isRecording)
		{
			isRecording = false;
			recordingTime = 0;

			if (opit != null)
			{
				opit.GetComponent<OpitControllerRewind>().CancelRecording();
			}

			Debug.Log("Habilidad cancelada: El personaje se queda donde est�.");
		}
	}
}
