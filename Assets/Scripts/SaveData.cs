using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SaveData
{
    public static class Level
    {
        private const string CurrentLevelIndexKey = "CurrentLevelIndex";

        public static int CurrentLevelIndex
        {
            get => PlayerPrefs.GetInt(CurrentLevelIndexKey, 0);
            set => PlayerPrefs.SetInt(CurrentLevelIndexKey, value);
        }
    }

    public static class Currency
    {
        private const string CurrencyKey = "Currency";

        private static string GetCurrencyKey(global::Currency.CurrencyType currencyType)
        {
            return CurrencyKey + "_" + currencyType + "_";
        }

        public static float GetCurrencyAmount(global::Currency.CurrencyType currencyType)
        {
            return PlayerPrefs.GetFloat(GetCurrencyKey(currencyType), 0);
        }

        public static void SetCurrencyAmount(global::Currency.CurrencyType currencyType, float amount)
        {
            PlayerPrefs.SetFloat(GetCurrencyKey(currencyType), amount);
        }
    }
}