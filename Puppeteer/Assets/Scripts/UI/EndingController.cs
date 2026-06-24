using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [Header("Ending display refs")]
    [SerializeField] private TMP_Text   endingTitleLabel;
    [SerializeField] private TMP_Text   epilogueLabel;
    [SerializeField] private GameObject returnButton;

    [Header("Ending data assets")]
    [SerializeField] private EndingSO endingComplicit;
    [SerializeField] private EndingSO endingWhistleblower;
    [SerializeField] private EndingSO endingPassive;

    private string[] _lines;
    private int      _lineIndex;
    private bool     _finished;

    private static readonly string[] EndingLines = {
        "<color=#aaaaaa>A quick reality check:</color>",
        "Deepfake files surged from <b>500K (2023) → 8M (2025)</b>.",
        "Fraud attempts spiked <b>3,000%</b> in 2023,\nwith 1,740% growth in North America.",
        "Voice cloning is the top attack vector:\n<b>cheap, fast, and convincing.</b>",
        "Human detection rates are just <b>24.5%</b> for high-quality video.",
        "You are more than just someone pressing buttons.\n\n<b>The question is: what role will you choose to play now?</b>"
    };

    private void Start()
    {
        if (returnButton != null) returnButton.SetActive(false);

        EndingType type = GameManager.PendingEnding;
        if (PlayerPrefs.HasKey("EndingType"))
            type = (EndingType)PlayerPrefs.GetInt("EndingType", (int)type);

        EndingSO data = type switch
        {
            EndingType.Whistleblower     => endingWhistleblower,
            EndingType.PassiveResistance => endingPassive,
            _                            => endingComplicit,
        };

        string title = data != null && !string.IsNullOrEmpty(data.title) ? data.title : type.ToString();
        if (endingTitleLabel != null) endingTitleLabel.text = title;

        if (epilogueLabel != null)
            epilogueLabel.text = "<color=#555555><i>click to continue</i></color>";

        _lines = EndingLines;
    }

    private void Update()
    {
        if (_finished) return;

        bool clicked = (Mouse.current    != null && Mouse.current.leftButton.wasPressedThisFrame) ||
                       (Keyboard.current != null && (Keyboard.current.spaceKey.wasPressedThisFrame ||
                                                     Keyboard.current.enterKey.wasPressedThisFrame));
        if (!clicked) return;

        Advance();
    }

    private void Advance()
    {
        if (_lineIndex < _lines.Length)
        {
            string line = _lines[_lineIndex++];
            if (epilogueLabel != null)
                epilogueLabel.text = _lineIndex == 1 ? line : epilogueLabel.text + "\n\n" + line;
        }

        if (_lineIndex >= _lines.Length && !_finished)
        {
            _finished = true;
            if (returnButton != null) returnButton.SetActive(true);
        }
    }

    public void GoToMainMenu() => SceneManager.LoadScene("MainMenu");
}
