// ContextPanelView.cs — attach to CompanyPanel
// Each screen calls Apply() in OnBeforeShow to set company / character context.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextPanelView : MonoBehaviour
{
    // [SerializeField] private TMP_Text  headerLabel;    // "COMPANY" / "COLLEAGUE" / etc.
    [SerializeField] private Image     portrait;
    [SerializeField] private TMP_Text  nameLabel;
    [SerializeField] private TMP_Text  taglineLabel;

    public void Apply(string header, Sprite sprite, string charName, string tagline)
    {
        // if (headerLabel)  headerLabel.text  = header;
        if (portrait && sprite) portrait.sprite = sprite;
        if (nameLabel)    nameLabel.text    = charName;
        if (taglineLabel) taglineLabel.text = tagline;
    }
}
