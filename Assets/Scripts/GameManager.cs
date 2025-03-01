using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;


public class GameManager : MonoBehaviour
{
    public enum GameStates
    {
        MainMenu,
        Gameplay,
        GameOver,
        LevelCompleted
    }

    public static UnityAction<GameStates> OnGameStateChange;

    private GameStates currentState;

    public GameStates CurrentState
    {
        get => currentState;
        set
        {
            currentState = value;
            OnGameStateChange?.Invoke(value);
        }
    }

    [Inject]
    private void Construct()
    {
        CurrentState = GameStates.MainMenu;
    }

    public void GameStarted()
    {
        CurrentState = GameStates.Gameplay;
    }

    public void GameOver()
    {
        CurrentState = GameStates.GameOver;
    }

    public void LevelCompleted()
    {
        CurrentState = GameStates.LevelCompleted;
    }
}