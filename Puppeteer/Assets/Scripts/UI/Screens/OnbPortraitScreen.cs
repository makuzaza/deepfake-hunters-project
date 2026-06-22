// OnbPortraitScreen.cs — attach to Onb_Portrait GameObject
// Serialized refs: portraitButtons (array of 3), continueButton
using UnityEngine;
using UnityEngine.UI;

public class OnbPortraitScreen : UIScreen
{
    [Header("References")]
    [SerializeField] private Button[] portraitButtons;   // length 3
    [SerializeField] private Button   continueButton;

    [Header("Colors")]
    [SerializeField] private Color selectedColor   = new Color(0.79f, 0.69f, 0.53f); // warm #caae87
    [SerializeField] private Color unselectedColor = new Color(0.28f, 0.36f, 0.36f); // teal #485d5c

    private int _selected = -1;

    protected override void Awake()
    {
        base.Awake();
        for (int i = 0; i < portraitButtons.Length; i++)
        {
            int idx = i;
            portraitButtons[i].onClick.AddListener(() => Select(idx));
        }
        continueButton.onClick.AddListener(OnContinue);
        continueButton.interactable = false;
    }

    protected override void OnBeforeShow() { _selected = -1; continueButton.interactable = false; ResetColors(); }

    private void Select(int idx)
    {
        _selected = idx;
        continueButton.interactable = true;
        for (int i = 0; i < portraitButtons.Length; i++)
            portraitButtons[i].GetComponent<Image>().color = (i == idx) ? selectedColor : unselectedColor;
    }

    private void ResetColors() { foreach (var b in portraitButtons) b.GetComponent<Image>().color = unselectedColor; }

    private void OnContinue()
    {
        if (_selected < 0) return;
        GameEvents.PortraitChosen(_selected);
    }
}
