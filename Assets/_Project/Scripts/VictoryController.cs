using UnityEngine;

public class VictoryController : MonoBehaviour
{
	[SerializeField] private string nextSceneName;

	private void OnTriggerEnter2D(Collider2D collision)
	{
		// Verificamos si es el jugador original el que entra
		if (collision.CompareTag("Player"))
		{
			// Buscamos el SceneController para que gestione la carga
			SceneController sc = FindFirstObjectByType<SceneController>();
			if (sc != null)
			{
				sc.CompleteLevel(nextSceneName);
			}
		}
	}
}