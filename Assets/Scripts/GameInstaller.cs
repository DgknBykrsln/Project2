using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField, Foldout("Setup")] private GameManager gameManager;
    [SerializeField, Foldout("Setup")] private CameraManager cameraManager;
    [SerializeField, Foldout("Setup")] private UiManager uiManager;
    [SerializeField, Foldout("Setup")] private ObjectPooler objectPooler;
    [SerializeField, Foldout("Setup")] private StackMaterialHolder stackMaterialHolder;
    [SerializeField, Foldout("Setup")] private LevelManager levelManager;
    [SerializeField, Foldout("Setup")] private StackManager stackManager;
    [SerializeField, Foldout("Setup")] private Player player;
    [SerializeField, Foldout("Setup")] private CurrencyManager currencyManager;
    [SerializeField, Foldout("Setup")] private SoundManager soundManager;

    public override void InstallBindings()
    {
        BindStateMachine<Player, Player.PlayerState>();
        BindStateMachine<Stack, Stack.StackState>();

        BindFromInstance(soundManager);
        BindFromInstance(currencyManager);
        BindFromInstance(player);
        BindFromInstance(stackManager);
        BindFromInstance(levelManager);
        BindFromInstance(uiManager);
        BindFromInstance(cameraManager);
        BindFromInstance(gameManager);
        BindFromInstance(objectPooler);
        BindFromInstance(stackMaterialHolder);
    }

    private void BindStateMachine<T, TState>() where T : MonoBehaviour where TState : struct, Enum
    {
        Container.Bind<StateMachine<T, TState>>().AsTransient();
    }

    private void BindFromInstance<T>(T instance) where T : class
    {
        Container.Bind<T>().FromInstance(instance).AsSingle().NonLazy();
    }
}