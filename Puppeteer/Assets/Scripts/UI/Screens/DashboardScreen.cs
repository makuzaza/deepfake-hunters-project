// DashboardScreen.cs — attach to a new Screen_Dashboard GameObject under ContentArea
// Serialized refs: listContent (the VLG Content), cardPrefab, contextPanel, items list
using System.Collections.Generic;
using UnityEngine;

public class DashboardScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Transform      listContent;    // the VLG Content inside Viewport
    [SerializeField] private InfoCardView   cardPrefab;
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    [Header("Context for this screen")]
    [SerializeField] private string contextLabel  = "COMPANY";
    [SerializeField] private string contextName   = "Human Agency";
    [SerializeField] private string contextTagline = "Digital Marketing · Est. 2019";

    [Header("Inbox items (set per day or populate from GameFlowController)")]
    [SerializeField] public List<InboxItemData> items = new();

    private readonly List<InfoCardView> _spawned = new();

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
