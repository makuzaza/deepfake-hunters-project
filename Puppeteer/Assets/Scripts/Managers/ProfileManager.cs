// ProfileManager.cs  -  Assets/_Project/Scripts/Managers
using UnityEngine;
public class ProfileManager : Singleton<ProfileManager>
{
    [SerializeField] private ProfileType current = ProfileType.Money;
    public ProfileType Current => current;
    public void Set(ProfileType p) { current = p; UIManager.I?.ShowNotification("Profile set: " + p); }
    public string Resolve(DialogueLine line) => line != null ? line.ResolveText(current) : string.Empty;
}
