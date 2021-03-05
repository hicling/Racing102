using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkGamePlayer102 : NetworkBehaviour
{
    [SyncVar]
    public string DisplayName = "Loading...";
    [SyncVar]
    public int Position;


    private NetworkManager102 room;
    private NetworkManager102 Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManager102;
        }
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }

    [Server]
    public void SetFinalPosition(int finalPosition)
    {
        Position = finalPosition;
    }
}
