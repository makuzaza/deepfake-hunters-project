// GameEvents.cs — FINAL. Matches exact call signatures from your actual scripts.
// GameManager calls:  RaiseClockAdvanced(int)
//                     RaiseStatsChanged(int, int)
//                     RaiseEndingReached(EndingType)
//                     RaiseActChanged(ActSO)
// AudioManager/SocialFeedManager use: TaskLaunched event (Action<TaskSO>)
using System;

public static class GameEvents
{
    // ── New architecture events ───────────────────────────────────────────
    public static event Action<ScreenId>    OnShowScreen;
    public static event Action<string>      OnNameEntered;
    public static event Action<int>         OnPortraitChosen;
    public static event Action<Motivation>  OnMotivationSet;
    public static event Action<InboxAction> OnInboxItemSelected;
    public static event Action              OnChatReplied;
    public static event Action              OnRefusedBothTasks;
    public static event Action              OnNextAfterResult;
    public static event Action              OnPhoneClosed;
    public static event Action<TaskSO>      OnTaskChosen;
    public static event Action<TaskResult>  OnTaskFinished;
    public static event Action              OnPlayerStateChanged;
    public static event Action<string, float> OnTransitionRequested;

    // ── Legacy events your existing scripts subscribe to ──────────────────
    public static event Action<int, int>    OnStatsChanged;    // (money, risk)
    public static event Action<int>         OnClockAdvanced;   // (clockMinutes)
    public static event Action<ActSO>       OnActChanged;      // (actSO)
    public static event Action<EndingType>  OnEndingReached;   // (endingType)

    // UIManager subscribes with these names
    public static event Action<int, int>   StatsChanged   { add => OnStatsChanged  += value; remove => OnStatsChanged  -= value; }
    public static event Action<int>        ClockAdvanced  { add => OnClockAdvanced += value; remove => OnClockAdvanced -= value; }
    public static event Action<ActSO>      ActChanged     { add => OnActChanged    += value; remove => OnActChanged    -= value; }

    // AudioManager / SocialFeedManager subscribe to TaskLaunched
    public static event Action<TaskSO> TaskLaunched
    {
        add    => OnTaskChosen += value;
        remove => OnTaskChosen -= value;
    }

    // ── New architecture raise helpers ────────────────────────────────────
    public static void ShowScreen(ScreenId id)       => OnShowScreen?.Invoke(id);
    public static void NameEntered(string n)         => OnNameEntered?.Invoke(n);
    public static void PortraitChosen(int i)         => OnPortraitChosen?.Invoke(i);
    public static void MotivationSet(Motivation m)   => OnMotivationSet?.Invoke(m);
    public static void InboxSelected(InboxAction a)  => OnInboxItemSelected?.Invoke(a);
    public static void ChatReplied()                 => OnChatReplied?.Invoke();
    public static void RefusedBoth()                 => OnRefusedBothTasks?.Invoke();
    public static void NextAfterResult()             => OnNextAfterResult?.Invoke();
    public static void PhoneClosed()                 => OnPhoneClosed?.Invoke();
    public static void TaskChosen(TaskSO t)          => OnTaskChosen?.Invoke(t);
    public static void TaskFinished(TaskResult r)    => OnTaskFinished?.Invoke(r);
    public static void PlayerChanged()               => OnPlayerStateChanged?.Invoke();
    public static void RequestTransition(string label, float duration)
        => OnTransitionRequested?.Invoke(label, duration);

    // ── Legacy raise helpers — exact signatures GameManager calls ─────────
    public static void RaiseStatsChanged(int money, int risk) => OnStatsChanged?.Invoke(money, risk);
    public static void RaiseClockAdvanced(int clockMinutes)   => OnClockAdvanced?.Invoke(clockMinutes);
    public static void RaiseEndingReached(EndingType ending)  => OnEndingReached?.Invoke(ending);
    public static void RaiseActChanged(ActSO act)             => OnActChanged?.Invoke(act);
    public static void RaiseTaskLaunched(TaskSO t)            => OnTaskChosen?.Invoke(t);
}
