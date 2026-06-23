// BriefQueueScreen.cs — attach to Screen_BriefQueue under ContentArea
//
// UPDATED for one-task-per-day flow:
//   - availableTasks holds ALL tasks in day order: [Task1, Task2, Task3, Task4].
//   - RebuildCards() now shows ONLY the task for the current day
//     (day 1 -> index 0, day 2 -> index 1, ...).
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BriefQueueScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Transform      cardsParent;   // horizontal or vertical layout parent
    [SerializeField] private TaskCardView   cardPrefab;
    [SerializeField] private Button         refuseButton;

    [Header("Player state (drag PlayerState.asset)")]
    [SerializeField] private PlayerStateSO  playerState;

    [Header("Tasks in DAY ORDER (Task1, Task2, Task3, Task4)")]
    [SerializeField] public List<TaskSO> availableTasks = new();

    [Header("Context")]
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    private readonly List<TaskCardView> _spawned = new();

    protected override void Awake()
    {
        base.Awake();
        if (refuseButton) refuseButton.onClick.AddListener(() => GameEvents.RefusedBoth());
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

        if (playerState == null || availableTasks == null) return;

        int dayIndex = playerState.day - 1;   // day 1 -> index 0
        if (dayIndex < 0 || dayIndex >= availableTasks.Count) return;

        var task = availableTasks[dayIndex];
        if (task == null) return;

        var card = Instantiate(cardPrefab, cardsParent);
        card.Setup(task, () => GameEvents.TaskChosen(task));
        _spawned.Add(card);
    }
}