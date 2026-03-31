using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TimeBody : MonoBehaviour
{
	//public bool isRecording = false;
	//public bool isRewinding = false;
	//public float recordTime = 10f;
	List<PointInTime> pointsInTime;
	Rigidbody2D rb;
	private SceneController sc;


	void Start()
    {
        pointsInTime = new List<PointInTime>();
		rb = GetComponent<Rigidbody2D>();
		sc = Object.FindAnyObjectByType<SceneController>();

	}

	void Update()
    {
		if (sc.isRewinding) { return; }
		if (Input.GetKeyDown(KeyCode.K))
		{
			if (!sc.isRecording) { 
				isRecording = true;
			}
			else {
				isRecording = false;
				StartRewind();
			}
		}
	}

	void FixedUpdate()
	{
		if (sc.isRecording)
			Record();
		if (sc.isRewinding)
			Rewind();
	}

	void Record()
	{
		if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime)) // Si han pasado mas de 10 segundos, dejar de grabar, rebobinar y reproducir la grabacion
		{
			isRecording = false;
			StartRewind();
		}
		Vector2 velocity = rb != null ? rb.linearVelocity : Vector2.zero;
		pointsInTime.Add(new PointInTime(transform.position, transform.rotation, velocity)); // Agregar el nuevo punto al final de la lista
	}

	void Rewind()
	{
		if (pointsInTime.Count > 0) // Si hay puntos guardados, rebobinar y eliminarlos puntos de la lista
		{

			PointInTime pointInTime = pointsInTime[pointsInTime.Count - 1];
			float error = Vector2.Distance(transform.position, pointInTime.position); Debug.Log("Error: " + error);
			
			transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			pointsInTime.RemoveAt(pointsInTime.Count - 1);
		}
		else
		{
			StopRewind();
		}
	}
	public void StartRewind()
	{
		isRewinding = true;
		if (rb != null) {
			rb.simulated = false;
		}
	}

	public void StopRewind ()
	{
		isRewinding = false;
		if (rb != null) {
			rb.simulated = true;
			rb.linearVelocity = Vector2.zero;
			rb.angularVelocity = 0f;
		}
	}
}
