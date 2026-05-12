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

	
}