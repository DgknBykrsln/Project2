using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [SerializeField, Foldout("Setup")] private PlayerAnimationController animationController;

    public enum PlayerStates
    {
        Intro,
        Stay,
        MovePath,
        Win,
        Fail
    }

    private StateMachine<Player, PlayerStates> stateMachine;

    [Inject]
    private void Construct(StateMachine<Player, PlayerStates> _stateMachine)
    {
        stateMachine = _stateMachine;
        stateMachine.Initialize(this);
        stateMachine.ChangeState(PlayerStates.Intro);
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

    private void MovePathEnter()
    {
        animationController.Run();
    }

    private void MovePathExecute()
    {
    }

    private void MovePathExit()
    {
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
}