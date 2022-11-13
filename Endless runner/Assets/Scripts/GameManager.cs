using System.Collections;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] PlayerController PlayerController;
    [SerializeField] MapGenerator MapGenerator;
    [SerializeField] UIManager UIManager;

    private void Awake()
    {
        PlayerController.GameOverEvent += OnGameOver; 
        PlayerController.ScoreUpdateEvent += UIManager.UpdateScore;
        PlayerController.TurnEvent += MapGenerator.AddNewDirection;

        UIManager.OnPlayGamePressedEvent += PlayGame;
        UIManager.OnQuitGamePressedEvent += QuitGame;
        UIManager.OnPauseGamePressedEvent += PauseGame;
        UIManager.OnResumeGamePressedEvent += UnpauseGame;
        UIManager.OnRestartGamePressedEvent += RestartGame;
        UIManager.OnMainMenuPressedEvent += ExitGame;
    }

    private void Start()
    {
        AudioManager.Instance.Play("MenuTheme");
    }

    private void QuitGame()
    {
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    private void PlayGame()
    {
        MapGenerator.StartGeneratingMap();
        StartCoroutine(EnablePLayer());
    }

    private IEnumerator EnablePLayer()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        PlayerController.gameObject.SetActive(true);
        AudioManager.Instance.Play("GameplayTheme");
    }

    private void PauseGame()
    {
        Time.timeScale = 0;
        PlayerController.SetPlayerMovementState(false);
    }

    private void UnpauseGame()
    {
        Time.timeScale = 1;
        PlayerController.SetPlayerMovementState(true);
    }

    private void ExitGame()
    {
        PlayerController.gameObject.SetActive(false);
        MapGenerator.ResetMap();
        PlayerController.ResetPlayer();
        UnpauseGame();

        AudioManager.Instance.Play("MenuTheme");
    }

    private void RestartGame()
    {
        PlayerController.gameObject.SetActive(false);
        MapGenerator.ResetMap();
        PlayerController.ResetPlayer();
        UnpauseGame();
        PlayGame();
    }

    private void OnGameOver(int score)
    {
        bool isHighscore = false;

        if((PlayerPrefs.HasKey("Highscore") && PlayerPrefs.GetInt("Highscore") > score) == false)
        { 
            PlayerPrefs.SetInt("Highscore", score);
            PlayerPrefs.Save();
            isHighscore = true;
        }

        UIManager.OpenGameOverPanel(score, isHighscore);

        AudioManager.Instance.Play("GameOver");
    }  
}
