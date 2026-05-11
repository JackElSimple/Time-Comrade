using UnityEngine;
using System.Collections;
public class VictoryController : MonoBehaviour
{
	[Header("Selecciona la escena del menú")]
	[SceneName] 
	[SerializeField] private string nextSceneName;

	private ParticleSystem part;
    private SceneController sc;
    private void Awake()
    {
        part = GetComponent<ParticleSystem>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{

            sc = FindFirstObjectByType<SceneController>();
			part.Emit(50);
            if (sc != null)
            {
                sc.ReproducirTerminarNivel();
            }
            StartCoroutine(WaitAndComplete(1.0f));
        }
	}
    IEnumerator WaitAndComplete(float segundos)
    {

        yield return new WaitForSeconds(segundos);
        if (sc != null)
        { 
            // Aquí usamos el string que el desplegable rellenó por nosotros
            sc.CompleteLevel(nextSceneName);
        }
    }

}