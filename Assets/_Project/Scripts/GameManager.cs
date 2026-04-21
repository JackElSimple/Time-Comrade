using UnityEngine;

[RequireComponent(typeof(AudioManager))]

public class GameManager : MonoBehaviour
{
	public static GameManager Instance { get; private set; }
    [HideInInspector] public AudioManager audioManager;

    private void Awake()
	{
        audioManager = GetComponent<AudioManager>();
        if (Instance != null && Instance != this)
		{
			Destroy(gameObject);
		}
		else
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
}