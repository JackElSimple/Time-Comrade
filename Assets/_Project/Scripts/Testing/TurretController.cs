using System.Collections.Generic;
using UnityEngine;
using static OpitControllerRewind;

public class TurretController : MovableObject
{
    [SerializeField] private GameObject bullet;
    [SerializeField] private float bulletSpeed = 3.0f;
    [SerializeField] private float shootInterval = 1.0f; //seconds
    private float timeCooldown;
    private float savedCooldown;
    private Vector3 direction;
    private List<BulletData> bulletsList = new List<BulletData>();
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        timeCooldown = shootInterval;
        InvokeRepeating("Fire", 0.0f, shootInterval);
    }
    void Fire()
    {
        direction = transform.GetChild(0).position - transform.GetChild(1).position; //punta - cañon
        GameObject obj = Instantiate<GameObject>(bullet);
        obj.transform.position = transform.GetChild(0).position;
        Projectil proj = obj.GetComponent<Projectil>();
        proj.speed = bulletSpeed * direction;
    }

    public struct BulletData //Struct for saving all the data of a bullet
    {
        public GameObject bullet;
        public Vector3 position;
        public Vector3 speed;
        public bool alive;

        public BulletData(GameObject b, Vector3 p, Vector3 s, bool a)
        {
            bullet = b;
            position = p;
            speed = s;
            alive = a;
        }
    }
    void Update()
    {
        timeCooldown += Time.deltaTime;// cada shootInterval segundos dispara y se reinicia el contador
        if (timeCooldown > shootInterval)
        {
            Fire();
            timeCooldown = 0;
        }
    }
    public void RecordCurrentState()
    {
        savedCooldown = timeCooldown;
        savedPosition = transform.position;
        savedRotation = transform.rotation;
        savedScale = transform.localScale;
    }

    public void RestoreState()
    {
        timeCooldown = savedCooldown;
        transform.position = savedPosition;
        transform.rotation = savedRotation;
        transform.localScale = savedScale;

    }

    // Update is called once per frame
    
}
