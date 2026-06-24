// ContextPanelView.cs — attach to CompanyPanel
// Each screen calls Apply() in OnBeforeShow to set company / character context.
// CHANGE: portrait now always updates — including hiding it when sprite is null —
// so one screen's portrait never lingers into another.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ContextPanelView : MonoBehaviour
{
    [SerializeField] private Image     portrait;
    [SerializeField] private TMP_Text  nameLabel;
    [SerializeField] private TMP_Text  taglineLabel;

    public void Apply(string header, Sprite sprite, string charName, string tagline)
    {
        if (portrait)
        {
            portrait.sprite  = sprite;
            // Hide the image when there's no sprite so the previous portrait
            // doesn't stay visible; show it (full color) when there is one.
            portrait.enabled = sprite != null;
        }
        if (nameLabel)    nameLabel.text    = charName;
        if (taglineLabel) taglineLabel.text = tagline;
    }
}