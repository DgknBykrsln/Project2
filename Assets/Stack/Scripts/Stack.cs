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
        Falling
    }

    public enum StackMoveDirection
    {
        Left,
        Right
    }

    [SerializeField, Foldout("Setup")] private MeshRenderer meshRenderer;
    [SerializeField, Foldout("Setup")] private Transform midPoint;

    public Transform MidPoint => midPoint;

    public StackState State => stateMachine.CurrentState;

    private StateMachine<Stack, StackState> stateMachine;

    private StackMoveDirection moveDirection;

    private Vector3 targetScale;

    private float moveDistance;
    private float moveSpeed;

    [Inject]
    private void Construct(StateMachine<Stack, StackState> _stateMachine)
    {
        transform.localScale = Vector3.zero;
        stateMachine = _stateMachine;
        stateMachine.Initialize(this);
        stateMachine.ChangeState(StackState.Intro);
    }

    public void Prepare(Transform parent, float zPosition, Material material, float _targetScale, float _moveDistance)
    {
        moveDistance = _moveDistance;
        targetScale = new Vector3(_targetScale, 1, _targetScale);
        transform.SetParent(parent);
        transform.localPosition = new Vector3(0, 0, zPosition);
        transform.localScale = targetScale;
        meshRenderer.material = material;
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

    #region Falling

    private void FallingEnter()
    {
    }

    private void FallingExecute()
    {
    }

    private void FallingExit()
    {
    }

    #endregion

    #region Methods

    public void Move(StackMoveDirection _moveDirection, float _moveSpeed)
    {
        moveSpeed = _moveSpeed;
        moveDirection = _moveDirection;
        stateMachine.ChangeState(StackState.Moving);
    }

    public void Close()
    {
        stateMachine.ChangeState(StackState.Closed);
    }

    public void Place()
    {
        stateMachine.ChangeState(StackState.Placed);
    }

    #endregion
}