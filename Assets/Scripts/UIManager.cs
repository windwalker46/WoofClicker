using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TextMeshProUGUI tapCounterText;    
    public TextMeshProUGUI goalText;         
    public TextMeshProUGUI tapHighscoreText;  
    public TextMeshProUGUI overallGoalText;   
    public TextMeshProUGUI timerText;         

    public GameObject gameOverPanel;
    public GameObject youWinPanel;
    public GameObject mainMenuPanel;  
    public GameObject gameplayPanel;

    public GameObject leaderboardPanel;

    public Button retryButton;
    public Button playAgainButton;
    public Button raceModeButton;
    public Button endlessModeButton;
    public Button returnToMenuFromGameOverButton;
    public Button returnToMenuFromWinButton;

    public GameManager gameManager; // Reference to the GameManager

    // Challenge UI fields
    public TextMeshProUGUI challengeDescriptionText;
    public TextMeshProUGUI challengeTimerText;
    public TextMeshProUGUI challengeProgressText;

    private void Start()
    {
        if (retryButton != null)
        {
            retryButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                gameManager.RestartGame();
                ShowGameplayUI();
                ShowNormalUIForCurrentMode();
            });
        }

        if (playAgainButton != null)
        {
            playAgainButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                gameManager.RestartGame();
                ShowGameplayUI();
                ShowNormalUIForCurrentMode();
            });
        }

        if (raceModeButton != null)
        {
            raceModeButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                gameManager.StartGame(false); // Race Mode
                ShowGameplayUI();
                ShowNormalUIForCurrentMode();
            });
        }

        if (endlessModeButton != null)
        {
            endlessModeButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                gameManager.StartGame(true); // Endless Mode
                ShowGameplayUI();
                ShowNormalUIForCurrentMode();
            });
        }

        if (returnToMenuFromGameOverButton != null)
        {
            returnToMenuFromGameOverButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                gameManager.BackToMainMenu();
                ShowMainMenu();
            });
        }

        if (returnToMenuFromWinButton != null)
        {
            returnToMenuFromWinButton.onClick.AddListener(() =>
            {
                PlayButtonClickSound();
                gameManager.BackToMainMenu();
                ShowMainMenu();
            });
        }

        // Set the initial UI state
        ResetUI(10, 30);
        ShowMainMenu();

        if (challengeDescriptionText != null) challengeDescriptionText.gameObject.SetActive(false);
        if (challengeTimerText != null) challengeTimerText.gameObject.SetActive(false);
        if (challengeProgressText != null) challengeProgressText.gameObject.SetActive(false);
    }

    public void UpdateTapCount(int count)
    {
        tapCounterText.text = "Taps: " + count;

        if (tapHighscoreText.gameObject.activeSelf)
        {
            UpdateTotalTaps(count);
        }
    }

    public void UpdateGoal(int goal)
    {
        goalText.text = "Level Goal: " + goal;
    }

    public void UpdateTimer(int time)
    {
        timerText.text = "Time: " + time + "s";
    }

    // Hides all panels
    public void HideAllPanels()
    {
        if (mainMenuPanel != null)
            mainMenuPanel.SetActive(false);
        if (gameplayPanel != null)
            gameplayPanel.SetActive(false);
        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);
        if (youWinPanel != null)
            youWinPanel.SetActive(false);
        if (leaderboardPanel != null)
            leaderboardPanel.SetActive(false);
    }

    public void ShowGameOverScreen()
    {
        HideAllPanels();
        gameOverPanel.SetActive(true);
    }

    public void ShowWinScreen()
    {
        HideAllPanels();
        youWinPanel.SetActive(true);
    }

    public void ShowMainMenu()
    {
        HideAllPanels();
        mainMenuPanel.SetActive(true);

        HideChallengeUI();

        if (tapCounterText != null) tapCounterText.gameObject.SetActive(false);
        if (goalText != null) goalText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (tapHighscoreText != null) tapHighscoreText.gameObject.SetActive(false);
        if (overallGoalText != null) overallGoalText.gameObject.SetActive(false);
    }

    public void ShowGameplayUI()
    {
        HideAllPanels();
        gameplayPanel.SetActive(true);
    }

    public void ResetUI(int goal, int timer)
    {
        HideAllPanels();
        UpdateGoal(goal);
        UpdateTimer(timer);
    }

    public void DisplayOverallGoal(int overallGoal)
    {
        if (overallGoalText != null)
        {
            overallGoalText.gameObject.SetActive(true);
            overallGoalText.text = "Goal: " + overallGoal;
        }

        if (tapHighscoreText != null)
        {
            tapHighscoreText.gameObject.SetActive(false);
        }
    }

    public void DisplayTotalTaps()
    {
        if (tapHighscoreText != null)
        {
            tapHighscoreText.gameObject.SetActive(true);
            tapHighscoreText.text = "Score: 0";
        }

        if (overallGoalText != null)
        {
            overallGoalText.gameObject.SetActive(false);
        }
    }

    public void UpdateTotalTaps(int totalTaps)
    {
        if (tapHighscoreText != null)
        {
            tapHighscoreText.text = "Score: " + totalTaps;
        }
    }

    private void PlayButtonClickSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.levelUpClip);
        }
    }

    // ----------------------------------------------------------------------
    // Challenge UI Methods
    // ----------------------------------------------------------------------

    public void ShowChallengeUI(int goal, float timeLimit)
    {
        // Hide normal UI
        if (overallGoalText != null) overallGoalText.gameObject.SetActive(false);
        if (goalText != null) goalText.gameObject.SetActive(false);
        if (timerText != null) timerText.gameObject.SetActive(false);
        if (tapCounterText != null) tapCounterText.gameObject.SetActive(false);
        if (tapHighscoreText != null) tapHighscoreText.gameObject.SetActive(false);

        // Show challenge texts
        if (challengeDescriptionText != null)
        {
            challengeDescriptionText.gameObject.SetActive(true);
            challengeDescriptionText.text = $"Challenge: Tap {goal} times in {timeLimit:F1}s!";
        }
        if (challengeTimerText != null)
        {
            challengeTimerText.gameObject.SetActive(true);
            challengeTimerText.text = $"Time Left: {timeLimit:F1}";
        }
        if (challengeProgressText != null)
        {
            challengeProgressText.gameObject.SetActive(true);
            challengeProgressText.text = $"Progress: 0 / {goal}";
        }
    }

    public void HideChallengeUI()
    {
        if (challengeDescriptionText != null) challengeDescriptionText.gameObject.SetActive(false);
        if (challengeTimerText != null) challengeTimerText.gameObject.SetActive(false);
        if (challengeProgressText != null) challengeProgressText.gameObject.SetActive(false);
    }

    public void ShowNormalUIForCurrentMode()
    {
        HideChallengeUI();

        if (goalText != null) goalText.gameObject.SetActive(true);
        if (timerText != null) timerText.gameObject.SetActive(true);
        if (tapCounterText != null) tapCounterText.gameObject.SetActive(true);

        if (gameManager != null && gameManager.IsEndlessMode())
        {
            if (tapHighscoreText != null) tapHighscoreText.gameObject.SetActive(true);
            if (overallGoalText != null) overallGoalText.gameObject.SetActive(false);
        }
        else
        {
            if (overallGoalText != null) overallGoalText.gameObject.SetActive(true);
            if (tapHighscoreText != null) tapHighscoreText.gameObject.SetActive(false);
        }
    }

    public void UpdateChallengeTimer(float timeLeft)
    {
        if (challengeTimerText != null)
        {
            challengeTimerText.text = $"Time Left: {timeLeft:F1}";
        }
    }

    public void UpdateChallengeProgress(int progress, int goal)
    {
        if (challengeProgressText != null)
        {
            challengeProgressText.text = $"Progress: {progress} / {goal}";
        }
    }
}
