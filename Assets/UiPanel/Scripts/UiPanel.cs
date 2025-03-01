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

    public PanelType Type => panelType;

    private Coroutine fadeRoutine;

    [Inject]
    private void Construct()
    {
        canvasGroup.alpha = 0;
        canvasGroup.interactable = false;
    }

    private IEnumerator FadeOutRoutine(float delay)
    {
        canvasGroup.interactable = false;
        yield return canvasGroup.DOFade(0, delay).WaitForCompletion();
    }

    private IEnumerator FadeInRoutine(float delay)
    {
        yield return canvasGroup.DOFade(1, delay).WaitForCompletion();
        canvasGroup.interactable = true;
    }

    private void StopFadeRoutine()
    {
        if (fadeRoutine != null)
        {
            StopCoroutine(fadeRoutine);
        }
    }

    public void FadeIn(float delay)
    {
        StopFadeRoutine();

        fadeRoutine = StartCoroutine(FadeInRoutine(delay));
    }


    public void FadeOut(float delay)
    {
        StopFadeRoutine();

        fadeRoutine = StartCoroutine(FadeOutRoutine(delay));
    }
}