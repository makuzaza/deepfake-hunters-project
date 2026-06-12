// TaskSO.cs  -  Assets/_Project/Scripts/Data
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Task_", menuName = "Puppeteer/Task")]
public class TaskSO : ScriptableObject
{
    [Header("Identity")] public string taskId = "task_id"; public TaskType type;
    [Header("Narrative")]
    public DialogueSO brief;
    public DialogueSO postLaunchBarks;
    public List<FeedPostSO> consequences = new List<FeedPostSO>();
    [Header("Flow")]
    public bool noncooperationEligible;
    public int clockMinutesToAdvance = 60;
    [Header("Type-specific config (only matching block is read)")]
    public string[] subtitleSegments;
    public string requiredText;
    public Sprite[] dragAssets;
    public string[] pipelineSteps;
}
