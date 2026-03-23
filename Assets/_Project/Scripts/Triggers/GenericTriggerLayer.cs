using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class GenericTriggerLayer : MonoBehaviour
{
    [Header("Configuración de Filtro")]
    [Tooltip("Lista de Tags que pueden activar este trigger")]
    [SerializeField] private List<string> targetLayers = new List<string> { "Opit" };

    [SerializeField] private bool destroyAfterUse = false;

    [Header("Eventos a Ejecutar")]
    public UnityEvent onTriggerEnter;

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("macarrones");
        if (IsTargetLayer(other.gameObject.layer))
        {
            // Ejecuta los metodos configurados en el Inspector
            onTriggerEnter?.Invoke();

            if (destroyAfterUse)
            {
                Destroy(gameObject);
            }
        }
    }

    private bool IsTargetLayer(int layerToCheck)
    {
        // Comprobamos si el tag del objeto esta en nuestra lista
        for (int i = 0; i < targetLayers.Count; i++)
        {
            if (LayerMask.NameToLayer(targetLayers[i]) == layerToCheck) { return true; }
        }
        return false;
    }
}