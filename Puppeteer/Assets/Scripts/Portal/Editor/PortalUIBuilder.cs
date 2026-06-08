#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using TMPro;

public static class PortalUIBuilder
{
    static readonly Color BG      = Hex("#eae0cc");
    static readonly Color DARK    = Hex("#162329");
    static readonly Color TEAL    = Hex("#485d5c");
    static readonly Color TEAL2   = Hex("#819794");
    static readonly Color WARM    = Hex("#caae87");
    static readonly Color GREEN2  = Hex("#98a686");
    static readonly Color RED     = Hex("#8e3943");
    static readonly Color PINK    = Hex("#ceb2af");

    [MenuItem("GameObject/Portal/Build Portal UI Canvas", false, 10)]
    static void Build()
    {
        // EventSystem — required for button clicks
        if (Object.FindObjectOfType<EventSystem>() == null)
        {
            var es = new GameObject("EventSystem");
            es.AddComponent<EventSystem>();
            es.AddComponent<InputSystemUIInputModule>();
            Undo.RegisterCreatedObjectUndo(es, "Create EventSystem");
        }

        // Root canvas
        var canvasGO = CreateGO("PortalCanvas", null);
        var canvas   = canvasGO.AddComponent<Canvas>();
        canvas.renderMode   = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 20;
        var scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode         = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1280, 720);
        scaler.screenMatchMode     = CanvasScaler.ScreenMatchMode.Expand;
        canvasGO.AddComponent<GraphicRaycaster>();

        // Dark backdrop (not a raycast target)
        var backdrop = CreatePanel("Backdrop", canvasGO.transform, new Color(0,0,0,0.8f));
        Stretch(backdrop);
        backdrop.GetComponent<Image>().raycastTarget = false;

        // Portal window — fills 96% of screen
        var window = CreatePanel("PortalWindow", canvasGO.transform, DARK);
        var wRT = window.GetComponent<RectTransform>();
        wRT.anchorMin = new Vector2(0.02f, 0.02f);
        wRT.anchorMax = new Vector2(0.98f, 0.98f);
        wRT.offsetMin = wRT.offsetMax = Vector2.zero;

        // Progress bar strip at top
        var pbBG  = CreatePanel("ProgressBarBG", window.transform, new Color(0.15f,0.15f,0.15f));
        SetAnchors(pbBG,  0,1, 1,1,  0,-5, 0,0);
        var pbFill = CreatePanel("ProgressFill", pbBG.transform, TEAL);
        Stretch(pbFill);
        pbFill.GetComponent<RectTransform>().anchorMax = new Vector2(0.17f, 1);

        // Screen label
        var lbl = CreateLabel("ScreenLabel", window.transform, "SCREEN 1 / 6 — LOGIN", 13, TEAL2);
        SetAnchors(lbl, 0,1, 1,1,  8,-22, 0,0);

        // Screens
        var s1 = BuildLogin(window.transform);
        var s2 = BuildDashboard(window.transform);
        var s3 = BuildHR(window.transform);
        var s4 = BuildMarcus(window.transform);
        var s5 = BuildBriefs(window.transform);
        var s6 = BuildTask1(window.transform);

        s2.SetActive(false); s3.SetActive(false); s4.SetActive(false);
        s5.SetActive(false); s6.SetActive(false);
        canvasGO.SetActive(false);

        // Wire PortalUIController
        var ctrl = canvasGO.AddComponent<PortalUIController>();
        ctrl.screenLogin     = s1;
        ctrl.screenDashboard = s2;
        ctrl.screenHR        = s3;
        ctrl.screenMarcus    = s4;
        ctrl.screenBriefs    = s5;
        ctrl.screenTask1     = s6;
        ctrl.progressFill    = pbFill.GetComponent<Image>();
        ctrl.screenLabel     = lbl.GetComponent<TMP_Text>();
        ctrl.nameInput       = s1.GetComponentInChildren<TMP_InputField>();

