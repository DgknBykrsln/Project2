using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class UiManager : MonoBehaviour
{
    [SerializeField] private List<UiPanel> uiPanels;

    [Inject]
    private void Construct()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameStates gameState)
    {
        switch (gameState)
        {
            case GameManager.GameStates.MainMenu:
                ChangePanel(UiPanel.PanelType.MainMenu);
                break;
            case GameManager.GameStates.Gameplay:
                ChangePanel(UiPanel.PanelType.Gameplay);
                break;
            case GameManager.GameStates.LevelCompleted:
                ChangePanel(UiPanel.PanelType.Win);
                break;
            case GameManager.GameStates.GameOver:
                ChangePanel(UiPanel.PanelType.Fail);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    private void ChangePanel(UiPanel.PanelType panelType)
    {
        foreach (var panel in uiPanels)
        {
            panel.gameObject.SetActive(panel.Type == panelType);
        }
    }
}