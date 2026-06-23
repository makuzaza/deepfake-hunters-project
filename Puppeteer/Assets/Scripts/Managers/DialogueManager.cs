// DialogueManager.cs  -  Assets/_Project/Scripts/Managers
// STUB with a usable Show() so the foundation can put text on screen. Full
// typewriter Play(DialogueSO) coroutine lands Day 2 (Dev A). See Architecture S6.
using UnityEngine;
public class DialogueManager : MonoBehaviour
{
    [SerializeField] private UIManager uiManager;

    public void Show(CharacterSO speaker, string text)
    {
        if (uiManager == null)
        {
            Debug.LogWarning("DialogueManager missing UIManager reference.");
            return;
        }

        uiManager.SetSpeaker(speaker != null ? speaker.displayName : "", speaker != null ? speaker.portrait : null);
        uiManager.SetDialogue(text);
    }
    // TODO Day 2 (Dev A): IEnumerator Play(DialogueSO dlg) with typewriter + skip.
}
