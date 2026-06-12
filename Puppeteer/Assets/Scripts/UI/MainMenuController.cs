// MainMenuController.cs  -  Assets/_Project/Scripts/UI
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class MainMenuController : MonoBehaviour
{
    public Transform uiRoot;
    private void Start()
    {
        Hook("PlayButton", () => SceneManager.LoadScene("MainGame"));
        Hook("QuitButton", Quit);
    }
    private void Hook(string name, UnityEngine.Events.UnityAction a)
    {
        var t = UIFind.Deep(uiRoot != null ? uiRoot : transform, name);
        var b = t ? t.GetComponent<Button>() : null;
        if (b) b.onClick.AddListener(a);
    }
    private void Quit()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
