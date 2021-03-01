using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PositionSystem : NetworkBehaviour
{
    public List<CarLapController> CarLapPlayers { get; } = new List<CarLapController>();

    private void Update()
    {
        Debug.Log(CarLapPlayers.Count);
        foreach (var player in CarLapPlayers)
        {
            Debug.Log(player.BestLapTime);
        }
    }
}
