using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class RoundSystem : NetworkBehaviour
{
    [SerializeField] private Animator animator = null;

    private int finnishPosition;
    private List<NetworkGamePlayer102> playingPlayers;

    private NetworkManager102 room;
    private NetworkManager102 Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManager102;
        }
    }

    public void CountdownEnded()
    {
        animator.enabled = false;
    }

    #region Server

    public override void OnStartServer()
    {
        NetworkManager102.OnServerStopped += CleanUpServer;
        NetworkManager102.OnServerReadied += CheckToStartRound;
        PlayerMp.OnFinnished += HandlePlayerFinnish;
    }

    [ServerCallback]
    private void OnDestroy() => CleanUpServer();

    [Server]
    private void CleanUpServer()
    {
        NetworkManager102.OnServerStopped -= CleanUpServer;
        NetworkManager102.OnServerReadied -= CheckToStartRound;
        PlayerMp.OnFinnished -= HandlePlayerFinnish;
    }

    [ServerCallback]
    public void StartRound()
    {
        RpcStartRound();
    }

    [Server]
    private void CheckToStartRound(NetworkConnection conn)
    {
        if(Room.GamePlayers.Count(x => x.connectionToClient.isReady) != Room.GamePlayers.Count) { return; }

        animator.enabled = true;

        playingPlayers = Room.GamePlayers;

        finnishPosition = 1;

        RpcStartCountdown();
    }

    [Server]
    private void HandlePlayerFinnish(PlayerMp player, bool finnished)
    {
        Debug.Log(player.OwnderId + " - " + finnishPosition + " - " + finnished);

        foreach (var gamePlayer in playingPlayers)
        {
            if (gamePlayer.connectionToClient == player.connectionToClient)
            {
                gamePlayer.SetFinalPosition(finnishPosition);
            }
        }

        finnishPosition++;

        if (finnishPosition - 1 == playingPlayers.Count)
        {
            Debug.Log("alla har gått imål!");
            EndRound();
        }
    }

    [ServerCallback]
    private void EndRound()
    {
        
    }

    #endregion

    #region Client

    [ClientRpc]
    private void RpcStartCountdown()
    {
        animator.enabled = true;
    }

    [ClientRpc]
    private void RpcStartRound()
    {
        InputManager.Remove(ActionMapNames.Player);
    }

    #endregion
}
