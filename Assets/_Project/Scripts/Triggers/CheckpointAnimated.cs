using UnityEngine;

public class CheckpointAnimated : MonoBehaviour
{
    private Animator anim;
    private AudioSource auso;

    private void Awake()
    {
        anim = GetComponent<Animator>();
        auso = GetComponent<AudioSource>();
    }
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            // Buscamos el SceneController y le pasamos nuestra posición
            SceneController sc = FindFirstObjectByType<SceneController>();
            if (sc != null)
            {

                sc.UpdateSpawnPoint(this.transform.GetChild(0));//Un empty más elevado ya que la lampara tiene el pivote en el pie
                anim.SetBool("On", true);
                auso.Play();
                Debug.Log("Checkpoint alcanzado: " + gameObject.name);
            }

            // Desactivar el trigger para que no se use más de una vez
            GetComponent<Collider2D>().enabled = false;
        }
    }
}