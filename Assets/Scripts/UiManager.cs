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

    public static bool IsReady;

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

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        StartCoroutine(OnGameStateChangeRoutine(gameState));
    }

    private IEnumerator OnGameStateChangeRoutine(GameManager.GameState gameState)
    {
        IsReady = false;

        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
                yield return ChangePanelRoutine(UiPanel.PanelType.Fade, true, true);
                yield return ChangePanelRoutine(UiPanel.PanelType.MainMenu, false, true);
                break;
            case GameManager.GameState.Gameplay:
                yield return ChangePanelRoutine(UiPanel.PanelType.Gameplay, false, false);
                break;
            case GameManager.GameState.LevelCompleted:
                yield return ChangePanelRoutine(UiPanel.PanelType.Win, false, false);
                break;
            case GameManager.GameState.GameOver:
                yield return ChangePanelRoutine(UiPanel.PanelType.Fail, false, false);
                break;
            case GameManager.GameState.PrepareLevel:
                yield return ChangePanelRoutine(UiPanel.PanelType.Fade, false, false, true);
                break;
        }

        IsReady = true;
    }

    private IEnumerator ChangePanelRoutine(UiPanel.PanelType panelType, bool isFadeOutInstant, bool isFadeInInstant, bool waitAfterFade = false)
    {
        var targetFadeOutDuration = isFadeOutInstant ? 0 : fadeDuration;
        var targetFadeInDuration = isFadeInInstant ? 0 : fadeDuration;

        if (currentPanel != null)
        {
            yield return currentPanel.FadeOutRoutine(targetFadeOutDuration);
        }

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

        if (waitAfterFade)
        {
            yield return new WaitForSeconds(targetFadeInDuration);
        }
    }
}