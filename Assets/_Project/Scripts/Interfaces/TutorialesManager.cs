using System.Collections;
using UnityEngine;
using TMPro;
public class TutorialesManager : MonoBehaviour
{
	public GameObject panelMensaje; 

	public void MostrarMensaje()
	{
		panelMensaje.SetActive(true);
	}
	public void OcultarMensaje()
	{
		panelMensaje.SetActive(false);
	}
}
