// ScreenRouter.cs — fixed version
// Removes the UIFX reference (UIFX.cs is in your project but might be in
// a different namespace or not yet imported). Uses a self-contained
// CanvasGroup fade instead — functionally identical.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenRouter : MonoBehaviour
{
    public static ScreenRouter Instance { get; private set; }

    [SerializeField] private List<UIScreen> screens = new();
    [SerializeField] private GameObject     deskRoot;  // Frame — hidden during full-screen overlays

    private readonly Dictionary<ScreenId, UIScreen> _map = new();
    private UIScreen _current;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        foreach (var s in screens)
            if (s != null) _map[s.Id] = s;
    }

    public void Show(ScreenId id)
    {
        if (!_map.TryGetValue(id, out UIScreen next))
        {
            Debug.LogWarning($"[ScreenRouter] No screen registered for: {id}");
            return;
        }
        if (next == _current) return;

        if (deskRoot) deskRoot.SetActive(!next.FullScreen);

        if (_current != null) _current.Hide();
        _current = next;
        next.Show();
    }

    public T Get<T>(ScreenId id) where T : UIScreen
        => _map.TryGetValue(id, out var s) ? s as T : null;
}
