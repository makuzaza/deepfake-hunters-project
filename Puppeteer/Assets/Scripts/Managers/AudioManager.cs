using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : Singleton<AudioManager>
{
    [SerializeField] private AudioSource sfxSource;

    protected override void Awake()
    {
        base.Awake();
        if (sfxSource == null)
        {
            sfxSource = gameObject.AddComponent<AudioSource>();
            sfxSource.playOnAwake  = false;
            sfxSource.spatialBlend = 0f;
        }
        EnforceOneListener();
    }

    private void OnEnable()
    {
        GameEvents.TaskLaunched      += OnLaunch;
        SceneManager.sceneLoaded     += OnSceneLoaded;
    }

    private void OnDisable()
    {
        GameEvents.TaskLaunched      -= OnLaunch;
        SceneManager.sceneLoaded     -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode) => EnforceOneListener();

    private static void EnforceOneListener()
    {
        var listeners = FindObjectsOfType<AudioListener>(false);
        bool kept = false;
        foreach (var l in listeners)
        {
            if (!kept) { kept = true; continue; }
            l.enabled = false;
        }
    }

    private void OnLaunch(TaskSO task) => PlayNotification();

    public void PlayNotification() { }

    public void PlaySFX(AudioClip clip, float volume = 1f)
    {
        if (clip != null && sfxSource != null)
            sfxSource.PlayOneShot(clip, volume);
    }
}
