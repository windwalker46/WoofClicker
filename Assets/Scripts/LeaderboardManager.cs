using System.Collections.Generic;
using UnityEngine;

public class LeaderboardManager : MonoBehaviour
{
    public static LeaderboardManager Instance;
    public int maxEntries = 10;

    private const string endlessKey = "EndlessLeaderboard";
    private const string raceKey = "RaceLeaderboard";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public List<int> GetLeaderboard(bool isEndless)
    {
        string key = isEndless ? endlessKey : raceKey;
        string savedData = PlayerPrefs.GetString(key, "");
        List<int> scores = new List<int>();

        if (!string.IsNullOrEmpty(savedData))
        {
            string[] parts = savedData.Split(',');
            foreach (string s in parts)
            {
                if (int.TryParse(s, out int score))
                {
                    scores.Add(score);
                }
            }
        }
        // Sort descending (highest first)
        scores.Sort((a, b) => b.CompareTo(a));
        return scores;
    }

    public void SaveScore(bool isEndless, int newScore)
    {
        List<int> scores = GetLeaderboard(isEndless);
        scores.Add(newScore);
        scores.Sort((a, b) => b.CompareTo(a));

        if (scores.Count > maxEntries)
        {
            scores = scores.GetRange(0, maxEntries);
        }

        string key = isEndless ? endlessKey : raceKey;
        string scoresString = string.Join(",", scores);
        PlayerPrefs.SetString(key, scoresString);
        PlayerPrefs.Save();
    }
}
