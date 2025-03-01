using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerAnimationController : MonoBehaviour
{
    [SerializeField, Foldout("Setup")] private Animator animator;

    private static readonly int stayKey = Animator.StringToHash("Stay");
    private static readonly int danceKey = Animator.StringToHash("Dance");
    private static readonly int runKey = Animator.StringToHash("Run");

    public void Stay()
    {
        animator.SetTrigger(stayKey);
    }

    public void Run()
    {
        animator.SetTrigger(runKey);
    }

    public void Dance()
    {
        animator.SetTrigger(danceKey);
    }
}