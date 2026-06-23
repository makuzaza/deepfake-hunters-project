// DashboardScreen.cs — attach to Screen_Dashboard GameObject under ContentArea
//
// UPDATED for day-by-day flow:
//   - Added SetItems() so GameFlowController can push the current day's inbox.
//   - The inspector `items` list is no longer the source of truth — clear it.
//     (If left populated it will simply be replaced when SetItems runs.)
using System.Collections.Generic;
using UnityEngine;

public class DashboardScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Transform        listContent;    // the VLG Content inside Viewport
    [SerializeField] private InfoCardView     cardPrefab;
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    [Header("Context for this screen")]
    [SerializeField] private string contextLabel   = "COMPANY";
    [SerializeField] private string contextName    = "Human Agency";
    [SerializeField] private string contextTagline = "Digital Marketing · Est. 2019";

    [Header("Inbox items (driven by GameFlowController per day — leave empty)")]
    [SerializeField] public List<InboxItemData> items = new();

    private readonly List<InfoCardView> _spawned = new();

    // Called by GameFlowController to set the current day's inbox, then rebuild.
    public void SetItems(List<InboxItemData> newItems)
    {
        items = newItems ?? new List<InboxItemData>();
        if (gameObject.activeInHierarchy) RebuildList();
    }

    protected override void OnBeforeShow()
    {
        if (contextPanel) contextPanel.Apply(contextLabel, null, contextName, contextTagline);
        if (speakerStrip) speakerStrip.Clear();
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