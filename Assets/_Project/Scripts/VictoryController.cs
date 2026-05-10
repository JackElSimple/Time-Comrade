using UnityEngine;
using System.Collections;
public class VictoryController : MonoBehaviour
{
	[Header("Selecciona la escena del menú")]
	[SceneName] 
	[SerializeField] private string nextSceneName;

	private ParticleSystem part;
    private void Awake()
    {
        part = GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{

			SceneController sc = FindFirstObjectByType<SceneController>();
			part.Emit(50);
            if (sc != null)
            {
                sc.ReproducirTerminarNivel();
            }
            StartCoroutine(WaitAndComplete(1.5f));
        }
	}
    IEnumerator WaitAndComplete(float segundos)
    {

        yield return new WaitForSeconds(segundos);
        SceneController sc = FindFirstObjectByType<SceneController>();
        if (sc != null)
        { 
            // Aquí usamos el string que el desplegable rellenó por nosotros
            sc.CompleteLevel(nextSceneName);
        }
    }
}