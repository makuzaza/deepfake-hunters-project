using UnityEngine;
using UnityEngine.InputSystem;

public class GlobalClickAudio : MonoBehaviour
{
    private static GlobalClickAudio _instance;
    private AudioSource _src;
    private AudioClip   _click;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void Init()
    {
        if (_instance != null) return;
        var go = new GameObject("[GlobalClickAudio]");
        DontDestroyOnLoad(go);
        go.AddComponent<GlobalClickAudio>();
    }

    private void Awake()
    {
        if (_instance != null && _instance != this) { Destroy(gameObject); return; }
        _instance = this;

        _src              = gameObject.AddComponent<AudioSource>();
        _src.spatialBlend = 0f;
        _src.playOnAwake  = false;

        _click = Resources.Load<AudioClip>("Audio/UI/mouse_click");
    }

    private void Update()
    {
        if (Mouse.current != null && Mouse.current.leftButton.wasPressedThisFrame && _click != null)
            _src.PlayOneShot(_click, 0.8f);
    }
}
