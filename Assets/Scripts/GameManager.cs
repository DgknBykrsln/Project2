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

    public static UnityAction OnGameStarted, OnGameSceneLoad;
    public static UnityAction OnGameOver, OnLevelComplete;
    public static UnityAction<GameStates> OnGameStateChange;

    private GameStates currentState = GameStates.MainMenu;

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
        OnGameSceneLoad?.Invoke();
    }

    public void GameStarted()
    {
        OnGameStarted?.Invoke();
        CurrentState = GameStates.Gameplay;

        //CameraManager.ChangeCamera(CameraType.Gameplay);
    }

    public void GameOver()
    {
        OnGameOver?.Invoke();
        CurrentState = GameStates.GameOver;
    }

    public void LevelCompleted()
    {
        OnLevelComplete?.Invoke();
        CurrentState = GameStates.LevelCompleted;
    }
}