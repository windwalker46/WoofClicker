using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance; 

    public AudioSource backgroundMusicSource; // Background music
    public AudioSource sfxSource; // Sound effects

    public AudioClip buttonTapClip;
    public AudioClip powerUpClip;
    public AudioClip levelUpClip;
    public AudioClip gameOverClip;
    public AudioClip winClip;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scenes
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }

    public void PlayBackgroundMusic()
    {
        if (backgroundMusicSource != null && !backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.loop = true;
            backgroundMusicSource.Play();
        }
    }

    public void StopBackgroundMusic()
    {
        if (backgroundMusicSource != null && backgroundMusicSource.isPlaying)
        {
            backgroundMusicSource.Stop();
        }
    }
}
