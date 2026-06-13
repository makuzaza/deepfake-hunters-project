// CharacterSO.cs  -  Assets/_Project/Scripts/Data
using UnityEngine;
[CreateAssetMenu(fileName = "Char_", menuName = "Puppeteer/Character")]
public class CharacterSO : ScriptableObject
{
    public string displayName = "New Character";
    [Tooltip("360x360 portrait. Defaults to placeholder silhouette.")]
    public Sprite portrait;
}
