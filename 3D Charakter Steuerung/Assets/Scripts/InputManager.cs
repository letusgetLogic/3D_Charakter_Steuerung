using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance;

    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        // load new, destroy old = reset to have references on serialized field.
        if (Instance != null) 
        {
            Destroy(Instance.gameObject);
            return;
        }
        Instance = this;
    }


    /// <summary>
    /// Called when menu button in settings is clicked.
    /// </summary>
    public void OnMenuButton()
    {
        SceneManager.LoadScene("MenuScene");
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Called when escape button is pressed. Not working in editor.
    /// </summary>
    /// <param name="_context"></param>
    public void OnEscape(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            // game is not started yet / return to the main menu
            if (SceneManager.GetActiveScene().name == "MenuScene")
            {
                SceneManager.LoadScene("MenuScene");
                Time.timeScale = 1.0f;
                return;
            }

            GameManager.Instance.SetPaused(!GameManager.Instance.IsPaused);
            settingsPanel.SetActive(GameManager.Instance.IsPaused);
        }
    }

    /// <summary>
    /// Called when prototype button in menu is clicked.
    /// </summary>
    public void OnPrototypeButton()
    {
        SceneManager.LoadScene("PrototypeScene");
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Called when ToTheMoon-Level button is clicked.
    /// </summary>
    public void OnToTheMoonButton()
    {
        SceneManager.LoadScene("ToTheMoon");
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// Called when quit button is clicked.
    /// </summary>
    public void OnQuitButton()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
