using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class PlayerFollower : MonoBehaviour
{
    private Player player;

    [Inject]
    private void Construct(Player player)
    {
        this.player = player;
    }

    private void Update()
    {
        var position = player.transform.position;
        transform.position = position;
    }
}