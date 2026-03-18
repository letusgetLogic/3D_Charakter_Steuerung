using UnityEngine;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{



    bool isPaused = false;



    /// <summary>
    /// Called when play button in menu is pressed.
    /// </summary>
    public void OnPlayButton()
    {
        SceneManager.LoadScene("PrototypeScene");
    }

    /// <summary>
    /// Called when menu button in settings is pressed.
    /// </summary>
    public void OnMenuButton()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
