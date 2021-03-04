using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class NetworkGamePlayer102 : NetworkBehaviour
{
    [SyncVar]
    private string displayName = "Loading...";
    [SyncVar]
    private int position;
    public string DisplayName => displayName;
    public int Position => position;

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
        this.displayName = displayName;
    }

    [Server]
    public void SetFinalPosition(int finalPosition)
    {
        position = finalPosition;
    }
}
