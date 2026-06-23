// PlayerStateSO.cs
// Create ONE asset: Assets/_Project/Data/PlayerState.asset
// Drag it into every script that needs player data.
using UnityEngine;
[CreateAssetMenu(menuName = "Puppeteer/Player State")]
public class PlayerStateSO : ScriptableObject
{
    [Header("Identity")]
    public string playerName = "Alex";
    public int    portraitIndex;
    public Motivation motivation;

    [Header("Economy")]
    public int money;
    public int taskPay;          // pending pay shown during task

    [Header("Time")]
    public int    day = 1;
    public string timeLabel = "09:00";

    [Header("Risk")]
    [Range(0,100)] public int accountRisk;

    [Header("Progress")]
    public int noncoopCount;     // times player used passive resistance
    public int tasksCompleted;

    // Call NewGame() before starting a new playthrough so SO values reset.
    public void NewGame()
    {
        playerName="Alex"; portraitIndex=0; motivation=Motivation.Money;
        money=0; taskPay=0; day=1; timeLabel="09:00"; accountRisk=0;
        noncoopCount=0; tasksCompleted=0;
    }
    public void AddMoney(int v)          { money += v; }
    public void SetRisk(int v)           { accountRisk = Mathf.Clamp(v,0,100); }
    public void SetTime(int d, string t) { day = d; timeLabel = t; }
}
