using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class BombPowerUp : MonoBehaviour
{
    public float countdownTime = 5f;
    public float penaltyTime = 10f;
    public Sprite trashcanSprite; // Sprite to switch to when the bomb is disposed
    private bool isDisposed = false;
    private Button button;
    private Image buttonImage;
    private TextMeshProUGUI countdownText;
    private PowerUpManager powerUpManager;
    public AudioClip bombSound;

    /// <summary>
    /// Initializes the bomb power-up.
    /// </summary>
    /// <param name="countdown">Countdown duration (in seconds) before the bomb penalizes the player.</param>
    /// <param name="penalty">The amount of time to reduce when the bomb goes off.</param>
    /// <param name="trashcan">Sprite to show when the bomb is disposed (clicked in time).</param>
    /// <param name="sound">Sound to play when the bomb goes off (penalty is applied).</param>
    /// <param name="manager">Reference to the PowerUpManager (for removal tracking).</param>
    public void Initialize(float countdown, float penalty, Sprite trashcan, AudioClip sound, PowerUpManager manager)
    {
        countdownTime = countdown;
        penaltyTime = penalty;
        trashcanSprite = trashcan;
        bombSound = sound;
        powerUpManager = manager;

        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();
        countdownText = GetComponentInChildren<TextMeshProUGUI>();

        // Remove any existing onClick listeners and add our own.
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnBombClicked);
    }

    private void OnEnable()
    {
        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        float timeLeft = countdownTime;
        while (timeLeft > 0f)
        {
            if (countdownText != null)
            {
                countdownText.text = Mathf.Ceil(timeLeft).ToString();
            }
            timeLeft -= Time.deltaTime;
            yield return null;
        }

        // Countdown finished: if the bomb hasn’t been disposed, apply its penalty.
        if (!isDisposed)
        {
            ApplyPenalty();
        }
    }

    private void ApplyPenalty()
    {
        // Play the bomb sound for the penalty effect.
        if (AudioManager.Instance != null && bombSound != null)
        {
            AudioManager.Instance.PlaySFX(bombSound);
        }

        // Reduce the timer using the GameManager.
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.ReduceTimer(penaltyTime);
        }

        // Remove this bomb from the PowerUpManager tracking.
        if (powerUpManager != null && button != null)
        {
            powerUpManager.RemovePowerUp(button);
        }

        Destroy(gameObject);
    }

    private void OnBombClicked()
    {
        if (isDisposed) return;
        isDisposed = true;

        // Change the bomb's sprite to the trashcan sprite.
        if (buttonImage != null && trashcanSprite != null)
        {
            buttonImage.sprite = trashcanSprite;
            // Reset the color so that any prefab color changes do not affect the trashcan.
            buttonImage.color = Color.white;
        }

        // Hide the countdown text so it does not appear on the trashcan.
        if (countdownText != null)
        {
            countdownText.gameObject.SetActive(false);
        }

        // Stop the countdown (the coroutine will check isDisposed) and schedule removal.
        StartCoroutine(DestroyAfterDelay());
    }

    private IEnumerator DestroyAfterDelay()
    {
        yield return new WaitForSeconds(0.5f);
        if (powerUpManager != null && button != null)
        {
            powerUpManager.RemovePowerUp(button);
        }
        Destroy(gameObject);
    }
}
