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

    [Inject]
    private void Construct()
    {
        prevPos = transform.position;
        targetRotation = transform.rotation;
    }

    private void Update()
    {
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