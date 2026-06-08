using UnityEngine;
using UnityEngine.InputSystem;

public class Interactable : MonoBehaviour
{
    [Header("Interaction Settings")]
    public string interactionName = "Interact";

    private bool playerInRange = false;

    // prompt that tells the player to press E to interact
    public GameObject prompt;

    void Update()
    {
        if (playerInRange && Keyboard.current.eKey.wasPressedThisFrame)
        {
            Debug.Log("Interacted with: " + interactionName);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            
            if (prompt != null) prompt.SetActive(true);// Prompt activation 
        }
    }


    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (prompt != null) prompt.SetActive(false); // Prompt deactivation
        }

    }


}
