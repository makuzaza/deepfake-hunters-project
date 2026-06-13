using UnityEngine;

public class QTEController : MonoBehaviour
{
    public RectTransform needle;
    public RectTransform qteBar;

    public float redSpeed = 100f;
    public float yellowSpeed = 150f;
    public float greenSpeed = 200f;

    float barWidth;
    int direction = 1; // 1 = right, -1 = left
    public bool isRunning = false;

    // Zone boundaries (normalized 0–1)
    float redLeftEnd = 0.15f;
    float yellowLeftEnd = 0.375f;
    float greenEnd = 0.625f;
    float yellowRightEnd = 0.85f;

    public System.Action OnSuccess;
    public System.Action OnFail;

    void Start()
    {
        barWidth = qteBar.rect.width;
    }

    void Update()
    {
        if (!isRunning) return;

        float pos = needle.anchoredPosition.x / barWidth;
        float speed = GetSpeed(pos);

        needle.anchoredPosition += new Vector2(speed * direction * Time.deltaTime, 0);

        // Bounce logic
        if (needle.anchoredPosition.x >= barWidth)
            direction = -1;
        else if (needle.anchoredPosition.x <= 0)
            direction = 1;
    }

    float GetSpeed(float pos)
    {
        if (pos < redLeftEnd) return redSpeed;
        if (pos < yellowLeftEnd) return yellowSpeed;
        if (pos < greenEnd) return greenSpeed;
        if (pos < yellowRightEnd) return yellowSpeed;
        return redSpeed;
    }

    public void PlayerPressedPause()
    {
        if (!isRunning) return;

        float pos = needle.anchoredPosition.x / barWidth;

        if (pos >= 0.375f && pos <= 0.625f)
            OnSuccess?.Invoke();
        else
            OnFail?.Invoke();
    }

    public void StartQTE()
    {
        isRunning = true;
        needle.anchoredPosition = new Vector2(0, needle.anchoredPosition.y);
        direction = 1;
    }
}
