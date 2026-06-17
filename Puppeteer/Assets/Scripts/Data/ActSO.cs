// ActSO.cs — fixed version
using UnityEngine;

[CreateAssetMenu(menuName = "Puppeteer/Act")]
public class ActSO : ScriptableObject
{
    public int    actNumber;
    public string title;
    public string subtitle;
    [TextArea(2,4)]
    public string description;
}
