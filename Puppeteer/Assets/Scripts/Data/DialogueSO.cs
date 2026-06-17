// DialogueSO.cs — fixed version
// Removes all ProfileType references. Uses Motivation directly.
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Dialogue")]
public class DialogueSO : ScriptableObject
{
    [System.Serializable]
    public class DialogueLine
    {
        public string    speakerName;
        [TextArea(2, 4)]
        public string    line;
        public Motivation targetMotivation = Motivation.Money; // which profile sees this line
        public bool      showForAllProfiles = true;            // if true, ignore targetMotivation
    }

    public List<DialogueLine> lines = new();

    // Returns lines appropriate for the given motivation profile
    public List<DialogueLine> GetLinesFor(Motivation motivation)
    {
        var result = new List<DialogueLine>();
        foreach (var l in lines)
            if (l.showForAllProfiles || l.targetMotivation == motivation)
                result.Add(l);
        return result;
    }
}
