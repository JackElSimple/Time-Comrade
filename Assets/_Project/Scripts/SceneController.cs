using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.EditorTools;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static event Action RewindStarted;
    public static event Action RewindEnded;

    [Header("Cosas Rewind")]
    [SerializeField] private float recordingDuration = 10.0f; // Duracion maxima de la grabacion, se podria hacer publica para que segun el nivel dure mas o menos

    [Header("Objetos en escena")]
	[SerializeField] private Transform currentSpawnPoint;
	[SerializeField] private GameObject personaje;
    [SerializeField] private GameObject sombra;
	[Header("Sonidos")]
    [SerializeField] private AudioClip theme, turretSound, levelEndsSound, deathSound, footstepSound;
    public static List<RecordSwitch> recordingListeners = new List<RecordSwitch>();
    public static List<SaveListener> saveListeners = new List<SaveListener>();
	public bool isRecording { get; private set; } 
	public bool isRewinding { get; private set; }
	private float recordingTime = 0;
    private GameObject opit;
    private GameObject clone;
	private OpitControllerRewind opitScript;
	private CloneController cloneScript;
	void Start()
    {
        if (theme != null)
        {
            GameManager.Instance.audioManager.PlayMusic(theme);
        }
        CreateOpit();
        
    }

    void FixedUpdate()
    {

        if (Time.timeScale == 0f) return;

        if (isRecording)
        {
            recordingTime += Time.fixedDeltaTime;
            if (recordingTime >= recordingDuration) // terminar grabacion y reproducirla
			{
                isRecording = false;
                LoadState();
                notifyListenersStop();
            }
        }
		if (isRewinding)
		{
			recordingTime -= Time.fixedDeltaTime; // El tiempo corre hacia atras

			if (recordingTime <= 0)
			{
				isRewinding = false;
				recordingTime = 0;
				EndGlobalRewind();
			}
		}

	}
	private void EndGlobalRewind()
	{
		isRewinding = false;
		for (int i = saveListeners.Count - 1; i >= 0; i--)
		{
			
			if (i < saveListeners.Count && saveListeners[i] != null)
			{
				saveListeners[i].OnRewindFinished();
			}
		}
		CreateClone();
        RewindEnded?.Invoke();
		Debug.Log("Sincronizacion completa: Todos los objetos han salido del modo rebobinado.");
	}
	public void GestionarHabilidad()
	{
		if (isRewinding) return;
		if (!isRecording)
		{
			recordingTime = 0; 
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
		opitScript = opit.GetComponent<OpitControllerRewind>();

		opit.transform.position = currentSpawnPoint.transform.position;
    }
    private void CreateClone()
    {

		if (sombra == null) return;

		clone = Instantiate(sombra);
		
		cloneScript = clone.GetComponent<CloneController>();

		// Pasamos todos los datos de una vez
		cloneScript.SetData(
			opitScript.recordedInputs,
			opitScript.initialPosition,
			opitScript.initialVelocity
		);
	}

	private void SaveState()
	{
		foreach (var obj in saveListeners) obj.SaveState();

		if (opit != null) {
			isRecording = true;
			opitScript.StartRecording();
		}
		if (clone != null) Destroy(clone); // Limpiamos el clon anterior si existe

	}

	private void LoadState() // Recording time ends or Player stops it manually
	{
		if (opit != null)
		{
			isRecording = false;
			isRewinding = true;
			opitScript.FinishRecording(); 
            RewindStarted?.Invoke();
		}

		foreach (var obj in saveListeners) obj.LoadState();
    }

    public void KillPlayer()//and respawn it
    {
        Destroy(opit);
        if (deathSound != null)
            GameManager.Instance.audioManager.PlaySound(deathSound);
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
			opitScript.FinishRecording();

		// Cargamos la siguiente escena
		Debug.Log("<color=green>[SCENE] Nivel Completado. Cargando: " + sceneName + "</color>");
		SceneManager.LoadScene(sceneName);
	}
	public void CancelarGrabacion()
	{
		if (isRecording)
		{
			isRecording = false;
			isRewinding = false; // SHouldnt happen but just in case
			recordingTime = 0;

			if (opit != null)
			{
				opitScript.CancelRecording();
				isRecording = false;
			}

			foreach (var obj in saveListeners) obj.CancelState();
			notifyListenersStop();

			Debug.Log("Habilidad cancelada: El personaje se queda donde esta.");
		}
	}

	public void ReproducirDisparo()
	{
        if (turretSound != null)
            GameManager.Instance.audioManager.PlaySound(turretSound);

    }
    public void ReproducirTerminarNivel()
    {
        if (levelEndsSound != null)
            GameManager.Instance.audioManager.StopMusic();
			GameManager.Instance.audioManager.PlaySound(levelEndsSound);

    }
    public void ReproducirPisada()
    {
        if (footstepSound != null)
            GameManager.Instance.audioManager.PlaySound(footstepSound);

    }
    private void OnDestroy()
    {
        recordingListeners.Clear();
        saveListeners.Clear();
    }
}
