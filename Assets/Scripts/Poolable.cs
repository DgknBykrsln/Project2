using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Poolable : MonoBehaviour
{
    public virtual void OnEnterPool()
    {
        gameObject.SetActive(false);
    }

    public virtual void OnExitPool()
    {
        gameObject.SetActive(true);
    }

    public virtual void ResetValues()
    {
    }
}