using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class PositionSystem : MonoBehaviour
{
    public List<CarLapController> CarLapPlayers { get; } = new List<CarLapController>();

    private List<CarLapController> sortedCarLapPlayers = null;
    //syncvar
    private int totalLaps;
    public int TotalLaps => totalLaps;

    private void Update()
    {
        foreach (var player in CarLapPlayers)
        {
            player.numberOfPlayers = CarLapPlayers.Count;
        }

        foreach (var player in CarLapPlayers)
        {
            if (player.Finnished)
            {
                Finnished(player);
            }
        }

        sortedCarLapPlayers = CarLapPlayers.OrderByDescending(o => o.CurrentLap).ThenByDescending(o => o.LastCheckpointPassed).ThenBy(o => o.DistanceToNextCheckpoint).ToList();

        for (int i = 0; i < sortedCarLapPlayers.Count; i++)
        {
            sortedCarLapPlayers[i].Position = i + 1;
        }
    }
    //server
    void Finnished(CarLapController player)
    {
        var playerMp = player.GetComponentInParent<PlayerMp>();
        playerMp.SetFinnish(true);
    }

    //server
    public void SetNumberOfLaps(int numberOfLaps)
    {
        totalLaps = numberOfLaps;
    }
}
