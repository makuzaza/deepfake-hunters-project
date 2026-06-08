using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the 6-screen in-game computer portal.
/// Attach to a Canvas GameObject. Wire all screen panels + UI elements in the Inspector.
///
/// Screen flow:
///   1 Login          → enter name → go to screen 2
///   2 Dashboard       → 3 inbox cards (HR, Marcus, Briefs) → each opens a screen
///   3 HR Form         → radio questions → submit → back to dashboard
///   4 Marcus Chat     → two choices → back to dashboard
///   5 Brief Queue     → pick a task → go to screen 6
///   6 Task 1 Demo     → placeholder / "coming soon" panel
/// </summary>
public class PortalUIController : MonoBehaviour
{
    // ── Screen panels ──────────────────────────────────────────────
    [Header("Screen Panels")]
    public GameObject screenLogin;
    public GameObject screenDashboard;
    public GameObject screenHR;
    public GameObject screenMarcus;
    public GameObject screenBriefs;
    public GameObject screenTask1;

    // ── Screen 1 – Login ───────────────────────────────────────────
    [Header("Login")]
    public TMP_InputField nameInput;

    // ── Screen 2 – Dashboard ───────────────────────────────────────
    [Header("Dashboard")]
    public TMP_Text dashboardGreeting;
    public GameObject cardHR;
    public GameObject cardMarcus;
    public GameObject cardBriefs;
    public GameObject badgeHR;
    public GameObject badgeMarcus;
    public GameObject badgeBriefs;

    // ── Screen 3 – HR Form ─────────────────────────────────────────
    [Header("HR Form")]
    public TMP_Text hrFormName;
    public Toggle[] q1Options;   // 3 toggles: A, B, C
    public Toggle[] q2Options;   // 2 toggles: A, B
    public Toggle[] q3Options;   // 3 toggles: A, B, C

    // ── Screen 4 – Marcus Chat ─────────────────────────────────────
    [Header("Marcus Chat")]
    public TMP_Text marcusGreeting;

    // ── Screen 5 – Briefs ──────────────────────────────────────────
    // (no extra refs needed beyond buttons wired in Inspector)

    // ── Screen 6 – Task 1 Demo ─────────────────────────────────────
    [Header("Task 1 Demo")]
    public TMP_Text task1Title;
    public TMP_Text task1Body;

    // ── Progress bar (optional) ────────────────────────────────────
    [Header("Progress")]
    public Image progressFill;
    public TMP_Text screenLabel;

    // ── State ──────────────────────────────────────────────────────
    string playerName = "Alex";
    bool hrDone     = false;
    bool marcusDone = false;
    bool briefsDone = false;

    // ──────────────────────────────────────────────────────────────
    //  Public API
    // ──────────────────────────────────────────────────────────────

    public void Open()
    {
        gameObject.SetActive(true);
        ShowScreen(1);
    }

    public void Close()
    {
        gameObject.SetActive(false);
        ResumePlayer();
    }

    // ──────────────────────────────────────────────────────────────
    //  Screen navigation
    // ──────────────────────────────────────────────────────────────

    void ShowScreen(int n)
    {
        screenLogin    .SetActive(n == 1);
        screenDashboard.SetActive(n == 2);
        screenHR       .SetActive(n == 3);
        screenMarcus   .SetActive(n == 4);
        screenBriefs   .SetActive(n == 5);
        screenTask1    .SetActive(n == 6);

        UpdateProgress(n);

        if (n == 2) RefreshDashboard();
        if (n == 3) RefreshHRForm();
        if (n == 4) RefreshMarcus();
        if (n == 6) RefreshTask1();
    }

    void UpdateProgress(int n)
    {
        if (progressFill != null) progressFill.fillAmount = n / 6f;
        if (screenLabel  != null)
        {
            string[] labels = { "", "LOGIN", "DASHBOARD — DAY 1", "HR — ONBOARDING",
                                 "MESSAGES — MARCUS", "BRIEF QUEUE — DAY 1", "TASK — VIDEO EDITOR" };
            screenLabel.text = $"SCREEN {n} / 6 — {labels[n]}";
        }
    }

    // ──────────────────────────────────────────────────────────────
    //  Screen 1 – Login
    // ──────────────────────────────────────────────────────────────

