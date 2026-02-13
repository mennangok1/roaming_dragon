using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameOverScreen;

    [SerializeField] private AudioClip gameOverSound;


    [SerializeField] private GameObject pauseScreen;

    private void Awake() {
        gameOverScreen.SetActive(false);
        pauseScreen.SetActive(false);
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
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void MainMenu()
    {
        SceneManager.LoadScene(0);
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
