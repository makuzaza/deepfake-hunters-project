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

    private static readonly string[] LinesComplicit = {
        "You close the laptop.",
        "Outside, the city looks the same.",
        "Somewhere, Diane is watching a video of a face that isn’t quite real.",
        "She believes every word."
    };

    private static readonly string[] LinesWhistleblower = {
        "You send the files.",
        "Your access is revoked by morning.",
        "Marcus calls. You don’t pick up.",
        "Diane’s posts disappear.\nSomething, at least, is real."
    };

    private static readonly string[] LinesPassive = {
        "You refused. You walked away.",
        "The work still got done.\nSomeone else did it.",
        "Outside, the city looks the same.",
        "Diane is still watching."
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
            epilogueLabel.text = "<color=#666666><i>click to continue</i></color>";

        _lines = type switch
        {
            EndingType.Whistleblower     => LinesWhistleblower,
            EndingType.PassiveResistance => LinesPassive,
            _                            => LinesComplicit,
        };
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
