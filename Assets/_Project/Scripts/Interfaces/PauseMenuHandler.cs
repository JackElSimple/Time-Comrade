using UnityEngine;
using UnityEngine.SceneManagement;
public class PauseMenuHandler : MonoBehaviour
{
	public static PauseMenuHandler Instance;

	[SerializeField] private string pauseSceneName = "PauseMenu";
	public bool isPaused { get; private set; }

	void Awake()
	{
		if (Instance == null) { Instance = this; DontDestroyOnLoad(gameObject); }
		else { Destroy(gameObject); }
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (!CanPauseInCurrentScene()) return;
			TogglePause();
		}
	}

	bool CanPauseInCurrentScene()
    {
        string escenaActiva = SceneManager.GetActiveScene().name;
        // Solo permitimos pausar si NO estamos en menús principales
        return escenaActiva != "MainMenu" && escenaActiva != "Splash" && escenaActiva != pauseSceneName;
    }

	public void TogglePause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			Time.timeScale = 0f;
			SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);
		}
		else
		{
			Time.timeScale = 1f;
			SceneManager.UnloadSceneAsync(pauseSceneName);
		}
	}
	public void QuitToMainMenu()
	{
		isPaused = false;
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}
}