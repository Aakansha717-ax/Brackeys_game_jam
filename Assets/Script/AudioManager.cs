using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources - Will be auto-created")]
    public AudioSource musicSource;
    public AudioSource sfxSource;

    [Header("Music Clips")]
    public AudioClip mainMenuMusic;
    public AudioClip gameMusic;
    public AudioClip creepyAmbience;

    [Header("SFX Clips")]
    public AudioClip[] footstepSounds;
    public AudioClip objectMoveSound;
    public AudioClip wallSlideSound;
    public AudioClip stalkerSound;
    public AudioClip deathSound;
    public AudioClip transitionSound;

    [Header("Volume Settings")]
    [Range(0, 2)] public float musicVolume = 1f;
    [Range(0, 2)] public float sfxVolume = 1.2f;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // AUTO-CREATE audio sources if not assigned
            CreateAudioSources();
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void CreateAudioSources()
    {
        // Create music source if not assigned
        if (musicSource == null)
        {
            musicSource = gameObject.AddComponent<AudioSource>();
            musicSource.loop = true;
            musicSource.playOnAwake = false;
            musicSource.volume = musicVolume;
            Debug.Log("Auto-created musicSource");
        }

        // Create SFX source if not assigned
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.loop = false;
            sfxSource.playOnAwake = false;
            sfxSource.volume = sfxVolume;
            Debug.Log("Auto-created sfxSource");
        }
    }

    void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        // DIAGNOSTIC
        Debug.Log("=== AUDIO MANAGER START ===");
        Debug.Log($"Music Source exists: {musicSource != null}");
        Debug.Log($"SFX Source exists: {sfxSource != null}");
        Debug.Log($"MainMenu Music assigned: {mainMenuMusic != null}");

        if (mainMenuMusic == null)
        {
            Debug.LogError("❌ CRITICAL: mainMenuMusic is not assigned in Inspector!");
            return;
        }

        // FORCE PLAY
        musicSource.clip = mainMenuMusic;
        musicSource.loop = true;
        musicSource.volume = musicVolume;
        musicSource.Play();

        Debug.Log($"🎵 NOW PLAYING: {mainMenuMusic.name}");
        Debug.Log($"Music Source is playing: {musicSource.isPlaying}");
        Debug.Log("=== AUDIO MANAGER END ===");
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"Scene loaded: {scene.name}");
        UpdateMusicForScene(scene.name);
    }

    void UpdateMusicForScene(string sceneName)
    {
        if (sceneName == "MainMenu")
            PlayMusic(mainMenuMusic);
        else if (sceneName == "Room_1" || sceneName == "Room_2")
            PlayMusic(gameMusic);
        else if (sceneName == "Credit")
            PlayMusic(creepyAmbience);
    }

    public void PlayMusic(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("No music clip assigned!");
            return;
        }

        if (musicSource == null)
        {
            Debug.LogError("musicSource is NULL!");
            return;
        }

        musicSource.clip = clip;
        musicSource.Play();
        Debug.Log($"Playing music: {clip.name}");
    }

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null)
        {
            Debug.LogWarning("No SFX clip assigned!");
            return;
        }

        if (sfxSource == null)
        {
            Debug.LogError("sfxSource is NULL!");
            return;
        }

        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void PlayRandomFootstep()
    {
        if (footstepSounds == null || footstepSounds.Length == 0)
        {
            Debug.LogWarning("No footstep sounds!");
            return;
        }

        int index = Random.Range(0, footstepSounds.Length);
        PlaySFX(footstepSounds[index]);
    }

    // Call this from other scripts
    public static void PlaySound(AudioClip clip)
    {
        if (Instance != null)
            Instance.PlaySFX(clip);
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}