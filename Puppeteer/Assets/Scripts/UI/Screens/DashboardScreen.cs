// DashboardScreen.cs — attach to Screen_Dashboard under ContentArea
// CHANGE: per-day speaker line now keys off playerState.tasksCompleted, which only
// increments when a task is genuinely finished. This is more reliable than `day`,
// which can stay at 1 if you return to the dashboard via the Home button (Home does
// not advance the day). Also logs the values so you can verify in the Console.
using System.Collections.Generic;
using UnityEngine;

public class DashboardScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Transform        listContent;
    [SerializeField] private InfoCardView     cardPrefab;
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    [Header("Player state (drag PlayerState.asset)")]
    [SerializeField] private PlayerStateSO playerState;

    [Header("Context for this screen")]
    [SerializeField] private string contextLabel   = "COMPANY";
    [SerializeField] private string contextName    = "Human Agency";
    [SerializeField] private string contextTagline = "Digital Marketing · Est. 2019";

    [Header("Company logo — drag Human Agency sprite here")]
    [SerializeField] private Sprite companyLogo;
    [SerializeField] private Sprite daisyPortrait;
    [SerializeField] private Sprite bossPortrait;

    [Header("Inbox items (populated from GameFlowController)")]
    [SerializeField] public List<InboxItemData> items = new();

    private readonly List<InfoCardView> _spawned = new();

    protected override void OnBeforeShow()
    {
        if (contextPanel) contextPanel.Apply(contextLabel, companyLogo, contextName, contextTagline);

        if (speakerStrip)
        {
            int done = playerState != null ? playerState.tasksCompleted : 0;
            int day  = playerState != null ? playerState.day : 1;
            string name = playerState != null ? playerState.playerName : "there";

            // DEBUG — remove once confirmed. Shows the real values in the Console
            // every time the dashboard opens, so you can see whether they advance.
            Debug.Log($"[Dashboard] day={day}  tasksCompleted={done}");

            // Greeting keyed off how many tasks are DONE (reliable signal).
            switch (done)
            {
                case 0:
                    speakerStrip.Say("HR DAISY",
                        "Hey! Welcome to your first day.");
                    break;
                case 1:
                    speakerStrip.Say("BOSS",
                        "Welcome to day two, " + name + ". Tony asked for you specifically — take your next task.");
                    break;
                case 2:
                    speakerStrip.Say("BOSS",
                        "Day three, " + name + ". There's something in the news, but your task comes first.");
                    break;
                case 3:
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