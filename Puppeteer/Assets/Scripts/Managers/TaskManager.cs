// TaskManager.cs  -  Assets/_Project/Scripts/Managers
// STUB. Day 3 (Lead): RunTask(TaskSO) phase coroutine + GetWidget + 15s noncoop
// timer (Architecture S5). SimulateLaunch lets the debug panel exercise the bus.
using UnityEngine;
public class TaskManager : Singleton<TaskManager>
{
    public void SimulateLaunch(TaskSO task)
    {
        GameEvents.RaiseTaskLaunched(task);
        GameManager.I?.AdvanceClock(task != null ? task.clockMinutesToAdvance : 60);
        GameManager.I?.AddMoney(500);
    }
}
