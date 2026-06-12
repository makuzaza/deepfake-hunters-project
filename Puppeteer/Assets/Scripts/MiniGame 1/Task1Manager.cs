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

    float timer = 15f;
    public int attemptsRemaining = 3;
    bool success = false;

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

    void Start()
    {
        editButton.interactable = false;
        launchButton.interactable = false;

        qte.OnSuccess += HandleSuccess;
        qte.OnFail += HandleFail;

        pauseButton.onClick.AddListener(OnPausePressed);

        qte.StartQTE();
    }

    void Update()
    {
        if (success) return;

        timer -= Time.deltaTime;
        countdownText.text = Mathf.CeilToInt(timer).ToString();

        if (timer <= 0)
        {
            HandleTimerExpired();
        }

        /*if (Input.GetKeyDown(KeyCode.Space))
            OnPausePressed();*/
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
        feedbackText.text = "Marcus: Now that's perfect! click to launch, and your masterpiece will be delievred to Natural Remedies. Good job."; 
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

        // Stop animation
        if (videoAnimator != null)
            videoAnimator.enabled = false;

        // Swap to pause sprite
        videoImage.sprite = pauseSprite;

        feedbackText.text = "Marcus: Good! You caught the right frame Click edit, who has never needed a small change?" ;
        editButton.interactable = true;

    }


    void HandleFail()
    {
        attemptsRemaining--;
        playerClickedThisAttempt = false;

        if (attemptsRemaining > 0)
        {
            feedbackText.text = "Marcus: Missed it! Try again.";
            timer = 15f;
            qte.StartQTE();
        }
        else
        {
            StartOutroAnimation();
            qte.isRunning = false;
        }
    }

    void HandleTimerExpired()
    {
        // Player never clicked at all
        if (!playerClickedThisAttempt)
        {
            feedbackText.text = "Marcus: Looks like you ran into some trouble, I'll handle it and send it to Tony.";
            StartOutroAnimation();
            return;
        }

        // Player clicked but failed
        attemptsRemaining--;

        if (attemptsRemaining > 0)
        {
            feedbackText.text = "Marcus: Time’s up! Try again.";
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
        qte.isRunning = false;
        timer = 0;
        feedbackText.text = "You failed the task.";
        // TODO: play your outro animation here
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
        successText.text = "Your campaign for Natural Remedies was launched!";
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
