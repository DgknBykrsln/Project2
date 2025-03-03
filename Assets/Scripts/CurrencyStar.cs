using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class CurrencyStar : Currency
{
    [SerializeField, Foldout("Setup")] private ParticleSystem trail;

    protected override void PlayCollectParticle()
    {
        var particleStar = objectPooler.GetObjectFromPool<ParticleStar>();
        particleStar.Play(particlePoint.position);
    }

    protected override IEnumerator OpenRoutine()
    {
        yield return base.OpenRoutine();
        trail.Play();
    }

    public override void OnEnterPool()
    {
        base.OnEnterPool();

        trail.Stop();
    }
}