using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using NaughtyAttributes;
using Zenject;

public class PlayerFollower : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private float rotateSpeed;

    private Player player;

    private Tween rotationTween;
    private Coroutine rotateCoroutine;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
        GameManager.OnGameStateChange += OnGameStateChange;
    }

    private void OnDestroy()
    {
        GameManager.OnGameStateChange -= OnGameStateChange;
    }

    private void Update()
    {
        var position = player.transform.position;
        position.x = 0;
        position.y = 0;
        transform.position = position;
    }

    private void OnGameStateChange(GameManager.GameState gameState)
    {
        if (gameState == GameManager.GameState.MainMenu)
        {
            rotationTween.Kill();
            transform.rotation = Quaternion.identity;

            if (rotateCoroutine != null)
            {
                StopCoroutine(rotateCoroutine);
            }
        }

        if (gameState == GameManager.GameState.LevelCompleted)
        {
            rotateCoroutine = StartCoroutine(RotateRoutine());
        }
    }

    private IEnumerator RotateRoutine()
    {
        yield return new WaitForSeconds(CameraManager.TransitionDuration + .1f);
        rotationTween = transform.DORotate(Vector3.up * 360f, rotateSpeed, RotateMode.FastBeyond360)
            .SetEase(Ease.Linear)
            .SetSpeedBased()
            .SetLoops(-1, LoopType.Incremental);
    }
}