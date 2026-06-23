using UnityEngine;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{
    [Header("Scene On Click")]
#if UNITY_EDITOR
    [SerializeField] private SceneAsset startScene;
#endif

    [SerializeField] private string startSceneName;

#if UNITY_EDITOR
    private void OnValidate()
    {
        if (startScene != null)
            startSceneName = startScene.name;
    }
#endif

    public void OnStartButton()
    {
        SceneManager.LoadScene(startSceneName);
    }

    public void OnOptionsButton()
    {
        Debug.Log("Options menu opened.");
    }

    public void OnQuitButton()
    {
        Application.Quit();
    }
}
