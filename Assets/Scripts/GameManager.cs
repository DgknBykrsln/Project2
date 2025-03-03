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
        CurrentState = GameStates.Gameplay;
    }

    public void MainMenu()
    {
        CurrentState = GameStates.MainMenu;
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