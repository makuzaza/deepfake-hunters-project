// ProfileManager.cs — fixed version
// ProfileType → Motivation everywhere. No more cast errors.
using UnityEngine;

public class ProfileManager : MonoBehaviour
{
    public static ProfileManager Instance { get; private set; }

    private Motivation _currentMotivation = Motivation.Money;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
    }

    private void OnEnable()
    {
        GameEvents.OnMotivationSet += SetMotivation;
    }

    private void OnDisable()
    {
        GameEvents.OnMotivationSet -= SetMotivation;
    }

    public void SetMotivation(Motivation m)
    {
        _currentMotivation = m;
        Debug.Log($"[ProfileManager] Motivation set to: {m}");
    }

    public Motivation GetMotivation() => _currentMotivation;

    // Legacy helper in case old code calls SetProfile(ProfileType)
    // ProfileType is now just an alias for Motivation (same int values)
    public void SetProfile(Motivation type) => SetMotivation(type);
    public Motivation GetProfile()          => _currentMotivation;
}
