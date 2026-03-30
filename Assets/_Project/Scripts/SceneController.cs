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
    [SerializeField] private float recordingDuration = 10.0f; // Duracion maxima de la grabacion, se podria hacer publica para que segun el nivel dure mas o menos

    [Header("Objetos en escena")]
	[SerializeField] private Transform currentSpawnPoint;
	[SerializeField] private GameObject personaje;
    [SerializeField] private GameObject sombra;
    public static List<RecordSwitch> recordingListeners = new List<RecordSwitch>();
    public static List<SaveListener> saveListeners = new List<SaveListener>();

    private bool isRecording;
    private float recordingTime = 0;
    private GameObject opit;
    private GameObject clone;

    void Start()
    {  
        CreateOpit();
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.timeScale == 0f) return;


        if (isRecording)
        {
            recordingTime += Time.deltaTime;
            if (recordingTime >= recordingDuration)
            {
                isRecording = false;
                LoadState();
                notifyListenersStop();
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

		if (sombra == null) return;

		clone = Instantiate(sombra);
		OpitControllerRewind playerScript = opit.GetComponent<OpitControllerRewind>();
		CloneController cloneScript = clone.GetComponent<CloneController>();

		// Pasamos todos los datos de una vez
		cloneScript.SetData(
			playerScript.getImputsList(),
			playerScript.getInitialPosition(),
			playerScript.getInitialVelocity()
		);
	}

	private void SaveState()
	{
		if (opit != null) opit.GetComponent<OpitControllerRewind>().StartRecording();
		if (clone != null) Destroy(clone); // Limpiamos el clon anterior si existe

		foreach (var obj in saveListeners) obj.SaveState();
	}

	private void LoadState()
    {
        opit.GetComponent<OpitControllerRewind>().FinishRecording(); //change because the OpitControllerRewind can change
        CreateClone();
		foreach (var obj in saveListeners)
            obj.LoadState();
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
    private void OnDestroy()
    {
        recordingListeners.Clear();
        saveListeners.Clear();
    }
}
