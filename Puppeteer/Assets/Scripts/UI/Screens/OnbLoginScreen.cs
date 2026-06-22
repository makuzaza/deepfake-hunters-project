// OnbLoginScreen.cs — attach to Onb_login GameObject
// Serialized refs: nameInput, continueButton
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class OnbLoginScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private TMP_InputField nameInput;
    [SerializeField] private Button         continueButton;

    protected override void Awake()
    {
        base.Awake();
        continueButton.onClick.AddListener(OnContinue);
        nameInput.onValueChanged.AddListener(v => continueButton.interactable = v.Trim().Length > 0);
        continueButton.interactable = false;
    }

    protected override void OnBeforeShow() { nameInput.text = ""; continueButton.interactable = false; }

    private void OnContinue()
    {
        string n = nameInput.text.Trim();
        if (string.IsNullOrEmpty(n)) return;
        GameEvents.NameEntered(n);
    }
}
