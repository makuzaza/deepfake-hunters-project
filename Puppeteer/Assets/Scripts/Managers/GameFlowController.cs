// GameFlowController.cs  -  Assets/_Project/Scripts/Managers
// Drives the WHOLE placeholder playthrough at runtime, finding UI by name:
//   Onboarding -> Act loop (Brief->Complete->Preview->Launch->Feed->Next) -> Final Choice -> Ending.
// No minigames: each task is a click-confirm placeholder. All three endings reachable.
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameFlowController : MonoBehaviour
{
    private Transform root;

    // panels
    private GameObject onboarding, taskPanel, finalChoice;
    // onboarding controls
    private TMP_InputField nameInput;
    private TMP_Text onboardingPrompt;
    // task controls
    private TMP_Text taskTitle, taskBrief;
    private Button completeBtn, previewBtn, launchBtn, reassignBtn;
    private Button cooperateBtn, exposeBtn;
    private Button beginBtn;

    // latches set by button clicks, polled by the coroutine
    private bool beginClicked, completeClicked, previewClicked, launchClicked, reassignClicked;
    private bool cooperateClicked, exposeClicked, profileChosen;

    private int eligibleTotal, reassignedTotal;
    private string playerName = "You";

    private void Start()
    {
        var canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null) { Debug.LogError("[Flow] No Canvas in scene."); return; }
        root = canvas.transform;

        onboarding = Go(UINames.OnboardingPanel);
        taskPanel  = Go(UINames.TaskPanel);
        finalChoice= Go(UINames.FinalChoicePanel);

        nameInput        = Comp<TMP_InputField>(UINames.NameInput);
        onboardingPrompt = Txt(UINames.OnboardingPrompt);
        taskTitle        = Txt(UINames.TaskTitle);
        taskBrief        = Txt(UINames.TaskBrief);

        completeBtn  = Btn(UINames.CompleteButton);
        previewBtn   = Btn(UINames.PreviewButton);
        launchBtn    = Btn(UINames.LaunchButton);
        reassignBtn  = Btn(UINames.ReassignButton);
        cooperateBtn = Btn(UINames.CooperateButton);
        exposeBtn    = Btn(UINames.ExposeButton);
        beginBtn     = Btn(UINames.BeginButton);

        Hook(beginBtn,     () => beginClicked = true);
        Hook(completeBtn,  () => completeClicked = true);
        Hook(previewBtn,   () => previewClicked = true);
        Hook(launchBtn,    () => launchClicked = true);
        Hook(reassignBtn,  () => reassignClicked = true);
        Hook(cooperateBtn, () => cooperateClicked = true);
        Hook(exposeBtn,    () => exposeClicked = true);
        Hook(Btn(UINames.QuizMoney),       () => { ProfileManager.I?.Set(ProfileType.Money); profileChosen = true; });
        Hook(Btn(UINames.QuizImpact),      () => { ProfileManager.I?.Set(ProfileType.Impact); profileChosen = true; });
        Hook(Btn(UINames.QuizRecognition), () => { ProfileManager.I?.Set(ProfileType.Recognition); profileChosen = true; });

        SetActive(onboarding, false); SetActive(taskPanel, false); SetActive(finalChoice, false);
        StartCoroutine(Run());
    }

    private IEnumerator Run()
    {
        // ---------- ONBOARDING ----------
        SetActive(onboarding, true);
        if (onboardingPrompt) onboardingPrompt.text =
            "It's your first day at Human Agency.\nEnter your name, pick a portrait, and answer: why are you here?";
        beginClicked = false;
        while (!beginClicked) yield return null;
        if (nameInput != null && !string.IsNullOrWhiteSpace(nameInput.text)) playerName = nameInput.text;
        if (!profileChosen) ProfileManager.I?.Set(ProfileType.Money); // safe default
        UIManager.I?.SetPlayer(playerName, null);
        SetActive(onboarding, false);

        // ---------- ACTS ----------
        var flow = GameManager.I != null ? GameManager.I.flow : null;
        if (flow != null && flow.acts != null)
        {
            foreach (var act in flow.acts)
                if (act != null && act.tasks != null)
                    foreach (var t in act.tasks)
                        if (t != null && t.noncooperationEligible) eligibleTotal++;

            foreach (var act in flow.acts)
            {
                if (act == null) continue;
                UIManager.I?.ShowTransition(act.actTitle, 1.4f);
                yield return new WaitForSeconds(1.5f);
                if (act.tasks != null)
                    foreach (var task in act.tasks)
                        if (task != null) yield return RunTask(task);
            }
        }
        else Debug.LogWarning("[Flow] GameFlow has no acts; skipping to final choice.");

        // ---------- PASSIVE RESISTANCE (Ending C) ----------
        if (eligibleTotal > 0 && reassignedTotal >= eligibleTotal)
        {
            GameManager.I?.ForceEnding(EndingType.PassiveResistance);
            yield break;
        }

        // ---------- FINAL CHOICE ----------
        SetActive(finalChoice, true);
        cooperateClicked = exposeClicked = false;
        while (!cooperateClicked && !exposeClicked) yield return null;
        GameManager.I?.ForceEnding(cooperateClicked ? EndingType.Complicit : EndingType.Whistleblower);
    }

    private IEnumerator RunTask(TaskSO task)
    {
        SetActive(taskPanel, true);
        if (taskTitle) taskTitle.text = task.taskId;
        if (taskBrief) taskBrief.text = FirstBrief(task);
        UIManager.I?.SetCompany("Human Agency", "AI content, perfected.");

        bool eligible = task.noncooperationEligible;
        completeClicked = previewClicked = launchClicked = reassignClicked = false;
        SetInteractable(completeBtn, true);
        SetInteractable(previewBtn, false);
        SetInteractable(launchBtn, false);
        SetActive(reassignBtn ? reassignBtn.gameObject : null, eligible);

        // step 1: complete (or reassign to Marcus, if eligible)
        while (!completeClicked && !reassignClicked) yield return null;
        if (reassignClicked && eligible)
        {
            reassignedTotal++;
            GameManager.I?.RecordNoncoop(task);
            UIManager.I?.ShowNotification("Reassigned to Marcus.");
            SetActive(taskPanel, false);
            yield break;
        }

        // step 2: preview
        SetInteractable(completeBtn, false);
        SetInteractable(previewBtn, true);
        while (!previewClicked) yield return null;

        // step 3: launch
        SetInteractable(previewBtn, false);
        SetInteractable(launchBtn, true);
        while (!launchClicked) yield return null;

        TaskManager.I?.SimulateLaunch(task);   // raises TaskLaunched -> feed + clock + money
        yield return new WaitForSeconds(1.0f);
        SetActive(taskPanel, false);
    }

    // ---------- helpers ----------
    private string FirstBrief(TaskSO task)
    {
        if (task.brief != null && task.brief.lines != null && task.brief.lines.Count > 0)
            return ProfileManager.I != null ? ProfileManager.I.Resolve(task.brief.lines[0]) : task.brief.lines[0].defaultText;
        return "Complete this task, then Preview and Launch.";
    }

    private GameObject Go(string n)  { var t = UIFind.Deep(root, n); return t ? t.gameObject : null; }
    private TMP_Text Txt(string n)   { var t = UIFind.Deep(root, n); return t ? t.GetComponent<TMP_Text>() : null; }
    private Button Btn(string n)     { var t = UIFind.Deep(root, n); return t ? t.GetComponent<Button>() : null; }
    private T Comp<T>(string n) where T : Component { var t = UIFind.Deep(root, n); return t ? t.GetComponent<T>() : null; }

    private void Hook(Button b, UnityEngine.Events.UnityAction a) { if (b != null) b.onClick.AddListener(a); }
    private void SetActive(GameObject g, bool v) { if (g != null) g.SetActive(v); }
    private void SetInteractable(Button b, bool v) { if (b != null) b.interactable = v; }
}
