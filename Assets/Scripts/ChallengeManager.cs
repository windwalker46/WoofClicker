using UnityEngine;

public class ChallengeManager : MonoBehaviour
{
    [Header("References")]
    public GameManager gameManager;
    public UIManager uiManager;
    public PowerUpManager powerUpManager;

    [Header("Challenge Trigger Settings")]
    public float challengeInterval = 30f;
    public int tapInterval = 100;

    [Header("Challenge Defaults")]
    public int defaultChallengeGoal = 50;
    public float defaultChallengeTime = 10f;

    private bool isChallengeActive = false;
    private float challengeTimeLeft;
    private int challengeGoal;
    private int currentChallengeTaps;

    private float lastChallengeTime;
    private int lastChallengeTapCount;

    private void Start()
    {
        lastChallengeTime = Time.time;
        lastChallengeTapCount = 0;
    }

    private void Update()
    {
        // Challenges only if the game is active, in Endless mode, and not in a challenge
        if (!gameManager.IsGameActive() || !gameManager.IsEndlessMode() || isChallengeActive)
            return;

        float timeSinceLastChallenge = Time.time - lastChallengeTime;
        int tapsSinceLastChallenge = gameManager.GetTotalTaps() - lastChallengeTapCount;

        if (timeSinceLastChallenge >= challengeInterval || tapsSinceLastChallenge >= tapInterval)
        {
            StartChallenge(defaultChallengeGoal, defaultChallengeTime);
        }
    }

    private void FixedUpdate()
    {
        if (isChallengeActive)
        {
            challengeTimeLeft -= Time.deltaTime;
            uiManager.UpdateChallengeTimer(challengeTimeLeft);

            if (challengeTimeLeft <= 0f)
            {
                EndChallenge(false);
            }
        }
    }

    public void StartChallenge(int goal, float timeLimit)
    {
        if (powerUpManager != null)
        {
            powerUpManager.isChallengeActive = true;
            powerUpManager.HideAllPowerUps();
        }

        isChallengeActive = true;
        challengeGoal = goal;
        challengeTimeLeft = timeLimit;
        currentChallengeTaps = 0;

        gameManager.FreezeTimer(Mathf.Infinity);

        uiManager.ShowChallengeUI(challengeGoal, challengeTimeLeft);

        lastChallengeTime = Time.time;
        lastChallengeTapCount = gameManager.GetTotalTaps();
    }

    public void UpdateChallengeOnTap()
    {
        if (!isChallengeActive) return;

        currentChallengeTaps++;
        uiManager.UpdateChallengeProgress(currentChallengeTaps, challengeGoal);

        if (currentChallengeTaps >= challengeGoal)
        {
            EndChallenge(true);
        }
    }

    private void EndChallenge(bool success)
    {
        isChallengeActive = false;

        if (success)
        {
            // Reward the player
            gameManager.AddTime(15f);
            gameManager.UnfreezeTimer();

            uiManager.HideChallengeUI();
            uiManager.ShowNormalUIForCurrentMode();
        }
        else
        {
            // If the challenge fails, the game ends
            gameManager.GameOver();
        }

        if (powerUpManager != null)
        {
            powerUpManager.isChallengeActive = false;
        }
    }

    public bool IsChallengeActive()
    {
        return isChallengeActive;
    }

    public void ResetChallenge()
    {
        isChallengeActive = false;
        challengeTimeLeft = 0f;
        challengeGoal = 0;
        currentChallengeTaps = 0;

        lastChallengeTime = Time.time;
        lastChallengeTapCount = 0;

        uiManager.HideChallengeUI();

        gameManager.UnfreezeTimer();

        if (powerUpManager != null)
        {
            powerUpManager.isChallengeActive = false;
            powerUpManager.HideAllPowerUps(); // optional, to ensure none remain
        }
    }
}
