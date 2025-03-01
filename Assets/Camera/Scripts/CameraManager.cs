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

    [Inject]
    private void Construct()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameStates gameState)
    {
        switch (gameState)
        {
            case GameManager.GameStates.MainMenu:
                ChangeCamera(CameraController.CameraType.IntroCamera);
                break;
            case GameManager.GameStates.Gameplay:
                ChangeCamera(CameraController.CameraType.GameplayCamera);
                break;
            case GameManager.GameStates.LevelCompleted:
                ChangeCamera(CameraController.CameraType.WinCamera);
                break;
            case GameManager.GameStates.GameOver:
                ChangeCamera(CameraController.CameraType.FailCamera);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(gameState), gameState, null);
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