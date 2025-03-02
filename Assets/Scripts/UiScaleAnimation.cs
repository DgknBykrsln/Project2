using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using TMPro;
using UnityEngine;

public class UiScaleAnimation : UiElement
{
    [SerializeField, BoxGroup("Settings")] private float targetScale;
    [SerializeField, BoxGroup("Settings")] private float duration;

    private Tween scaleTween;

    public override void Appear()
    {
        base.Appear();

        scaleTween?.Kill();
        transform.localScale = Vector3.one;

        scaleTween = transform.DOScale(targetScale, duration).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.InOutSine);
    }

    public override void Disappear()
    {
        base.Disappear();

        scaleTween?.Kill();
    }
}