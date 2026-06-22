// HRFormScreen.cs — attach to Screen_HRForm under ContentArea
// Q1 answers set the player motivation profile.
using UnityEngine;
using UnityEngine.UI;

public class HRFormScreen : UIScreen
{
    [Header("Q1 — Fulfilment (determines Motivation)")]
    [SerializeField] private Button q1Money;        // "A. Earning a fortune"
    [SerializeField] private Button q1Impact;       // "B. Having a positive impact"
    [SerializeField] private Button q1Recognition;  // "C. Receiving recognition"

    [Header("Q2 — Results preference")]
    [SerializeField] private Button q2Data;
    [SerializeField] private Button q2Qualitative;

    [Header("Q3 — Career concern")]
    [SerializeField] private Button q3AI;
    [SerializeField] private Button q3Fired;
    [SerializeField] private Button q3Invisible;

    [Header("Submit")]
    [SerializeField] private Button submitButton;

    [Header("Context")]
    [SerializeField] private ContextPanelView contextPanel;
    [SerializeField] private SpeakerStripView speakerStrip;

    [Header("Colors")]
    [SerializeField] private Color selectedColor   = new Color(0.79f, 0.69f, 0.53f);
    [SerializeField] private Color unselectedColor = new Color(0.31f, 0.20f, 0.29f); // plum

    private int _q1=-1, _q2=-1, _q3=-1;

    protected override void Awake()
    {
        base.Awake();
        AddQ(q1Money,0,ref _q1,1); AddQ(q1Impact,1,ref _q1,1); AddQ(q1Recognition,2,ref _q1,1);
        AddQ(q2Data,0,ref _q2,2); AddQ(q2Qualitative,1,ref _q2,2);
        AddQ(q3AI,0,ref _q3,3); AddQ(q3Fired,1,ref _q3,3); AddQ(q3Invisible,2,ref _q3,3);
        submitButton.onClick.AddListener(OnSubmit);
        submitButton.interactable = false;
    }

    private void AddQ(Button btn, int val, ref int field, int qNum)
    {
        int v=val; int q=qNum;
        btn.onClick.AddListener(() => { SetQ(q,v); CheckSubmit(); });
    }

    private void SetQ(int q, int v)
    {
        Button[] grp = q==1 ? new[]{q1Money,q1Impact,q1Recognition}
                     : q==2 ? new[]{q2Data,q2Qualitative}
                     :        new[]{q3AI,q3Fired,q3Invisible};
        for(int i=0;i<grp.Length;i++) grp[i].GetComponent<Image>().color = (i==v)?selectedColor:unselectedColor;
        if(q==1) _q1=v; else if(q==2) _q2=v; else _q3=v;
    }

    private void CheckSubmit() { submitButton.interactable = (_q1>=0 && _q2>=0 && _q3>=0); }

    protected override void OnBeforeShow()
    {
        _q1=_q2=_q3=-1; submitButton.interactable=false;
        ResetGroup(q1Money,q1Impact,q1Recognition);
        ResetGroup(q2Data,q2Qualitative);
        ResetGroup(q3AI,q3Fired,q3Invisible);
        if (contextPanel) contextPanel.Apply("HR", null, "HR Daisy", "Human Resources");
        if (speakerStrip) speakerStrip.Say("HR DAISY", "Hey! Thank you for being on time! Please fill in this form.");
    }

    private void ResetGroup(params Button[] btns)
    { foreach(var b in btns) b.GetComponent<Image>().color = unselectedColor; }

    private void OnSubmit()
    {
        Motivation m = _q1==0 ? Motivation.Money : _q1==1 ? Motivation.Impact : Motivation.Recognition;
        GameEvents.MotivationSet(m);
    }
}
