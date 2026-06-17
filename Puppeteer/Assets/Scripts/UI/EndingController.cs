// EndingController.cs — fixed version
// Removes UIFind. Uses serialized refs for ending text labels.
using TMPro;
using UnityEngine;

public class EndingController : MonoBehaviour
{
    [Header("Ending display refs — drag from your Ending scene")]
    [SerializeField] private TMP_Text endingTitleLabel;
    [SerializeField] private TMP_Text epilogueLabel;

    [Header("Ending data assets")]
    [SerializeField] private EndingSO endingComplicit;
    [SerializeField] private EndingSO endingWhistleblower;
    [SerializeField] private EndingSO endingPassive;

    private void Start()
    {
        // Read which ending was chosen from PlayerPrefs (set by GameFlowController)
        int endingType = PlayerPrefs.GetInt("EndingType", 0);
        ShowEnding((EndingType)endingType);
    }

    private void ShowEnding(EndingType type)
    {
        EndingSO data = type switch
        {
            EndingType.Whistleblower    => endingWhistleblower,
            EndingType.PassiveResistance => endingPassive,
            _                           => endingComplicit,
        };

        if (data == null) return;
        if (endingTitleLabel) endingTitleLabel.text = data.title;
        if (epilogueLabel)    epilogueLabel.text    = data.epilogueText;
    }
}
