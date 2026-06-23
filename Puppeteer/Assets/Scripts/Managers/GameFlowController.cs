// GameFlowController.cs — attach to Managers GameObject
// ALL screen transitions live here. Uses serialized refs + GameEvents (no name lookups).
//
// UPDATED for day-by-day flow:
//   - Dashboard inbox is now built per day (BuildInboxForDay) and pushed to the
//     dashboard before showing (ShowDashboardForToday).
//   - Brief queue shows only the current day's task (handled in BriefQueueScreen).
//   - After the final task, routes to the Reflection screen instead of Dashboard.
using System.Collections.Generic;
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

    [Header("Flow config")]
    [SerializeField] private int totalTasks = 4;   // game ends after this many completed tasks

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
        _hrDone = false;
        _chatDone = false;
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
        ShowDashboardForToday();
    }

    // ── Step 3: inbox card clicked ────────────────────────────────────────
    private void HandleInboxItem(InboxAction action)
    {
        switch (action)
        {
            case InboxAction.OpenHRForm:     screenManager.Show(ScreenId.HRForm);     break;
            case InboxAction.OpenChat:       screenManager.Show(ScreenId.Chat);       break;
            case InboxAction.OpenBriefQueue: screenManager.Show(ScreenId.BriefQueue); break;
            case InboxAction.OpenNews:       /* Phase 6: screenManager.Show(ScreenId.Article); */ break;
            case InboxAction.Dismiss:        ShowDashboardForToday(); break;
        }
    }

    // ── HR form submitted ─────────────────────────────────────────────────
    private void HandleMotivation(Motivation m)
    {
        playerState.motivation = m;
        GameEvents.PlayerChanged();
        _hrDone = true;
        ShowDashboardForToday();
    }

    // ── Chat replied ──────────────────────────────────────────────────────
    private void HandleChatReplied()
    {
        _chatDone = true;
        ShowDashboardForToday();
    }

    // ── Refused the task ──────────────────────────────────────────────────
    private void HandleRefused()
    {
        // With one task per day, refusing skips the day. (You may later route this
        // through the noncoop counter in Phase 3.)
        AdvanceDay();
        ShowDashboardForToday();
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

    // ── Phone closed → next day, OR end the game after the final task ─────
    private void HandlePhoneClosed()
    {
        if (playerState.tasksCompleted >= totalTasks)
        {
            // Final task done → end of game.
            // PHASE 4: once you add ScreenId.Reflection to Enums.cs and build the
            // Screen_Reflection panel, replace the line below with:
            //     screenManager.Show(ScreenId.Reflection);
            // For now (day-by-day phase only) we keep it on the dashboard so the
            // file compiles without the new enum value.
            ShowDashboardForToday();
            return;
        }
        AdvanceDay();
        ShowDashboardForToday();
    }

    // ── Helpers ───────────────────────────────────────────────────────────
    private void AdvanceDay()
    {
        playerState.SetTime(playerState.day + 1, "09:00");
    }

    // Builds the dashboard for the current day and shows it.
    private void ShowDashboardForToday()
    {
        dashboardScreen.SetItems(BuildInboxForDay(playerState.day));
        screenManager.Show(ScreenId.Dashboard);
    }

    // The per-day inbox. Edit text/tags to taste. Day 1 = onboarding,
    // Day 2+ = the day's task + Marcus, Day 3 also adds the news article.
    private List<InboxItemData> BuildInboxForDay(int day)
    {
        var list = new List<InboxItemData>();

        if (day == 1)
        {
            list.Add(new InboxItemData {
                title = "HR — Onboarding Form",
                subline = "Please complete before starting work",
                tag = "NEW", action = InboxAction.OpenHRForm, completed = _hrDone
            });
            list.Add(new InboxItemData {
                title = "Marcus",
                subline = "\"hey, let me know when you're settled in!\"",
                tag = "MSG", action = InboxAction.OpenChat, completed = _chatDone
            });
            list.Add(new InboxItemData {
                title = "Brief Queue",
                subline = "1 task available today",
                tag = "1", action = InboxAction.OpenBriefQueue, completed = false
            });
        }
        else
        {
            list.Add(new InboxItemData {
                title = "New task from Tony — Natural Remedies",
                subline = "He has another request for you specifically",
                tag = "BRIEF", action = InboxAction.OpenBriefQueue, completed = false
            });
            list.Add(new InboxItemData {
                title = "Marcus",
                subline = "\"It's just filling in the gaps. Tony approved it.\"",
                tag = "MSG", action = InboxAction.OpenChat, completed = _chatDone
            });

            if (day == 3)
            {
                list.Add(new InboxItemData {
                    title = "Health misinformation in advertising on the rise, experts warn",
                    subline = "tap to read · tap X to dismiss",
                    tag = "", action = InboxAction.OpenNews, completed = false
                });
            }
        }
        return list;
    }
}