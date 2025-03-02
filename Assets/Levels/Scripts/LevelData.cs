using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "Game/LevelData")]
public class LevelData : ScriptableObject
{
    [SerializeField, BoxGroup("Settings")] private int stackAmount;

    public int StackAmount => stackAmount;
}