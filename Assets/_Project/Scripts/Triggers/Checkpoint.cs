using UnityEngine;

public class Checkpoint : MonoBehaviour
{
	private void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player"))
		{
			// Buscamos el SceneController y le pasamos nuestra posición
			SceneController sc = FindFirstObjectByType<SceneController>();
			if (sc != null)
			{
				sc.UpdateSpawnPoint(this.transform);
				Debug.Log("Checkpoint alcanzado: " + gameObject.name);
			}

			// Desactivar el trigger para que no se use más de una vez
			GetComponent<Collider2D>().enabled = false;
		}
	}
}