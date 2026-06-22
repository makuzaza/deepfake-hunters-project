// EndingController.cs — FIXED.
// Problem: read PlayerPrefs.GetInt("EndingType") which NOTHING ever wrote,
// so every playthrough showed ending 0 (Complicit).
// Fix: read the static GameManager.PendingEnding that ForceEnding already sets,
// with a PlayerPrefs fallback for safety.
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
        // Primary: the static field set by GameManager.ForceEnding before scene load.
        EndingType type = GameManager.PendingEnding;

        // Fallback (in case GameManager was destroyed): PlayerPrefs.
        if (PlayerPrefs.HasKey("EndingType"))
            type = (EndingType)PlayerPrefs.GetInt("EndingType", (int)type);

        ShowEnding(type);
    }

    private void ShowEnding(EndingType type)
    {
        EndingSO data = type switch
        {
            EndingType.Whistleblower     => endingWhistleblower,
            EndingType.PassiveResistance => endingPassive,
            _                            => endingComplicit,
        };

        if (data == null) return;
        if (endingTitleLabel) endingTitleLabel.text = data.title;
        if (epilogueLabel)    epilogueLabel.text    = data.epilogueText;
    }
}