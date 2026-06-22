// UIScreen.cs — FIXED (Hide guard added for Łódź build)
// Original bug: Show() failed on inactive objects (already fixed below).
// NEW bug found at runtime: Hide() calls StartCoroutine on a GameObject that is
// already inactive (BriefQueue gets deactivated while a task scene runs, then
// ScreenManager.Show(Result) calls _current.Hide() on it). Unity cannot start a
// coroutine on an inactive object -> exception, and the next screen never shows.
// Fix: in Hide(), if the object is already inactive, just finish instantly
// (no coroutine). Same for the case where the object isn't active in the hierarchy.
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
        _cg.alpha          = 0f;
        _cg.interactable   = false;
        _cg.blocksRaycasts = false;
    }

    // Called by ScreenManager
    public void Show()
    {
        gameObject.SetActive(true);   // must be active before StartCoroutine
        if (_cg == null) _cg = GetComponent<CanvasGroup>();
        OnBeforeShow();
        StartCoroutine(FadeIn());
    }

    public void Hide()
    {
        // GUARD: cannot StartCoroutine on an inactive GameObject.
        // If this screen is already inactive (e.g. it was hidden while a task
        // scene ran), just snap it to the hidden state instantly and bail.
        if (!gameObject.activeInHierarchy)
        {
            if (_cg == null) _cg = GetComponent<CanvasGroup>();
            _cg.alpha          = 0f;
            _cg.interactable   = false;
            _cg.blocksRaycasts = false;
            gameObject.SetActive(false);
            OnAfterHide();
            return;
        }

        StartCoroutine(FadeOut());
    }

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