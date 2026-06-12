// EndingSO.cs  -  Assets/_Project/Scripts/Data
using UnityEngine;
[CreateAssetMenu(fileName = "Ending_", menuName = "Puppeteer/Ending")]
public class EndingSO : ScriptableObject
{
    public EndingType type;
    [TextArea(3, 6)] public string epilogueText;
    public Sprite background;
}
