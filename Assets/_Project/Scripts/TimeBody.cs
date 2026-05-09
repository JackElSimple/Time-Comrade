using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TimeBody : MonoBehaviour, SaveListener
{
	private const float RewindSpeedMultiplier = 2f; // Must match its scenecontroller counterpart !!!!
	
	List<PointInTime> pointsInTime = new List<PointInTime>();
	Rigidbody2D rb;
	protected SceneController sc;
	private float rewindAccumulator = 0f;
	private bool createdDuringRecording = false;

	protected void Update() 
	{
		if (sc != null && sc.isRewinding) return;
		OnUpdate();
	}
	protected virtual void OnUpdate()
	{
		// Vacio, para ser sobrescrito
	}
	protected virtual void Awake()
    {
		rb = GetComponent<Rigidbody2D>();
		sc = Object.FindAnyObjectByType<SceneController>();
		createdDuringRecording = sc.isRecording;

	}

	void FixedUpdate()
	{
		if (Time.timeScale == 0f) return;

		if (sc.isRecording)
			Record();
		else if (sc.isRewinding) // Puede haber desincronizacion si hay muchos objetos con rewind,ya que quiza varia el numero exacto de puntos guardados
			Rewind();
		else if(pointsInTime.Count > 0) StopRewind(); // Si no se esta grabando ni rebobinando, StopRewind por si se quedaron puntos pendientes
		
	}

	protected virtual void Record()
	{
		Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
		pointsInTime.Add(new PointInTime(transform.position, transform.rotation, velocity)); // Agregar el nuevo punto al final de la lista
	}

	protected virtual void Rewind()
	{
		rewindAccumulator += RewindSpeedMultiplier;
		int snapshotsToRemove = Mathf.FloorToInt(rewindAccumulator);
		
		for (int i = 0; i < snapshotsToRemove && pointsInTime.Count > 0; i++)
		{
			PointInTime pointInTime = pointsInTime[pointsInTime.Count - 1];
			transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			pointsInTime.RemoveAt(pointsInTime.Count - 1);
		}

		rewindAccumulator -= snapshotsToRemove;

		if (pointsInTime.Count == 0)
		{
			StopRewind();
		}
	}
	

	public void StopRewind()
	{
		if (pointsInTime.Count == 0 && createdDuringRecording)
		{
			Destroy(gameObject);
			return;
		}

		if (pointsInTime.Count > 0)
		{
			PointInTime firstPoint = pointsInTime[0]; // Destino final
			transform.position = firstPoint.position;
			transform.rotation = firstPoint.rotation;

			pointsInTime.Clear(); // Vaciamos todo
		}
		if (rb != null) {
			rb.simulated = true;
			rb.linearVelocity = Vector2.zero;
			rb.angularVelocity = 0f;
		}
	}
	public void StartRewind()
	{
		if (rb != null)
		{
			rb.simulated = false;
		}
	}
	public virtual void SaveState() // Recording time starts
	{
		pointsInTime.Clear();
		Record(); // T0
		createdDuringRecording = false;
	}

	public virtual void LoadState() // Recording time ends or Player stops it manually
	{
		rewindAccumulator = 0f;
		StartRewind();
	}

	public virtual void CancelState()
	{
		rewindAccumulator = 0f;
		pointsInTime.Clear();
		if (rb != null)
		{
			rb.simulated = true;
		}
	}

	public virtual void OnRewindFinished()
	{
		rewindAccumulator = 0f;
		StopRewind();
	}
	void OnEnable()
	{
		SceneController.saveListeners.Add(this);
	}

	void OnDisable()
	{
		SceneController.saveListeners.Remove(this);
	}
}
