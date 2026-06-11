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
    public RectTransform canvasRectRef;   // drag MinigameCanvas here
    public Text          accuracyText;
    public Slider        accuracySlider;

    [Header("Brush")]
    public float wideBrushRadius      = 35f;
    public float precisionBrushRadius = 12f;

    [Header("Hand settings")]
    // Offset so the cursor sits at the tip of the pointing finger.
    // Negative Y moves the hand DOWN so the finger tip (top of image) reaches the cursor.
    public Vector2 rightHandFingerOffset = new(0f, -230f);
    public float   handFollowSpeed       = 14f;

    [Header("Undo")]
    public int maxUndoSteps = 15;

    // ── private ────────────────────────────────────────────────────
    Texture2D     overlayTex;
    RectTransform overlayRect;

    bool  isWideTool = true;
    float accuracy   = 0f;

    readonly Stack<Color32[]> undoStack = new();

    // ──────────────────────────────────────────────────────────────
    void Start()
    {
        overlayRect = overlayImage.rectTransform;

        // Put right hand on top of every other UI element
        if (rightHand != null)
            rightHand.SetAsLastSibling();

        Invoke(nameof(BuildOverlay), 0.1f);
    }

    // ── Right hand follows cursor every frame ─────────────────────
    void Update()
    {
        if (rightHand == null || Mouse.current == null) return;

        RectTransform cRect = canvasRectRef != null
            ? canvasRectRef
            : GetComponentInParent<Canvas>()?.GetComponent<RectTransform>();
        if (cRect == null) return;

        Vector2 screenPos = Mouse.current.position.ReadValue();

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            cRect, screenPos, null, out Vector2 mouseCanvas);

        // Fingertip offset: hand moves down so its top (finger tip) is at cursor
        rightHand.anchoredPosition = Vector2.Lerp(
            rightHand.anchoredPosition,
            mouseCanvas + rightHandFingerOffset,
            Time.deltaTime * handFollowSpeed);
    }

    // ── Overlay texture ────────────────────────────────────────────
    void BuildOverlay()
    {
        int w = Mathf.RoundToInt(overlayRect.rect.width);
        int h = Mathf.RoundToInt(overlayRect.rect.height);

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
    }

    // ── Sculpting input ────────────────────────────────────────────
    public void OnPointerDown(PointerEventData e)
    {
        if (overlayTex == null) return;
        PushUndo();
        Sculpt(e.position);
    }

    public void OnDrag(PointerEventData e)
    {
        if (overlayTex == null) return;
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
        int r  = Mathf.RoundToInt((isWideTool ? wideBrushRadius : precisionBrushRadius)
                                  / overlayRect.rect.width * texW);

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
        int cleared = 0;
        foreach (Color32 c in px) if (c.a < 10) cleared++;

        accuracy = (float)cleared / px.Length * 100f;
        if (accuracyText)   accuracyText.text   = $"{accuracy:F0}%";
        if (accuracySlider) accuracySlider.value = accuracy / 100f;
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

    // ── Tool select ────────────────────────────────────────────────
    public void SelectWideTool()      => isWideTool = true;
    public void SelectPrecisionTool() => isWideTool = false;

    // ── Button handlers ────────────────────────────────────────────
    public void OnConfirm()     => Debug.Log($"Confirmed! Accuracy: {accuracy:F1}%");
    public void OnSkip()        => Debug.Log("Level skipped");
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
