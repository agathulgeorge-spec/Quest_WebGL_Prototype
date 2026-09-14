using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager Instance;

    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioClip music;

    [Range(0f, 1f)]
    [SerializeField] private float volume = 0.5f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        musicSource.loop = true;
        musicSource.playOnAwake = false;
        musicSource.volume = volume;

        if (music != null)
        {
            musicSource.clip = music;
            musicSource.Play();
        }
    }
}