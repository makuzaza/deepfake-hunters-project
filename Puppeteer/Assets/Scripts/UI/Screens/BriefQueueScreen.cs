using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BriefQueueScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Transform      cardsParent;
    [SerializeField] private TaskCardView   cardPrefab;
    [SerializeField] private Button         refuseButton;

    [Header("Tasks (drag TaskSO assets here)")]
    [SerializeField] public List<TaskSO> availableTasks = new();

    [Header("Context")]
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    private readonly List<TaskCardView>  _spawned    = new();
    private readonly HashSet<TaskSO>     _takenTasks = new();

    protected override void Awake()
    {
        base.Awake();
        refuseButton.onClick.AddListener(() => GameEvents.RefusedBoth());
    }

    protected override void OnBeforeShow()
    {
        if (contextPanel) contextPanel.Apply("MANAGER", null, "Boss", "Senior Account Director\n\"You came to the right place.\"");
        if (speakerStrip) speakerStrip.Say("BOSS", "Here's your queue for today. Pick what you want to work on.");
        RebuildCards();
    }

    private void RebuildCards()
    {
        foreach (var c in _spawned) if (c) Destroy(c.gameObject);
        _spawned.Clear();

        foreach (var task in availableTasks)
        {
            var card = Instantiate(cardPrefab, cardsParent);
            var t = task;
            card.Setup(t, () =>
            {
                _takenTasks.Add(t);
                GameEvents.TaskChosen(t);
            });

            if (_takenTasks.Contains(task))
                card.MarkDone();

            _spawned.Add(card);
        }
    }
}
