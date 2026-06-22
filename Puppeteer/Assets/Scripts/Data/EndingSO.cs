// EndingSO.cs — fixed version with all fields
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Ending")]
public class EndingSO : ScriptableObject
{
    public EndingType endingType;
    public string     title;
    [TextArea(2,4)]
    public string     epilogueText;
    public string     condition;
    public string     dianeOutcome;
    public string     unlockCondition;
}
