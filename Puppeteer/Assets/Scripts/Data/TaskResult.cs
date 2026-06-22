// TaskResult.cs — plain data struct passed back from task scenes
[System.Serializable]
public struct TaskResult
{
    public bool   launched;
    public int    payEarned;
    public int    riskDelta;
    public string clientFeedback;   // shown in Result screen
    public string dianePost;        // shown in Phone screen
    public string narratorNote;     // italic note under phone (e.g. "Just a normal post.")
}
