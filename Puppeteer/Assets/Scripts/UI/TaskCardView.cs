using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TaskCardView : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private TMP_Text clientLabel;
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text payText;
    [SerializeField] private TMP_Text riskText;
    [SerializeField] private Image    riskPill;
    [SerializeField] private Button   takeButton;
    [SerializeField] private Image    cardBackground;

    [Header("Risk pill colors")]
    [SerializeField] private Color riskNone   = new Color(0.28f,0.36f,0.36f);
    [SerializeField] private Color riskLow    = new Color(0.60f,0.65f,0.53f);
    [SerializeField] private Color riskMedium = new Color(0.79f,0.69f,0.53f);
    [SerializeField] private Color riskHigh   = new Color(0.56f,0.22f,0.26f);

    [Header("Hover")]
    [SerializeField] private float hoverScale   = 1.04f;
    [SerializeField] private float animDuration = 0.15f;
    [SerializeField] private Color hoverTint    = new Color(0.18f, 0.27f, 0.31f);

    [Header("Sounds")]
    [SerializeField] private AudioClip hoverClip;
    [SerializeField] private AudioClip clickClip;

    private Color _baseColor;
    private Coroutine _anim;
    private bool _done;

    // Shared AudioSource that survives screen transitions / deskRoot deactivation
    private static AudioSource _uiAudio;

    private static void PlayUI(AudioClip clip, float volume = 1f)
    {
        if (clip == null) return;
        if (_uiAudio == null)
        {
            var go = new GameObject("[UIAudio]");
            UnityEngine.Object.DontDestroyOnLoad(go);
            _uiAudio = go.AddComponent<AudioSource>();
            _uiAudio.spatialBlend = 0f;
            _uiAudio.playOnAwake  = false;
        }
        _uiAudio.PlayOneShot(clip, volume);
    }

    private void Awake()
    {
        if (cardBackground) _baseColor = cardBackground.color;
    }

    public void Setup(TaskSO task, Action onTake)
    {
        if (clientLabel) clientLabel.text = "CLIENT: " + task.clientName.ToUpper();
        if (titleText)   titleText.text   = task.taskTitle;
        if (payText)     payText.text     = "€" + task.pay;
        if (riskText)    riskText.text    = "RISK: " + task.riskLevel.ToString().ToUpper();
        if (riskPill)    riskPill.color   = task.riskLevel switch {
            RiskLevel.None   => riskNone,
            RiskLevel.Low    => riskLow,
            RiskLevel.Medium => riskMedium,
            RiskLevel.High   => riskHigh,
            _                => riskNone
        };
        takeButton.onClick.RemoveAllListeners();
        takeButton.onClick.AddListener(() =>
        {
            MarkDone();
            onTake?.Invoke();
        });
    }

    public void OnPointerEnter(PointerEventData _)
    {
        if (_done) return;
        PlayUI(hoverClip, 0.5f);
        Animate(hoverScale, hoverTint);
    }
    public void OnPointerExit(PointerEventData _)
    {
        if (_done) return;
        Animate(1f, _baseColor);
    }

    public void MarkDone()
    {
        _done = true;

        // Stop any hover animation and snap back to normal scale
        if (_anim != null) { StopCoroutine(_anim); _anim = null; }
        transform.localScale = Vector3.one;

        var dimColor  = new Color(0.35f, 0.35f, 0.35f, 1f);
        var darkBg    = new Color(0.09f, 0.09f, 0.09f, 1f);
        var doneGray  = new Color(0.30f, 0.30f, 0.30f, 1f);

        if (cardBackground) cardBackground.color = darkBg;
        if (clientLabel)    clientLabel.color     = dimColor;
        if (titleText)      titleText.color       = dimColor;
        if (payText)        payText.color         = dimColor;
        if (riskPill)       riskPill.color        = doneGray;
        if (riskText)       riskText.text         = "DONE";

        takeButton.interactable = false;
        var btnImg = takeButton.GetComponent<Image>();
        if (btnImg) btnImg.color = new Color(0.12f, 0.12f, 0.12f, 1f);

        var btnText = takeButton.GetComponentInChildren<TMP_Text>();
        if (btnText) { btnText.text = "TAKEN"; btnText.color = dimColor; }
    }

    private void Animate(float targetScale, Color targetColor)
    {
        if (_anim != null) StopCoroutine(_anim);
        _anim = StartCoroutine(AnimRoutine(targetScale, targetColor));
    }

    private IEnumerator AnimRoutine(float targetScale, Color targetColor)
    {
        Vector3 fromScale = transform.localScale;
        Color fromColor   = cardBackground ? cardBackground.color : Color.white;
        float elapsed = 0f;

        while (elapsed < animDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / animDuration);
            transform.localScale = Vector3.LerpUnclamped(fromScale, Vector3.one * targetScale, t);
            if (cardBackground) cardBackground.color = Color.Lerp(fromColor, targetColor, t);
            yield return null;
        }

        transform.localScale = Vector3.one * targetScale;
        if (cardBackground) cardBackground.color = targetColor;
    }
}
