using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    public AudioSource musicSource;
    public AudioSource sfxSource;

    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;
    public AudioClip creepyAmbience;

    public AudioClip[] footstepSounds;
    public AudioClip objectMoveSound;
    public AudioClip wallSlideSound;
    public AudioClip stalkerSound;

    void Awake()
    {
        // Singleton pattern - keep one AudioManager across scenes
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Start with menu music
        PlayMusic(mainMenuMusic);
    }

    public void PlayMusic(AudioClip music)
    {
        if (musicSource != null && music != null)
        {
            musicSource.clip = music;
            musicSource.loop = true;
            musicSource.Play();
        }
    }

    public void PlaySFX(AudioClip sound)
    {
        if (sfxSource != null && sound != null)
        {
            sfxSource.PlayOneShot(sound);
        }
    }

    public void PlayRandomFootstep()
    {
        if (footstepSounds.Length > 0)
        {
            AudioClip step = footstepSounds[Random.Range(0, footstepSounds.Length)];
            sfxSource.PlayOneShot(step, 0.5f);
        }
    }

    // Call this when entering game
    public void SwitchToGameMusic()
    {
        PlayMusic(gameMusic);
    }
}