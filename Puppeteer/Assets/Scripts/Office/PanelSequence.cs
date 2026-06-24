using UnityEngine;
using TMPro;
using System.Collections;

public class PanelSequence : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup panelGroup;
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI line1Text;
    public TextMeshProUGUI line2Text;
    public TextMeshProUGUI finalText;

    [Header("Settings")]
    public float typeSpeed = 0.03f;
    public float delayBetweenTexts = 1.2f;
    public float fadeSpeed = 1f;

    void Start()
    {
        // Panel visible desde el inicio
        panelGroup.alpha = 1;

        // Desactivar textos para que no aparezcan los de testeo
        titleText.gameObject.SetActive(false);
        line1Text.gameObject.SetActive(false);
        line2Text.gameObject.SetActive(false);
        finalText.gameObject.SetActive(false);

        StartCoroutine(PlaySequence());
    }

    IEnumerator PlaySequence()
    {
        // 1. Título
        titleText.gameObject.SetActive(true);
        yield return StartCoroutine(TypeText(titleText, "Welcome to your first day at Human Agency Marketing. We’re excited to have you on board."));
        yield return new WaitForSeconds(delayBetweenTexts);

        // 2. Línea 1
        line1Text.gameObject.SetActive(true);
        yield return StartCoroutine(TypeText(line1Text, "Our agency values creativity, adaptability, and a strong sense of responsibility."));
        yield return new WaitForSeconds(delayBetweenTexts);

        // 3. Línea 2
        line2Text.gameObject.SetActive(true);
        yield return StartCoroutine(TypeText(line2Text, "You were selected because you consistently deliver results and understand what audiences respond to"));
        yield return new WaitForSeconds(delayBetweenTexts);

        // 4. Desaparecer textos anteriores LIMPIA LA PANTALLA***
        titleText.gameObject.SetActive(false);
        line1Text.gameObject.SetActive(false);
        line2Text.gameObject.SetActive(false);

        // 5. Texto final
        finalText.gameObject.SetActive(true);
        yield return StartCoroutine(TypeText(finalText, "Your ability to persuade others is exactly why the agency chose you."));
        yield return new WaitForSeconds(delayBetweenTexts);

        // 6. Fade out del panel
        yield return StartCoroutine(FadeCanvas(panelGroup, 1, 0));
    }

    IEnumerator TypeText(TextMeshProUGUI textUI, string fullText)
    {
        textUI.text = "";
        foreach (char c in fullText)
        {
            textUI.text += c;
            yield return new WaitForSeconds(typeSpeed);
        }
    }

    IEnumerator FadeCanvas(CanvasGroup cg, float from, float to)
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.deltaTime * fadeSpeed;
            cg.alpha = Mathf.Lerp(from, to, t);
            yield return null;
        }
    }
}
