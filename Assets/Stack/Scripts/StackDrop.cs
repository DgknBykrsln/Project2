using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using Zenject;

public class StackDrop : Poolable
{
    [SerializeField, BoxGroup("Settings")] private float returnPoolDelay = 2f;

    [SerializeField, Foldout("Setup")] private Rigidbody rb;
    [SerializeField, Foldout("Setup")] private MeshRenderer meshRenderer;

    private ObjectPooler objectPooler;

    private Coroutine dropRoutine;

    [Inject]
    private void Construct(ObjectPooler _objectPooler)
    {
        objectPooler = _objectPooler;
    }

    private IEnumerator DropRoutine(Vector3 targetPosition, Vector3 targetScale, Material material)
    {
        transform.position = targetPosition;
        transform.localScale = targetScale;
        meshRenderer.material = material;
        rb.isKinematic = false;
        rb.useGravity = true;

        yield return new WaitForSeconds(returnPoolDelay);

        rb.isKinematic = true;
        rb.useGravity = false;
        objectPooler.SendObjectToPool(this);
    }

    public void Drop(Vector3 targetPosition, Vector3 targetScale, Material material)
    {
        if (dropRoutine != null)
        {
            StopCoroutine(dropRoutine);
        }

        dropRoutine = StartCoroutine(DropRoutine(targetPosition, targetScale, material));
    }
}