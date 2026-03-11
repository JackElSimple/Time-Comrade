using UnityEngine;

public class VictoryController : MonoBehaviour
{
	[Header("Selecciona la escena del menú")]
	[SceneName] // <--- Esta es la magia
	[SerializeField] private string nextSceneName;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		if (collision.CompareTag("Player"))
		{
			SceneController sc = FindFirstObjectByType<SceneController>();
			if (sc != null)
			{
				// Aquí usamos el string que el desplegable rellenó por nosotros
				sc.CompleteLevel(nextSceneName);
			}
		}
	}
}