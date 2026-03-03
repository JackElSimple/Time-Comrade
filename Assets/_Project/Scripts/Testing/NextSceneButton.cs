using UnityEngine;
using UnityEngine.SceneManagement;

public class NextSceneButton : MonoBehaviour
{
	public void NextScene()
	{
		int totalScenes = SceneManager.sceneCountInBuildSettings;
		int nextIndex = (SceneManager.GetActiveScene().buildIndex + 1) % totalScenes;
		SceneManager.LoadScene(nextIndex);
	}

	private void OnGUI()
	{
		if (GUI.Button(new Rect(10, Screen.height - 50, 150, 30), "Next Scene"))
		{
			NextScene();
		}
	}
}