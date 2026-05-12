using UnityEngine;

public class Projectil : TimeBody
{
    public Vector2 speed;
    private AudioSource auso;
	[SerializeField] private bool redBullet = false;

	protected override void Awake()
    { 
        base.Awake();
        auso = transform.GetComponent<AudioSource>();
    }
    private void makeSound()
    {
        auso.Play();
    }
    protected override void OnUpdate()
    {
        Vector2 off = speed * Time.deltaTime;
        transform.position += new Vector3(off.x, off.y);
    }
	private void OnTriggerEnter2D(Collider2D collision)
	{
        if (collision.gameObject.CompareTag("Player"))
		{
			Debug.Log("El proyectil ha golpeado al jugador");		
            sc.KillPlayer();
            makeSound();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Clone") && !redBullet)
        {
            Debug.Log("El proyectil ha golpeado al clon");
            makeSound();
            Destroy(gameObject);
        }
        if (collision.gameObject.CompareTag("Wall"))
        {
            makeSound();
            Destroy(gameObject);

        }
    }
    
}