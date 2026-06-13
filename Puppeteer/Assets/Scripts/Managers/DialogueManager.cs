// DialogueManager.cs  -  Assets/_Project/Scripts/Managers
// STUB with a usable Show() so the foundation can put text on screen. Full
// typewriter Play(DialogueSO) coroutine lands Day 2 (Dev A). See Architecture S6.
using UnityEngine;
public class DialogueManager : Singleton<DialogueManager>
{
    public void Show(CharacterSO speaker, string text)
    {
        UIManager.I?.SetSpeaker(speaker != null ? speaker.displayName : "", speaker != null ? speaker.portrait : null);
        UIManager.I?.SetDialogue(text);
    }
    // TODO Day 2 (Dev A): IEnumerator Play(DialogueSO dlg) with typewriter + skip.
}
