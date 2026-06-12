// DialogueSO.cs  -  Assets/_Project/Scripts/Data
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class DialogueLine
{
    public CharacterSO speaker;
    [TextArea(2, 4)] public string defaultText;
    [Header("Optional personality variants (blank = use default)")]
    [TextArea(2, 4)] public string moneyText;
    [TextArea(2, 4)] public string impactText;
    [TextArea(2, 4)] public string recognitionText;

    public string ResolveText(ProfileType p)
    {
        switch (p)
        {
            case ProfileType.Money:       return string.IsNullOrEmpty(moneyText)       ? defaultText : moneyText;
            case ProfileType.Impact:      return string.IsNullOrEmpty(impactText)      ? defaultText : impactText;
            case ProfileType.Recognition: return string.IsNullOrEmpty(recognitionText) ? defaultText : recognitionText;
            default:                      return defaultText;
        }
    }
}

[CreateAssetMenu(fileName = "Dlg_", menuName = "Puppeteer/Dialogue")]
public class DialogueSO : ScriptableObject { public List<DialogueLine> lines = new List<DialogueLine>(); }
