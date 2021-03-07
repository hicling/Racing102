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
    [SerializeField] private GameObject FinnishPanel = null;
    [SerializeField] private TMP_Text goldText;
    [SerializeField] private TMP_Text silverText;
    [SerializeField] private TMP_Text bronzeText;
    [SerializeField] private GameObject BronzeImage;

    private int finnishPosition;
    [SyncVar]
    [SerializeField] private string valueGoldText;
    [SyncVar]
    [SerializeField] private string valueSilverText;
    [SyncVar]
    [SerializeField] private string valueBronzeText;
    [SyncVar]
    private int numberOfPlayers;

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

        numberOfPlayers = Room.GamePlayers.Count;

        finnishPosition = 1;

        RpcStartCountdown();
    }

    [Server]
    private void HandlePlayerFinnish(PlayerMp player, bool finnished)
    {
        Debug.Log(player.OwnderId + " - " + finnishPosition + " - " + finnished);

        foreach (var gamePlayer in Room.GamePlayers)
        {
            if (gamePlayer.connectionToClient == player.connectionToClient)
            {
                gamePlayer.SetFinalPosition(finnishPosition);
                if (finnishPosition == 1)
                {
                    valueGoldText = gamePlayer.DisplayName;
                }
                else if (finnishPosition == 2)
                {
                    valueSilverText = gamePlayer.DisplayName;
                }
                else if (finnishPosition == 3)
                {
                    valueBronzeText = gamePlayer.DisplayName;
                }
            }
        }

        finnishPosition++;

        if (finnishPosition - 1 == Room.GamePlayers.Count)
        {
            Debug.Log("alla har gått imål!");
            StartCoroutine(WaitEndRound());            
        }
    }

    IEnumerator WaitEndRound()
    {
        yield return new WaitForSecondsRealtime(2);
        EndRound();
    }

    [ServerCallback]
    private void EndRound()
    {
        RpcEnableFinnish();
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

    [ClientRpc]
    private void RpcEnableFinnish()
    {
        if (numberOfPlayers > 2)
        {
            BronzeImage.SetActive(true);
            bronzeText.text = valueBronzeText;
        }

        goldText.text = valueGoldText;
        silverText.text = valueSilverText;

        FinnishPanel.SetActive(true);
    }

    #endregion
}
