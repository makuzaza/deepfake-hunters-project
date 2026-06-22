// UIManager.cs — FIXED.
// Problem: UIManager AND PlayerPanelView both wrote riskLabel + riskFill, but
// UIManager.RefreshStats was driven by GameManager which always passed risk = 0,
// so it kept stomping the real value back to 0%.
// Fix: UIManager NO LONGER touches risk at all. PlayerPanelView owns money/time/risk.
// UIManager keeps ONLY the speaker strip + transition overlay (its unique jobs).
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager I { get; private set; }

    [Header("Speaker Strip — drag refs from SpeakerStrip")]
    [SerializeField] private TMP_Text  speakerNameLabel;
    [SerializeField] private TMP_Text  speakerDialogueLabel;
    [SerializeField] private Image     speakerPortraitImage;

    [Header("Transition overlay (optional)")]
    [SerializeField] private GameObject transitionPanel;
    [SerializeField] private TMP_Text   transitionLabel;

    private void Awake()
    {
        if (I != null && I != this) { Destroy(this); return; }
        I = this;
    }

    // ── Called by DialogueManager ─────────────────────────────────────────
    public void SetSpeaker(string speakerName, Sprite portrait)
    {
        if (speakerNameLabel)   speakerNameLabel.text   = speakerName;
        if (speakerPortraitImage && portrait != null) speakerPortraitImage.sprite = portrait;
    }

    public void SetDialogue(string text)
    {
        if (speakerDialogueLabel) speakerDialogueLabel.text = text;
    }

    // ── Called by GameManager ─────────────────────────────────────────────
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
}

// NOTE: I removed moneyLabel, timeLabel, riskLabel, playerNameLabel, riskFill,
// statusLabel, progressFill, playerState and all the Refresh* methods.
// PlayerPanelView already handles every one of those via GameEvents.OnPlayerStateChanged.
// After swapping this file in, the missing inspector fields on the UIManager
// component will simply disappear — that is expected and harmless.