using UnityEngine;

public class Projectil : TimeBody
{
    public Vector2 speed;
    
    protected override void OnUpdate()
    {
        transform.Rotate(0.0f, 0.0f, -360.0f * Time.deltaTime);
        Vector2 off = speed * Time.deltaTime;
        transform.position += new Vector3(off.x, off.y);
    }
	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.gameObject.CompareTag("Player"))
		{
			Debug.Log("El proyectil ha golpeado al jugador");
			Destroy(gameObject);
			sc.KillPlayer();
		}
	}
}