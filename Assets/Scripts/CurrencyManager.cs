using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class CurrencyManager : MonoBehaviour
{
    private ObjectPooler objectPooler;

    [Inject]
    private void Construct(ObjectPooler _objectPooler)
    {
        objectPooler = _objectPooler;
    }

    public Currency GetRandomCurrency()
    {
        var selectedType = (Currency.CurrencyType)Random.Range(0, System.Enum.GetValues(typeof(Currency.CurrencyType)).Length);

        return selectedType switch
        {
            Currency.CurrencyType.Coin => objectPooler.GetObjectFromPool<CurrencyCoin>(),
            Currency.CurrencyType.Diamond => objectPooler.GetObjectFromPool<CurrencyDiamond>(),
            Currency.CurrencyType.Star => objectPooler.GetObjectFromPool<CurrencyStar>(),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}