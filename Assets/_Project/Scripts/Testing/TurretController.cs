using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class TurretController : TimeBody
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed = 3.0f;
    [SerializeField] private float shootInterval = 1.0f; //seconds
    private float timeCooldown;
    private float savedCooldown;
    private Vector3 direction;
    private List<BulletData> bulletsList = new List<BulletData>();
    private List<BulletData> bulletsSaved = new List<BulletData>();
    //private Vector3 savedPosition;
    //private Quaternion savedRotation;
    private int numberBullets = 0;
    [SerializeField] private bool automated = false; //if true it shoots all the time
    [SerializeField]  private int distanciaRayo = 15;
    private Vector2 Horizontal = new Vector2(-1,0);
	private void Start()
    {
        timeCooldown = shootInterval;

        Debug.Log(transform.GetChild(0).transform.position);
        Debug.Log(transform.GetChild(1).transform.position);
       
        
    }
    void Fire()
    {
        direction = transform.GetChild(0).position - transform.GetChild(1).position; //punta - cañon
        GameObject obj = Instantiate<GameObject>(bullet);
        obj.transform.position = transform.GetChild(0).position;
        Projectil proj = obj.GetComponent<Projectil>();
        proj.speed = bulletSpeed * direction;
        bulletsList.Add(new BulletData(obj,obj.transform.position,proj.speed));
        numberBullets++;
        print(numberBullets);
    }

    public struct BulletData //Struct for saving all the data of a bullet
    {
        public GameObject bullet;
        public Vector3 position;
        public Vector3 speed;

        public BulletData(GameObject b, Vector3 p, Vector3 s)
        {

            bullet = b;
            position = p;
            speed = s;
        }
    }
     protected override void OnUpdate()
    {
		Debug.DrawRay(transform.GetChild(0).transform.position, Horizontal* distanciaRayo, Color.red);
        timeCooldown += Time.deltaTime;// cada shootInterval segundos dispara y se reinicia el contador
        if (timeCooldown > shootInterval)
        { //solo si ve al jugador que no sea a través del muro
            if (automated){ 
                Fire();
                timeCooldown = 0;
            }
            else { 
                RaycastHit2D[] hits = Physics2D.RaycastAll(transform.GetChild(0).transform.position, Horizontal, distanciaRayo);
                foreach(RaycastHit2D hit in hits) {
                    if( hit.collider != null && hit.collider.gameObject.layer == LayerMask.NameToLayer("Ground"))
                        { break;}
                    if ((hit.collider != null && hit.collider.gameObject.CompareTag("Player")))
                    {
                        Fire();
                        timeCooldown = 0;
                        break;
                    }
                }
            }



        }
        for (int i = 0; i < numberBullets; i++)
        { 
            BulletData data = bulletsList[i];
            if (data.bullet != null){ 
               
                data.position = data.bullet.transform.position;
                data.speed = data.bullet.GetComponent<Projectil>().speed;
            }
            bulletsList[i]= data;
        }
    }

    public override void SaveState()
    {
        Debug.Log("xd");
        savedCooldown = timeCooldown;
		base.SaveState();
		//savedPosition = transform.position;
		//savedRotation = transform.rotation;
		bulletsSaved = bulletsList;
       
    }

    public override void LoadState()
    {
        Debug.Log("xdd");
        timeCooldown = savedCooldown;
       // transform.position = savedPosition;
       // transform.rotation = savedRotation;
	   base.LoadState();
		//UpdateBullets();
       // bulletsList = bulletsSaved;
        

    }
	public override void OnRewindFinished(){ 
		base.OnRewindFinished();
		bulletsList = bulletsSaved;
		//UpdateBullets();

	}

	private void UpdateBullets()
    {
        for (int i = 0; i < numberBullets; i++)
        {
            BulletData data = bulletsList[i];
            if (data.bullet != null)
            {
                Destroy(data.bullet); //delete the bullet
                data.bullet = null;
            }
            BulletData updatedBullets = bulletsSaved[i];
            if(updatedBullets.bullet != null) //generates the bullets in the state which were saved
            {
                Debug.Log(updatedBullets.position);
                direction = transform.GetChild(0).position - transform.GetChild(1).position; //punta - cañon
                GameObject obj = Instantiate<GameObject>(bullet);
                obj.transform.position = updatedBullets.position;
                Projectil proj = obj.GetComponent<Projectil>();
                proj.speed = updatedBullets.speed;
            }
        }
    }
}
