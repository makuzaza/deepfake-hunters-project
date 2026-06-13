using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class SculptingManager : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    [Header("Scene References")]
    public Image         overlayImage;
    public RectTransform rightHand;
    public RectTransform canvasRectRef;
    public Text          accuracyText;
    public Slider        accuracySlider;
    public Slider        brushSlider;
    public Font          pixelFont;

    [Header("Difficulty")]
    public Image[] levelShapes = new Image[3]; // 0=Easy 1=Medium 2=Hard
    public int     difficulty  = 2;

    Image  shapeImage;
    int    currentDifficulty;
    Button nextButton;

    Text  resultText;
    Text  topAccuracyText;
    Text  topTimerText;
    float elapsedTime = 0f;

    [Header("Brush")]
    public float brushRadius    = 35f;
    public float brushMinRadius = 5f;
    public float brushMaxRadius = 80f;

    [Header("Hand settings")]
    public Vector2 rightHandFingerOffset = new(0f, -230f);
    public float   handFollowSpeed       = 14f;

    [Header("Undo")]
    public int maxUndoSteps = 15;

    // ── private ────────────────────────────────────────────────────
    Texture2D     overlayTex;
    RectTransform overlayRect;

    float accuracy  = 0f;
    bool  confirmed = false;

    bool[] shapeMask;
    int    totalShapePixels;

    readonly Stack<Color32[]> undoStack = new();

    // ──────────────────────────────────────────────────────────────
    void Start()
    {
        overlayRect = overlayImage.rectTransform;

        currentDifficulty = difficulty;
        for (int i = 0; i < levelShapes.Length; i++)
            if (levelShapes[i] != null)
                levelShapes[i].enabled = (i == currentDifficulty);
        shapeImage = currentDifficulty < levelShapes.Length ? levelShapes[currentDifficulty] : null;

        if (rightHand != null)
            rightHand.SetAsLastSibling();

        if (brushSlider != null)
        {
            brushSlider.minValue = brushMinRadius;
            brushSlider.maxValue = brushMaxRadius;
            brushSlider.value    = brushRadius;
            brushSlider.onValueChanged.AddListener(v => brushRadius = v);
        }

        BuildResultText();
        BuildNextButton();
        BuildTopHUD();
        BuildOverlay();
    }

    Font GetFont() => pixelFont != null
        ? pixelFont
        : Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");

    // ── Result text ────────────────────────────────────────────────
    void BuildResultText()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("ResultText", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        resultText = go.AddComponent<Text>();
        resultText.font          = GetFont();
        resultText.fontSize      = 72;
        resultText.fontStyle     = FontStyle.Bold;
        resultText.alignment     = TextAnchor.MiddleCenter;
        resultText.color         = Color.white;
        resultText.raycastTarget = false;

        var outline = go.AddComponent<Outline>();
        outline.effectColor    = Color.black;
        outline.effectDistance = new Vector2(3, -3);

        go.SetActive(false);
    }

    // ── Next Level button ──────────────────────────────────────────
    void BuildNextButton()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("NextButton", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, -200f);
        rt.sizeDelta        = new Vector2(320f, 72f);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.55f, 0.15f, 1f);

        nextButton = go.AddComponent<Button>();
        nextButton.targetGraphic = img;
        nextButton.onClick.AddListener(OnNext);

        var label = new GameObject("Label", typeof(RectTransform));
        label.transform.SetParent(go.transform, false);
        var lrt = label.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

        var txt = label.AddComponent<Text>();
        txt.font          = GetFont();
        txt.fontSize      = 42;
        txt.fontStyle     = FontStyle.Bold;
        txt.alignment     = TextAnchor.MiddleCenter;
        txt.color         = Color.white;
        txt.raycastTarget = false;
        txt.text          = "Next Level →";

        var outline = label.AddComponent<Outline>();
        outline.effectColor    = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        go.SetActive(false);
    }

    public void OnNext()
    {
        currentDifficulty++;
        if (currentDifficulty >= levelShapes.Length) return;

        // Switch shape — toggle Image.enabled so the overlay child of ShapeHard stays active
        if (levelShapes[currentDifficulty - 1] != null)
            levelShapes[currentDifficulty - 1].enabled = false;
        if (levelShapes[currentDifficulty] != null)
            levelShapes[currentDifficulty].enabled = true;
        shapeImage = levelShapes[currentDifficulty];

        // Reset state
        confirmed     = false;
        accuracy      = 0f;
        elapsedTime   = 0f;
        undoStack.Clear();

        if (nextButton  != null) nextButton.gameObject.SetActive(false);
        if (resultText  != null) resultText.gameObject.SetActive(false);
        if (topAccuracyText != null) topAccuracyText.text = "Accuracy: 0%";
        if (topTimerText    != null) topTimerText.text    = "0:00";

        // Re-enable buttons and slider
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            foreach (var btn in canvas.GetComponentsInChildren<Button>())
                btn.interactable = true;
        if (brushSlider != null) brushSlider.interactable = true;

        BuildOverlay();
    }

    // ── Top HUD (accuracy + timer) ────────────────────────────────
    void BuildTopHUD()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        topAccuracyText = MakeHUDLabel(canvas, "HUD_Accuracy", new Vector2(0, 1), new Vector2(0, 1), new Vector2(0, 1), new Vector2(32, -24), new Vector2(320, 60));
        topAccuracyText.alignment = TextAnchor.UpperLeft;
        topAccuracyText.text = "Accuracy: 0%";

        topTimerText = MakeHUDLabel(canvas, "HUD_Timer", new Vector2(1, 1), new Vector2(1, 1), new Vector2(1, 1), new Vector2(-32, -24), new Vector2(200, 60));
        topTimerText.alignment = TextAnchor.UpperRight;
        topTimerText.text = "0:00";
    }

    Text MakeHUDLabel(Canvas canvas, string objName,
        Vector2 anchorMin, Vector2 anchorMax, Vector2 pivot,
        Vector2 anchoredPos, Vector2 sizeDelta)
    {
        var go = new GameObject(objName, typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = anchorMin;
        rt.anchorMax        = anchorMax;
        rt.pivot            = pivot;
        rt.anchoredPosition = anchoredPos;
        rt.sizeDelta        = sizeDelta;

        var txt = go.AddComponent<Text>();
        txt.font          = GetFont();
        txt.fontSize      = 42;
        txt.fontStyle     = FontStyle.Bold;
        txt.color         = Color.white;
        txt.raycastTarget = false;

        var outline = go.AddComponent<Outline>();
        outline.effectColor    = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        return txt;
    }

    // ── Right hand follows cursor every frame ─────────────────────
    void Update()
    {
        if (!confirmed)
        {
            elapsedTime += Time.deltaTime;
            if (topTimerText != null)
            {
                int m = (int)(elapsedTime / 60);
                int s = (int)(elapsedTime % 60);
                topTimerText.text = $"{m}:{s:D2}";
            }
        }

        if (rightHand == null || Mouse.current == null) return;

        RectTransform cRect = canvasRectRef != null
            ? canvasRectRef
            : GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
        if (cRect == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cRect, screenPos, null, out Vector2 mouseCanvas);

        rightHand.anchoredPosition = Vector2.Lerp(
            rightHand.anchoredPosition,
            mouseCanvas + rightHandFingerOffset,
            Time.deltaTime * handFollowSpeed);
    }

    // ── Overlay texture ────────────────────────────────────────────
    void BuildOverlay()
    {
        Canvas.ForceUpdateCanvases();

        int w = Mathf.RoundToInt(overlayRect.rect.width);
        int h = Mathf.RoundToInt(overlayRect.rect.height);

        Debug.Log($"[BuildOverlay] w={w} h={h} shapeImage={shapeImage?.name ?? "NULL"} sprite={shapeImage?.sprite?.name ?? "NULL"}");

        if (w <= 0 || h <= 0) { Invoke(nameof(BuildOverlay), 0.05f); return; }

        overlayTex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            { filterMode = FilterMode.Bilinear };

        Color32 raw  = new(30, 200, 160, 200);
        Color32[] px = new Color32[w * h];
        for (int i = 0; i < px.Length; i++) px[i] = raw;
        overlayTex.SetPixels32(px);
        overlayTex.Apply();

        overlayImage.sprite = Sprite.Create(overlayTex,
            new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);

        BuildShapeMask(w, h);
    }

    void BuildShapeMask(int overlayW, int overlayH)
    {
        shapeMask        = null;
        totalShapePixels = 0;

        if (shapeImage == null || shapeImage.sprite == null) return;

        Texture2D shapeTex = shapeImage.sprite.texture;
        if (shapeTex == null) return;

        RectTransform shapeRect   = shapeImage.rectTransform;
        Rect          shapeRLocal = shapeRect.rect;
        Rect          overlayRLocal = overlayRect.rect;
        Rect          spriteRect  = shapeImage.sprite.textureRect;

        int texOX = Mathf.RoundToInt(spriteRect.x);
        int texOY = Mathf.RoundToInt(spriteRect.y);
        int texW  = Mathf.RoundToInt(spriteRect.width);
        int texH2 = Mathf.RoundToInt(spriteRect.height);

        shapeMask = new bool[overlayW * overlayH];

        for (int py = 0; py < overlayH; py++)
        for (int px2 = 0; px2 < overlayW; px2++)
        {
            // Pixel center in overlay's own local space
            float lx = overlayRLocal.x + (px2 + 0.5f) / overlayW * overlayRLocal.width;
            float ly = overlayRLocal.y + (py  + 0.5f) / overlayH * overlayRLocal.height;

            // Convert to world, then to shape image local space
            Vector3 worldPt    = overlayRect.TransformPoint(lx, ly, 0f);
            Vector2 shapePt    = shapeRect.InverseTransformPoint(worldPt);

            // Skip if outside shape image rect
            if (!shapeRLocal.Contains(shapePt)) continue;

            // [0,1] UV within shape rect
            float u = (shapePt.x - shapeRLocal.x) / shapeRLocal.width;
            float v = (shapePt.y - shapeRLocal.y) / shapeRLocal.height;

            int sx = texOX + Mathf.Clamp(Mathf.RoundToInt(u * texW),  0, texW  - 1);
            int sy = texOY + Mathf.Clamp(Mathf.RoundToInt(v * texH2), 0, texH2 - 1);

            if (shapeTex.GetPixel(sx, sy).a > 0.1f)
            {
                shapeMask[py * overlayW + px2] = true;
                totalShapePixels++;
            }
        }

        Debug.Log($"[ShapeMask] {totalShapePixels} / {overlayW * overlayH} pixels inside shape");
    }

    // ── Sculpting input ────────────────────────────────────────────
    public void OnPointerDown(PointerEventData e)
    {
        Debug.Log($"[Click] overlayTex={overlayTex != null} confirmed={confirmed}");
        if (overlayTex == null || confirmed) return;
        PushUndo();
        Sculpt(e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        if (overlayTex == null || confirmed) return;
        Sculpt(e.position);
    }

    public void OnPointerUp(PointerEventData e) { }

    void Sculpt(Vector2 screenPos)
    {
        if (!RectTransformUtility.ScreenPointToLocalPointInRectangle(
                overlayRect, screenPos, null, out Vector2 local)) return;

        int texW = overlayTex.width;
        int texH = overlayTex.height;

        float cx01 = (local.x + overlayRect.rect.width  * 0.5f) / overlayRect.rect.width;
        float cy01 = (local.y + overlayRect.rect.height * 0.5f) / overlayRect.rect.height;

        int px = Mathf.RoundToInt(cx01 * texW);
        int py = Mathf.RoundToInt(cy01 * texH);
        int r  = Mathf.RoundToInt(brushRadius / overlayRect.rect.width * texW);

        EraseCircle(px, py, r);
        overlayTex.Apply();
        RefreshAccuracy();
    }

    void EraseCircle(int cx, int cy, int r)
    {
        int w = overlayTex.width, h = overlayTex.height, r2 = r * r;
        for (int dx = -r; dx <= r; dx++)
        for (int dy = -r; dy <= r; dy++)
        {
            if (dx * dx + dy * dy > r2) continue;
            int x = cx + dx, y = cy + dy;
            if (x < 0 || x >= w || y < 0 || y >= h) continue;
            overlayTex.SetPixel(x, y, Color.clear);
        }
    }

    // ── Accuracy ───────────────────────────────────────────────────
    void RefreshAccuracy()
    {
        Color32[] px = overlayTex.GetPixels32();

        if (shapeMask != null && totalShapePixels > 0)
        {
            // intersection  = erased AND outside (correct strokes)
            // missedOutside = not erased AND outside (area still to clear)
            // clearedInside = erased AND inside body (over-erase, penalised lightly)
            int intersection = 0, missedOutside = 0, clearedInside = 0;
            for (int i = 0; i < px.Length; i++)
            {
                bool erased  = px[i].a < 10;
                bool outside = !shapeMask[i];
                if      ( erased &&  outside) intersection++;
                else if (!erased &&  outside) missedOutside++;
                else if ( erased && !outside) clearedInside++;
            }
            float[] penalties = { 0.1f, 0.15f, 0.2f };
            float penalty     = penalties[Mathf.Clamp(currentDifficulty, 0, penalties.Length - 1)];
            float outsideTotal = intersection + missedOutside;
            float progress     = outsideTotal > 0f ? (float)intersection / outsideTotal : 0f;
            float insideRatio  = (float)clearedInside / totalShapePixels;
            accuracy = Mathf.Clamp01(progress - penalty * insideRatio) * 100f;
        }
        else
        {
            int cleared = 0;
            foreach (Color32 c in px) if (c.a < 10) cleared++;
            accuracy = (float)cleared / px.Length * 100f;
        }

        if (accuracyText)      accuracyText.text      = $"{accuracy:F0}%";
        if (accuracySlider)    accuracySlider.value    = accuracy / 100f;
        if (topAccuracyText)   topAccuracyText.text    = $"Accuracy: {Mathf.FloorToInt(accuracy)}%";
        Debug.Log($"Accuracy: {accuracy:F1}%");
    }

    // ── Undo ───────────────────────────────────────────────────────
    void PushUndo()
    {
        undoStack.Push(overlayTex.GetPixels32());
        if (undoStack.Count <= maxUndoSteps) return;
        var tmp = new List<Color32[]>(undoStack);
        tmp.RemoveAt(tmp.Count - 1);
        undoStack.Clear();
        for (int i = tmp.Count - 1; i >= 0; i--) undoStack.Push(tmp[i]);
    }

    public void Undo()
    {
        if (undoStack.Count == 0) return;
        overlayTex.SetPixels32(undoStack.Pop());
        overlayTex.Apply();
        RefreshAccuracy();
    }

    // ── Button handlers ────────────────────────────────────────────
    public void OnConfirm()
    {
        confirmed = true;

        if (brushSlider != null) brushSlider.interactable = false;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            foreach (var btn in canvas.GetComponentsInChildren<Button>())
                btn.interactable = false;

        bool success     = accuracy >= 90f;
        bool isLastLevel = currentDifficulty >= levelShapes.Length - 1;

        string msg = isLastLevel
            ? (success
                ? $"All Done!\nAccuracy: {accuracy:F1}%\nPerfect work!"
                : $"All Done!\nAccuracy: {accuracy:F1}%\nBetter luck next time!")
            : (success
                ? $"Success!\nAccuracy: {accuracy:F1}%\nGreat sculpting work!"
                : $"Fail!\nAccuracy: {accuracy:F1}%\nTry to stay on the edges!");

        Debug.Log(msg);
        if (resultText != null)
        {
            resultText.text  = msg;
            resultText.color = success ? new Color(0.2f, 0.85f, 0.3f) : new Color(0.9f, 0.2f, 0.2f);
            resultText.gameObject.SetActive(true);
        }

        if (!isLastLevel && nextButton != null)
            nextButton.gameObject.SetActive(true);
    }

    public void OnSkip()
    {
        if (currentDifficulty >= levelShapes.Length - 1) return;
        confirmed = false;
        OnNext();
    }
    public void OnFastForward() => Debug.Log("Fast-forward");

    public void OnUndo()
    {
        Undo();
        Debug.Log("Undo");
    }

    public void OnMusicToggle()
    {
        AudioListener.pause = !AudioListener.pause;
        Debug.Log($"Music: {(AudioListener.pause ? "OFF" : "ON")}");
    }
}
