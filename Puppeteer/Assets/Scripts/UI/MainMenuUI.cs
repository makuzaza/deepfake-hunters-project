using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

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
        sceneLoader.LoadScene(startSceneName);
    }

    public void OnOptionsButton()
    {
        Debug.Log("Options menu opened.");
    }

    public void OnQuitButton()
    {
        sceneLoader.QuitGame();
    }
}
