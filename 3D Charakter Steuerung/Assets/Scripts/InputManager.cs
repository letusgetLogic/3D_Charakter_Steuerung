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

    public void OnEscape(InputAction.CallbackContext _context)
    {
        if (_context.started)
        {
            // game is not started yet / return to the main menu
            if (SceneManager.GetActiveScene().name == "MenuScene")
            {
                SceneManager.LoadScene("MenuScene");
                return;
            }

            GameManager.Instance.SetPaused(!GameManager.Instance.IsPaused);
            settingsPanel.SetActive(GameManager.Instance.IsPaused);
        }
    }

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
