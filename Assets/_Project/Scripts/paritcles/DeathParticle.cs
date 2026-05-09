using UnityEngine;

public class DeathParticle: MonoBehaviour
{
    [HideInInspector] public float lifetime = 0.5f;

    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
        Destroy(gameObject, lifetime);
    }

    void Update()
    {
        Color c = sr.color;

        c.a -= Time.deltaTime / lifetime;

        sr.color = c;
    }
}