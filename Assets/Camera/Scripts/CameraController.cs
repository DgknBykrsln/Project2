using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public enum CameraType
    {
        IntroCamera,
        GameplayCamera,
        WinCamera,
        FailCamera
    }

    [SerializeField, BoxGroup("Settings")] private CameraType cameraType;

    [SerializeField, Foldout("Setup")] private CinemachineVirtualCamera virtualCamera;

    public CameraType Type => cameraType;
}