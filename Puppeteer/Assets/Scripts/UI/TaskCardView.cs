// TaskCardView.cs — attach to TaskCard prefab (one per task in BriefQueue)
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TaskCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text clientLabel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text payText;
    [SerializeField] private TMP_Text riskText;
    [SerializeField] private Image    riskPill;
    [SerializeField] private Button   takeButton;

    [Header("Risk pill colors")]
    [SerializeField] private Color riskNone   = new Color(0.28f,0.36f,0.36f);
    [SerializeField] private Color riskLow    = new Color(0.60f,0.65f,0.53f);
    [SerializeField] private Color riskMedium = new Color(0.79f,0.69f,0.53f);
    [SerializeField] private Color riskHigh   = new Color(0.56f,0.22f,0.26f);

    public void Setup(TaskSO task, Action onTake)
    {
        if (clientLabel) clientLabel.text = "CLIENT: " + task.clientName.ToUpper();
        if (titleText)   titleText.text   = task.taskTitle;
        if (payText)     payText.text     = "€" + task.pay;
        if (riskText)    riskText.text    = "RISK: " + task.riskLevel.ToString().ToUpper();
        if (riskPill)    riskPill.color   = task.riskLevel switch {
            RiskLevel.None   => riskNone,
            RiskLevel.Low    => riskLow,
            RiskLevel.Medium => riskMedium,
            RiskLevel.High   => riskHigh,
            _                => riskNone
        };
        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(() => onTake?.Invoke());
    }
}
