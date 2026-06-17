// TaskSceneController.cs — teammates add this to the ROOT of each task scene.
// Reference the SAME TaskChannelSO asset as MainGame's TaskLauncher.
// Wire their existing Launch button to OnLaunchPressed().
using UnityEngine;

public class TaskSceneController : MonoBehaviour
{
    [Header("Drag the shared TaskChannelSO asset here")]
    [SerializeField] private TaskChannelSO channel;

    private TaskSO _task;

    private void Start()
    {
        _task = channel.currentTask;
        // Teammates: use _task.clientName, _task.description, _task.pay etc.
        // to populate their task UI here.
    }

    // Wire to the Launch button in the task scene
    public void OnLaunchPressed()
    {
        channel.Complete(new TaskResult
        {
            launched       = true,
            payEarned      = _task != null ? _task.pay : 0,
            riskDelta      = _task != null ? _task.riskDelta : 10,
            clientFeedback = "This is awesome. Thank you for saving me time! 😊",
            dianePost      = "Diane (54) liked a Natural Remedies post.",
            narratorNote   = "Just a normal post. Nothing suspicious."
        });
    }
}
