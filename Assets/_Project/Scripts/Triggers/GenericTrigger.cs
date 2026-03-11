using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTrigger : MonoBehaviour
{
	[Header("Configuración de Filtro")]
	[Tooltip("Lista de Tags que pueden activar este trigger")]
	[SerializeField] private List<string> targetTags = new List<string> { "Player" };

	[SerializeField] private bool destroyAfterUse = false;

	[Header("Eventos a Ejecutar")]
	public UnityEvent onTriggerEnter;

	private void OnTriggerEnter2D(Collider2D other)
	{
		if (IsTargetTag(other.tag))
		{
			// Ejecuta los metodos configurados en el Inspector
			onTriggerEnter?.Invoke();

			if (destroyAfterUse)
			{
				Destroy(gameObject);
			}
		}
	}

	private bool IsTargetTag(string tagToCheck)
	{
		// Comprobamos si el tag del objeto esta en nuestra lista
		return targetTags.Contains(tagToCheck);
	}
}