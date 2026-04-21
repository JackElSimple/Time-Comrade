using UnityEngine;

public class AudioManager : MonoBehaviour
{
    [SerializeField] private AudioSource musicSource, sfxSource;
    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.isPlaying && musicSource.clip == clip)
            return; // do not restart the song if it is already playing
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }
    public void StopMusic()
    {
        musicSource.Stop();
    }
    public void PlaySound(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }
}