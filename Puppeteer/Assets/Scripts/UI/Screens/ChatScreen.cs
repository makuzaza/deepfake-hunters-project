// ChatScreen.cs — attach to Screen_Chat under ContentArea
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChatScreen : UIScreen
{
    [Header("Chat Content")]
    [SerializeField] private TMP_Text   messageA;    // "Hey Alex! I'm Marcus..."
    [SerializeField] private TMP_Text   messageB;    // "Want me to walk you through..."

    [Header("Reply Buttons")]
    [SerializeField] private Button replyWalkThrough;  // "Walk me through it."
    [SerializeField] private Button replyFigureOut;    // "I'll figure it out."

    [Header("Context")]
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;
    [SerializeField] private Sprite marcusPortrait;

    protected override void Awake()
    {
        base.Awake();
        replyWalkThrough.onClick.AddListener(() => GameEvents.ChatReplied());
        replyFigureOut.onClick.AddListener(()   => GameEvents.ChatReplied());
    }

    protected override void OnBeforeShow()
    {
        if (contextPanel) contextPanel.Apply("COLLEAGUE", marcusPortrait, "Marcus", "Junior Account Manager\nJoined 3 months ago");
        if (speakerStrip) speakerStrip.Say("MARCUS", "Hey! I'm Marcus. Looks like we are on the same team.");
    }
}