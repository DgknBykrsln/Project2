using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AutoRotate : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float moveThreshold = 0.1f;
    [SerializeField, BoxGroup("Settings")] private float easeSpeed = 1f;

    private Quaternion targetRotation;

    private Vector3 prevPos;

    private GameManager gameManager;

    [Inject]
    private void Construct(GameManager _gameManager)
    {
        gameManager = _gameManager;
        prevPos = transform.position;
        targetRotation = transform.rotation;
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.Gameplay)
        {
            prevPos = transform.position;
        }
    }

    private void Update()
    {
        if (gameManager.CurrentState != GameManager.GameState.Gameplay) return;

        var newPos = transform.position;
        var displacement = newPos - prevPos;
        var sqrDist = displacement.sqrMagnitude;
        if (sqrDist >= moveThreshold * moveThreshold)
        {
            var dir = displacement.normalized;

            targetRotation = Quaternion.LookRotation(dir, Vector3.up);
            prevPos = newPos;
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * easeSpeed);
    }
}