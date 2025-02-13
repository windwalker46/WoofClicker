using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class PowerUpManager : MonoBehaviour
{
    [Header("Power-Up Prefabs")]
    public Button multiplierPrefab;
    public Button freezeTimePrefab;
    public Button bombPrefab;

    [Header("Canvas & Limits")]
    public Transform canvasTransform;
    public int maxActivePowerUps = 3;
    public float displayDuration = 3f;

    [Header("Spawn Chances")]
    [Range(0f, 1f)] public float multiplierSpawnChance = 0.3f;
    [Range(0f, 1f)] public float freezeTimeSpawnChance = 0.2f;
    [Range(0f, 1f)] public float bombSpawnChance = 0.1f;

    [Header("Audio Clips")]
    public AudioClip multiplierSound;
    public AudioClip freezeTimeSound;
    public AudioClip bombSound;

    [Header("Bomb Settings")]
    public Sprite bombTrashcanSprite; // Sprite to use when bomb is disposed

    [HideInInspector]
    public bool isChallengeActive = false;

    private List<Button> activePowerUps = new List<Button>();
    private Dictionary<Button, int> powerUpToSpawnIndex = new Dictionary<Button, int>();
    private Vector3[] spawnPoints;
    private bool[] usedSpawnPoints;

    private void Start()
    {
        // Spawn positions
        spawnPoints = new Vector3[]
        {
            new Vector3(-362, 100, 0),
            new Vector3(362, -70, 0),
            new Vector3(180, -160, 0),
            new Vector3(-180, -160, 0),
            new Vector3(-362, -70, 0),
            new Vector3(-362, 19, 0),
            new Vector3(362, 19, 0),
            new Vector3(362, 100, 0)
        };

        usedSpawnPoints = new bool[spawnPoints.Length];
        for (int i = 0; i < usedSpawnPoints.Length; i++)
        {
            usedSpawnPoints[i] = false;
        }
    }

    public void TrySpawnPowerUp()
    {
        // If challenge is active, skip spawning
        if (isChallengeActive) return;

        // If 'maxActivePowerUps' in the scene, remove the oldest
        if (activePowerUps.Count >= maxActivePowerUps)
        {
            RemoveOldestPowerUp();
        }

        float chance = Random.value;
        Button chosenPrefab = null;

        if (chance < multiplierSpawnChance)
        {
            chosenPrefab = multiplierPrefab;
        }
        else if (chance < multiplierSpawnChance + freezeTimeSpawnChance)
        {
            chosenPrefab = freezeTimePrefab;
        }
        else if (chance < multiplierSpawnChance + freezeTimeSpawnChance + bombSpawnChance)
        {
            chosenPrefab = bombPrefab;
        }

        // If no power-up selected, skip
        if (chosenPrefab == null) return;

        // Find a free spawn point
        int freeIndex = GetFreeSpawnPointIndex();
        if (freeIndex == -1) return; // No free spots

        usedSpawnPoints[freeIndex] = true;

        Button newPowerUp = Instantiate(chosenPrefab, canvasTransform);
        newPowerUp.transform.localPosition = spawnPoints[freeIndex];

        powerUpToSpawnIndex[newPowerUp] = freeIndex;
        activePowerUps.Add(newPowerUp);

        if (chosenPrefab == bombPrefab)
        {
            // For bomb power-up
            BombPowerUp bombScript = newPowerUp.gameObject.AddComponent<BombPowerUp>();
            bombScript.Initialize(5f, 10f, bombTrashcanSprite, bombSound, this);

        }
        else
        {
            newPowerUp.onClick.AddListener(() =>
            {
                ActivatePowerUp(newPowerUp, chosenPrefab);
                DestroyPowerUp(newPowerUp);
            });

            // Destroy after displayDuration if not clicked
            StartCoroutine(DestroyPowerUpAfterDelay(newPowerUp, displayDuration));
        }
    }

    private int GetFreeSpawnPointIndex()
    {
        List<int> freeIndexes = new List<int>();
        for (int i = 0; i < usedSpawnPoints.Length; i++)
        {
            if (!usedSpawnPoints[i])
            {
                freeIndexes.Add(i);
            }
        }

        if (freeIndexes.Count == 0) return -1;
        int rand = Random.Range(0, freeIndexes.Count);
        return freeIndexes[rand];
    }

    private void RemoveOldestPowerUp()
    {
        if (activePowerUps.Count == 0) return;
        Button oldest = activePowerUps[0];
        DestroyPowerUp(oldest);
    }

    private IEnumerator DestroyPowerUpAfterDelay(Button powerUp, float delay)
    {
        yield return new WaitForSeconds(delay);
        if (powerUp != null)
        {
            DestroyPowerUp(powerUp);
        }
    }

    public void RemovePowerUp(Button powerUp)
    {
        if (activePowerUps.Contains(powerUp))
        {
            activePowerUps.Remove(powerUp);
        }

        if (powerUpToSpawnIndex.ContainsKey(powerUp))
        {
            int idx = powerUpToSpawnIndex[powerUp];
            usedSpawnPoints[idx] = false;
            powerUpToSpawnIndex.Remove(powerUp);
        }
    }

    private void DestroyPowerUp(Button powerUp)
    {
        RemovePowerUp(powerUp);
        if (powerUp != null)
        {
            Destroy(powerUp.gameObject);
        }
    }

    private void ActivatePowerUp(Button powerUpButton, Button prefabType)
    {
        if (prefabType == multiplierPrefab)
        {
            ActivateMultiplier();
            PlayAudio(multiplierSound);
        }
        else if (prefabType == freezeTimePrefab)
        {
            FreezeTime();
            PlayAudio(freezeTimeSound);
        }
        else if (prefabType == bombPrefab)
        {
            
        }
    }

    private void ActivateMultiplier()
    {
        TapHandler tapHandler = Object.FindAnyObjectByType<TapHandler>();
        if (tapHandler != null)
        {
            tapHandler.ActivateMultiplier(2, 1.5f);
        }

        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null && gm.IsGameActive() && gm.IsEndlessMode())
        {
            gm.AddTime(1f);
        }
    }

    private void FreezeTime()
    {
        GameManager gm = Object.FindAnyObjectByType<GameManager>();
        if (gm != null)
        {
            gm.FreezeTimer(2f);
        }
    }

    private void PlayAudio(AudioClip clip)
    {
        if (clip != null && AudioManager.Instance != null)
        {
            AudioManager.Instance.PlaySFX(clip);
        }
    }

    public void HideAllPowerUps()
    {
        for (int i = 0; i < activePowerUps.Count; i++)
        {
            Button powerUp = activePowerUps[i];
            if (powerUp != null)
            {
                Destroy(powerUp.gameObject);
            }
        }

        activePowerUps.Clear();
        powerUpToSpawnIndex.Clear();

        for (int i = 0; i < usedSpawnPoints.Length; i++)
        {
            usedSpawnPoints[i] = false;
        }
    }
}
