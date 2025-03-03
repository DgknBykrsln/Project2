using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;
using Zenject;

public class UiPanel : MonoBehaviour
{
    public enum PanelType
    {
        Fade,
        MainMenu,
        Gameplay,
        Win,
        Fail
    }

    [SerializeField, BoxGroup("Settings")] private PanelType panelType;

    [SerializeField, Foldout("Setup")] private CanvasGroup canvasGroup;

    [SerializeField] private List<UiElement> uiElements;

    public PanelType Type => panelType;


    [Inject]
    private void Construct()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    public IEnumerator FadeOutRoutine(float duration)
    {
        canvasGroup.interactable = false;
        canvasGroup.alpha = 1;
        yield return canvasGroup.DOFade(0, duration).WaitForCompletion();

        foreach (var uiElement in uiElements)
        {
            uiElement.Disappear();
        }
    }

    public IEnumerator FadeInRoutine(float duration)
    {
        foreach (var uiElement in uiElements)
        {
            uiElement.Appear();
        }

        canvasGroup.alpha = 0;

        yield return canvasGroup.DOFade(1, duration).WaitForCompletion();
        canvasGroup.interactable = true;
    }
}