using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public abstract class ParticleController : Poolable
{
    [SerializeField, BoxGroup("Settings")] private float returnPoolTime = 1f;

    [SerializeField, Foldout("Setup")] private ParticleSystem particle;

    private ObjectPooler objectPooler;

    private Coroutine returnPoolRoutine;

    [Inject]
    private void Construct(ObjectPooler _objectPooler)
    {
        objectPooler = _objectPooler;
    }

    private IEnumerator ReturnPoolRoutine()
    {
        yield return new WaitForSeconds(returnPoolTime);
        if (gameObject.activeInHierarchy)
        {
            objectPooler.SendObjectToPool(this);
        }
    }

    public void Play(Vector3 position)
    {
        transform.position = position;
        particle.Play();
        if (returnPoolRoutine != null)
        {
            StopCoroutine(returnPoolRoutine);
        }

        returnPoolRoutine = StartCoroutine(ReturnPoolRoutine());
    }
}