using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TimeBody : MonoBehaviour, SaveListener
{
	List<PointInTime> pointsInTime = new List<PointInTime>();
	Rigidbody2D rb;
	protected SceneController sc;

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
		if (pointsInTime.Count > 0) // Si hay puntos guardados, rebobinar y eliminarlos puntos de la lista
		{

			PointInTime pointInTime = pointsInTime[pointsInTime.Count - 1];
			//float error = Vector2.Distance(transform.position, pointInTime.position); Debug.Log("Error: " + error);
			transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			pointsInTime.RemoveAt(pointsInTime.Count - 1);
		}
		else
		{
			StopRewind();
		}
	}
	

	public void StopRewind()
	{
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
	}

	public virtual void LoadState() // Recording time ends or Player stops it manually
	{
		StartRewind();
	}

	public virtual void CancelState()
	{
		pointsInTime.Clear();
		if (rb != null)
		{
			rb.simulated = true;
		}
	}

	public virtual void OnRewindFinished()
	{
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
