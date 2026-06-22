// ResultScreen.cs — fixed version
// Removes deprecated FindObjectOfType. Uses serialized PlayerStateSO ref instead.
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
        if (bossLine) bossLine.text = $"Great work today, {name}. You're a natural.";
    }
}
