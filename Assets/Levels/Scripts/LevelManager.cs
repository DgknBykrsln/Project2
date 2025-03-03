using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Zenject;

public class LevelManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private int repeatIndex;

    [SerializeField] private List<LevelData> levelDatas;

    public LevelData CurrentLevelData => currentLevelData;

    private int currentLevelIndex => SaveData.Level.CurrentLevelIndex;

    private LevelData currentLevelData => levelDatas[GetSelectedLevelIndex()];


    [Inject]
    private void Construct()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private static void OnGameStateChange(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.LevelCompleted)
        {
            IncreaseLevel();
        }
    }

    private int GetSelectedLevelIndex()
    {
        var selectedLevelIndex = 0;

        if (currentLevelIndex < levelDatas.Count)
        {
            selectedLevelIndex = currentLevelIndex % levelDatas.Count;
        }
        else
        {
            var repeatRange = levelDatas.Count - repeatIndex;

            selectedLevelIndex = ((currentLevelIndex - levelDatas.Count) % repeatRange) + repeatIndex;
        }

        return selectedLevelIndex;
    }

    private static void IncreaseLevel()
    {
        SaveData.Level.CurrentLevelIndex++;
    }
}