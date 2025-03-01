using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Zenject;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] private List<Poolable> poolables;

    private readonly Dictionary<string, List<Poolable>> poolDictionary = new();

    private DiContainer container;

    [Inject]
    private void Construct(DiContainer _container)
    {
        container = _container;
        CreatePools();
    }

    private void CreatePools()
    {
        foreach (var poolable in poolables)
        {
            var pool = new List<Poolable>();

            poolDictionary.Add(poolable.GetType().FullName, pool);
        }
    }

    public void SendObjectToPool(Poolable poolable)
    {
        poolDictionary[poolable.GetType().FullName].Add(poolable);
        poolable.OnEnterPool();
        poolable.transform.SetParent(transform);
    }

    public T GetObjectFromPool<T>() where T : Poolable
    {
        var tag = typeof(T).FullName;
        var pool = poolDictionary[tag];

        if (!pool.Any())
        {
            foreach (var poolable in poolables)
            {
                if (tag == poolable.GetType().FullName)
                {
                    var newPoolObject = container.InstantiatePrefabForComponent<T>(poolable);

                    newPoolObject.OnExitPool();

                    return newPoolObject;
                }
            }
        }

        var poolObject = pool.First() as T;

        pool.RemoveAt(0);

        poolObject.OnExitPool();

        return poolObject;
    }
}
