using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public Action OnPlayGamePressedEvent;
    public Action OnQuitGamePressedEvent;
    public Action OnPauseGamePressedEvent;
    public Action OnResumeGamePressedEvent;
    public Action OnRestartGamePressedEvent;
    public Action OnMainMenuPressedEvent;

    [SerializeField] GameObject mainMenuPanel;
    [SerializeField] GameObject gameplayPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject pausePanel;
    [SerializeField] GameObject optionsPanel;

    [SerializeField] Button playGameButton;
    [SerializeField] Button optionsButton;
    [SerializeField] Button quitGameButton;

    [SerializeField] Button pauseButton;

    [SerializeField] Button retryButton;
    [SerializeField] Button mainMenuGameOverButton;

    [SerializeField] Button resumeButton;
    [SerializeField] Button restartButton;
    [SerializeField] Button mainMenuPauseButton;

    [SerializeField] Button optionsOKButton;

    [SerializeField] TextMeshProUGUI scoreText;
    [SerializeField] TextMeshProUGUI gameOverScoreText;
    [SerializeField] TextMeshProUGUI newHighscoreText;

    private void Awake()
    {
        playGameButton.onClick.AddListener(OnPlayGamePressed);
        quitGameButton.onClick.AddListener(OnQuitGamePressed);
        optionsButton.onClick.AddListener(OnOptionsPressed);
        pauseButton.onClick.AddListener(OnPausePressed);
        retryButton.onClick.AddListener(OnRetryPressed);
        mainMenuGameOverButton.onClick.AddListener(MainMenuFromGameOverPressed);
        resumeButton.onClick.AddListener(OnResumeGamePressed);
        restartButton.onClick.AddListener(OnRestartGamePressed);
        mainMenuPauseButton.onClick.AddListener(MainMenuFromPausePressed);
        optionsOKButton.onClick.AddListener(OptionsOKButtonPressed);
    }


    public void OpenGameOverPanel(int score, bool isHighscore)
    {
        newHighscoreText.gameObject.SetActive(isHighscore);
        gameOverScoreText.text = score.ToString();
        SwitchPanels(gameplayPanel, gameOverPanel);
    }

    public void UpdateScore(int score)
    {
        scoreText.text = score.ToString();
    }

    private void MainMenuFromPausePressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        OnMainMenuPressedEvent?.Invoke();
        SwitchPanels(pausePanel, mainMenuPanel);
    }

    private void OnRestartGamePressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        OnRestartGamePressedEvent?.Invoke();
        SwitchPanels(pausePanel, gameplayPanel);
    }

    private void OnResumeGamePressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        AudioManager.Instance.UnpauseMusic();
        OnResumeGamePressedEvent?.Invoke();
        SwitchPanels(pausePanel, gameplayPanel);
    }

    private void MainMenuFromGameOverPressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        OnMainMenuPressedEvent?.Invoke();
        SwitchPanels(gameOverPanel, mainMenuPanel);
    }

    private void OnRetryPressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        OnRestartGamePressedEvent?.Invoke();
        SwitchPanels(gameOverPanel, gameplayPanel);
    }

    private void OnPausePressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        AudioManager.Instance.PauseMusic(); 
        OnPauseGamePressedEvent?.Invoke();
        SwitchPanels(gameplayPanel, pausePanel);
    }

    private void OnOptionsPressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        SwitchPanels(mainMenuPanel, optionsPanel);
    }

    private void OnPlayGamePressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        OnPlayGamePressedEvent?.Invoke();
        SwitchPanels(mainMenuPanel, gameplayPanel);
    }

    private void OnQuitGamePressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        OnQuitGamePressedEvent?.Invoke();
    }

    private void OptionsOKButtonPressed()
    {
        AudioManager.Instance.Play("ButtonClick");
        SwitchPanels(optionsPanel, mainMenuPanel);
    }

    private void SwitchPanels(GameObject closePanel, GameObject openPanel)
    {
        closePanel.SetActive(false);
        openPanel.SetActive(true);
    }
}
