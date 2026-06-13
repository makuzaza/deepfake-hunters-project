// SocialFeedManager.cs  -  Assets/_Project/Scripts/Managers
// Listens for TaskLaunched and spawns consequence posts into the feed scroll view.
using UnityEngine;
public class SocialFeedManager : Singleton<SocialFeedManager>
{
    public Transform feedContent;     // assigned by build tool (FeedContent)
    public PostItem postItemPrefab;   // assigned by build tool

    protected override void Awake()
    {
        base.Awake();
        if (feedContent == null)
        {
            var canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null) { var t = UIFind.Deep(canvas.transform, UINames.FeedContent); if (t) feedContent = t; }
        }
    }

    private void OnEnable()  => GameEvents.TaskLaunched += OnLaunch;
    private void OnDisable() => GameEvents.TaskLaunched -= OnLaunch;

    private void OnLaunch(TaskSO task)
    {
        if (task == null || task.consequences == null) return;
        foreach (var p in task.consequences) Spawn(p);
    }

    private void Spawn(FeedPostSO post)
    {
        if (postItemPrefab == null || feedContent == null || post == null) return;
        var item = Instantiate(postItemPrefab, feedContent);
        item.Bind(post);
        UIManager.I?.UnlockFeed();
    }

    // Used by the debug panel to prove the feed works without authored content.
    public void SpawnPlaceholderPost()
    {
        if (postItemPrefab == null || feedContent == null) return;
        var item = Instantiate(postItemPrefab, feedContent);
        item.Bind("Diane (54)", "A doctor online said it's fine to stop my prescription. I trust her.");
        UIManager.I?.UnlockFeed();
    }
}
