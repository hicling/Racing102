using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;

public class NetworkManager102Old : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    private int numberOfLaps;

    [SerializeField] private string menuScene = string.Empty;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer102 roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer102 gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;
    [SerializeField] private GameObject skidMarks = null;
    [SerializeField] private PositionSystem positionSystem = null;

    public int NumberOfLaps
    {
        set
        {
            numberOfLaps = value;
        }
    }
    public static event Action OnClientConnected;
    public static event Action OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;
    public static event Action OnSceneChange;

    public List<NetworkRoomPlayer102> RoomPlayers { get; } = new List<NetworkRoomPlayer102>();
    public List<NetworkGamePlayer102> GamePlayers { get; } = new List<NetworkGamePlayer102>();

    public override void OnStartServer()
    {
        //spawnPrefabs.Clear();
        //spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();
    }

    public override void OnStartClient()
    {
        //spawnPrefabs.Clear();
        //spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        //foreach (var prefab in spawnPrefabs)
        //{
        //    ClientScene.RegisterPrefab(prefab);
        //}
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);

        OnClientConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        SceneManager.LoadScene(offlineScene);

        OnClientDisconnected?.Invoke();
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (SceneManager.GetActiveScene().name != menuScene)
        {
            conn.Disconnect();
            return;
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;

            NetworkRoomPlayer102 roomPlayerInstance = Instantiate(roomPlayerPrefab);

            roomPlayerInstance.IsLeader = isLeader;

            NetworkServer.AddPlayerForConnection(conn, roomPlayerInstance.gameObject);
        }
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        if (conn.identity != null)
        {
            var player = conn.identity.GetComponent<NetworkBehaviour>();

            if (player is NetworkRoomPlayer102 roomPlayer)
            {
                RoomPlayers.Remove(roomPlayer);

                NotifyPlayersOfReadyState();
            }
            else if (player is NetworkGamePlayer102 gamePlayer)
            {
                GamePlayers.Remove(gamePlayer);
            }
        }
        base.OnServerDisconnect(conn);
    }

    public override void OnStopServer()
    {
        OnServerStopped?.Invoke();

        RoomPlayers.Clear();
        GamePlayers.Clear();
    }

    public void NotifyPlayersOfReadyState()
    {
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
        }
    }

    public void NotifyPlayersOfReadyStateGame()
    {
        foreach (var player in GamePlayers)
        {
            player.HandleReadyToStart(IsReadyToStartGame());
        }
    }

    private bool IsReadyToStart()
    {
        if (numPlayers < minPlayers)
        {
            return false;
        }
        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }
        return true;
    }

    private bool IsReadyToStartGame()
    {
        if (numPlayers < minPlayers)
        {
            return false;
        }
        foreach (var player in GamePlayers)
        {
            if (!player.IsReady)
            {
                return false;
            }
        }
        return true;
    }

    public void StartGame()
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            if (!IsReadyToStart()) { return; }

            ServerChangeScene("Map_01");
        }
    }

    public void EndGame()
    {
        StopHost();

        return;
    }
    public void RestartRound()
    {
        ServerChangeScene("Map_01");
    }

    public override void ServerChangeScene(string newSceneName)
    {
        OnSceneChange?.Invoke();
        if (SceneManager.GetActiveScene().name == menuScene && newSceneName.StartsWith("Map"))
        {
            for (int i = RoomPlayers.Count - 1; i >= 0; i--)
            {
                var conn = RoomPlayers[i].connectionToClient;
                var gameplayerInstance = Instantiate(gamePlayerPrefab);
                gameplayerInstance.SetDisplayName(RoomPlayers[i].DisplayName);
                gameplayerInstance.IsLeader = RoomPlayers[i].IsLeader;

                NetworkServer.ReplacePlayerForConnection(conn, gameplayerInstance.gameObject);
            }
        }
        base.ServerChangeScene(newSceneName);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName.StartsWith("Map"))
        {
            GameObject playerSpawnSystemInstance = Instantiate(playerSpawnSystem);
            NetworkServer.Spawn(playerSpawnSystemInstance);

            GameObject roundSystemInstance = Instantiate(roundSystem);
            NetworkServer.Spawn(roundSystemInstance);

            GameObject SkidMarksInstance = Instantiate(skidMarks);
            NetworkServer.Spawn(SkidMarksInstance);

            var PositionSystemInstance = Instantiate(positionSystem);
            PositionSystemInstance.SetNumberOfLaps(numberOfLaps);
            NetworkServer.Spawn(PositionSystemInstance.gameObject);
        }
    }

    public override void OnServerReady(NetworkConnection conn)
    {
        base.OnServerReady(conn);

        OnServerReadied?.Invoke(conn);
    }
}
