// TaskSceneController.cs — add to the ROOT of each task scene.
// Reference the SAME TaskChannelSO asset (Assets/Data/Content/TaskChannel.asset).
//
// TWO return paths:
//   OnLaunchPressed()    — player SUCCEEDED → success feedback + full pay
//   OnFailedTakeover()   — player FAILED, Marcus took over → fail feedback + no pay
using UnityEngine;

public class TaskSceneController : MonoBehaviour
{
    [Header("Drag the shared TaskChannelSO asset here")]
    [SerializeField] private TaskChannelSO channel;

    [HideInInspector] public int payOverride = -1;

    private TaskSO _task;
    private bool _completed;

    private void Start()
    {
        Time.timeScale = 1f;
        _task = channel != null ? channel.currentTask : null;
    }

    public int GetTaskPay() => _task != null ? _task.pay : 0;

    // ── SUCCESS ───────────────────────────────────────────────────────────
    public void OnLaunchPressed()
    {
        if (_completed) return;
        if (channel == null) { Debug.LogError("[TaskSceneController] channel not assigned!"); return; }
        _completed = true;
        Time.timeScale = 1f;

        channel.Complete(new TaskResult
        {
            launched       = true,
            payEarned      = payOverride >= 0 ? payOverride : (_task != null ? _task.pay : 0),
            riskDelta      = _task != null ? _task.riskDelta : 10,
            clientFeedback = !string.IsNullOrEmpty(_task?.clientFeedback)
                             ? _task.clientFeedback
                             : "Thanks — that's exactly what I needed!",
            bossLine       = "Great work today, {name}. You're a natural.",
            dianePost      = !string.IsNullOrEmpty(_task?.dianePost)
                             ? _task.dianePost
                             : "Diane (54) liked a Natural Remedies post.",
            narratorNote   = !string.IsNullOrEmpty(_task?.narratorNote)
                             ? _task.narratorNote
                             : "Just a normal post. Nothing suspicious."
        });
    }

    // ── FAIL (Marcus took over) ───────────────────────────────────────────
    public void OnFailedTakeover()
    {
        if (_completed) return;
        if (channel == null) { Debug.LogError("[TaskSceneController] channel not assigned!"); return; }
        _completed = true;
        Time.timeScale = 1f;

        channel.Complete(new TaskResult
        {
            launched       = false,
            payEarned      = 0,
            riskDelta      = _task != null ? _task.riskDelta : 10,
            clientFeedback = "Marcus stepped in and finished it. The client never knew.",
            bossLine       = "You let this one slip, {name}. Marcus had to cover for you.",
            dianePost      = !string.IsNullOrEmpty(_task?.dianePost)
                             ? _task.dianePost
                             : "Diane (54) liked a Natural Remedies post.",
            narratorNote   = "You didn't finish this one. Someone else did."
        });
    }
}