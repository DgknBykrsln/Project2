using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float speedDivisor;

    [SerializeField, Foldout("Setup")] private PlayerAnimationController animationController;

    public enum PlayerState
    {
        Intro,
        Stay,
        MovePath,
        Win,
        Fail
    }

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
    }

    private void IntroExecute()
    {
    }

    private void IntroExit()
    {
    }

    #endregion

    #region Stay

    private void StayEnter()
    {
        animationController.Stay();
    }

    private void StayExecute()
    {
    }

    private void StayExit()
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
        var pathLength = GetPathLength(pathPoints);
        var targetMoveSpeed = pathLength / speedDivisor;

        moveTween?.Kill();
        moveTween = transform.DOPath(pathPoints, targetMoveSpeed).SetSpeedBased().SetEase(Ease.Linear).OnComplete(() =>
        {
            if (stackManager.IsReachedFinish)
            {
                stateMachine.ChangeState(PlayerState.Win);
            }
        });
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

    private void FailEnter()
    {
    }

    private void FailExecute()
    {
    }

    private void FailExit()
    {
    }

    #endregion

    #region Methods

    private void OnGameStateChange(GameManager.GameStates gameState)
    {
        switch (gameState)
        {
            case GameManager.GameStates.MainMenu:
                stateMachine.ChangeState(PlayerState.Intro);
                break;
            case GameManager.GameStates.Gameplay:
                stateMachine.ChangeState(PlayerState.MovePath);
                break;
            case GameManager.GameStates.LevelCompleted:
                stateMachine.ChangeState(PlayerState.Win);
                break;
            case GameManager.GameStates.GameOver:
                stateMachine.ChangeState(PlayerState.Fail);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
        }
    }

    #endregion
}