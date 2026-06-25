using UnityEngine;

public class MainMenuMusic : MonoBehaviour
{
    [SerializeField] private AudioClip bgmClip;
    [Range(0f, 1f)]
    [SerializeField] private float volume = 1f;

    private void Awake()
    {
        if (bgmClip == null) return;
        var src = gameObject.AddComponent<AudioSource>();
        src.clip = bgmClip;
        src.loop = true;
        src.spatialBlend = 0f;
        src.volume = volume;
        src.Play();
    }
}
