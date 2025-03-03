using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyCoin : Currency
{
    protected override void PlayCollectParticle()
    {
        var coinParticle = objectPooler.GetObjectFromPool<ParticleCoin>();
        coinParticle.Play(particlePoint.position);
    }
}