        Selection.activeGameObject = canvasGO;
        Debug.Log("[PortalUIBuilder] Built. Drag PortalCanvas → ComputerInteractable.Portal field.");
    }

    // ════════════════════════════════════════════════════════════
    //  SCREEN 1 — Login  (anchor-based — works at any resolution)
    // ════════════════════════════════════════════════════════════
    static GameObject BuildLogin(Transform parent)
    {
        var s = CreatePanel("Screen1_Login", parent, BG);
        Stretch(s); PadTop(s, 26);

        // Logo — top 8% of screen, centered
        var logo = CreatePanel("Logo", s.transform, TEAL);
        SetAnchors(logo, 0.47f,0.84f, 0.53f,0.94f, 0,0,0,0);

        // Company
        var comp = CreateLabel("Company", s.transform, "HUMAN AGENCY INC.", 16, TEAL, center:true);
        SetAnchors(comp, 0.15f,0.77f, 0.85f,0.83f, 0,0,0,0);
        comp.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // Title
        var title = CreateLabel("Title", s.transform, "Welcome to Human Agency", 40, DARK, bold:true, center:true);
        SetAnchors(title, 0.1f,0.65f, 0.9f,0.76f, 0,0,0,0);
        title.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // Date
        var date = CreateLabel("Date", s.transform, "Monday, May 23  —  Employee Portal", 18, TEAL2, center:true);
        SetAnchors(date, 0.1f,0.58f, 0.9f,0.65f, 0,0,0,0);
        date.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;

        // Name label
        var nameLbl = CreateLabel("NameLbl", s.transform, "YOUR NAME", 15, TEAL);
        SetAnchors(nameLbl, 0.2f,0.50f, 0.8f,0.56f, 0,0,0,0);

        // Input field
        var inp = MakeInputFieldAnchored("NameInput", s.transform, "Type your name...", 0.2f,0.40f, 0.8f,0.49f);

        // Button
        BtnAnchored("StartBtn", s.transform, "Start your first day  →", 0.2f,0.28f, 0.8f,0.38f, DARK, BG, "login");

        return s;
    }

    // ════════════════════════════════════════════════════════════
    //  SCREEN 2 — Dashboard
    // ════════════════════════════════════════════════════════════
    static GameObject BuildDashboard(Transform parent)
    {
        var s = CreatePanel("Screen2_Dashboard", parent, Color.clear);
        Stretch(s); PadTop(s, 26);

        var (left, center, right) = ThreeCol(s.transform, WARM, BG, TEAL);
        ColHeader(left,  "COMPANY", "Human Agency",    "Digital Marketing\nEst. 2019");
        ColHeader(right, "PLAYER",  "Alex",            "Day 1 — 09:00");
        TopBar(center, "DASHBOARD — DAY 1");

        var iz = IZone(center);
        CreateLabel("IZLabel",  iz.transform, "INBOX — 3 ITEMS WAITING",                  14, TEAL2, 0,-34, 700,22);
        CreateLabel("Greeting", iz.transform, "Good morning, Alex. You have 3 items waiting.", 18, DARK,  0,-66, 700,28);

        InboxCard("CardHR",     iz.transform, "📧  HR — Onboarding Form",   "Please complete before starting work",     "NEW", RED,  -110, "card_hr");
        InboxCard("CardMarcus", iz.transform, "💬  Marcus",                 "\"hey, let me know when you're settled!\"","MSG", TEAL, -186, "card_marcus");
        InboxCard("CardBriefs", iz.transform, "📂  Brief Queue",            "2 tasks available today",                  "2",   WARM, -262, "card_briefs");

        TextBox(center, "🏢", "SYSTEM", "Good morning! You have 3 items waiting. Click any to open.");
        return s;
    }

    // ════════════════════════════════════════════════════════════
    //  SCREEN 3 — HR Form
    // ════════════════════════════════════════════════════════════
    static GameObject BuildHR(Transform parent)
    {
        var s = CreatePanel("Screen3_HR", parent, Color.clear);
        Stretch(s); PadTop(s, 26);

        var (left, center, right) = ThreeCol(s.transform, PINK, BG, TEAL);
        ColHeader(left, "HR DAISY", "Human Agency", "Human Resources");
        TopBar(center, "HR — ONBOARDING");

        var iz = IZone(center);
        CreateLabel("IZLabel", iz.transform, "EMPLOYEE ONBOARDING FORM", 14, TEAL2, 0,-34, 640,22);

        var form = CreatePanel("HRForm", iz.transform, Color.white);
        SetAnchors(form, 0.02f,0.08f, 0.98f,0.94f, 0,0, 0,0);

        float y = -20;
        Lbl(form, "HUMAN AGENCY — NEW EMPLOYEE INTAKE", 13, TEAL,  ref y, 16, 600);
        Lbl(form, "FULL NAME",                          12, TEAL2, ref y, 28, 600);
        Lbl(form, "Alex",                               20, DARK,  ref y, 20, 600, bold:true, id:"NameValue");
        Lbl(form, "START DATE",                         12, TEAL2, ref y, 32, 600);
        Lbl(form, "Monday, May 23",                     20, DARK,  ref y, 20, 600, bold:true);

        y -= 12;
        Lbl(form, "Q1. WHAT BRINGS YOU THE GREATEST FULFILMENT AT WORK?", 13, TEAL2, ref y, 18, 600);
        RadioOpt(form.transform, "Q1A", "A. Earning a fortune",                    ref y);
        RadioOpt(form.transform, "Q1B", "B. Having a positive impact on others",   ref y);
        RadioOpt(form.transform, "Q1C", "C. Receiving recognition and validation", ref y);
        y -= 8;
        Lbl(form, "Q2. HOW DO YOU PREFER RESULTS TO BE DISPLAYED?",       13, TEAL2, ref y, 18, 600);
        RadioOpt(form.transform, "Q2A", "A. Hard data and metrics",                ref y);
        RadioOpt(form.transform, "Q2B", "B. Qualitative and emotional feedback",   ref y);

        y -= 12;
        Btn("SubmitBtn", form.transform, "Submit & return to desk  →", 0, y-28, 600, 52, DARK, BG, "hr");

        TextBox(center, "D", "HR DAISY", "Hey! Please fill in this form. We'll forward results to your supervisor.");
        return s;
    }

    // ════════════════════════════════════════════════════════════
    //  SCREEN 4 — Marcus Chat
    // ════════════════════════════════════════════════════════════
    static GameObject BuildMarcus(Transform parent)
    {
        var s = CreatePanel("Screen4_Marcus", parent, Color.clear);
        Stretch(s); PadTop(s, 26);

        var (left, center, right) = ThreeCol(s.transform, GREEN2, BG, TEAL);
        ColHeader(left, "MARCUS", "Junior Account Mgr", "Joined 3 months ago");
        TopBar(center, "MESSAGES — MARCUS");

        var iz = IZone(center);
        ChatBubble(iz.transform, "Hey Alex! I'm Marcus. Looks like we're on the same team 🙂", -50);
        ChatBubble(iz.transform, "Want me to walk you through everything, or do you prefer figuring it out yourself?", -130);

        Btn("BtnGuide", iz.transform, "Walk me through it.",  0,-230, 640,56, TEAL,           Color.white, "marcus_guide");
        Btn("BtnSelf",  iz.transform, "I'll figure it out.",  0,-300, 640,56, Hex("#243030"), BG,          "marcus_self");

        TextBox(center, "M", "MARCUS", "Hey! I'm Marcus. Looks like we're on the same team.");
        return s;
    }

    // ════════════════════════════════════════════════════════════
    //  SCREEN 5 — Brief Queue
    // ════════════════════════════════════════════════════════════
    static GameObject BuildBriefs(Transform parent)
    {
        var s = CreatePanel("Screen5_Briefs", parent, Color.clear);
        Stretch(s); PadTop(s, 26);

        var (left, center, right) = ThreeCol(s.transform, TEAL, BG, TEAL);
        ColHeader(left, "BOSS", "Sr. Account Director", "\"You came to the right place.\"");
        TopBar(center, "BRIEF QUEUE — DAY 1");

        var iz = IZone(center);
        CreateLabel("IZL", iz.transform, "SELECT A TASK — THE OTHER WILL BE HANDLED BY MARCUS", 14, TEAL2, 0,-34, 700,22);

        // Card A
        var ca = CreatePanel("CardA", iz.transform, Hex("#1e2b2b"));
        AnchorCard(ca, -180, -130, 310, 260);
        CreateLabel("CL", ca.transform, "CLIENT: NATURAL REMEDIES",                             13, TEAL2,      0,-22, 280,18);
        CreateLabel("CD", ca.transform, "Edit a promo video —\nremove stutters from Tony",      16, BG,         0,-68, 280,52, bold:true);
        CreateLabel("CP", ca.transform, "€80",                                                  26, Color.white, 0,-132,280,34, bold:true);
        CreateLabel("CR", ca.transform, "RISK: NONE",                                           12, DARK,        0,-168,130,20);
        Btn("TakeA", ca.transform, "Take this task →", 0,-208, 280,46, DARK,  BG,          "task1");

        // Card B
        var cb = CreatePanel("CardB", iz.transform, Hex("#1e2b2b"));
        AnchorCard(cb, 180, -130, 310, 260);
        CreateLabel("CL", cb.transform, "CLIENT: FRESHLIFE SUPPLEMENTS",                        13, TEAL2,      0,-22, 280,18);
        CreateLabel("CD", cb.transform, "Write social media copy\nfor a new product launch",    16, BG,         0,-68, 280,52, bold:true);
        CreateLabel("CP", cb.transform, "€120",                                                 26, Color.white, 0,-132,280,34, bold:true);
        CreateLabel("CR", cb.transform, "RISK: LOW",                                            12, DARK,        0,-168,130,20);
        Btn("TakeB", cb.transform, "Take this task →", 0,-208, 280,46, TEAL2, Color.white, "task2");

        Btn("Refuse", iz.transform, "Refuse both — Marcus handles everything. You earn €0 today.", 0,-350, 680,36, Color.clear, TEAL2, "refuse");

        TextBox(center, "B", "BOSS", "Here's your queue for today. Pick what you want to work on.");
        return s;
    }

    // ════════════════════════════════════════════════════════════
    //  SCREEN 6 — Task 1 Demo
    // ════════════════════════════════════════════════════════════
    static GameObject BuildTask1(Transform parent)
    {
        var s = CreatePanel("Screen6_Task1", parent, Color.clear);
        Stretch(s); PadTop(s, 26);

        var (left, center, right) = ThreeCol(s.transform, WARM, BG, TEAL);
        ColHeader(left, "CLIENT", "Natural Remedies", "\"Natural health for\nmodern living\"");
        TopBar(center, "TASK — VIDEO EDITOR");

        var iz = IZone(center);
        CreateLabel("TaskTitle",  iz.transform, "TASK 1 — VIDEO EDITOR",                                     22, BG,    0,-44,  700,30, bold:true);
        CreateLabel("TaskClient", iz.transform, "CLIENT: NATURAL REMEDIES",                                  14, TEAL2, 0,-84,  700,22);
        CreateLabel("TaskDesc",   iz.transform, "Edit a promo video — remove stutters from Tony's recording.",17, BG,    0,-118, 680,50);

        var tl = CreatePanel("TimelineMock", iz.transform, Hex("#0d1a1a"));
        SetAnchors(tl, 0.03f,0.32f, 0.97f,0.62f, 0,0, 0,0);
        CreateLabel("TaskBody", tl.transform,
            "[ MINI-GAME COMING SOON ]\n\nYou will click red timeline segments to select them,\nthen delete to clean up the video before launching.",
            16, TEAL2, 0,0, 640,120, center:true);

        Btn("ExitBtn", iz.transform, "Exit Portal", 0,-24, 220,52, RED, Color.white, "exit");
        var exitRT = iz.transform.Find("ExitBtn").GetComponent<RectTransform>();
        exitRT.anchorMin = new Vector2(0.5f, 0);
        exitRT.anchorMax = new Vector2(0.5f, 0);
        exitRT.anchoredPosition = new Vector2(0, 40);

        TextBox(center, "M", "MARCUS", "Click red segments to select, then delete. Then launch when ready.");
        return s;
    }

    // ════════════════════════════════════════════════════════════
    //  Shared layout
    // ════════════════════════════════════════════════════════════

    static (GameObject l, GameObject c, GameObject r) ThreeCol(Transform parent,
        Color lc, Color cc, Color rc)
    {
        var l = CreatePanel("ColLeft",   parent, lc);
        var c = CreatePanel("ColCenter", parent, cc);
        var r = CreatePanel("ColRight",  parent, rc);
        SetAnchors(l, 0,    0, 0.18f, 1, 0,0,0,0);
        SetAnchors(c, 0.18f,0, 0.82f, 1, 0,0,0,0);
        SetAnchors(r, 0.82f,0, 1,     1, 0,0,0,0);
        return (l,c,r);
    }

    static void ColHeader(GameObject col, string tag, string name, string info)
    {
        CreateLabel("Tag",  col.transform, tag,  13, TEAL2,            0,-18,  200,20);
        CreateLabel("Name", col.transform, name, 17, DARK,             0,-160, 200,24, bold:true);
        CreateLabel("Info", col.transform, info, 13, Hex("#5e4149"),   0,-188, 200,44);
    }

    static void TopBar(GameObject col, string title)
    {
        var bar = CreatePanel("TopBar", col.transform, DARK);
        SetAnchors(bar, 0,1, 1,1, 0,-52, 0,0);
        CreateLabel("Title", bar.transform, title, 15, TEAL2, -12,0, 420,52);
    }

    static GameObject IZone(GameObject col)
    {
        var iz = CreatePanel("IZone", col.transform, Hex("#1e2b2b"));
        SetAnchors(iz, 0,0, 1,1, 0,0, 0,-52);
        return iz;
    }

    static void TextBox(GameObject col, string portrait, string speaker, string text)
    {
        var tb = CreatePanel("TextBox", col.transform, DARK);
        SetAnchors(tb, 0,0, 1,0, 0,0, 0,96);
        CreateLabel("Speaker", tb.transform, speaker, 13, TEAL2, 64,-14, 520,20);
        CreateLabel("Text",    tb.transform, text,    15, BG,    64,-38, 520,52);
        var pb = CreatePanel("Portrait", tb.transform, TEAL);
        SetAnchors(pb, 0,0.5f, 0,0.5f, 8,-32, 56,32);
        CreateLabel("PL", pb.transform, portrait, 22, Color.white, 0,0,48,64, bold:true, center:true);
    }

    static void InboxCard(string id, Transform parent, string title, string sub,
        string badgeTxt, Color badgeCol, float y, string action)
    {
        var card = CreatePanel(id, parent, BG);
        SetAnchors(card, 0.01f,1, 0.99f,1, 0,y-68, 0,y);
        var btn = card.AddComponent<Button>();
        btn.targetGraphic = card.GetComponent<Image>();
        var nav = btn.navigation; nav.mode = Navigation.Mode.None; btn.navigation = nav;
        card.AddComponent<PortalButton>().action = action;
        CreateLabel("Title", card.transform, title, 18, DARK,  14,-14, 580,26, bold:true);
        CreateLabel("Sub",   card.transform, sub,   14, TEAL2, 14,-44, 580,20);
        var badge = CreatePanel("Badge", card.transform, badgeCol);
        SetAnchors(badge, 1,0.5f, 1,0.5f, -80,-18, 0,18);
        CreateLabel("BT", badge.transform, badgeTxt, 14, badgeCol==WARM?DARK:Color.white, 0,0,80,36, bold:true, center:true);
    }

    static void ChatBubble(Transform parent, string text, float y)
    {
        var b = CreatePanel("Bubble", parent, Hex("#243030"));
        SetAnchors(b, 0.02f,1, 0.86f,1, 0,y-56, 0,y);
        CreateLabel("T", b.transform, text, 16, BG, 14,-10, 580,48);
    }

    static void RadioOpt(Transform parent, string id, string label, ref float y)
    {
        var go = CreateGO(id, parent);
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1);
        rt.sizeDelta        = new Vector2(580, 26);
        rt.anchoredPosition = new Vector2(14, y);
        go.AddComponent<Toggle>();
        CreateLabel("L", go.transform, label, 15, DARK, 24,0,556,26);
        y -= 30;
    }

    // Helper: label with ref y (advances y)
    static void Lbl(GameObject parent, string text, float size, Color col,
        ref float y, float h, float w, bool bold=false, string id=null)
    {
        var go = CreateLabel(id ?? text.Substring(0,Mathf.Min(4,text.Length)),
            parent.transform, text, size, col, 14, y, w, h, bold:bold);
        y -= (h + 6);
    }

    static void AnchorCard(GameObject go, float x, float y, float w, float h)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1);
        rt.sizeDelta        = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(x, y);
    }

    // ════════════════════════════════════════════════════════════
    //  Factories
    // ════════════════════════════════════════════════════════════

    // Btn — uses PortalButton (serialized, survives scene save)
    static GameObject Btn(string name, Transform parent, string label,
        float x, float y, float w, float h, Color bg, Color fg, string action)
    {
        var go  = CreatePanel(name, parent, bg);
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1);
        rt.sizeDelta        = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(x, y);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();
        var nav = btn.navigation; nav.mode = Navigation.Mode.None; btn.navigation = nav;
        go.AddComponent<PortalButton>().action = action;
        CreateLabel("Label", go.transform, label, 17, fg, 0,0,w,h, bold:true, center:true);
        return go;
    }

    // Anchor-based input field (fills the rect defined by anchors)
    static GameObject MakeInputFieldAnchored(string name, Transform parent, string placeholder,
        float axMin, float ayMin, float axMax, float ayMax)
    {
        var go = CreatePanel(name, parent, Color.white);
        SetAnchors(go, axMin, ayMin, axMax, ayMax, 0,0,0,0);

        var area = CreateGO("TextArea", go.transform);
        area.AddComponent<RectMask2D>();
        var aRT = area.GetComponent<RectTransform>();
        aRT.anchorMin = Vector2.zero; aRT.anchorMax = Vector2.one;
        aRT.offsetMin = new Vector2(12,-4); aRT.offsetMax = new Vector2(-12,4);

        var txt = CreateLabel("Text",        area.transform, "",           22, DARK,                       0,0,400,50);
        var ph  = CreateLabel("Placeholder", area.transform, placeholder,  22, new Color(0.5f,0.5f,0.5f), 0,0,400,50);
        Stretch(txt); Stretch(ph);

        var field = go.AddComponent<TMP_InputField>();
        field.textViewport  = aRT;
        field.textComponent = txt.GetComponent<TMP_Text>();
        field.placeholder   = ph.GetComponent<TMP_Text>();
        field.text          = "Alex";
        return go;
    }

    // Anchor-based button
    static GameObject BtnAnchored(string name, Transform parent, string label,
        float axMin, float ayMin, float axMax, float ayMax, Color bg, Color fg, string action)
    {
        var go = CreatePanel(name, parent, bg);
        SetAnchors(go, axMin, ayMin, axMax, ayMax, 0,0,0,0);
        var btn = go.AddComponent<Button>();
        btn.targetGraphic = go.GetComponent<Image>();
        var nav = btn.navigation; nav.mode = Navigation.Mode.None; btn.navigation = nav;
        go.AddComponent<PortalButton>().action = action;
        var lbl = CreateLabel("Label", go.transform, label, 20, fg, bold:true, center:true);
        Stretch(lbl);
        lbl.GetComponent<TMP_Text>().alignment = TextAlignmentOptions.Center;
        return go;
    }

    static GameObject MakeInputField(string name, Transform parent, string placeholder,
        float x, float y, float w, float h)
    {
        var go  = CreatePanel(name, parent, Color.white);
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1);
        rt.sizeDelta        = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(x, y);

        var area = CreateGO("TextArea", go.transform);
        area.AddComponent<RectMask2D>();
        var aRT = area.GetComponent<RectTransform>();
        aRT.anchorMin = Vector2.zero; aRT.anchorMax = Vector2.one;
        aRT.offsetMin = new Vector2(12,-4); aRT.offsetMax = new Vector2(-12,4);

        var txt = CreateLabel("Text",        area.transform, "",          22, DARK,                        0,0,w,h);
        var ph  = CreateLabel("Placeholder", area.transform, placeholder, 22, new Color(0.5f,0.5f,0.5f), 0,0,w,h);

        var field = go.AddComponent<TMP_InputField>();
        field.textViewport  = aRT;
        field.textComponent = txt.GetComponent<TMP_Text>();
        field.placeholder   = ph.GetComponent<TMP_Text>();
        field.text          = "Alex";
        return go;
    }

    static GameObject CreateGO(string name, Transform parent)
    {
        var go = new GameObject(name);
        if (parent) go.transform.SetParent(parent, false);
        go.AddComponent<RectTransform>();
        return go;
    }

    static GameObject CreatePanel(string name, Transform parent, Color col)
    {
        var go  = CreateGO(name, parent);
        go.AddComponent<Image>().color = col;
        return go;
    }

    static GameObject CreateLabel(string name, Transform parent, string text,
        float size, Color col, float x=0, float y=0, float w=400, float h=30,
        bool bold=false, bool center=false)
    {
        var go  = CreateGO(name, parent);
        var rt  = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1);
        rt.sizeDelta        = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(x, y);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.text              = text;
        tmp.fontSize          = size;
        tmp.color             = col;
        tmp.fontStyle         = bold ? FontStyles.Bold : FontStyles.Normal;
        tmp.alignment         = center ? TextAlignmentOptions.Center : TextAlignmentOptions.TopLeft;
        tmp.enableWordWrapping = true;
        return go;
    }

    // ════════════════════════════════════════════════════════════
    //  RectTransform helpers
    // ════════════════════════════════════════════════════════════

    static void Stretch(GameObject go)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero; rt.anchorMax = Vector2.one;
        rt.offsetMin = rt.offsetMax = Vector2.zero;
    }

    static void PadTop(GameObject go, float top)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.offsetMax = new Vector2(rt.offsetMax.x, -top);
    }

    static void SetAnchors(GameObject go,
        float axMin, float ayMin, float axMax, float ayMax,
        float oxMin, float oyMin, float oxMax, float oyMax)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(axMin, ayMin);
        rt.anchorMax = new Vector2(axMax, ayMax);
        rt.offsetMin = new Vector2(oxMin, oyMin);
        rt.offsetMax = new Vector2(oxMax, oyMax);
    }

    static void Center(GameObject go, float w, float h)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f,0.5f);
        rt.sizeDelta        = new Vector2(w, h);
        rt.anchoredPosition = Vector2.zero;
    }

    static void TopCenter(GameObject go, float w, float h, float yOffset)
    {
        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = rt.anchorMax = new Vector2(0.5f, 1);
        rt.sizeDelta        = new Vector2(w, h);
        rt.anchoredPosition = new Vector2(0, yOffset);
    }

    static Color Hex(string h) { ColorUtility.TryParseHtmlString(h, out var c); return c; }
}
#endif
