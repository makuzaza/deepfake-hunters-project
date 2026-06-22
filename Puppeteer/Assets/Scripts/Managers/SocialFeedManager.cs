// SocialFeedManager.cs — FIXED.
// Was: spawned posts at task SELECTION (OnTaskChosen) and never bound their content.
// Now: spawns AFTER the task is finished (OnTaskFinished) and binds the text.
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
        // Show consequences only after the player actually launches the task.
        GameEvents.OnTaskFinished += HandleTaskFinished;
    }

    private void OnDisable()
    {
        GameEvents.OnTaskFinished -= HandleTaskFinished;
    }

    private void HandleTaskFinished(TaskResult r)
    {
        // TaskResult already carries the dianePost string from the TaskSO.
        // Spawn a single feed post from it (simplest reliable path for the demo).
        if (!string.IsNullOrEmpty(r.dianePost))
            SpawnPost("Diane", r.dianePost);
    }

    private void SpawnPost(string author, string body)
    {
        if (feedContent == null || postItemPrefab == null) return;
        var item = Instantiate(postItemPrefab, feedContent);
        item.Bind(author, body);          // PostItem has Bind(string, string)
        _spawned.Add(item);
    }

    public void ClearFeed()
    {
        foreach (var item in _spawned)
            if (item) Destroy(item.gameObject);
        _spawned.Clear();
    }
}