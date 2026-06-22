// ProfileSO.cs — fixed version
// Uses Motivation (not ProfileType). Fields match what CreateContentAssets sets.
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Profile")]
public class ProfileSO : ScriptableObject
{
    public Motivation motivation;
    [TextArea(1,3)]
    public string exampleBossLine;
    [TextArea(1,3)]
    public string bossDialogueVariant;
}
