using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using Zenject;


public class GameManager : MonoBehaviour
{
    public enum GameState
    {
        MainMenu,
        Gameplay,
        GameOver,
        LevelCompleted,
        PrepareLevel
    }

    public static UnityAction<GameState> OnGameStateChange;

    private GameState currentState;

    public GameState CurrentState
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
        CurrentState = GameState.MainMenu;
        Player.PlayerFailed += GameOver;
    }

    private void OnDestroy()
    {
        Player.PlayerFailed -= GameOver;
    }

    public void GameStarted()
    {
        StartCoroutine(GateStartedRoutine());
    }

    private IEnumerator GateStartedRoutine()
    {
        yield return new WaitForEndOfFrame();
        CurrentState = GameState.Gameplay;
    }

    private IEnumerator PrepareLevelRoutine()
    {
        CurrentState = GameState.PrepareLevel;
        yield return new WaitUntil(() => UiManager.IsReady);
        CurrentState = GameState.MainMenu;
    }

    public void PrepareLevel()
    {
        StartCoroutine(PrepareLevelRoutine());
    }

    public void GameOver()
    {
        CurrentState = GameState.GameOver;
    }

    public void LevelCompleted()
    {
        CurrentState = GameState.LevelCompleted;
    }
}