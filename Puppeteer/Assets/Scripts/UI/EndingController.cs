// EndingController.cs  -  Assets/_Project/Scripts/UI
// Reads GameManager.PendingEnding (carried across the scene load) and shows the
// authored EndingSO loaded from Resources/Endings. No scene reference required.
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class EndingController : MonoBehaviour
{
    public Transform uiRoot;

    private void Start()
    {
        var e = GameManager.PendingEnding;
        var so = Resources.Load<EndingSO>("Endings/Ending_" + e);

        var title = Text("EndingTitle");
        var body  = Text("EndingDescription");
        if (title) title.text = Headline(e);
        if (body)  body.text  = so != null && !string.IsNullOrEmpty(so.epilogueText) ? so.epilogueText : Fallback(e);

        var back = Btn("ReturnToMenuButton");
        if (back) back.onClick.AddListener(() => SceneManager.LoadScene("MainMenu"));
    }

    private TMP_Text Text(string n){ var t = UIFind.Deep(uiRoot ? uiRoot : transform, n); return t ? t.GetComponent<TMP_Text>() : null; }
    private Button   Btn(string n) { var t = UIFind.Deep(uiRoot ? uiRoot : transform, n); return t ? t.GetComponent<Button>() : null; }

    private string Headline(EndingType e)
    {
        switch (e)
        {
            case EndingType.Whistleblower:     return "ENDING B - EXPOSE THE TRUTH";
            case EndingType.PassiveResistance: return "ENDING C - PASSIVE RESISTANCE";
            default:                           return "ENDING A - COOPERATE";
        }
    }
    private string Fallback(EndingType e)
    {
        switch (e)
        {
            case EndingType.Whistleblower:     return "You leaked the evidence. It cost you everything. It was worth it.";
            case EndingType.PassiveResistance: return "You never pressed launch. They never knew why.";
            default:                           return "You took the bonus and signed the NDA. You were very good at your job.";
        }
    }
}
