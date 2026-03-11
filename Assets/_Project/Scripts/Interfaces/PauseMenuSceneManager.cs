using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuSceneManager : MonoBehaviour
{

	public Button defaultButton;
	public GameObject canvasCredits;
	public GameObject canvasControls;

	private string pauseSceneName = "PauseMenu";
	void Start()
	{
		Cursor.visible = true;
		Cursor.lockState = CursorLockMode.None;
	}
	void Update()
	{
		if (canvasCredits.activeSelf || canvasControls.activeSelf)
		{
			if (Input.anyKeyDown)
			{
				canvasCredits.SetActive(false);
				canvasControls.SetActive(false);
			}
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			Reanudar();
		}
	}



	public void Reanudar()
	{
		PauseMenuHandler.Instance.TogglePause();
	}

	public void BotonSalir()
	{
		PauseMenuHandler.Instance.QuitToMainMenu();
	}

}
