// TaskChannelSO.cs — the bridge between MainGame and teammate task scenes.
// Create ONE asset. Drag the SAME asset into both MainGame (TaskLauncher)
// and the task scene root (TaskSceneController).
using System;
using UnityEngine;
[CreateAssetMenu(menuName = "Puppeteer/Task Channel")]
public class TaskChannelSO : ScriptableObject
{
    [HideInInspector] public TaskSO currentTask;
    public event Action<TaskResult> OnCompleted;
    public void Complete(TaskResult r) => OnCompleted?.Invoke(r);
}
