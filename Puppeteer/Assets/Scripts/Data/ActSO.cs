// ActSO.cs  -  Assets/_Project/Scripts/Data
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Act_", menuName = "Puppeteer/Act")]
public class ActSO : ScriptableObject
{
    public string actTitle = "New Act";
    public DialogueSO actIntro;
    public List<TaskSO> tasks = new List<TaskSO>();
}
