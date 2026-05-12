using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuHandler : MonoBehaviour
{
	public static PauseMenuHandler Instance;

	[SerializeField] private string pauseSceneName = "PauseMenu";
	[SerializeField] private AudioClip enterPauseSound, exitPauseSound;

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

		if (Input.GetKeyDown(KeyCode.R))
		{
			if (!CanRestartCurrentScene()) return;
			RestartCurrentScene();
		}
	}

	bool CanPauseInCurrentScene()
	{
		string escenaActiva = SceneManager.GetActiveScene().name;
		// Solo permitimos pausar si NO estamos en menus principales
		return escenaActiva != "MainMenu_00" && escenaActiva != "Splash" && escenaActiva != pauseSceneName;
	}

	bool CanRestartCurrentScene()
	{
		return !isPaused && CanPauseInCurrentScene();
	}

	void RestartCurrentScene()
	{
		Time.timeScale = 1f;
		Scene escenaActiva = SceneManager.GetActiveScene();
		SceneManager.LoadScene(escenaActiva.name);
	}

	public void TogglePause()
	{
		isPaused = !isPaused;

		if (isPaused)
		{
			if (enterPauseSound != null)
				GameManager.Instance.audioManager.PlaySound(enterPauseSound);

			Time.timeScale = 0f;
			SceneManager.LoadScene(pauseSceneName, LoadSceneMode.Additive);
		}
		else
		{
			if (exitPauseSound != null)
				GameManager.Instance.audioManager.PlaySound(exitPauseSound);
			Time.timeScale = 1f;
			SceneManager.UnloadSceneAsync(pauseSceneName);
		}
	}

	public void QuitToMainMenu()
	{
		isPaused = false;
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu_00");
	}
}
