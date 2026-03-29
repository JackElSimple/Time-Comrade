using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class TimeBody : MonoBehaviour
{
	public bool isRewinding = false;
	public float recordTime = 10f;
	List<PointInTime> pointsInTime;
	Rigidbody2D rb;


	void Start()
    {
        pointsInTime = new List<PointInTime>();
		rb = GetComponent<Rigidbody2D>();
	}

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.K))
			StartRewind();

		if (Input.GetKeyUp(KeyCode.K))
			StopRewind();
	}

	void FixedUpdate()
	{
		if (isRewinding)
			Rewind();
		else
			Record();
	}

	void Record()
	{
		if (pointsInTime.Count > Mathf.Round(recordTime / Time.fixedDeltaTime)) // Si han pasado mas de 10 segundos, eliminar el punto mas antiguo
		{
			pointsInTime.RemoveAt(pointsInTime.Count - 1);
		}
		pointsInTime.Insert(0,new PointInTime(transform.position, transform.rotation));
	}

	void Rewind()
	{
		if (pointsInTime.Count > 0)
		{
			PointInTime pointInTime = pointsInTime[0];
			transform.position = pointInTime.position;
			transform.rotation = pointInTime.rotation;
			pointsInTime.RemoveAt(0);
		}
		else
		{
			StopRewind();
		}
	}
	public void StartRewind()
	{
		isRewinding = true;
		if (rb != null) { rb.bodyType = RigidbodyType2D.Kinematic; }


	}

	public void StopRewind ()
	{
		isRewinding = false;
		if (rb != null) {rb.bodyType = RigidbodyType2D.Dynamic;}
	}
}
