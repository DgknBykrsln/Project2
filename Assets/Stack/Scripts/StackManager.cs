using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class StackManager : MonoBehaviour
{
    public enum StackManagerState
    {
        Gameplay,
        Fail,
        Win
    }

    [SerializeField, BoxGroup("Settings")] private int openStartAmount;
    [SerializeField, BoxGroup("Settings")] private float moveSpeed;
    [SerializeField, BoxGroup("Settings")] private float moveDistance;
    [SerializeField, BoxGroup("Settings")] private float stackXLength;
    [SerializeField, BoxGroup("Settings")] private float stackZLength;
    [SerializeField, BoxGroup("Settings")] private float perfectPlacementOffset;

    [SerializeField, ReadOnly] private List<Stack> stacks;

    public static Action StackPlaced;

    public StackManagerState StackState => stackManagerState;

    private Stack currentMovingStack => stacks[currentMovingStackIndex];
    private Stack previousMovingStack => stacks[currentMovingStackIndex - 1];

    private LevelManager levelManager;
    private ObjectPooler objectPooler;
    private StackMaterialHolder stackMaterialHolder;

    private Stack.StackMoveDirection currentStackMoveDirection;

    private readonly List<Vector3> pathPoints = new();

    private GameManager.GameStates currentGameState;

    private int currentMovingStackIndex;
    private float currentXLenght;

    private StackManagerState stackManagerState;

    [Inject]
    private void Construct(LevelManager _levelManager, ObjectPooler _objectPooler, StackMaterialHolder _stackMaterialHolder)
    {
        objectPooler = _objectPooler;
        levelManager = _levelManager;
        stackMaterialHolder = _stackMaterialHolder;
        GameManager.OnGameStateChange += OnGameStateChange;
        currentStackMoveDirection = Stack.StackMoveDirection.Left;
        currentXLenght = stackXLength;
        stackManagerState = StackManagerState.Gameplay;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void Update()
    {
        if (currentGameState != GameManager.GameStates.Gameplay) return;

        if (stackManagerState != StackManagerState.Gameplay) return;

        if (Input.GetMouseButtonDown(0))
        {
            ProcessStackPlacement();
        }
    }

    private void ProcessStackPlacement()
    {
        var previousXLength = previousMovingStack.XLenght;
        var previousXPosition = previousMovingStack.transform.position.x;
        var currentXPosition = currentMovingStack.transform.position.x;

        var overlap = CalculateOverlap(previousXLength, previousXPosition, currentXPosition);
        if (overlap <= 0)
        {
            currentMovingStack.Close();
            SpawnStackDrop(currentMovingStack.transform.position, currentMovingStack.XLenght);
            stackManagerState = StackManagerState.Fail;
            StackPlaced?.Invoke();
            return;
        }

        var cutoffLength = currentMovingStack.XLenght - overlap;

        if (cutoffLength <= perfectPlacementOffset)
        {
            overlap = currentMovingStack.XLenght;
            currentXPosition = previousXPosition;
            cutoffLength = 0;
        }

        currentXLenght = overlap;

        if (cutoffLength > 0)
        {
            HandleStackCutoff(previousXLength, previousXPosition, currentXPosition);
        }

        var targetXPosition = previousXPosition + (currentXPosition - previousXPosition) / 2;
        currentMovingStack.Place(currentXLenght, targetXPosition);

        currentMovingStackIndex++;

        if (currentMovingStackIndex < stacks.Count - 1)
        {
            MoveCurrentStack();
        }
        else
        {
            stackManagerState = StackManagerState.Win;
        }

        StackPlaced?.Invoke();
    }

    private static float CalculateOverlap(float previousXLength, float previousXPosition, float currentXPosition)
    {
        return previousXLength - Mathf.Abs(previousXPosition - currentXPosition);
    }

    private void HandleStackCutoff(float previousXLength, float previousXPosition, float currentXPosition)
    {
        var cutoffLength = currentMovingStack.XLenght - currentXLenght;
        if (cutoffLength <= 0) return;

        var dropPosition = DetermineDropPosition(previousXLength, previousXPosition, currentXPosition, cutoffLength);
        SpawnStackDrop(dropPosition, cutoffLength);
    }

    private Vector3 DetermineDropPosition(float previousXLength, float previousXPosition, float currentXPosition, float cutoffLength)
    {
        var dropXPosition = currentXPosition > previousXPosition
            ? previousXPosition + previousXLength / 2 + cutoffLength / 2
            : previousXPosition - previousXLength / 2 - cutoffLength / 2;

        var movingStackPosition = currentMovingStack.transform.position;
        return new Vector3(dropXPosition, movingStackPosition.y, movingStackPosition.z);
    }

    private void SpawnStackDrop(Vector3 position, float cutoffLength)
    {
        var stackDrop = objectPooler.GetObjectFromPool<StackDrop>();
        stackDrop.Drop(position, new Vector3(cutoffLength, 1, stackZLength), currentMovingStack.Material);
    }

    private void MoveCurrentStack()
    {
        currentMovingStack.Move(currentStackMoveDirection, moveSpeed, currentXLenght);
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

        var targetZ = 0f;

        for (var i = 0; i < targetStackAmount; i++)
        {
            var stack = objectPooler.GetObjectFromPool<Stack>();
            stacks.Add(stack);

            var material = stackMaterialHolder.GetMaterial(i);
            stack.Prepare(transform, targetZ, material, moveDistance, stackZLength, stackXLength);
            targetZ += stackZLength;

            if (i < openStartAmount)
            {
                stack.Place(stackXLength, 0f);
                currentMovingStackIndex++;
            }
            else
            {
                stack.Close();
            }
        }

        var stackFinish = objectPooler.GetObjectFromPool<StackFinish>();
        stacks.Add(stackFinish);
        stackFinish.Prepare(transform, targetZ);
        stackFinish.Place();

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
        pathPoints.Clear();

        for (var i = 0; i < stacks.Count - 1; i++)
        {
            var stack = stacks[i];

            if (stack.State == Stack.StackState.Placed)
            {
                var midPoint = stack.MidPoint;

                if (midPoint.position.z > position.z)
                {
                    pathPoints.Add(midPoint.position);
                }
            }
        }


        if (stackManagerState == StackManagerState.Win)
        {
            var finishStack = stacks[^1];
            pathPoints.Add(finishStack.MidPoint.position);
        }

        if (stackManagerState == StackManagerState.Fail)
        {
        }

        return pathPoints.ToArray();
    }
}