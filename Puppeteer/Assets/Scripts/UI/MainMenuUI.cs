using UnityEngine;
using UnityEditor;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    [Header("Scene On Click")]
    [SerializeField] private SceneAsset startScene;

    // Called by the Start button
    public void OnStartButton()
    {
        sceneLoader.LoadScene(startScene.name);
    }

    // Called by the Options button
    public void OnOptionsButton()
    {
        // Show options panel, play sound, animate, etc.
        Debug.Log("Options menu opened.");
    }

    // Called by the Quit button
    public void OnQuitButton()
    {
        sceneLoader.QuitGame();
    }
}