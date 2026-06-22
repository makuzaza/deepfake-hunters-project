// TaskLauncher.cs — attach to Managers GameObject
// Loads teammate task scenes additively; bridges back via TaskChannelSO.
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TaskLauncher : MonoBehaviour
{
    [Header("The shared channel — drag SAME asset into task scenes too")]
    [SerializeField] private TaskChannelSO channel;

    [Header("Desk root — hidden while task runs")]
    [SerializeField] private GameObject deskRoot;

    private string _loadedScene;

    private void OnEnable()  { channel.OnCompleted += HandleCompleted; }
    private void OnDisable() { channel.OnCompleted -= HandleCompleted; }

    public void Launch(TaskSO task)
    {
        channel.currentTask = task;
        _loadedScene = task.sceneName;
        if (deskRoot) deskRoot.SetActive(false);
        SceneManager.LoadSceneAsync(task.sceneName, LoadSceneMode.Additive);
    }

    private void HandleCompleted(TaskResult r)
    {
        if (!string.IsNullOrEmpty(_loadedScene))
            SceneManager.UnloadSceneAsync(_loadedScene);
        if (deskRoot) deskRoot.SetActive(true);
        GameEvents.TaskFinished(r);
    }
}
