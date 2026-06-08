using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;

/// <summary>
/// Attach to the Computer/Desk GameObject in the scene.
/// Requires a 2D Trigger Collider on the same object.
///
/// Flow:
///   player enters range  →  "Press E" prompt appears
///   player presses E     →  dialogue panel: "Would you like to start your working day?"
///   YES                  →  player freezes, portal (login screen) opens
///   NO                   →  dialogue hides, player can keep walking
/// </summary>
public class ComputerInteractable : MonoBehaviour
{
    [Header("Press-E Prompt")]
    public GameObject prompt;           // the "Press E" world-space text prefab

    [Header("Dialogue Panel (UI Canvas)")]
    public GameObject dialoguePanel;    // the whole panel GO
    public TMP_Text   dialogueText;     // the question text inside it
    public Button     yesButton;
    public Button     noButton;

    [Header("Portal")]
    public PortalUIController portal;   // drag PortalCanvas here

    bool playerInRange = false;

    void Start()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);

        if (yesButton != null) yesButton.onClick.AddListener(OnYes);
        if (noButton  != null) noButton .onClick.AddListener(OnNo);

        if (dialogueText != null)
            dialogueText.text = "Would you like to start\nyour working day?";
    }

    void Update()
    {
        if (Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("[Computer] E pressed. playerInRange=" + playerInRange);
            if (playerInRange) ShowDialogue();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log("[Computer] Trigger entered by: " + other.name + " tag=" + other.tag);
        if (!other.CompareTag("Player")) return;
        playerInRange = true;
        if (prompt != null) prompt.SetActive(true);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        playerInRange = false;
        if (prompt != null) prompt.SetActive(false);
    }

    void ShowDialogue()
    {
        Debug.Log("[Computer] ShowDialogue. panel=" + dialoguePanel + " portal=" + portal);
        if (prompt != null) prompt.SetActive(false);
        if (dialoguePanel != null) dialoguePanel.SetActive(true);
        SetPlayerMovement(false);
    }

    void OnYes()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        if (portal != null) portal.Open();
        // player stays frozen — portal re-enables movement on close
    }

    void OnNo()
    {
        if (dialoguePanel != null) dialoguePanel.SetActive(false);
        SetPlayerMovement(true);
    }

    static void SetPlayerMovement(bool enabled)
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        if (player == null) return;
        var mv = player.GetComponent<PlayerMovement>();
        if (mv != null) mv.enabled = enabled;
    }
}
