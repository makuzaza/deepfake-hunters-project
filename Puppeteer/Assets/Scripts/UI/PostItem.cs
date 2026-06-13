// PostItem.cs  -  Assets/_Project/Scripts/UI
using UnityEngine;
using TMPro;
public class PostItem : MonoBehaviour
{
    public TMP_Text authorText;
    public TMP_Text bodyText;

    private void Awake()
    {
        if (authorText == null || bodyText == null)
        {
            var ts = GetComponentsInChildren<TMP_Text>(true);
            foreach (var t in ts)
            {
                if (t.name == "Author") authorText = t;
                else if (t.name == "Body") bodyText = t;
            }
        }
    }

    public void Bind(FeedPostSO p)
    {
        if (p == null) return;
        if (authorText) authorText.text = p.author != null ? p.author.displayName : "Unknown";
        if (bodyText) bodyText.text = p.body;
    }

    public void Bind(string author, string body)
    {
        if (authorText) authorText.text = author;
        if (bodyText) bodyText.text = body;
    }
}
