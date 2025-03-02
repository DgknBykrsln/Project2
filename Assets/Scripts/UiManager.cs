using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class UiManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float fadeDuration = 1f;

    [SerializeField] private List<UiPanel> uiPanels;

    private UiPanel currentPanel;

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
        StartCoroutine(OnGameStateChangeRoutine(gameState));
    }

    private IEnumerator OnGameStateChangeRoutine(GameManager.GameStates gameState)
    {
        switch (gameState)
        {
            case GameManager.GameStates.MainMenu:
                yield return ChangePanelRoutine(UiPanel.PanelType.Fade, true, true);
                yield return ChangePanelRoutine(UiPanel.PanelType.MainMenu, false, false);
                break;
            case GameManager.GameStates.Gameplay:
                yield return ChangePanelRoutine(UiPanel.PanelType.Gameplay, false, false);
                break;
            case GameManager.GameStates.LevelCompleted:
                yield return ChangePanelRoutine(UiPanel.PanelType.Win, false, false);
                break;
            case GameManager.GameStates.GameOver:
                yield return ChangePanelRoutine(UiPanel.PanelType.Fail, false, false);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    private IEnumerator ChangePanelRoutine(UiPanel.PanelType panelType, bool isFadeOutInstant, bool isFadeInInstant)
    {
        var targetFadeOutDuration = isFadeOutInstant ? 0 : fadeDuration;
        var targetFadeInDuration = isFadeInInstant ? 0 : fadeDuration;

        if (currentPanel != null)
        {
            currentPanel.FadeOutRoutine(targetFadeOutDuration);
        }

        yield return new WaitForSeconds(targetFadeOutDuration / 2f);

        foreach (var panel in uiPanels)
        {
            if (panel.Type == panelType)
            {
                panel.gameObject.SetActive(true);
                currentPanel = panel;
            }
            else
            {
                panel.gameObject.SetActive(false);
            }
        }

        yield return currentPanel.FadeInRoutine(targetFadeInDuration);
    }
}