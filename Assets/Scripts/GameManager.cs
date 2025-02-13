using UnityEngine;

public class GameManager : MonoBehaviour
{
    public UIManager uiManager;
    public PowerUpManager powerUpManager;
    public TapHandler tapHandler;
    public ChallengeManager challengeManager;

    public int overallGoal = 100;
    public int goal = 10;
    private float timer = 30f;
    private bool isGameActive = false;
    private bool isTimerFrozen = false;
    private bool isEndlessMode = false;
    private int totalTaps = 0;

    private void Start()
    {
        AudioManager.Instance.PlayBackgroundMusic();
    }

    private void Update()
    {
        if (!isGameActive || isTimerFrozen) return;

        timer -= Time.deltaTime;
        uiManager.UpdateTimer(Mathf.CeilToInt(timer));

        // TEMPORARY: Press 'G' to force Game Over (for testing)
        /*if (Input.GetKeyDown(KeyCode.G))
        {
            Debug.Log("Key 'G' pressed – forcing GameOver()");
            GameOver();
        }*/

        if (timer <= 0f)
        {
            GameOver();
        }
    }

    public void StartGame(bool endlessMode)
    {
        UnfreezeTimer();

        if (challengeManager != null)
        {
            challengeManager.ResetChallenge();
        }

        isGameActive = true;
        isEndlessMode = endlessMode;
        timer = 30f;
        goal = 10;
        totalTaps = 0;

        tapHandler.ResetTapCount();
        uiManager.UpdateTapCount(0);

        if (isEndlessMode)
        {
            uiManager.DisplayTotalTaps();
        }
        else
        {
            uiManager.DisplayOverallGoal(overallGoal);
        }

        uiManager.ResetUI(goal, Mathf.CeilToInt(timer));
        powerUpManager.HideAllPowerUps();

        uiManager.ShowGameplayUI();
        uiManager.ShowNormalUIForCurrentMode();
    }

    public void LevelUp()
    {
        goal += 10;
        uiManager.UpdateGoal(goal);
        AudioManager.Instance.PlaySFX(AudioManager.Instance.levelUpClip);

        if (!challengeManager.IsChallengeActive())
        {
            powerUpManager.TrySpawnPowerUp();
        }

        if (isEndlessMode)
        {
            AddTime(10f);
        }
    }

    public void UpdateTotalTaps(int taps)
    {
        totalTaps = taps;
        uiManager.UpdateTapCount(taps);
    }

    public int GetTotalTaps()
    {
        return totalTaps;
    }

    public void WinGame()
    {
        if (isEndlessMode) return;

        isGameActive = false;
        uiManager.ShowWinScreen();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.winClip);
        powerUpManager.HideAllPowerUps();

        // Save the score for Race mode
        LeaderboardManager.Instance.SaveScore(false, goal);

        // (Ad calls commented out to avoid errors)
        // #if UNITY_WEBGL && !UNITY_EDITOR
        // Debug.Log("WinGame() called - calling ShowAd()...");
        // Application.ExternalCall("ShowAd");
        // #endif
    }

    public void GameOver()
    {
        Debug.Log("GameOver() called.");
        isGameActive = false;
        uiManager.ShowGameOverScreen();
        AudioManager.Instance.PlaySFX(AudioManager.Instance.gameOverClip);
        powerUpManager.HideAllPowerUps();

        if (IsEndlessMode())
        {
            LeaderboardManager.Instance.SaveScore(true, totalTaps);
        }
        else
        {
            LeaderboardManager.Instance.SaveScore(false, goal);
        }

        // (Ad calls commented out to avoid errors)
        // #if UNITY_WEBGL && !UNITY_EDITOR
        // Debug.Log("GameOver() - calling ShowAd()...");
        // Application.ExternalCall("ShowAd");
        // #endif
    }

    public void RestartGame()
    {
        UnfreezeTimer();

        if (challengeManager != null)
        {
            challengeManager.ResetChallenge();
        }

        isGameActive = true;
        timer = 30f;
        goal = 10;
        totalTaps = 0;

        tapHandler.ResetTapCount();
        uiManager.UpdateTapCount(0);

        uiManager.ResetUI(goal, Mathf.CeilToInt(timer));
        powerUpManager.HideAllPowerUps();

        if (isEndlessMode)
        {
            uiManager.DisplayTotalTaps();
        }
        else
        {
            uiManager.DisplayOverallGoal(overallGoal);
        }
    }

    public bool IsGameActive()
    {
        return isGameActive;
    }

    public bool IsEndlessMode()
    {
        return isEndlessMode;
    }

    public void FreezeTimer(float duration)
    {
        StartCoroutine(FreezeTimerCoroutine(duration));
    }

    public void UnfreezeTimer()
    {
        StopAllCoroutines();
        isTimerFrozen = false;
    }

    private System.Collections.IEnumerator FreezeTimerCoroutine(float duration)
    {
        isTimerFrozen = true;
        yield return new WaitForSeconds(duration);
        isTimerFrozen = false;
    }

    public void ReduceTimer(float amount)
    {
        timer -= amount;
        if (timer < 0)
        {
            timer = 0;
        }
    }

    public void AddTime(float amount)
    {
        timer += amount;
        uiManager.UpdateTimer(Mathf.CeilToInt(timer));
    }

    public void BackToMainMenu()
    {
        isGameActive = false;
        timer = 30f;
        goal = 10;
        totalTaps = 0;

        if (challengeManager != null)
        {
            challengeManager.ResetChallenge();
        }

        uiManager.HideChallengeUI();
        powerUpManager.HideAllPowerUps();

        uiManager.ShowMainMenu();
    }
}
