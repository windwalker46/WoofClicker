using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class LeaderboardUI : MonoBehaviour
{
    [Header("Leaderboard UI References")]
    public GameObject leaderboardPanel;              
    public TextMeshProUGUI endlessLeaderboardText;   
    public TextMeshProUGUI raceLeaderboardText;      
    public Button closeButton;                       

    [Header("Other UI References")]
    public UIManager uiManager;                      

    // Maximum number of scores to display
    private const int maxDisplayCount = 10;

    private void Start()
    {
        if (closeButton != null)
        {
            closeButton.onClick.AddListener(HideLeaderboard);
        }
    }

    public void ShowLeaderboard()
    {
        if (uiManager != null)
        {
            uiManager.HideAllPanels();
        }

        // Activate the leaderboard panel
        leaderboardPanel.SetActive(true);
        UpdateLeaderboard();
    }

    public void UpdateLeaderboard()
    {
        // Get all scores from LeaderboardManager
        List<int> endlessScores = LeaderboardManager.Instance.GetLeaderboard(true);
        List<int> raceScores = LeaderboardManager.Instance.GetLeaderboard(false);

        endlessLeaderboardText.text = "Endless Mode Leaderboard:\n";
        raceLeaderboardText.text = "Race Mode Leaderboard:\n";

        // Display top 10 Endless scores
        for (int i = 0; i < endlessScores.Count && i < maxDisplayCount; i++)
        {
            endlessLeaderboardText.text += (i + 1) + ". " + endlessScores[i] + "\n";
        }

        // Display top 10 Race scores
        for (int i = 0; i < raceScores.Count && i < maxDisplayCount; i++)
        {
            raceLeaderboardText.text += (i + 1) + ". " + raceScores[i] + "\n";
        }
    }


    public void HideLeaderboard()
    {
        leaderboardPanel.SetActive(false);
        if (uiManager != null)
        {
            uiManager.ShowMainMenu();
        }
    }
}
