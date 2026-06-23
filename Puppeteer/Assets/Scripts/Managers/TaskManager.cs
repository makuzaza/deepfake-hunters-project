// TaskManager.cs  -  Assets/_Project/Scripts/Managers
// STUB. Day 3 (Lead): RunTask(TaskSO) phase coroutine + GetWidget + 15s noncoop
// timer (Architecture S5). SimulateLaunch lets the debug panel exercise the bus.
using UnityEngine;
public class TaskManager : Singleton<TaskManager>
{
    [SerializeField] private GameManager gameManager;

    public void SimulateLaunch(TaskSO task)
    {
        GameEvents.RaiseTaskLaunched(task);
        if (gameManager == null)
        {
            Debug.LogWarning("TaskManager missing GameManager reference.");
            return;
        }

        gameManager.AdvanceClock(task != null ? task.clockMinutesToAdvance : 60);
        gameManager.AddMoney(500);
    }
}
