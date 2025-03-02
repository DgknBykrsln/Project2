using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class StackManager : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float moveSpeed;
    [SerializeField, BoxGroup("Settings")] private float moveDistance;
    [SerializeField, BoxGroup("Settings")] private float stackLength;
    [SerializeField, BoxGroup("Settings")] private int openStartAmount;

    [SerializeField, ReadOnly] private List<Stack> stacks;

    public static Action StackPlaced;

    private Stack currentMovingStack => stacks[currentMovingStackIndex];

    private LevelManager levelManager;
    private ObjectPooler objectPooler;
    private StackMaterialHolder stackMaterialHolder;

    private int currentMovingStackIndex;

    private Stack.StackMoveDirection currentStackMoveDirection;

    private Vector3[] pathPoints = new Vector3[100];

    private GameManager.GameStates currentGameState;

    [Inject]
    private void Construct(LevelManager _levelManager, ObjectPooler _objectPooler, StackMaterialHolder _stackMaterialHolder)
    {
        objectPooler = _objectPooler;
        levelManager = _levelManager;
        stackMaterialHolder = _stackMaterialHolder;
        GameManager.OnGameStateChange += OnGameStateChange;
        currentStackMoveDirection = Stack.StackMoveDirection.Left;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void Update()
    {
        if (currentGameState != GameManager.GameStates.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            currentMovingStack.Place();
            currentMovingStackIndex++;
            MoveCurrentStack();
            StackPlaced?.Invoke();
        }
    }

    private void MoveCurrentStack()
    {
        currentMovingStack.Move(currentStackMoveDirection, moveSpeed);
        SwitchMoveDirection();
    }

    private void OnGameStateChange(GameManager.GameStates gameState)
    {
        currentGameState = gameState;

        if (gameState == GameManager.GameStates.MainMenu)
        {
            CreateStacks();
        }
    }

    private void CreateStacks()
    {
        var currentLevelData = levelManager.CurrentLevelData;
        var targetStackAmount = currentLevelData.StackAmount;

        for (var i = 0; i < targetStackAmount; i++)
        {
            var stack = objectPooler.GetObjectFromPool<Stack>();
            stacks.Add(stack);

            var targetZ = i * stackLength;
            var material = stackMaterialHolder.GetMaterial(i);
            stack.Prepare(transform, targetZ, material, stackLength, moveDistance);

            if (i < openStartAmount)
            {
                stack.Place();
                currentMovingStackIndex++;
            }
            else
            {
                stack.Close();
            }
        }

        MoveCurrentStack();
    }

    private void SwitchMoveDirection()
    {
        currentStackMoveDirection = currentStackMoveDirection == Stack.StackMoveDirection.Right
            ? Stack.StackMoveDirection.Left
            : Stack.StackMoveDirection.Right;
    }

    public Vector3[] GetMovePath(Vector3 position)
    {
        var index = 0;
        Array.Clear(pathPoints, 0, pathPoints.Length);

        foreach (var stack in stacks)
        {
            if (stack.State == Stack.StackState.Placed)
            {
                var midPoint = stack.MidPoint;

                if (midPoint.position.z > position.z)
                {
                    if (index < pathPoints.Length)
                    {
                        pathPoints[index] = midPoint.position;
                        index++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        return pathPoints;
    }
}