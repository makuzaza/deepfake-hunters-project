using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;


public class Task1Manager : MonoBehaviour
{
    public QTEController qte;
    public TextMeshProUGUI countdownText;
    public TextMeshProUGUI feedbackText;
    public Button pauseButton;
    public Button editButton;
    public Button launchButton;

    float timer = 8f;
    public int attemptsRemaining = 3;
    bool success = false;
    bool isEnding = false;

    public RectTransform qteContainer;
    public GameObject blackOverlay;
    public TextMeshProUGUI successText;
    public TextMeshProUGUI successText2;

    //Video Animation stuff
    [Header("Video Thumbnail & Animation Stuff :)")] 
    public Image videoImage;          // The UI Image showing the animation
    public Sprite pauseSprite;        // Sprite shown when success happens
    public Sprite editedSprite;       // Sprite shown after Edit
    public Animator videoAnimator;    // If your animation uses Animator

    public Image greenFlash;

    bool playerClickedThisAttempt = false;

    [Header("Dialogue")]
    public string successDialogue;
    public string editDialogue;
    public string failDialogue;
    public string marcusTakeoverDialogue;
    public string successFinalText;
    public string successFinalText2;
    public string failFinalText1;
    public string failFinalText2;



    [Header("Marcus Takeover Settings")]
    public RectTransform videoRect;
    public RectTransform cursorRect;
    public GameObject alternativeOutroUI;
    public float cursorMoveSpeed = 1f;   // speed multiplier
    public float dragSpeed = 1f;         // speed multiplier
    public float preDragDelay = 1.5f;    // time before cursor moves

    [Header("Fail Outro UI")]
    public GameObject blackOverlayFail;
    public TextMeshProUGUI failText1;
    public TextMeshProUGUI failText2;

    [Header("Audio")]
    public AudioClip qteBarClip;
    private AudioSource _qteAudio;

    void Start()
    {
        editButton.interactable = false;
        launchButton.interactable = false;

        qte.OnSuccess += HandleSuccess;
        qte.OnFail += HandleFail;

        pauseButton.onClick.AddListener(OnPausePressed);

        if (qteBarClip != null)
        {
            _qteAudio = gameObject.AddComponent<AudioSource>();
            _qteAudio.clip = qteBarClip;
            _qteAudio.loop = true;
            _qteAudio.spatialBlend = 0f;
            _qteAudio.playOnAwake = false;
            _qteAudio.Play();
        }

        qte.StartQTE();
    }

    private void StopQTESound()
    {
        if (_qteAudio != null && _qteAudio.isPlaying)
            _qteAudio.Stop();
    }

    void Update()
    {
        if (success || isEnding) return;  // prevents timer from running

        timer -= Time.deltaTime;
        if (timer < 0) timer = 0;         // clamp

        countdownText.text = Mathf.CeilToInt(timer).ToString();

        if (timer <= 0)
            HandleTimerExpired();
    }

    void OnPausePressed()
    {
        qte.PlayerPressedPause();
        playerClickedThisAttempt = true; //flags when player tries (para el dialogo alternativo)
    }

    public void EditVideo()
    {
        // Swap to edited sprite
        videoImage.sprite = editedSprite;
        StartCoroutine(FlashGreen());


        launchButton.interactable = true;
        feedbackText.text = editDialogue;
    }

    IEnumerator FlashGreen()
    {
        greenFlash.gameObject.SetActive(true);
        greenFlash.color = new Color(0, 1, 0, 0.4f);

        float t = 0f;
        while (t < 0.2f)
        {
            t += Time.deltaTime;
            greenFlash.color = new Color(0, 1, 0, Mathf.Lerp(0.4f, 0f, t / 0.2f));
            yield return null;
        }

        greenFlash.gameObject.SetActive(false);
    }

    void HandleSuccess()
    {
        success = true;
        qte.isRunning = false;
        StopQTESound();

        // Stop animation
        if (videoAnimator != null)
            videoAnimator.enabled = false;

        // Swap to pause sprite
        videoImage.sprite = pauseSprite;

        feedbackText.text = successDialogue;
        editButton.interactable = true;

    }


    void HandleFail()
    {
        attemptsRemaining--;
        playerClickedThisAttempt = false;

        if (attemptsRemaining > 0)
        {
            feedbackText.text = failDialogue;
            timer = 8f;
            qte.StartQTE();
        }
        else
        {
            isEnding = true;
            timer = 0;
            countdownText.text = "0";

            StartCoroutine(MarcusTakeoverSequence());
        }
    }

