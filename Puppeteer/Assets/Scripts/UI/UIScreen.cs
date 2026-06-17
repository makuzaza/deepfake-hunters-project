// UIScreen.cs — FIXED
// The bug: StartCoroutine fails on inactive GameObjects.
// The fix: SetActive(true) BEFORE starting the fade coroutine.
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CanvasGroup))]
public abstract class UIScreen : MonoBehaviour
{
    [Header("Screen Config")]
    [SerializeField] private ScreenId screenId;
    [SerializeField] private bool     isFullScreen;

    public ScreenId Id         => screenId;
    public bool     FullScreen => isFullScreen;

    private CanvasGroup _cg;

    protected virtual void Awake()
    {
        _cg = GetComponent<CanvasGroup>();
        // Start invisible — ScreenManager will call Show() when needed
        _cg.alpha          = 0f;
        _cg.interactable   = false;
        _cg.blocksRaycasts = false;
        // DO NOT SetActive(false) here — Awake doesn't run on inactive objects anyway.
        // Set each screen inactive manually in the Editor Hierarchy instead.
    }

    // Called by ScreenManager
    public void Show()
    {
        // MUST activate BEFORE StartCoroutine — Unity cannot start
        // coroutines on inactive GameObjects.
        gameObject.SetActive(true);
        OnBeforeShow();
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        StartCoroutine(FadeOut());
    }

    // Override in subclass to refresh content before the screen fades in
    protected virtual void OnBeforeShow() { }
    protected virtual void OnAfterHide()  { }

    private IEnumerator FadeIn()
    {
        _cg.interactable   = false;
        _cg.blocksRaycasts = false;
        float elapsed = 0f;
        const float duration = 0.22f;
        while (elapsed < duration)
        {
            elapsed    += Time.unscaledDeltaTime;
            _cg.alpha   = Mathf.Clamp01(elapsed / duration);
            yield return null;
        }
        _cg.alpha          = 1f;
        _cg.interactable   = true;
        _cg.blocksRaycasts = true;
    }

    private IEnumerator FadeOut()
    {
        _cg.interactable   = false;
        _cg.blocksRaycasts = false;
        float elapsed = 0f;
        const float duration = 0.18f;
        while (elapsed < duration)
        {
            elapsed    += Time.unscaledDeltaTime;
            _cg.alpha   = Mathf.Clamp01(1f - (elapsed / duration));
            yield return null;
        }
        _cg.alpha = 0f;
        gameObject.SetActive(false);
        OnAfterHide();
    }
}