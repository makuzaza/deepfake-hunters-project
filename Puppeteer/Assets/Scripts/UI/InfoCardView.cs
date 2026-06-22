// InfoCardView.cs — attach to InfoCard prefab (one per inbox row)
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoCardView : MonoBehaviour
{
    [SerializeField] private TMP_Text titleText;
    [SerializeField] private TMP_Text sublineText;
    [SerializeField] private TMP_Text tagText;
    [SerializeField] private Image    iconImage;
    [SerializeField] private Button   cardButton;
    [SerializeField] private TMP_Text    completedOverlay;  // grey tint, active when item.completed

    public void Setup(InboxItemData data, Action onClick)
    {
        if (titleText)   titleText.text   = data.title;
        if (sublineText) sublineText.text = data.subline;
        if (tagText)     tagText.text     = data.tag;
        if (iconImage && data.icon) iconImage.sprite = data.icon;
        cardButton.onClick.RemoveAllListeners();
        cardButton.onClick.AddListener(() => {
            Debug.Log("CARD CLICK FIRED: " + data.title);
            Debug.Log($"Card {data.title}: button interactable = {cardButton.interactable}, button enabled = {cardButton.enabled}");
            onClick?.Invoke();
        });
        cardButton.interactable = !data.completed;
        if (completedOverlay) completedOverlay.gameObject.SetActive(data.completed);
    }
}
