// TaskSceneController.cs — FIXED for Łódź build.
// Add to the ROOT of each task scene (MG1 root, MiniGame2 MinigameCanvas, MG3 root).
// Reference the SAME TaskChannelSO asset (Assets/Data/Content/TaskChannel.asset).
// Wire the scene's Launch / Confirm / Complete / Skip buttons to OnLaunchPressed().
using UnityEngine;

public class TaskSceneController : MonoBehaviour
{
    [Header("Drag the shared TaskChannelSO asset here")]
    [SerializeField] private TaskChannelSO channel;

    [HideInInspector] public int payOverride = -1;  // set by minigames for proportional pay

    private TaskSO _task;
    private bool _completed;   // guard: OnLaunchPressed can be wired to several buttons

    private void Start()
    {
        // SAFETY: a previous task scene (MiniGame2) may have left timeScale at 0.
        // Always restore normal time when any task scene loads.
        Time.timeScale = 1f;

        _task = channel != null ? channel.currentTask : null;
        // Teammates: use _task.clientName, _task.description, _task.pay etc.
        // to populate their task UI here.
    }

    public int GetTaskPay() => _task != null ? _task.pay : 0;

    // Wire to the Launch / Confirm / Complete / Skip button(s) in the task scene.
    // Safe to wire to more than one button — only the first press counts.
    public void OnLaunchPressed()
    {
        if (_completed) return;          // ignore double-fire (Confirm + Complete both wired)
        if (channel == null) { Debug.LogError("[TaskSceneController] channel not assigned!"); return; }
        _completed = true;

        Time.timeScale = 1f;             // belt-and-braces in case minigame paused time

        channel.Complete(new TaskResult
        {
            launched       = true,
            payEarned      = payOverride >= 0 ? payOverride : (_task != null ? _task.pay : 0),
            riskDelta      = _task != null ? _task.riskDelta : 10,
            // USE THE PER-TASK STRINGS so every task reads differently.
            clientFeedback = !string.IsNullOrEmpty(_task?.clientFeedback)
                             ? _task.clientFeedback
                             : "Thanks — that's exactly what I needed!",
            dianePost      = !string.IsNullOrEmpty(_task?.dianePost)
                             ? _task.dianePost
                             : "Diane (54) liked a Natural Remedies post.",
            narratorNote   = !string.IsNullOrEmpty(_task?.narratorNote)
                             ? _task.narratorNote
                             : "Just a normal post. Nothing suspicious."
        });
    }
}