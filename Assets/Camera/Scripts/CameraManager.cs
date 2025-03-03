using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class CameraManager : MonoBehaviour
{
    [SerializeField, Foldout("Setup")] private Camera mainCamera;

    [SerializeField] private List<CameraController> cameras;

    public static float TransitionDuration => 1f;

    [Inject]
    private void Construct()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        switch (gameState)
        {
            case GameManager.GameState.MainMenu:
                ChangeCamera(CameraController.CameraType.IntroCamera);
                break;
            case GameManager.GameState.Gameplay:
                ChangeCamera(CameraController.CameraType.GameplayCamera);
                break;
            case GameManager.GameState.LevelCompleted:
                ChangeCamera(CameraController.CameraType.WinCamera);
                break;
            case GameManager.GameState.GameOver:
                ChangeCamera(CameraController.CameraType.FailCamera);
                break;
        }
    }

    private void ChangeCamera(CameraController.CameraType cameraType)
    {
        foreach (var cam in cameras)
        {
            cam.gameObject.SetActive(cam.Type == cameraType);
        }
    }
}