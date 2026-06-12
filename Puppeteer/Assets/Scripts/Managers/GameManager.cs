// GameManager.cs  -  Assets/_Project/Scripts/Managers
// Owns flow, the diegetic clock, money, noncooperation tracking, ending resolution.
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Content")] public GameFlowSO flow;            // assign GameFlow.asset in the MainGame scene
    [Header("Runtime state")]
    [SerializeField] private int clockMinutes = 9 * 60;    // starts 09:00
    [SerializeField] private int money = 0;

    private int _noncoopUsed;
    private int _noncoopEligibleTotal;

    // Carried across the scene load into the Ending scene.
    public static EndingType PendingEnding = EndingType.Complicit;

    private void Start()
    {
        GameEvents.RaiseStatsChanged(money, 0);
        GameEvents.RaiseClockAdvanced(clockMinutes);
    }

    public void AdvanceClock(int minutes) { clockMinutes += minutes; GameEvents.RaiseClockAdvanced(clockMinutes); }
    public void AddMoney(int amount)      { money += amount; GameEvents.RaiseStatsChanged(money, 0); }

    public void RecordNoncoop(TaskSO task) => _noncoopUsed++;
    public bool ShouldTriggerPassiveEnding() => _noncoopEligibleTotal > 0 && _noncoopUsed >= _noncoopEligibleTotal;

    // true = leak (B), false = stay quiet (A). See Architecture S8.
    public void SubmitFinalChoice(bool leak) => ForceEnding(leak ? EndingType.Whistleblower : EndingType.Complicit);

    public void ForceEnding(EndingType ending)
    {
        PendingEnding = ending;
        GameEvents.RaiseEndingReached(ending);
        SceneManager.LoadScene("Ending");
    }

    // Placeholder until the full flow coroutine lands (Day 2). See Architecture S9.
    public void DebugJumpToAct(int index)
    {
        if (flow != null && index >= 0 && index < flow.acts.Count && flow.acts[index] != null)
            GameEvents.RaiseActChanged(flow.acts[index]);
        else
            UIManager.I?.ShowTransition("Act " + (index + 1), 1.5f);
    }
}
