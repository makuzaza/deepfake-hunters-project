// ScreenRouter.cs — compatibility adapter.
// Canonical screen orchestration now lives in ScreenManager.
using UnityEngine;

public class ScreenRouter : MonoBehaviour
{
    [SerializeField] private ScreenManager screenManager;

    private void Awake()
    {
        if (screenManager == null)
            screenManager = GetComponent<ScreenManager>();

        if (screenManager == null)
            Debug.LogWarning("ScreenRouter has no ScreenManager reference. Route calls will be ignored.");
    }

    public void Show(ScreenId id)
    {
        screenManager?.Show(id);
    }

    public T Get<T>(ScreenId id) where T : UIScreen
        => screenManager != null ? screenManager.Get<T>(id) : null;
}
