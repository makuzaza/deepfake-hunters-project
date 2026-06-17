// FeedPostSO.cs — FINAL. Matches exact field access in your PostItem.cs:
// PostItem calls: p.author.displayName  (so author is a CharacterSO)
//                 p.body
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Feed Post")]
public class FeedPostSO : ScriptableObject
{
    [Header("Content")]
    public CharacterSO author;        // PostItem uses author.displayName
    public string      body;
    public string      subline;
    public string      narratorNote;

    [Header("Convenience fields (used by CreateContentAssets)")]
    public string      poster;        // display name string fallback
    public string      avatarLetter;

    [Header("Flow")]
    public int         taskIndex;     // shown after this task number completes
    public bool        isHarmful;
}
