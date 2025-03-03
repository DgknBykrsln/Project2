using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField, BoxGroup("Settings")] private Vector2 runSpeedRange;

    [SerializeField, Foldout("Setup")] private Animator animator;

    private static readonly int stayKey = Animator.StringToHash("Stay");
    private static readonly int danceKey = Animator.StringToHash("Dance");
    private static readonly int runKey = Animator.StringToHash("Run");
    private static readonly int runSpeedKey = Animator.StringToHash("RunSpeed");

    public void Stay()
    {
        animator.SetTrigger(stayKey);
    }

    public void Run()
    {
        animator.SetTrigger(runKey);
    }

    public void SetRunSpeed(float speedRatio)
    {
        var speed = Mathf.Lerp(runSpeedRange.x, runSpeedRange.y, speedRatio);
        animator.SetFloat(runSpeedKey, speed);
    }

    public void Dance()
    {
        animator.SetTrigger(danceKey);
    }
}