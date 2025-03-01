using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using NaughtyAttributes;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private CameraManager.CameraType cameraType;

    [SerializeField, Foldout("Setup")] private CinemachineVirtualCamera virtualCamera;

    public CameraManager.CameraType CameraType => cameraType;
}