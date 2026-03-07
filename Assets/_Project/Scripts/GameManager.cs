using UnityEngine;

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }

	private void Awake()
	{
		if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}

        // Capamos los FPS de renderizado
        Application.targetFrameRate = 60;

        // Quitamos el VSync (si está activo, ignorará el targetFrameRate)
        QualitySettings.vSyncCount = 0;
    }
}