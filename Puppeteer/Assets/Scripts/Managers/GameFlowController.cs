// GameFlowController.cs — attach to Managers GameObject
// ALL screen transitions live here. Uses serialized refs + GameEvents (no name lookups).
//
// ADDED:
//   - Center header now updates on every screen (SetHeader + centerHeaderLabel ref).
//   - Home button returns to the dashboard (btnHome + GoHome).
//   - Existing 4-task ending logic kept unchanged.
using UnityEngine;
using TMPro;

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

    // ADDED — drag the DashboardTitle TMP here (the center header showing "Day - 2")
    [Header("Center header — drag DashboardTitle TMP")]
    [SerializeField] private TMP_Text centerHeaderLabel;

    // ADDED — drag BtnHome here
    [Header("Home button")]
    [SerializeField] private UnityEngine.UI.Button btnHome;

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

        // ADDED — wire the home button
        if (btnHome) btnHome.onClick.AddListener(GoHome);

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
        SetHeader("DASHBOARD \u2014 DAY " + playerState.day);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Step 3: inbox card clicked ────────────────────────────────────────
    private void HandleInboxItem(InboxAction action)
    {
        switch (action)
        {
            case InboxAction.OpenHRForm:
                SetHeader("HR ONBOARDING FORM");
                screenManager.Show(ScreenId.HRForm);
                break;
            case InboxAction.OpenChat:
                SetHeader("MESSAGES \u2014 MARCUS");
                screenManager.Show(ScreenId.Chat);
                break;
            case InboxAction.OpenBriefQueue:
                SetHeader("BRIEF QUEUE \u2014 DAY " + playerState.day);
                screenManager.Show(ScreenId.BriefQueue);
                break;
        }
    }

    // ── HR form submitted ─────────────────────────────────────────────────
    private void HandleMotivation(Motivation m)
    {
        playerState.motivation = m;
        GameEvents.PlayerChanged();
        _hrDone = true;
        MarkCardDone(InboxAction.OpenHRForm);
        SetHeader("DASHBOARD \u2014 DAY " + playerState.day);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Chat replied ──────────────────────────────────────────────────────
    private void HandleChatReplied()
    {
        _chatDone = true;
        MarkCardDone(InboxAction.OpenChat);
        SetHeader("DASHBOARD \u2014 DAY " + playerState.day);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Refused both tasks ────────────────────────────────────────────────
    private void HandleRefused()
    {
        playerState.noncoopCount++;
        AdvanceDay();
        SetHeader("DASHBOARD \u2014 DAY " + playerState.day);
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
        SetHeader("RESULT");
        screenManager.Show(ScreenId.Result);
    }

    // ── Result → Next → Phone ─────────────────────────────────────────────
    private void HandleNextAfterResult()
    {
        SetHeader("YOUR PHONE BUZZES");
        screenManager.Show(ScreenId.Phone);
    }

    // ── Phone closed → back to dashboard, advance day ────────────────────
    private void HandlePhoneClosed()
    {
        // End the run after the last task (4 live tasks: MG1, MiniGame2, MG3, MiniGame4).
        const int TASKS_IN_DEMO = 4;
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
        SetHeader("DASHBOARD \u2014 DAY " + playerState.day);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Home button → return to the dashboard for the current day ─────────
    public void GoHome()
    {
        Time.timeScale = 1f;
        SetHeader("DASHBOARD \u2014 DAY " + playerState.day);
        screenManager.Show(ScreenId.Dashboard);
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private void AdvanceDay()
    {
        playerState.SetTime(playerState.day + 1, "09:00");
    }

    private void SetHeader(string text)
    {
        if (centerHeaderLabel) centerHeaderLabel.text = text;
    }

    private void MarkCardDone(InboxAction action)
    {
        foreach (var item in dashboardScreen.items)
            if (item.action == action) item.completed = true;
        dashboardScreen.RebuildList();
    }
}