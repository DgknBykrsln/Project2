using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField, Foldout("Setup")] private GameManager gameManager;
    [SerializeField, Foldout("Setup")] private CameraManager cameraManager;
    [SerializeField, Foldout("Setup")] private ObjectPooler objectPooler;

    public override void InstallBindings()
    {
        BindFromInstance(cameraManager);
        BindFromInstance(gameManager);
        BindFromInstance(objectPooler);
    }

    private void BindFromInstance<T>(T instance) where T : class
    {
        Container.Bind<T>().FromInstance(instance).AsSingle().NonLazy();
    }
}