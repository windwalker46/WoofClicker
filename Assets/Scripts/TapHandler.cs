using UnityEngine;

public class TapHandler : MonoBehaviour
{
    private int tapCount = 0;
    private int multiplier = 1;

    public GameManager gameManager;
    public UIManager uiManager;
    public ParticleSystem tapParticles;
    public RectTransform buttonTransform;

    private bool isAnimating = false;

    public void OnTap()
    {
        if (!gameManager.IsGameActive()) return;

        tapCount += multiplier;
        uiManager.UpdateTapCount(tapCount);

        // Play tap sound
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(AudioManager.Instance.buttonTapClip);
        }
        else
        {
            Debug.LogWarning("AudioManager instance is null!");
        }

        // Play particle effect 
        if (tapParticles != null)
        {
            tapParticles.Clear();
            tapParticles.Play();
        }

        if (buttonTransform != null && !isAnimating)
        {
            StartCoroutine(AnimateButton());
        }

        if (tapCount >= gameManager.goal)
        {
            gameManager.LevelUp();
        }

        if (tapCount >= gameManager.overallGoal)
        {
            gameManager.WinGame();
        }

        // Update total taps in GameManager
        gameManager.UpdateTotalTaps(tapCount);

        // If we are in Endless Mode and have a ChallengeManager
        if (gameManager.IsEndlessMode() && gameManager.challengeManager != null)
        {
            gameManager.challengeManager.UpdateChallengeOnTap();
        }
    }

    public void ResetTapCount()
    {
        tapCount = 0;
    }

    public void ActivateMultiplier(int newMultiplier, float duration)
    {
        StartCoroutine(ActivateMultiplierCoroutine(newMultiplier, duration));
    }

    private System.Collections.IEnumerator ActivateMultiplierCoroutine(int newMultiplier, float duration)
    {
        multiplier = newMultiplier;
        yield return new WaitForSeconds(duration);
        multiplier = 1;
    }

    private System.Collections.IEnumerator AnimateButton()
    {
        isAnimating = true; // Mark as animating
        Vector3 originalScale = buttonTransform.localScale;
        Vector3 pressedScale = originalScale * 0.9f;

        buttonTransform.localScale = pressedScale;
        yield return new WaitForSeconds(0.1f);

        buttonTransform.localScale = originalScale;
        isAnimating = false; // Animation finished
    }
}
