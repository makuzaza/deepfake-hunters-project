// FeedPostSO.cs  -  Assets/_Project/Scripts/Data
using UnityEngine;
[CreateAssetMenu(fileName = "Post_", menuName = "Puppeteer/Feed Post")]
public class FeedPostSO : ScriptableObject
{
    public CharacterSO author;
    [TextArea(2, 4)] public string body;
    public string timeLabel = "09:00";
    public string metric;
    [Tooltip("Client praise: always positive, styled apart from real-world posts.")]
    public bool isClientFeedback;
}
