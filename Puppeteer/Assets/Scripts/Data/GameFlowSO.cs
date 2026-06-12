// GameFlowSO.cs  -  Assets/_Project/Scripts/Data
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "GameFlow", menuName = "Puppeteer/Game Flow")]
public class GameFlowSO : ScriptableObject
{
    public List<ActSO> acts = new List<ActSO>();
    [Header("Endings")]
    public EndingSO endingComplicit;
    public EndingSO endingWhistleblower;
    public EndingSO endingPassive;
}
