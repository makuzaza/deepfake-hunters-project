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
    public Text          instructionText;
    public Slider        accuracySlider;
    public Slider        brushSlider;
    public Font          pixelFont;

    [Header("Difficulty")]
    public Image[] levelShapes = new Image[3]; // 0=Easy 1=Medium 2=Hard
    public int     difficulty  = 2;

    Image  shapeImage;
    int    currentDifficulty;
    Button nextButton;
    Button completeButton;

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

    [Header("Debug")]
    public bool debugMask = false;

    [Header("Audio")]
    public AudioClip bgMusicClip;
    private AudioSource _bgAudio;

    // ── private ────────────────────────────────────────────────────
    Texture2D     overlayTex;
    RectTransform overlayRect;

    float accuracy       = 0f;
    float materialDamage = 0f;   // 0-1: fraction of body pixels erased
    bool  confirmed      = false;

    int   levelsSucceeded = 0;   // counts only accuracy>=90% AND damage<=20%

    bool  playerHasActed = false;
    float idleTime        = 0f;
    const float IdleThreshold = 15f;
    Text  marcusText;

    bool[] shapeMask;
    int    totalShapePixels;
    int    prevClearedInside = 0;

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

        if (bgMusicClip != null)
        {
            _bgAudio = gameObject.AddComponent<AudioSource>();
            _bgAudio.clip = bgMusicClip;
            _bgAudio.loop = true;
            _bgAudio.spatialBlend = 0f;
            _bgAudio.playOnAwake = false;
            _bgAudio.Play();
        }

        BuildResultText();
        BuildNextButton();
        BuildCompleteButton();
        BuildTopHUD();
        BuildMarcusText();
        BuildOverlay();
    }

    private void StopBGMusic()
    {
        if (_bgAudio != null && _bgAudio.isPlaying)
            _bgAudio.Stop();
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

    // ── Complete button ────────────────────────────────────────────
    void BuildCompleteButton()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("CompleteButton", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0.5f);
        rt.anchorMax        = new Vector2(0.5f, 0.5f);
        rt.pivot            = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0f, -200f);
        rt.sizeDelta        = new Vector2(450f, 72f);

        var img = go.AddComponent<Image>();
        img.color = new Color(0.15f, 0.15f, 0.55f, 1f);

        completeButton = go.AddComponent<Button>();
        completeButton.targetGraphic = img;
        completeButton.onClick.AddListener(OnComplete);

        var label = new GameObject("Label", typeof(RectTransform));
        label.transform.SetParent(go.transform, false);
        var lrt = label.GetComponent<RectTransform>();
        lrt.anchorMin = Vector2.zero; lrt.anchorMax = Vector2.one;
        lrt.offsetMin = Vector2.zero; lrt.offsetMax = Vector2.zero;

        var txt = label.AddComponent<Text>();
        txt.font          = GetFont();
        txt.fontSize      = 38;
        txt.fontStyle     = FontStyle.Bold;
        txt.alignment     = TextAnchor.MiddleCenter;
        txt.color         = Color.white;
        txt.raycastTarget = false;
        txt.text          = "Complete the task ✓";

        var outline = label.AddComponent<Outline>();
        outline.effectColor    = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        go.SetActive(false);
    }

    public void OnComplete()
    {
        // Disable all interactive controls
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            foreach (var btn in canvas.GetComponentsInChildren<Button>())
                btn.interactable = false;
        if (brushSlider != null) brushSlider.interactable = false;

        if (completeButton != null) completeButton.gameObject.SetActive(false);

        levelsSucceeded = 0;
        StopBGMusic();
        var tsc = GetComponentInParent<TaskSceneController>();
        if (tsc!=null)tsc.OnLaunchPressed();
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
        confirmed          = false;
        accuracy           = 0f;
        materialDamage     = 0f;
        elapsedTime        = 0f;
        idleTime           = 0f;
        playerHasActed     = false;
        prevClearedInside  = 0;
        undoStack.Clear();

        if (marcusText != null) marcusText.gameObject.SetActive(false);

        if (nextButton      != null) nextButton.gameObject.SetActive(false);
        if (completeButton  != null) completeButton.gameObject.SetActive(false);
        if (resultText      != null) resultText.gameObject.SetActive(false);
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

    // ── Marcus idle dialogue ───────────────────────────────────────
    void BuildMarcusText()
    {
        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas == null) return;

        var go = new GameObject("MarcusText", typeof(RectTransform));
        go.transform.SetParent(canvas.transform, false);

        var rt = go.GetComponent<RectTransform>();
        rt.anchorMin        = new Vector2(0.5f, 0f);
        rt.anchorMax        = new Vector2(0.5f, 0f);
        rt.pivot            = new Vector2(0.5f, 0f);
        rt.anchoredPosition = new Vector2(0f, 24f);
        rt.sizeDelta        = new Vector2(1000f, 80f);

        marcusText = go.AddComponent<Text>();
        marcusText.font          = GetFont();
        marcusText.fontSize      = 36;
        marcusText.fontStyle     = FontStyle.Italic;
        marcusText.alignment     = TextAnchor.MiddleCenter;
        marcusText.color         = new Color(1f, 0.92f, 0.6f);
        marcusText.raycastTarget = false;
        marcusText.text          = "Marcus: Alright, let's shape this face! Click and erase…";

        var outline = go.AddComponent<Outline>();
        outline.effectColor    = Color.black;
        outline.effectDistance = new Vector2(2, -2);

        go.SetActive(false);
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

            if (!playerHasActed)
            {
                idleTime += Time.deltaTime;
                if (idleTime >= IdleThreshold && marcusText != null && !marcusText.gameObject.activeSelf)
                    marcusText.gameObject.SetActive(true);
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

        // All shapes must occupy the same rect as the hard shape (index 2) so the
        // overlay covers the full monitor screen area for every difficulty level.
        if (levelShapes != null && levelShapes.Length > 2 && levelShapes[2] != null)
        {
            RectTransform hardRT  = levelShapes[2].rectTransform;
            Vector2 targetSize    = hardRT.sizeDelta;
            Vector2 targetPos     = hardRT.anchoredPosition;
            for (int i = 0; i < levelShapes.Length - 1; i++)
                if (levelShapes[i] != null)
                {
                    levelShapes[i].rectTransform.anchoredPosition = targetPos;
                    levelShapes[i].rectTransform.sizeDelta         = targetSize;
                }
            Canvas.ForceUpdateCanvases();
        }

        // Snap the overlay rect to exactly match the current shape image.
        if (shapeImage != null)
        {
            RectTransform sr = shapeImage.rectTransform;
            overlayRect.anchorMin = new Vector2(0.5f, 0.5f);
            overlayRect.anchorMax = new Vector2(0.5f, 0.5f);
            overlayRect.pivot     = new Vector2(0.5f, 0.5f);
            overlayRect.position  = sr.position;
            overlayRect.sizeDelta = new Vector2(sr.rect.width, sr.rect.height);
            Canvas.ForceUpdateCanvases();
        }

        int w = Mathf.RoundToInt(overlayRect.rect.width);
        int h = Mathf.RoundToInt(overlayRect.rect.height);

        if (w <= 0 || h <= 0) { Invoke(nameof(BuildOverlay), 0.05f); return; }

        Debug.Log($"[BuildOverlay] overlay={w}x{h}  shape={shapeImage?.sprite?.name ?? "NULL"}");

        overlayTex = new Texture2D(w, h, TextureFormat.RGBA32, false)
            { filterMode = FilterMode.Bilinear };

        Color32 fill  = new(30, 200, 160, 200);
        Color32[] px  = new Color32[w * h];
        for (int i = 0; i < px.Length; i++) px[i] = fill;
        overlayTex.SetPixels32(px);
        overlayTex.Apply();

        overlayImage.sprite = Sprite.Create(overlayTex,
            new Rect(0, 0, w, h), new Vector2(0.5f, 0.5f), 100f);

        BuildShapeMask(w, h);

        if (debugMask) PaintMaskDebug();
    }

    // ── Shape mask ─────────────────────────────────────────────────
    // The overlay is snapped to the same rect as shapeImage, so overlay pixel
    // (px2, py) maps 1-to-1 onto sprite UV (px2/w, py/h) — no offset math needed.
    void BuildShapeMask(int overlayW, int overlayH)
    {
        shapeMask        = null;
        totalShapePixels = 0;

        if (shapeImage == null || shapeImage.sprite == null) return;

        Texture2D shapeTex = shapeImage.sprite.texture;
        if (shapeTex == null) return;

        // textureRect: sub-region of the PNG where this sprite lives (bottom-left origin)
        Rect  sr  = shapeImage.sprite.textureRect;
        float tx0 = sr.x,    ty0 = sr.y;
        float tsw = sr.width, tsh = sr.height;

        // ── PreserveAspect letterbox calculation ──────────────────────
        // The shape Image has PreserveAspect=true, so the sprite is scaled
        // uniformly to fit within the (overlayW × overlayH) rect, leaving
        // empty margins on two sides.  Only mark pixels inside the
        // actually-displayed sprite area as potential body pixels.
        float spriteAspect = tsw / tsh;
        float rectAspect   = (float)overlayW / overlayH;

        float spU0, spV0, spU1, spV1;   // overlay-UV range where sprite is shown
        if (spriteAspect > rectAspect)
        {
            // sprite wider than rect → letterbox top & bottom
            float displayedH = overlayW / spriteAspect;
            float vPad = (overlayH - displayedH) * 0.5f / overlayH;
            spU0 = 0f;    spU1 = 1f;
            spV0 = vPad;  spV1 = 1f - vPad;
        }
        else
        {
            // sprite taller than rect → letterbox left & right
            float displayedW = overlayH * spriteAspect;
            float uPad = (overlayW - displayedW) * 0.5f / overlayW;
            spU0 = uPad;  spU1 = 1f - uPad;
            spV0 = 0f;    spV1 = 1f;
        }

        int cTx0 = Mathf.RoundToInt(tx0);
        int cTy0 = Mathf.RoundToInt(ty0);
        int cTx1 = Mathf.RoundToInt(tx0 + tsw) - 1;
        int cTy1 = Mathf.RoundToInt(ty0 + tsh) - 1;

        Debug.Log($"[ShapeMask] sprite='{shapeImage.sprite.name}'  " +
                  $"aspect={spriteAspect:F3}  displayUV U=[{spU0:F3},{spU1:F3}] V=[{spV0:F3},{spV1:F3}]  " +
                  $"tex={shapeTex.width}x{shapeTex.height} rect=({tx0},{ty0},{tsw},{tsh})");

        shapeMask = new bool[overlayW * overlayH];

        for (int py = 0; py < overlayH; py++)
        for (int px2 = 0; px2 < overlayW; px2++)
        {
            float u = (float)px2 / overlayW;
            float v = (float)py  / overlayH;

            // Skip letterbox margins — sprite is not displayed there
            if (u < spU0 || u > spU1 || v < spV0 || v > spV1) continue;

            // Remap overlay UV to sprite UV [0,1]
            float su = (u - spU0) / (spU1 - spU0);
            float sv = (v - spV0) / (spV1 - spV0);

            // Map sprite UV to texture pixel (bottom-left origin, same as GetPixel)
            int tx = Mathf.Clamp(Mathf.RoundToInt(tx0 + su * tsw), cTx0, cTx1);
            int ty = Mathf.Clamp(Mathf.RoundToInt(ty0 + sv * tsh), cTy0, cTy1);

            if (shapeTex.GetPixel(tx, ty).a > 0.1f)
            {
                shapeMask[py * overlayW + px2] = true;
                totalShapePixels++;
            }
        }

        Debug.Log($"[ShapeMask] body={totalShapePixels}/{overlayW * overlayH} " +
                  $"({100f * totalShapePixels / (overlayW * overlayH):F1}%)");
    }

    // Paints overlay red=body / teal=outside. Enable debugMask in Inspector to see it.
    void PaintMaskDebug()
    {
        if (overlayTex == null || shapeMask == null) return;
        int len      = overlayTex.width * overlayTex.height;
        Color32[] dbg = new Color32[len];
        for (int i = 0; i < len; i++)
            dbg[i] = shapeMask[i]
                ? new Color32(220, 30,  30,  200)
                : new Color32(30,  200, 160, 200);
        overlayTex.SetPixels32(dbg);
        overlayTex.Apply();
        Debug.Log($"[MaskDebug] red=body({totalShapePixels}px) teal=outside");
    }

    // ── Sculpting input ────────────────────────────────────────────
    public void OnPointerDown(PointerEventData e)
    {
        if (overlayTex == null || confirmed) return;

        if (!playerHasActed)
        {
            playerHasActed = true;
            if (marcusText != null) marcusText.gameObject.SetActive(false);
        }

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
            float[] penalties = { 0.15f, 0.2f, 0.25f };
            float penalty     = penalties[Mathf.Clamp(currentDifficulty, 0, penalties.Length - 1)];
            float outsideTotal = intersection + missedOutside;
            float progress     = outsideTotal > 0f ? (float)intersection / outsideTotal : 0f;
            float insideRatio  = (float)clearedInside / totalShapePixels;
            materialDamage     = insideRatio;
            accuracy = Mathf.Clamp01(progress - penalty * insideRatio) * 100f;
            if (clearedInside > prevClearedInside)
                Debug.Log($"Accuracy: {accuracy:F1}% | Damage: {materialDamage:F2}");
            else
                Debug.Log($"Accuracy: {accuracy:F1}%");
            prevClearedInside = clearedInside;

            if (materialDamage > 0.20f && !confirmed)
                OnConfirm();
        }
        else
        {
            int cleared = 0;
            foreach (Color32 c in px) if (c.a < 10) cleared++;
            accuracy = (float)cleared / px.Length * 100f;
            Debug.Log($"Accuracy: {accuracy:F1}%");
        }

        if (accuracyText)      accuracyText.text      = $"{accuracy:F0}%";
        if (accuracySlider)    accuracySlider.value    = accuracy / 100f;
        if (topAccuracyText)   topAccuracyText.text    = $"Accuracy: {Mathf.FloorToInt(accuracy)}%";
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

        if (marcusText != null) marcusText.gameObject.SetActive(false);
        if (brushSlider != null) brushSlider.interactable = false;

        Canvas canvas = GetComponentInParent<Canvas>();
        if (canvas != null)
            foreach (var btn in canvas.GetComponentsInChildren<Button>())
                btn.interactable = false;

        bool success     = accuracy >= 90f && materialDamage <= 0.20f;
        bool isLastLevel = currentDifficulty >= levelShapes.Length - 1;

        if (success) levelsSucceeded++;

        string failReason = materialDamage > 0.20f ? "Too much body damage!" : "Try to stay on the edges!";

        string msg = isLastLevel
            ? (success
                ? $"All Done!\nLevels passed: {levelsSucceeded}/{levelShapes.Length}\nPerfect work!"
                : $"All Done!\nLevels passed: {levelsSucceeded}/{levelShapes.Length}\n{failReason}")
            : (success
                ? $"Success!\nAccuracy: {accuracy:F1}%\nGreat sculpting work!"
                : $"Fail!\nAccuracy: {accuracy:F1}%\n{failReason}");

        Debug.Log(msg);
        if (resultText != null)
        {
            resultText.text  = msg;
            resultText.color = success ? new Color(0.2f, 0.85f, 0.3f) : new Color(0.9f, 0.2f, 0.2f);
            resultText.gameObject.SetActive(true);
        }

        if (!isLastLevel && nextButton != null)
            nextButton.gameObject.SetActive(true);

        if (isLastLevel && completeButton != null)
            completeButton.gameObject.SetActive(true);
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
}
