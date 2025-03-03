using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using Zenject;

public class UiLevelText : UiElement
{
    [SerializeField, Foldout("Setup")] private TextMeshProUGUI levelText;

    public override void Appear()
    {
        base.Appear();

        levelText.text = $"Level {SaveData.Level.CurrentLevelIndex + 1}";
    }
}