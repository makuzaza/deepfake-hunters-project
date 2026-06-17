// SpeakerStripView.cs — attach to SpeakerStrip
// Screens call Say() to set the speaker name + line at the bottom.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerStripView : MonoBehaviour
{
    [SerializeField] private TMP_Text speakerName;    // object SpeakerName
    [SerializeField] private TMP_Text speakerText;    // object SpeakerText
    [SerializeField] private Image    speakerAvatar;  // SpeakerAvatar

    public void Say(string speaker, string line)
    {
        if (speakerName) speakerName.text = speaker;
        if (speakerText) speakerText.text = line;
    }

    public void Clear()
    {
        if (speakerName) speakerName.text = "";
        if (speakerText) speakerText.text = "";
    }
}
