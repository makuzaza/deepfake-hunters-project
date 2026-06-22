// InboxItemData.cs — serializable data for one inbox card row
using UnityEngine;
[System.Serializable]
public class InboxItemData
{
    public string      title;
    public string      subline;
    public string      tag;         // e.g. "€200", "MSG", "2"
    public Sprite      icon;
    public InboxAction action;
    public bool        completed;   // shows tick, greys out the card
}