    // Called by "Start your first day →" button
    public void OnLoginSubmit()
    {
        if (nameInput != null && nameInput.text.Trim().Length > 0)
            playerName = nameInput.text.Trim();
        ShowScreen(2);
    }

    // ──────────────────────────────────────────────────────────────
    //  Screen 2 – Dashboard
    // ──────────────────────────────────────────────────────────────

    void RefreshDashboard()
    {
        if (dashboardGreeting != null)
            dashboardGreeting.text = $"Good morning, {playerName}. You have 3 items waiting.";

        // Grey out completed cards
        if (badgeHR     != null) badgeHR    .SetActive(!hrDone);
        if (badgeMarcus != null) badgeMarcus.SetActive(!marcusDone);
        if (badgeBriefs != null) badgeBriefs.SetActive(!briefsDone);

        SetCardInteractable(cardHR,      !hrDone);
        SetCardInteractable(cardMarcus,  !marcusDone);
        SetCardInteractable(cardBriefs,  !briefsDone);
    }

    void SetCardInteractable(GameObject card, bool active)
    {
        if (card == null) return;
        var cg = card.GetComponent<CanvasGroup>();
        if (cg == null) cg = card.AddComponent<CanvasGroup>();
        cg.alpha          = active ? 1f : 0.4f;
        cg.interactable   = active;
        cg.blocksRaycasts = active;
    }

    // Card buttons call these:
    public void OnCardHR()      => ShowScreen(3);
    public void OnCardMarcus()  => ShowScreen(4);
    public void OnCardBriefs()  => ShowScreen(5);

    // ──────────────────────────────────────────────────────────────
    //  Screen 3 – HR Form
    // ──────────────────────────────────────────────────────────────

    void RefreshHRForm()
    {
        if (hrFormName != null) hrFormName.text = playerName;
    }

    // Called by "Submit & return to desk →" button
    public void OnHRSubmit()
    {
        hrDone = true;
        ShowScreen(2);
    }

    // ──────────────────────────────────────────────────────────────
    //  Screen 4 – Marcus Chat
    // ──────────────────────────────────────────────────────────────

    void RefreshMarcus()
    {
        if (marcusGreeting != null)
            marcusGreeting.text = $"Hey {playerName}! I'm Marcus. Looks like we're on the same team.";
    }

    // Choice buttons:
    public void OnMarcusGuide()
    {
        marcusDone = true;
        // "Walk me through it" — player chose guided experience (store if needed later)
        ShowScreen(2);
    }

    public void OnMarcusSelf()
    {
        marcusDone = true;
        // "I'll figure it out" — independent play style
        ShowScreen(2);
    }

    // ──────────────────────────────────────────────────────────────
    //  Screen 5 – Brief Queue
    // ──────────────────────────────────────────────────────────────

    // Task A: Natural Remedies video edit — leads to Task 1 demo
    public void OnTakeTask1()
    {
        briefsDone = true;
        ShowScreen(6);
    }

    // Task B: Social media copy — placeholder, same destination for now
    public void OnTakeTask2()
    {
        briefsDone = true;
        ShowScreen(6);
    }

    // "Refuse both" link
    public void OnRefuseBoth()
    {
        briefsDone = true;
        ShowScreen(2);
    }

    // ──────────────────────────────────────────────────────────────
    //  Screen 6 – Task 1 Demo
    // ──────────────────────────────────────────────────────────────

    void RefreshTask1()
    {
        if (task1Title != null)
            task1Title.text = "TASK — VIDEO EDITOR";
        if (task1Body != null)
            task1Body.text =
                "CLIENT: NATURAL REMEDIES\n\n" +
                "Edit a promo video — remove stutters and mistakes from Tony's recording.\n\n" +
                "[Mini-game coming soon]\n\n" +
                "You will be able to click on red timeline segments to select them,\n" +
                "then delete them to clean up the video before launching it.";
    }

    // Called by "Close / Exit" button on Task 1 screen
    public void OnTask1Exit()
    {
        Close();
    }

    // ──────────────────────────────────────────────────────────────
    //  Helpers
    // ──────────────────────────────────────────────────────────────

    void ResumePlayer()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            var movement = player.GetComponent<PlayerMovement>();
            if (movement != null) movement.enabled = true;
        }
    }
}
