using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{

    [Header( "Audio")]
    [SerializeField] private AudioClip gameOverSound;


    [Header("UI Elements")]
    [SerializeField] private GameObject gameOverScreen;
    [SerializeField] private GameObject pauseScreen;
    [SerializeField] private GameObject healthBar;
    [SerializeField] private GameObject globalHealthBar;
    [SerializeField] private GameObject mainMenuScreen;

    private void Awake() {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
        healthBar.SetActive(false);
        globalHealthBar.SetActive(false);
        mainMenuScreen.SetActive(true);
    }
    
    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (pauseScreen.activeInHierarchy)
            {
                PauseGame(false);
            }
            else
            {
                PauseGame(true);
            }
        }
    }
    public void GameOver()
    {
        gameOverScreen.SetActive(true);
        SoundManager.instance.PlaySound(gameOverSound);
    }

    public void Restart()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        mainMenuScreen.SetActive(true);
        Time.timeScale = 1f;
        Restart();
    }
    public void Quit()
    {
        Application.Quit();
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; 
        #endif
    }

    #region  Pause
    
    public void PauseGame(bool status)
    {
        pauseScreen.SetActive(status);
        if (status)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }

    public void StartGame()
    {
        pauseScreen.SetActive(false);
        gameOverScreen.SetActive(false);
        healthBar.SetActive(true);
        globalHealthBar.SetActive(true);
        mainMenuScreen.SetActive(false);
    }
    

    public void ChangeVolume()
    {
        SoundManager.instance.ChangeSoundVolume(0.2f);
    }

    public void ChangeMusic()
    {
        SoundManager.instance.ChangeMusicVolume(0.2f);
    }
    #endregion
}
