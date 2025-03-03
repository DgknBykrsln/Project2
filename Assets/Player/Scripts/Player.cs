using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float initialZPos;
    [SerializeField, BoxGroup("Settings")] private float speedDivisor;
    [SerializeField, BoxGroup("Settings")] private float finishSpeed;
    [SerializeField, BoxGroup("Settings")] private AnimationCurve jumpCurve;
    [SerializeField, BoxGroup("Settings")] private Vector2 speedRange;

    [SerializeField, Foldout("Setup")] private Transform visual;
    [SerializeField, Foldout("Setup")] private Transform jumpRoot;
    [SerializeField, Foldout("Setup")] private PlayerAnimationController animationController;

    public enum PlayerState
    {
        Intro,
        MovePath,
        Win,
        Fail
    }

    public static Action PlayerFailed;

    private StateMachine<Player, PlayerState> stateMachine;
    private StackManager stackManager;

    [Inject]
    private void Construct(StateMachine<Player, PlayerState> _stateMachine, StackManager _stackManager)
    {
        stackManager = _stackManager;
        stateMachine = _stateMachine;
        stateMachine.Initialize(this);
        stateMachine.ChangeState(PlayerState.Intro);
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void Update()
    {
        stateMachine.Execute();
    }

    #region Intro

    private void IntroEnter()
    {
        visual.gameObject.SetActive(true);
        jumpRoot.localPosition = Vector3.zero;
        animationController.Stay();
        transform.position = Vector3.forward * initialZPos;
        transform.rotation = Quaternion.identity;
    }

    private void IntroExecute()
    {
    }

    private void IntroExit()
    {
    }

    #endregion

    #region MovePath

    private Tween moveTween;

    private void MovePathEnter()
    {
        animationController.Run();

        StackManager.StackPlaced += OnStackPlaced;
        OnStackPlaced();
    }

    private void MovePathExecute()
    {
    }

    private void MovePathExit()
    {
        moveTween?.Kill();
        StackManager.StackPlaced -= OnStackPlaced;
    }

    private static float GetPathLength(Vector3[] pathPoints)
    {
        var totalLength = 0f;

        for (var i = 1; i < pathPoints.Length; i++)
        {
            totalLength += Vector3.Distance(pathPoints[i - 1], pathPoints[i]);
        }

        return totalLength;
    }


    private void OnStackPlaced()
    {
        var pathPoints = stackManager.GetMovePath(transform.position);
        var targetMoveSpeed = CalculateTargetSpeed(pathPoints);

        var speedRatio = Mathf.InverseLerp(speedRange.x, speedRange.y, targetMoveSpeed);
        animationController.SetRunSpeed(speedRatio);

        moveTween?.Kill();
        moveTween = transform
            .DOPath(pathPoints, targetMoveSpeed)
            .SetSpeedBased()
            .SetEase(Ease.Linear)
            .OnComplete(() => HandleStateChange(stackManager.StackState));
    }

    private float CalculateTargetSpeed(Vector3[] pathPoints)
    {
        var pathLength = GetPathLength(pathPoints);
        var speed = pathLength / speedDivisor;

        var isWin = stackManager.StackState == StackManager.StackManagerState.Win;

        speed = isWin ? finishSpeed : Mathf.Clamp(speed, speedRange.x, speedRange.y);

        return speed;
    }

    private void HandleStateChange(StackManager.StackManagerState stackState)
    {
        switch (stackState)
        {
            case StackManager.StackManagerState.Win:
                stateMachine.ChangeState(PlayerState.Win);
                break;
            case StackManager.StackManagerState.Fail:
                stateMachine.ChangeState(PlayerState.Fail);
                break;
            case StackManager.StackManagerState.Gameplay:
                stateMachine.ChangeState(PlayerState.Fail);
                stackManager.Fail();
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(stackState), stackState, null);
        }
    }

    #endregion

    #region Win

    private void WinEnter()
    {
        animationController.Dance();
    }

    private void WinExecute()
    {
    }

    private void WinExit()
    {
    }

    #endregion

    #region Fail

    private Tween jumpTween;
    private Tween failMoveTween;
    private Coroutine failCoroutine;

    private void FailEnter()
    {
        if (failCoroutine != null)
        {
            StopCoroutine(failCoroutine);
        }

        failCoroutine = StartCoroutine(FailRoutine());
    }

    private void FailExecute()
    {
    }

    private void FailExit()
    {
    }

    private IEnumerator FailRoutine()
    {
        var zPos = transform.position.z;
        animationController.SetRunSpeed(0f);
        failMoveTween = transform.DOMoveZ(zPos + stackManager.StackZLength * .4f, speedRange.x).SetSpeedBased();
        yield return failMoveTween.WaitForCompletion();

        animationController.Jump();
        yield return new WaitForSeconds(.25f);
        zPos = transform.position.z;
        failMoveTween = transform.DOMoveZ(zPos + stackManager.StackZLength * .25f, speedRange.x).SetSpeedBased();

        const float fallSpeed = 7f;
        yield return jumpRoot.DOMoveY(-10, fallSpeed).SetEase(jumpCurve).SetSpeedBased().WaitForCompletion();
        PlayerFailed?.Invoke();
        failMoveTween?.Kill();
        visual.gameObject.SetActive(false);
    }

    #endregion

    #region Methods

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
                stateMachine.ChangeState(PlayerState.Intro);
                break;
            case GameManager.GameState.Gameplay:
                stateMachine.ChangeState(PlayerState.MovePath);
                break;
            case GameManager.GameState.LevelCompleted:
                stateMachine.ChangeState(PlayerState.Win);
                break;
        }
    }

    #endregion
}