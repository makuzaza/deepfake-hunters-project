// SocialFeedManager.cs — fixed version
// Removes UIFind/UINames. Uses serialized reference for the feed container.
// Subscribes to OnTaskChosen (was: TaskLaunched) to show consequences.
using System.Collections.Generic;
using UnityEngine;

public class SocialFeedManager : MonoBehaviour
{
    [Header("Drag the feed scroll Content transform here")]
    [SerializeField] private Transform feedContent;

    [Header("Post item prefab")]
    [SerializeField] private PostItem postItemPrefab;

    private readonly List<PostItem> _spawned = new();

    private void OnEnable()
    {
        GameEvents.OnTaskChosen += HandleTaskChosen;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskChosen -= HandleTaskChosen;
    }

    private void HandleTaskChosen(TaskSO task)
    {
        if (task == null || task.consequences == null) return;
        foreach (var post in task.consequences)
            SpawnPost(post);
    }

    private void SpawnPost(FeedPostSO post)
    {
        if (feedContent == null || postItemPrefab == null || post == null) return;
        var item = Instantiate(postItemPrefab, feedContent);
        // PostItem.Setup() — call whatever method your PostItem uses
        // item.Setup(post);
        _spawned.Add(item);
    }

    public void ClearFeed()
    {
        foreach (var item in _spawned)
            if (item) Destroy(item.gameObject);
        _spawned.Clear();
    }
}
