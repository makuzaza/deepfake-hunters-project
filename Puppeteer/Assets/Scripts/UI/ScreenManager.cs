// ScreenManager.cs — attach to Managers GameObject.
// Assign deskRoot + drag every UIScreen subclass into the screens list.
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    [Tooltip("The Frame/Screen shell (the persistent desk). Hidden during full-screen overlays.")]
    [SerializeField] private GameObject deskRoot;

    [Tooltip("Drag every UIScreen panel here (Login, Portrait, Dashboard, etc.)")]
    [SerializeField] private List<UIScreen> screens = new();

    private readonly Dictionary<ScreenId, UIScreen> _map = new();
    private UIScreen _current;

    private void Awake()
    {
        foreach (var s in screens)
            if (s != null) _map[s.Id] = s;
    }

    public void Show(ScreenId id)
    {
        if (!_map.TryGetValue(id, out UIScreen next)) { Debug.LogWarning($"ScreenManager: no screen for {id}"); return; }
        if (next == _current) return;

        // Show or hide the desk shell based on whether this is a full-screen overlay
        if (deskRoot) deskRoot.SetActive(!next.FullScreen);

        if (_current != null) _current.Hide();
        _current = next;
        next.Show();
    }

    public T Get<T>(ScreenId id) where T : UIScreen
        => _map.TryGetValue(id, out var s) ? s as T : null;
}
