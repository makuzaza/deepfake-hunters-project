// UIFX.cs  -  Assets/_Project/Scripts/UI
// Smooth show/hide for any UI panel. Replaces hard SetActive(true/false) with a fade
// (and optional upward slide). Works on time-independent unscaled time so it runs even if
// the game is paused. Add nothing in the inspector — it adds a CanvasGroup automatically.
using System.Collections;
using UnityEngine;

public static class Uifx
{
    // Fade a panel IN. slideFrom = pixels to rise from (0 = pure fade, e.g. 40 = slide up while fading).
    public static IEnumerator FadeIn(GameObject go, float dur = 0.35f, float slideFrom = 0f)
    {
        if (go == null) yield break;
        go.SetActive(true);
        var cg = Group(go);
        var rt = go.transform as RectTransform;
        Vector2 home = rt ? rt.anchoredPosition : Vector2.zero;
        cg.alpha = 0f; cg.interactable = false; cg.blocksRaycasts = false;

        float t = 0f;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            float k = Mathf.Clamp01(t / dur);
            float e = 1f - (1f - k) * (1f - k);                 // ease-out
            cg.alpha = e;
            if (rt && slideFrom != 0f) rt.anchoredPosition = home + new Vector2(0f, slideFrom * (1f - e));
            yield return null;
        }
        cg.alpha = 1f;
        if (rt) rt.anchoredPosition = home;
        cg.interactable = true; cg.blocksRaycasts = true;
    }

    // Fade a panel OUT, then deactivate it.
    public static IEnumerator FadeOut(GameObject go, float dur = 0.25f)
    {
        if (go == null) yield break;
        var cg = Group(go);
        float start = cg.alpha, t = 0f;
        cg.interactable = false; cg.blocksRaycasts = false;
        while (t < dur)
        {
            t += Time.unscaledDeltaTime;
            cg.alpha = Mathf.Lerp(start, 0f, t / dur);
            yield return null;
        }
        cg.alpha = 0f;
        go.SetActive(false);
    }

    // Crossfade from one panel to another (old fades out, new fades in with a slight rise).
    public static IEnumerator Swap(MonoBehaviour host, GameObject from, GameObject to,
                                   float outDur = 0.22f, float inDur = 0.35f, float slide = 40f)
    {
        if (from) yield return host.StartCoroutine(FadeOut(from, outDur));
        if (to)   yield return host.StartCoroutine(FadeIn(to, inDur, slide));
    }

    private static CanvasGroup Group(GameObject go)
    {
        var cg = go.GetComponent<CanvasGroup>();
        return cg ? cg : go.AddComponent<CanvasGroup>();
    }
}
