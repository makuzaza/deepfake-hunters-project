// CharacterSO.cs — fixed version
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Character")]
public class CharacterSO : ScriptableObject
{
    public string displayName;
    public string role;
    public string contextLabel;   // e.g. "MANAGER", "HR", "COLLEAGUE"
    public string tagline;
    public string avatarLetter;
    public Sprite portrait;
}
