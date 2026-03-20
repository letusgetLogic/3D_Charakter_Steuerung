using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public bool IsPaused { get; private set; } = false;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Set game paused/continue + mouse locked/free
    /// </summary>
    /// <param name="_isPaused"></param>
    public void SetPaused(bool _isPaused)
    {
        IsPaused = _isPaused;
        Time.timeScale = _isPaused ? 0f : 1f;

        if (_isPaused)
            Cursor.lockState = CursorLockMode.None;
        else
            Cursor.lockState = CursorLockMode.Locked;
    }
}
