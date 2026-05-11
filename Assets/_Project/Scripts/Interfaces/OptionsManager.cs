using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;

public class OptionsManager : MonoBehaviour
{
    public GameObject optionsPanel;
    public CanvasGroup brightnessOverlay;
    public Slider brightnessSlider;
    public AudioMixer audioMixer;
    public Slider musicSlider;
    public GameObject soundOn;
    public GameObject soundOff;

    void Start()
    {
        // Cargar valores guardados (o usar los de por defecto)
        float savedBright = PlayerPrefs.GetFloat("Brightness", 1f);
        float savedMusic = PlayerPrefs.GetFloat("MusicVolume", 0.7f);

        brightnessSlider.value = savedBright;
        musicSlider.value = savedMusic;

        SetBrightness(savedBright);
        SetMusicVolume(savedMusic);

        // Escuchar cambios en los sliders
        brightnessSlider.onValueChanged.AddListener(SetBrightness);
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    public void OpenOptions()
    {
        optionsPanel.SetActive(true);
    }

    public void CloseOptions()
    {
        optionsPanel.SetActive(false);
        PlayerPrefs.Save(); // guarda en disco
    }

    void SetBrightness(float value)
    {
        // 0 = muy oscuro, 1 = brillo normal
        brightnessOverlay.alpha = 1f - value;
        PlayerPrefs.SetFloat("Brightness", value);
    }

    void SetMusicVolume(float value)
    {
        // Convierte lineal → logarítmico (así suena natural)
        float db = value > 0.001f
            ? Mathf.Log10(value) * 20f
            : -80f;
        audioMixer.SetFloat("MusicVolume", db);
        PlayerPrefs.SetFloat("MusicVolume", value);
        if (value < 0.01)
        {
            soundOn.SetActive(false);
            soundOff.SetActive(true);
        }
        else
        {
            soundOn.SetActive(true);
            soundOff.SetActive(false);
        }
    }
}