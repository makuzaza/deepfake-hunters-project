using UnityEngine;

public class AmbientTyping : MonoBehaviour
{
    [SerializeField] private AudioClip typingClip;
    [SerializeField] private float duration = 18f;

    private void Start()
    {
        if (typingClip == null) return;
        var src = gameObject.AddComponent<AudioSource>();
        src.clip        = typingClip;
        src.loop        = true;
        src.playOnAwake = false;
        src.Play();
        Invoke(nameof(StopTyping), duration);
    }

    private void StopTyping() => GetComponent<AudioSource>()?.Stop();
}