    void HandleTimerExpired()
    {
        // Player never clicked at all
        if (!playerClickedThisAttempt)
        {
            isEnding = true;
            timer = 0;
            countdownText.text = "0";


            feedbackText.text = marcusTakeoverDialogue;

            StartCoroutine(MarcusTakeoverSequence());

            //StartOutroAnimation();
            return;
        }

        // Player clicked but failed
        attemptsRemaining--;

        if (attemptsRemaining > 0)
        {
            feedbackText.text = failDialogue;
            timer = 15f;
            playerClickedThisAttempt = false; // reset for next attempt
            qte.StartQTE();
        }
        else
        {
            StartOutroAnimation();
        }
    }

    void StartOutroAnimation()
    {
        isEnding = true;
        timer = 0;
        countdownText.text = "0";

        qte.isRunning = false;
        StopQTESound();
        feedbackText.text = failDialogue;
        // TODO: play (and create) outro animation here
    }

    IEnumerator MarcusTakeoverSequence()
    {
        qte.isRunning = false;
        StopQTESound();

        // Disable all buttons
        pauseButton.interactable = false;
        editButton.interactable = false;
        launchButton.interactable = false;

        // Marcus dialogue
        feedbackText.text = marcusTakeoverDialogue;

        // Let player read
        yield return new WaitForSeconds(preDragDelay);

        // Cursor appears
        cursorRect.gameObject.SetActive(true);

        // Move cursor to video
        Vector3 startPos = cursorRect.anchoredPosition;
        Vector3 targetPos = videoRect.anchoredPosition + new Vector2(-50, 50);

        float t = 0f;
        float moveTime = 1f / cursorMoveSpeed;

        while (t < moveTime)
        {
            t += Time.deltaTime;
            cursorRect.anchoredPosition = Vector3.Lerp(startPos, targetPos, t / moveTime);
            yield return null;
        }

        // Drag video + cursor together
        Vector3 videoStart = videoRect.anchoredPosition;
        Vector3 videoEnd = videoStart + new Vector3(900, -400, 0); // tweak direction

        Vector3 cursorStart = cursorRect.anchoredPosition;
        Vector3 cursorEnd = cursorStart + new Vector3(900, -400, 0);

        t = 0f;
        float dragTime = 1.2f / dragSpeed;

        while (t < dragTime)
        {
            t += Time.deltaTime;
            float p = Mathf.Clamp01(t / dragTime);

            videoRect.anchoredPosition = Vector3.Lerp(videoStart, videoEnd, p);
            cursorRect.anchoredPosition = Vector3.Lerp(cursorStart, cursorEnd, p);

            yield return null;
        }


        videoRect.gameObject.SetActive(false);
        cursorRect.gameObject.SetActive(false);

        // Fade in fail overlay
        blackOverlayFail.SetActive(true);

        failText1.alpha = 0;
        failText2.alpha = 0;

        float fadeTime = 0.8f;
        float f = 0f;

        while (f < fadeTime)
        {
            f += Time.deltaTime;
            float a = f / fadeTime;

            failText1.alpha = Mathf.Lerp(0, 1, a);
            failText2.alpha = Mathf.Lerp(0, 1, a);

            yield return null;
        }

        alternativeOutroUI.SetActive(true);
    }



    public void PlaySuccessAnimation()
    {
        StartCoroutine(SuccessSequence());
    }

    IEnumerator SuccessSequence()
    {
        // 1. TV turn-off animation (scale Y down)
        float duration = 0.25f;
        float t = 0f;

        Vector3 startScale = qteContainer.localScale;
        Vector3 endScale = new Vector3(startScale.x, 0f, startScale.z);

        while (t < duration)
        {
            t += Time.deltaTime;
            float progress = t / duration;
            qteContainer.localScale = Vector3.Lerp(startScale, endScale, progress);
            yield return null;
        }

        // 2. Hide QTE UI
        qteContainer.gameObject.SetActive(false);

        // 3. Show black overlay
        blackOverlay.SetActive(true);

        // 4. Fade in success text
        successText.text = successFinalText;
        successText2.text = successFinalText2;
        successText.alpha = 0;
        successText2.alpha = 0;

    float fadeTime = 0.5f;
        float f = 0f;

        while (f < fadeTime)
        {
            f += Time.deltaTime;
            successText.alpha = Mathf.Lerp(0, 1, f / fadeTime);
            successText2.alpha = Mathf.Lerp(0, 1, f / fadeTime); //make texts show over the black overlay (i'll maybe change later)
            yield return null;
        }
    }

}
