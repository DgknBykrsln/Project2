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
}