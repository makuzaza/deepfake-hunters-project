// PhoneScreen.cs — attach to Screen_Phone (full-screen, under OverlayLayer)
// "YOUR PHONE BUZZES" — shows social post, Open / Ignore
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneScreen : UIScreen
{
    [Header("Content")]
    [SerializeField] private TMP_Text posterName;     // "Marcus posted"
    [SerializeField] private TMP_Text postBody;       // post text
    [SerializeField] private TMP_Text narratorNote;   // italic note below buttons

    [Header("Buttons")]
    [SerializeField] private Button openButton;
    [SerializeField] private Button ignoreButton;

    private TaskResult _data;

    protected override void Awake()
    {
        base.Awake();
        openButton.onClick.AddListener(()   => GameEvents.PhoneClosed());
        ignoreButton.onClick.AddListener(() => GameEvents.PhoneClosed());
    }

    public void Setup(TaskResult r) { _data = r; }

    protected override void OnBeforeShow()
    {
        if (postBody)     postBody.text     = _data.dianePost;
        if (narratorNote) narratorNote.text = _data.narratorNote;
    }
}
