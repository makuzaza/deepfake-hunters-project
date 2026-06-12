// UIManager.cs  -  Assets/_Project/Scripts/Managers
// Binds the panels by name (single uiRoot reference), updates readouts from the
// event bus, and exposes show/hide helpers the rest of the game calls.
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : Singleton<UIManager>
{
    public Transform uiRoot;   // assigned by build tool to the Canvas

    private GameObject companyPanel, taskPanel, dialoguePanel, profilePanel, feedPanel, notificationPanel, transitionPanel, debugPanel;
    private TMP_Text companyTitle, companyTagline, speakerName, dialogueText, playerName, moneyLabel, timeLabel, notificationText, transitionText;
    private Image speakerPortrait, playerPortrait;
    private Coroutine notifyRoutine;

    protected override void Awake() { base.Awake(); Bind(); }

    private void OnEnable()  { GameEvents.ClockAdvanced += OnClock; GameEvents.StatsChanged += OnStats; GameEvents.ActChanged += OnAct; }
    private void OnDisable() { GameEvents.ClockAdvanced -= OnClock; GameEvents.StatsChanged -= OnStats; GameEvents.ActChanged -= OnAct; }

    private void Bind()
    {
        if (uiRoot == null) { var c = FindFirstObjectByType<Canvas>(); if (c) uiRoot = c.transform; }
        if (uiRoot == null) return;

        companyPanel = Go(UINames.CompanyPanel);  taskPanel = Go(UINames.TaskPanel);  dialoguePanel = Go(UINames.DialoguePanel);
        profilePanel = Go(UINames.ProfilePanel);  feedPanel = Go(UINames.FeedPanel);  notificationPanel = Go(UINames.NotificationPanel);
        transitionPanel = Go(UINames.TransitionPanel);  debugPanel = Go(UINames.DebugPanel);

        companyTitle = Txt(UINames.CompanyTitle);  companyTagline = Txt(UINames.CompanyTagline);  speakerName = Txt(UINames.SpeakerName);
        dialogueText = Txt(UINames.DialogueText);  playerName = Txt(UINames.PlayerName);  moneyLabel = Txt(UINames.MoneyLabel);
        timeLabel = Txt(UINames.TimeLabel);  notificationText = Txt(UINames.NotificationText);  transitionText = Txt(UINames.TransitionText);

        speakerPortrait = Img(UINames.SpeakerPortrait);  playerPortrait = Img(UINames.PlayerPortrait);

        if (feedPanel) feedPanel.SetActive(false);
        if (notificationPanel) notificationPanel.SetActive(false);
        if (transitionPanel) transitionPanel.SetActive(false);
    }

    private GameObject Go(string n) { var t = UIFind.Deep(uiRoot, n); return t ? t.gameObject : null; }
    private TMP_Text   Txt(string n){ var t = UIFind.Deep(uiRoot, n); return t ? t.GetComponent<TMP_Text>() : null; }
    private Image      Img(string n){ var t = UIFind.Deep(uiRoot, n); return t ? t.GetComponent<Image>() : null; }

    private void OnClock(int minutes) { if (timeLabel) timeLabel.text = Format(minutes); }
    private void OnStats(int money, int rep) { if (moneyLabel) moneyLabel.text = "$" + money; }
    private void OnAct(ActSO act) { if (act != null) ShowTransition(act.actTitle, 1.5f); }

    private string Format(int m) { int hh = (m / 60) % 24, mm = m % 60; return hh.ToString("00") + ":" + mm.ToString("00"); }

    public void SetCompany(string title, string tagline) { if (companyTitle) companyTitle.text = title; if (companyTagline) companyTagline.text = tagline; }
    public void SetSpeaker(string name, Sprite portrait)  { if (speakerName) speakerName.text = name; if (speakerPortrait && portrait) speakerPortrait.sprite = portrait; }
    public void SetDialogue(string text)                  { if (dialogueText) dialogueText.text = text; }
    public void SetPlayer(string name, Sprite portrait)   { if (playerName) playerName.text = name; if (playerPortrait && portrait) playerPortrait.sprite = portrait; }
    public void UnlockFeed()                              { if (feedPanel) feedPanel.SetActive(true); }

    public void ShowNotification(string text)
    {
        if (!notificationPanel) return;
        if (notificationText) notificationText.text = text;
        notificationPanel.SetActive(true);
        if (notifyRoutine != null) StopCoroutine(notifyRoutine);
        notifyRoutine = StartCoroutine(HideAfter(notificationPanel, 2.5f));
    }

    public void ShowTransition(string text, float seconds)
    {
        if (!transitionPanel) return;
        if (transitionText) transitionText.text = text;
        transitionPanel.SetActive(true);
        StartCoroutine(HideAfter(transitionPanel, seconds));
    }

    private IEnumerator HideAfter(GameObject go, float s) { yield return new WaitForSeconds(s); if (go) go.SetActive(false); }
}
