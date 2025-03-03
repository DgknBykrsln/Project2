using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyDiamond : Currency
{
    protected override void PlayCollectParticle()
    {
        var particleDiamond = objectPooler.GetObjectFromPool<ParticleDiamond>();
        particleDiamond.Play(particlePoint.position);
    }
}