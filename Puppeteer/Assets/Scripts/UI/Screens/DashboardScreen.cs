// DashboardScreen.cs — attach to Screen_Dashboard under ContentArea
// CHANGE: speaker strip now shows a per-day welcome line (HR on day 1, Boss after)
// instead of being cleared/blank.
using System.Collections.Generic;
using UnityEngine;

public class DashboardScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Transform        listContent;   // the VLG Content inside Viewport
    [SerializeField] private InfoCardView     cardPrefab;
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    [Header("Player state (drag PlayerState.asset) — for per-day speaker line")]
    [SerializeField] private PlayerStateSO playerState;

    [Header("Context for this screen")]
    [SerializeField] private string contextLabel   = "COMPANY";
    [SerializeField] private string contextName    = "Human Agency";
    [SerializeField] private string contextTagline = "Digital Marketing · Est. 2019";

    [Header("Inbox items (populated from GameFlowController)")]
    [SerializeField] public List<InboxItemData> items = new();

    private readonly List<InfoCardView> _spawned = new();

    protected override void OnBeforeShow()
    {
        if (contextPanel) contextPanel.Apply(contextLabel, null, contextName, contextTagline);

        // Per-day speaker strip — HR welcomes on day 1, Boss greets afterward.
        if (speakerStrip)
        {
            int day = playerState != null ? playerState.day : 1;
            string name = playerState != null ? playerState.playerName : "there";

            switch (day)
            {
                case 1:
                    speakerStrip.Say("HR DAISY",
                        "Hey! Welcome to your first day.");
                    break;
                case 2:
                    speakerStrip.Say("BOSS",
                        "Welcome to day two, " + name + ". Tony asked for you specifically — take a look at your task.");
                    break;
                case 3:
                    speakerStrip.Say("BOSS",
                        "Day three already, " + name + ". There's something in the news, but your task comes first.");
                    break;
                case 4:
                    speakerStrip.Say("BOSS",
                        "Last day, " + name + ". One final task — make it count.");
                    break;
                default:
                    speakerStrip.Say("BOSS", "Good morning, " + name + ".");
                    break;
            }
        }

        RebuildList();
    }

    public void RebuildList()
    {
        foreach (var c in _spawned) if (c) Destroy(c.gameObject);
        _spawned.Clear();
        foreach (var item in items)
        {
            var card = Instantiate(cardPrefab, listContent);
            var captured = item;
            card.Setup(captured, () => GameEvents.InboxSelected(captured.action));
            _spawned.Add(card);
        }
    }
}