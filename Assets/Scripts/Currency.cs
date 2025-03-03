using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public abstract class Currency : Poolable
{
    public enum CurrencyType
    {
        Coin,
        Diamond,
        Star
    }

    [SerializeField, BoxGroup("Settings")] private CurrencyType currencyType;
    [SerializeField, BoxGroup("Settings")] private float value;

    [SerializeField, Foldout("Setup")] private Collider coll;
    [SerializeField, Foldout("Setup")] private Transform yRoot;
    [SerializeField, Foldout("Setup")] protected Transform particlePoint;

    public CurrencyType Type => currencyType;

    protected ObjectPooler objectPooler;

    private Tween moveTween;

    private Transform target;

    private Coroutine openRoutine;

    protected abstract void PlayCollectParticle();

    [Inject]
    private void Construct(ObjectPooler _objectPooler)
    {
        objectPooler = _objectPooler;
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.PrepareLevel)
        {
            SendToPool();
        }
    }

    private void Update()
    {
        transform.position = target.position;
    }

    protected virtual IEnumerator OpenRoutine()
    {
        coll.enabled = true;
        yield return transform.DOScale(1f, 0.5f).SetEase(Ease.OutBack).WaitForCompletion();

        moveTween = yRoot.DOMoveY(1f, 1f).SetLoops(-1, LoopType.Yoyo).SetEase(Ease.Linear);
    }

    public void Prepare(Transform _target)
    {
        moveTween?.Kill();
        target = _target;
        coll.enabled = false;
        transform.localScale = Vector3.zero;
        yRoot.localPosition = Vector3.zero;
        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
        }

        openRoutine = StartCoroutine(OpenRoutine());
    }

    public void SendToPool()
    {
        if (!gameObject.activeInHierarchy) return;

        if (openRoutine != null)
        {
            StopCoroutine(openRoutine);
        }

        moveTween.Kill();
        yRoot.localPosition = Vector3.zero;
        objectPooler.SendObjectToPool(this);
    }

    public void Collect()
    {
        var currencyAmount = SaveData.Currency.GetCurrencyAmount(currencyType);
        currencyAmount += value;
        SaveData.Currency.SetCurrencyAmount(currencyType, currencyAmount);
        PlayCollectParticle();
        SendToPool();
    }
}