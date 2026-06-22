// TaskSO.cs — fixed version
// Keeps all new fields AND restores the old fields your existing TaskManager
// and SocialFeedManager depend on (clockMinutesToAdvance, consequences).
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Task Definition")]
public class TaskSO : ScriptableObject
{
    [Header("Display")]
    public string clientName;
    public string taskTitle;
    [TextArea(2,4)] public string description;
    public string companyTagline;
    public Sprite clientPortrait;

    [Header("Economy & Risk")]
    public int       pay;
    public RiskLevel riskLevel = RiskLevel.Low;
    public int       riskDelta = 10;

    [Header("Scene Integration")]
    [Tooltip("Exact scene name in Build Settings (your teammate's task scene)")]
    public string sceneName;

    [Header("Act")]
    public int act = 1;

    [Header("Narrative")]
    public string clientFeedback;
    public string dianePost;
    public string narratorNote;

    // ── Legacy fields used by your existing TaskManager / SocialFeedManager ──
    [Header("Legacy (keep for existing scripts)")]
    [Tooltip("How many in-game minutes to advance the clock after this task")]
    public int clockMinutesToAdvance = 30;

    [Tooltip("Feed posts / consequences that appear after this task is launched")]
    public List<FeedPostSO> consequences = new();
}
