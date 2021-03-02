using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using System;

public class PositionSystem : NetworkBehaviour
{
    public List<CarLapController> CarLapPlayers { get; } = new List<CarLapController>();

    private List<CarLapController> sortedCarLapPlayers = null;

    public int TotalLaps { get; private set; }

    public static event Action<float> OnFinnished;

    private NetworkManager102 room;
    private NetworkManager102 Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManager102;
        }
    }

    void Awake()
    {
        TotalLaps = Room.NumberOfLaps;
    }

    [Server]
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
                Finnished(player.BestLapTime);
            }
        }

        sortedCarLapPlayers = CarLapPlayers.OrderByDescending(o => o.CurrentLap).ThenByDescending(o => o.LastCheckpointPassed).ThenBy(o => o.DistanceToNextCheckpoint).ToList();

        for (int i = 0; i < sortedCarLapPlayers.Count; i++)
        {
            sortedCarLapPlayers[i].Position = i + 1;
        }
    }

    void Finnished(float bestLap)
    {
        OnFinnished?.Invoke(bestLap);
    }
}
