using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class Stack : Poolable
{
    public enum StackState
    {
        Intro,
        Closed,
        Moving,
        Placed,
    }

    public enum StackMoveDirection
    {
        Left,
        Right
    }

    [Range(0, 1)] [SerializeField, BoxGroup("Settings")] private float currencyChance;

    [SerializeField, Foldout("Setup")] private MeshRenderer meshRenderer;
    [SerializeField, Foldout("Setup")] private List<Transform> stackPoints;

    public List<Transform> StackPoints => stackPoints;

    public StackState State => stateMachine.CurrentState;
    public float XLenght => targetScale.x;
    public Material Material => meshRenderer.material;

    private Transform midPoint => stackPoints[^1];

    private CurrencyManager currencyManager;
    private StateMachine<Stack, StackState> stateMachine;

    private StackMoveDirection moveDirection;

    private Currency currency;
    private Vector3 targetScale;

    private float moveDistance;
    private float moveSpeed;

    [Inject]
    private void Construct(StateMachine<Stack, StackState> _stateMachine, CurrencyManager _currencyManager)
    {
        currencyManager = _currencyManager;
        stateMachine = _stateMachine;
        stateMachine.Initialize(this);
        stateMachine.ChangeState(StackState.Intro);
    }

    private void Update()
    {
        stateMachine.Execute();
    }

    #region Intro

    private void IntroEnter()
    {
    }

    private void IntroExecute()
    {
    }

    private void IntroExit()
    {
    }

    #endregion

    #region Closed

    private void ClosedEnter()
    {
        transform.localScale = Vector3.zero;
    }

    private void ClosedExecute()
    {
    }

    private void ClosedExit()
    {
    }

    #endregion

    #region Moving

    private Tween moveTween;
    private Tween scaleTween;

    private void MovingEnter()
    {
        scaleTween?.Kill();
        scaleTween = transform.DOScale(targetScale, 0.5f).SetEase(Ease.OutSine);

        var directionMultiplier = moveDirection == StackMoveDirection.Left ? -1 : 1;

        moveTween?.Kill();
        moveTween = transform.DOLocalMoveX(-directionMultiplier * moveDistance, moveSpeed)
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .SetLoops(-1, LoopType.Yoyo)
            .From(directionMultiplier * moveDistance);
    }


    private void MovingExecute()
    {
    }

    private void MovingExit()
    {
        moveTween?.Kill();
        scaleTween?.Kill();
    }

    #endregion

    #region Placed

    private void PlacedEnter()
    {
        moveTween?.Kill();
    }

    private void PlacedExecute()
    {
    }

    private void PlacedExit()
    {
    }

    #endregion

    #region Methods

    public void Prepare(Transform parent, float zPosition, Material material, float _moveDistance, float targetZScale, float targetXScale)
    {
        moveDistance = _moveDistance;
        targetScale = new Vector3(targetXScale, 1, targetZScale);
        Prepare(parent, zPosition);
        transform.localScale = targetScale;
        meshRenderer.material = material;
    }

    public void Prepare(Transform parent, float zPosition)
    {
        transform.SetParent(parent);
        transform.localPosition = new Vector3(0, 0, zPosition);
    }

    public void Move(StackMoveDirection _moveDirection, float _moveSpeed, float xScale)
    {
        var haveCurrency = Random.value < currencyChance;

        if (haveCurrency)
        {
            currency = currencyManager.GetRandomCurrency();
            currency.Prepare(midPoint);
        }

        targetScale = new Vector3(xScale, 1, targetScale.z);
        moveSpeed = _moveSpeed;
        moveDirection = _moveDirection;
        stateMachine.ChangeState(StackState.Moving);
    }

    public void CloseAndLoseCurrency()
    {
        currency?.SendToPool();
        Close();
    }

    public void Close()
    {
        stateMachine.ChangeState(StackState.Closed);
    }

    public void Place(float xScale, float xPosition)
    {
        transform.localPosition = new Vector3(xPosition, transform.localPosition.y, transform.localPosition.z);
        targetScale = new Vector3(xScale, 1, targetScale.z);
        transform.localScale = targetScale;
        Place();
    }

    public void Place()
    {
        stateMachine.ChangeState(StackState.Placed);
    }

    #endregion
}