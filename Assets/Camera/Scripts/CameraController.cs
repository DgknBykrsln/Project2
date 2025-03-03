using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    public enum CameraType
    {
        IntroCamera,
        GameplayCamera,
        WinCamera,
        FailCamera
    }

    [SerializeField, BoxGroup("Settings")] private bool canResetPosition;
    [SerializeField, BoxGroup("Settings")] private CameraType cameraType;

    [SerializeField, Foldout("Setup")] private CinemachineVirtualCamera virtualCamera;

    public CameraType Type => cameraType;

    private Vector3 defaultPosition;

    [Inject]
    private void Construct()
    {
        GameManager.OnGameStateChange += OnGameStateChange;
        defaultPosition = transform.position;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.PrepareLevel && canResetPosition)
        {
            transform.position = defaultPosition;
        }
    }
}