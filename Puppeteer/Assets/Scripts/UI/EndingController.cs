using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EndingController : MonoBehaviour
{
    [Header("Ending display refs")]
    [SerializeField] private TMP_Text   endingTitleLabel;
    [SerializeField] private TMP_Text   epilogueLabel;
    [SerializeField] private GameObject returnButton;

    [Header("Ending data assets")]
    [SerializeField] private EndingSO endingComplicit;
    [SerializeField] private EndingSO endingWhistleblower;
    [SerializeField] private EndingSO endingPassive;

    [Header("Content")]
    [SerializeField] private string endingTitle = "Congratulations!";
    [SerializeField] [TextArea(6, 20)] private string epilogueText =
        "You completed the challenge.\n\n" +
        "You just spent a few minutes spotting deepfakes. In the real world, it's much harder.\n\n" +
        "Deepfake content has exploded from 500,000 files in 2023 to an estimated 8 million in 2026.\n\n" +
        "Every share, click, and reaction helps shape what others believe.\n\n" +
        "Stay skeptical.\n" +
        "Verify before sharing!";

    [Header("Audio")]
    [SerializeField] private AudioClip typingClip;

    private AudioSource _audio;

    private void Start()
    {
        if (returnButton     != null) returnButton.SetActive(false);
        if (endingTitleLabel != null) endingTitleLabel.text = endingTitle;

        if (typingClip != null)
        {
            _audio             = gameObject.AddComponent<AudioSource>();
            _audio.clip        = typingClip;
            _audio.loop        = true;
            _audio.playOnAwake = false;
        }

        StartCoroutine(TypeEpilogue());
    }

    private IEnumerator TypeEpilogue()
    {
        if (epilogueLabel == null) yield break;

        epilogueLabel.text               = epilogueText;
        epilogueLabel.maxVisibleCharacters = 0;
        epilogueLabel.ForceMeshUpdate();

        int total = epilogueLabel.textInfo.characterCount;

        if (_audio != null) _audio.Play();

        for (int i = 0; i <= total; i++)
        {
            epilogueLabel.maxVisibleCharacters = i;
            yield return new WaitForSeconds(0.03f);
        }

        if (_audio != null) _audio.Stop();
        if (returnButton != null) returnButton.SetActive(true);
    }

    public void GoToMainMenu() => SceneManager.LoadScene("Office");
}
