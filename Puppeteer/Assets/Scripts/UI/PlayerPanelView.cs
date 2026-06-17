// PlayerPanelView.cs — attach to PlayerPanel
// Subscribes to PlayerStateSO.OnPlayerStateChanged via GameEvents and updates its own refs.
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerPanelView : MonoBehaviour
{
    [Header("PlayerStateSO asset — drag from Project")]
    [SerializeField] private PlayerStateSO player;

    [Header("Serialized UI refs (no name lookups)")]
    [SerializeField] private TMP_Text  nameLabel;
    [SerializeField] private TMP_Text  moneyLabel;     // object named MoneyLabel
    [SerializeField] private TMP_Text  dayTimeLabel;   // object named TimeLabel
    [SerializeField] private TMP_Text  riskLabel;
    [SerializeField] private Image     portraitImage;
    [SerializeField] private Image     riskFill;       // Image Type = Filled, Horizontal
    [SerializeField] private Sprite[]  portraitSprites;// length 3, matches portrait indices

    [Header("Risk colors")]
    [SerializeField] private Color riskSafe   = new Color(0.60f, 0.65f, 0.53f); // green2
    [SerializeField] private Color riskDanger = new Color(0.56f, 0.22f, 0.26f); // red

    private void OnEnable()  { GameEvents.OnPlayerStateChanged += Refresh; Refresh(); }
    private void OnDisable() { GameEvents.OnPlayerStateChanged -= Refresh; }

    private void Refresh()
    {
        if (player == null) return;
        if (nameLabel)    nameLabel.text    = player.playerName;
        if (moneyLabel)   moneyLabel.text   = "€" + player.money;
        if (dayTimeLabel) dayTimeLabel.text = player.day + " — " + player.timeLabel;
        if (riskLabel)    riskLabel.text    = player.accountRisk + "%";
        float f = player.accountRisk / 100f;
        if (riskFill)    { riskFill.fillAmount = f; riskFill.color = Color.Lerp(riskSafe, riskDanger, f); }
        if (portraitImage && portraitSprites != null && portraitSprites.Length > 0)
            portraitImage.sprite = portraitSprites[Mathf.Clamp(player.portraitIndex, 0, portraitSprites.Length-1)];
    }
}
