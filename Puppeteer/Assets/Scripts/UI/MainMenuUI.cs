using UnityEngine;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;

    // Called by the Start button
    public void OnStartButton()
    {
        sceneLoader.LoadScene("Act1");
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