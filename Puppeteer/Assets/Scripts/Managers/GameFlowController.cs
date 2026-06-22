// GameFlowController.cs — attach to Managers GameObject
// ALL screen transitions live here. Uses serialized refs + GameEvents (no name lookups).
using UnityEngine;

public class GameFlowController : MonoBehaviour
{
    [Header("Manager refs (drag from Managers object)")]
    [SerializeField] private ScreenManager  screenManager;
    [SerializeField] private PlayerStateSO  playerState;
    [SerializeField] private TaskLauncher   taskLauncher;

    [Header("Screen refs (drag each screen panel)")]
    [SerializeField] private OnbLoginScreen    loginScreen;
    [SerializeField] private OnbPortraitScreen portraitScreen;
    [SerializeField] private DashboardScreen   dashboardScreen;
    [SerializeField] private HRFormScreen      hrFormScreen;
    [SerializeField] private ChatScreen        chatScreen;
    [SerializeField] private BriefQueueScreen  briefQueueScreen;
    [SerializeField] private ResultScreen      resultScreen;
    [SerializeField] private PhoneScreen       phoneScreen;

    // Tracks whether HR form and chat have been done (to grey out those inbox cards)
    private bool _hrDone, _chatDone;
    private TaskResult _lastResult;

    private void OnEnable()
    {
        GameEvents.OnNameEntered         += HandleNameEntered;
        GameEvents.OnPortraitChosen      += HandlePortraitChosen;
        GameEvents.OnMotivationSet       += HandleMotivation;
        GameEvents.OnInboxItemSelected   += HandleInboxItem;
        GameEvents.OnChatReplied         += HandleChatReplied;
        GameEvents.OnRefusedBothTasks    += HandleRefused;
        GameEvents.OnTaskChosen          += HandleTaskChosen;
        GameEvents.OnTaskFinished        += HandleTaskFinished;
        GameEvents.OnNextAfterResult     += HandleNextAfterResult;
        GameEvents.OnPhoneClosed         += HandlePhoneClosed;
    }
    private void OnDisable()
    {
        GameEvents.OnNameEntered         -= HandleNameEntered;
        GameEvents.OnPortraitChosen      -= HandlePortraitChosen;
        GameEvents.OnMotivationSet       -= HandleMotivation;
        GameEvents.OnInboxItemSelected   -= HandleInboxItem;
        GameEvents.OnChatReplied         -= HandleChatReplied;
        GameEvents.OnRefusedBothTasks    -= HandleRefused;
        GameEvents.OnTaskChosen          -= HandleTaskChosen;
        GameEvents.OnTaskFinished        -= HandleTaskFinished;
        GameEvents.OnNextAfterResult     -= HandleNextAfterResult;
        GameEvents.OnPhoneClosed         -= HandlePhoneClosed;
    }

    private void Start()
    {
        playerState.NewGame();
        screenManager.Show(ScreenId.OnbLogin);
    }

    // ── Step 1: name entered ──────────────────────────────────────────────
    private void HandleNameEntered(string n)
    {
        playerState.playerName = n;
        GameEvents.PlayerChanged();
        screenManager.Show(ScreenId.OnbPortrait);
    }

    // ── Step 2: portrait chosen ───────────────────────────────────────────
    private void HandlePortraitChosen(int idx)
    {
        playerState.portraitIndex = idx;
        GameEvents.PlayerChanged();
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Step 3: inbox card clicked ────────────────────────────────────────
    private void HandleInboxItem(InboxAction action)
    {
        switch (action)
        {
            case InboxAction.OpenHRForm:     screenManager.Show(ScreenId.HRForm);     break;
            case InboxAction.OpenChat:       screenManager.Show(ScreenId.Chat);       break;
            case InboxAction.OpenBriefQueue: screenManager.Show(ScreenId.BriefQueue); break;
        }
    }

    // ── HR form submitted ─────────────────────────────────────────────────
    private void HandleMotivation(Motivation m)
    {
        playerState.motivation = m;
        GameEvents.PlayerChanged();
        _hrDone = true;
        MarkCardDone(InboxAction.OpenHRForm);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Chat replied ──────────────────────────────────────────────────────
    private void HandleChatReplied()
    {
        _chatDone = true;
        MarkCardDone(InboxAction.OpenChat);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Refused both tasks ────────────────────────────────────────────────
    private void HandleRefused()
    {
        playerState.noncoopCount++;
        AdvanceDay();
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Task chosen → launch ─────────────────────────────────────────────
    private void HandleTaskChosen(TaskSO task) { taskLauncher.Launch(task); }

    // ── Task finished (from teammate's scene) ────────────────────────────
    private void HandleTaskFinished(TaskResult r)
    {
        _lastResult = r;
        playerState.AddMoney(r.payEarned);
        playerState.SetRisk(playerState.accountRisk + r.riskDelta);
        playerState.tasksCompleted++;
        resultScreen.Setup(r);
        phoneScreen.Setup(r);
        screenManager.Show(ScreenId.Result);
    }

    // ── Result → Next → Phone ─────────────────────────────────────────────
    private void HandleNextAfterResult() { screenManager.Show(ScreenId.Phone); }

    // ── Phone closed → back to dashboard, advance day ────────────────────
    private void HandlePhoneClosed()
    {
        // End the run after the last task (you have 3 live tasks: MG1, MiniGame2, MG3).
        const int TASKS_IN_DEMO = 3;
        if (playerState.tasksCompleted >= TASKS_IN_DEMO)
        {
            EndingType ending;
            if (playerState.noncoopCount >= TASKS_IN_DEMO)        ending = EndingType.PassiveResistance; // never launched anything
            else if (playerState.accountRisk >= 50)              ending = EndingType.Whistleblower;      // high-risk path -> they bailed
            else                                                  ending = EndingType.Complicit;          // default compliance
            GameManager.I?.ForceEnding(ending);
            return;
        }
 
        AdvanceDay();
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private void AdvanceDay()
    {
        playerState.SetTime(playerState.day + 1, "09:00");
    }

    private void MarkCardDone(InboxAction action)
    {
        foreach (var item in dashboardScreen.items)
            if (item.action == action) item.completed = true;
        dashboardScreen.RebuildList();
    }
}
