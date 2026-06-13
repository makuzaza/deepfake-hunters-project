// GameEvents.cs  -  Assets/_Project/Scripts/Core
// Static event bus. Raisers call Raise*; listeners subscribe in OnEnable / unsubscribe in OnDisable.
using System;
using UnityEngine;

public static class GameEvents
{
    public static event Action<TaskSO>     TaskLaunched;
    public static event Action<int>        ClockAdvanced;   // total diegetic minutes
    public static event Action<ActSO>      ActChanged;
    public static event Action<EndingType> EndingReached;
    public static event Action<int, int>   StatsChanged;    // money, reputation

    public static void RaiseTaskLaunched(TaskSO t)        => TaskLaunched?.Invoke(t);
    public static void RaiseClockAdvanced(int m)          => ClockAdvanced?.Invoke(m);
    public static void RaiseActChanged(ActSO a)           => ActChanged?.Invoke(a);
    public static void RaiseEndingReached(EndingType e)   => EndingReached?.Invoke(e);
    public static void RaiseStatsChanged(int money, int reputation) => StatsChanged?.Invoke(money, reputation);

#if UNITY_EDITOR
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
    private static void ResetStatics()
    {
        TaskLaunched = null; ClockAdvanced = null; ActChanged = null;
        EndingReached = null; StatsChanged = null;
    }
#endif
}
