// ProfileSO.cs  -  Assets/_Project/Scripts/Data
using UnityEngine;
[CreateAssetMenu(fileName = "Profile_", menuName = "Puppeteer/Profile")]
public class ProfileSO : ScriptableObject
{
    public ProfileType type;
    public string displayName;
    [TextArea(2, 3)] public string description;
}
