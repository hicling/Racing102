using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;
using TMPro;

public class RoundSystem : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private Animator animator = null;

    private int finnishPosition;

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

        finnishPosition = 1;

        RpcStartCountdown();
    }

    [Server]
    private void HandlePlayerFinnish(PlayerMp player, bool finnished)
    {
        foreach (var gamePlayer in Room.GamePlayers)
        {
            if (gamePlayer.connectionToClient == player.connectionToClient)
            {
                gamePlayer.SetFinalPosition(finnishPosition);
            }
        }
        finnishPosition++;
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
