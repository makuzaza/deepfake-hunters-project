// UIManager.cs — FINAL. Matches exact calls from your actual scripts.
// DialogueManager calls: UIManager.I.SetSpeaker(string, Sprite)
//                        UIManager.I.SetDialogue(string)
// GameManager calls:     UIManager.I.ShowTransition(string, float)
// UIManager subscribes:  OnClockAdvanced(int), OnStatsChanged(int, int)
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("Player Panel — drag refs from PlayerPanel")]
    [SerializeField] private TMP_Text  moneyLabel;
    [SerializeField] private TMP_Text  timeLabel;
    [SerializeField] private TMP_Text  riskLabel;
    [SerializeField] private TMP_Text  playerNameLabel;
    [SerializeField] private Image     riskFill;

    [Header("Speaker Strip — drag refs from SpeakerStrip")]
    [SerializeField] private TMP_Text  speakerNameLabel;
    [SerializeField] private TMP_Text  speakerDialogueLabel;
    [SerializeField] private Image     speakerPortraitImage;

    [Header("Top Bar")]
    [SerializeField] private TMP_Text  statusLabel;
    [SerializeField] private Image     progressFill;

    [Header("Transition overlay (optional)")]
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private TMP_Text   transitionLabel;

    [Header("Data")]
    [SerializeField] private PlayerStateSO playerState;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(this); return; }
        I = this;
    }

    private void OnEnable()
    {
        GameEvents.OnClockAdvanced += RefreshClock;
        GameEvents.OnStatsChanged  += RefreshStats;
        GameEvents.OnActChanged    += RefreshAct;
        GameEvents.OnPlayerStateChanged += RefreshAll;
    }

    private void OnDisable()
    {
        GameEvents.OnClockAdvanced -= RefreshClock;
        GameEvents.OnStatsChanged  -= RefreshStats;
        GameEvents.OnActChanged    -= RefreshAct;
        GameEvents.OnPlayerStateChanged -= RefreshAll;
    }

    private void Start() => RefreshAll();

    // ── Called by GameEvents (exact signatures) ───────────────────────────

    // OnClockAdvanced passes int clockMinutes
    private void RefreshClock(int clockMinutes)
    {
        int h = clockMinutes / 60;
        int m = clockMinutes % 60;
        string t = $"{h:00}:{m:00}";
        int day = playerState != null ? playerState.day : 1;
        if (timeLabel)   timeLabel.text   = day + " — " + t;
        if (statusLabel) statusLabel.text = "DAY " + day + " — " + t;
        // also update playerState so PlayerPanelView stays in sync
        if (playerState != null) { playerState.timeLabel = t; }
    }

    // OnStatsChanged passes (int money, int risk)
    private void RefreshStats(int money, int risk)
    {
        if (moneyLabel) moneyLabel.text = "€" + money;
        if (riskLabel)  riskLabel.text  = risk + "%";
        if (riskFill)   riskFill.fillAmount = risk / 100f;
        if (playerState != null)
        {
            playerState.money = money;
            playerState.accountRisk = risk;
        }
    }

    private void RefreshAct(ActSO act) { /* hook up act transition visuals here if needed */ }

    public void RefreshAll()
    {
        if (playerState == null) return;
        if (moneyLabel)      moneyLabel.text      = "€" + playerState.money;
        if (timeLabel)       timeLabel.text        = playerState.day + " — " + playerState.timeLabel;
        if (riskLabel)       riskLabel.text        = playerState.accountRisk + "%";
        if (playerNameLabel) playerNameLabel.text  = playerState.playerName;
        if (riskFill)        riskFill.fillAmount   = playerState.accountRisk / 100f;
    }

    // ── Called by DialogueManager ─────────────────────────────────────────

    // DialogueManager calls: UIManager.I.SetSpeaker(speaker.displayName, speaker.portrait)
    public void SetSpeaker(string speakerName, Sprite portrait)
    {
        if (speakerNameLabel)   speakerNameLabel.text   = speakerName;
        if (speakerPortraitImage && portrait != null) speakerPortraitImage.sprite = portrait;
    }

    // DialogueManager calls: UIManager.I.SetDialogue(text)
    public void SetDialogue(string text)
    {
        if (speakerDialogueLabel) speakerDialogueLabel.text = text;
    }

    // ── Called by GameManager ─────────────────────────────────────────────

    // GameManager calls: UIManager.I.ShowTransition("Act 1", 1.5f)
    public void ShowTransition(string label, float duration)
    {
        if (transitionPanel) transitionPanel.SetActive(true);
        if (transitionLabel) transitionLabel.text = label;
        StartCoroutine(HideTransitionAfter(duration));
    }

    private System.Collections.IEnumerator HideTransitionAfter(float seconds)
    {
        yield return new UnityEngine.WaitForSeconds(seconds);
        if (transitionPanel) transitionPanel.SetActive(false);
    }

    // ── General helpers ───────────────────────────────────────────────────
    public void SetDay(int day)
    {
        if (playerState != null) playerState.day = day;
    }
}
