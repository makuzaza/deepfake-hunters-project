// ResultScreen.cs
// CHANGE: the BOSS line is no longer hardcoded "you're a natural". It now reads
// _data.bossLine, so success and fail show different boss text. Falls back to a
// default only if the result didn't supply one.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResultScreen : UIScreen
{
    [Header("Phone Mockup")]
    [SerializeField] private TMP_Text senderName;
    [SerializeField] private TMP_Text messageText;

    [Header("Bottom section")]
    [SerializeField] private TMP_Text bossLabel;
    [SerializeField] private TMP_Text bossLine;
    [SerializeField] private TMP_Text dianePost;
    [SerializeField] private Button   nextButton;

    [Header("Data")]
    [SerializeField] private PlayerStateSO playerState;  // drag PlayerState.asset here

    private TaskResult _data;

    protected override void Awake()
    {
        base.Awake();
        if (nextButton) nextButton.onClick.AddListener(() => GameEvents.NextAfterResult());
    }

    public void Setup(TaskResult r) { _data = r; }

    protected override void OnBeforeShow()
    {
        if (messageText) messageText.text = _data.clientFeedback;
        if (dianePost)   dianePost.text   = _data.dianePost;

        string name = playerState != null ? playerState.playerName : "Alex";

        // Boss line now comes from the result. Fall back to success text only if empty.
        if (bossLine)
        {
            bossLine.text = !string.IsNullOrEmpty(_data.bossLine)
                ? _data.bossLine.Replace("{name}", name)
                : $"Great work today, {name}. You're a natural.";
        }
    }
}