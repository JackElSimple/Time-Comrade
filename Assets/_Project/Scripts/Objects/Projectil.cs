using UnityEngine;

public class Projectil : TimeBody
{
    public Vector2 speed;
    private void makeSound()
    {
        SceneController sc = FindFirstObjectByType<SceneController>();
        sc.ReproducirBalaDestruida();
    }
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
            makeSound();
            sc.KillPlayer();
		}
        if (collision.gameObject.CompareTag("Clone"))
        {
            Debug.Log("El proyectil ha golpeado al clon");
            makeSound();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            Destroy(gameObject);
            makeSound();
        }
    }
    
}