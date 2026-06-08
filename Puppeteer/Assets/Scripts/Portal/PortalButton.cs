using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PortalButton : MonoBehaviour
{
    public string action;

    void Start()
    {
        GetComponent<Button>().onClick.AddListener(Fire);
    }

    void Fire()
    {
        var ctrl = GetComponentInParent<PortalUIController>();
        if (ctrl == null) return;
        switch (action)
        {
            case "login":         ctrl.OnLoginSubmit();  break;
            case "hr":            ctrl.OnHRSubmit();     break;
            case "marcus_guide":  ctrl.OnMarcusGuide();  break;
            case "marcus_self":   ctrl.OnMarcusSelf();   break;
            case "task1":         ctrl.OnTakeTask1();    break;
            case "task2":         ctrl.OnTakeTask2();    break;
            case "refuse":        ctrl.OnRefuseBoth();   break;
            case "exit":          ctrl.OnTask1Exit();    break;
            case "card_hr":       ctrl.OnCardHR();       break;
            case "card_marcus":   ctrl.OnCardMarcus();   break;
            case "card_briefs":   ctrl.OnCardBriefs();   break;
        }
    }
}
