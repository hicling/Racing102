using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using System.Linq;
using UnityEngine.SceneManagement;
using PlayFab;
using kcp2k;

public class NetworkManager102 : NetworkManager
{
    [SerializeField] private int minPlayers = 2;
    private int numberOfLaps;

    [SerializeField] private string menuScene = string.Empty;

    [Header("Playfab")]
    [SerializeField] Configuration _configuration = default;

    [Header("Room")]
    [SerializeField] private NetworkRoomPlayer102 roomPlayerPrefab = null;

    [Header("Game")]
    [SerializeField] private NetworkGamePlayer102 gamePlayerPrefab = null;
    [SerializeField] private GameObject playerSpawnSystem = null;
    [SerializeField] private GameObject roundSystem = null;
    [SerializeField] private GameObject skidMarks = null;
    [SerializeField] private PositionSystem positionSystem = null;

    public Configuration Config
    {
        get
        {
            return _configuration;
        }
    }

    public KcpTransport Transport
    {
        get
        {
            return transport as KcpTransport;
        }
        set
        {
            transport = value;
        }
    }

    public int NumberOfLaps
    {
        set
        {
            numberOfLaps = value;
        }
    }
    public static event Action<NetworkConnection> OnClientConnected;
    public static event Action<NetworkConnection> OnClientDisconnected;
    public static event Action<NetworkConnection> OnServerReadied;
    public static event Action OnServerStopped;
    public static event Action OnSceneChange;
    public event Action<string> OnPlayerAdded;
    public event Action<string> OnPlayerRemoved;

    public List<UnityNetworkConnection> Connections { get; set; }
    public List<NetworkRoomPlayer102> RoomPlayers { get; } = new List<NetworkRoomPlayer102>();
    public List<NetworkGamePlayer102> GamePlayers { get; } = new List<NetworkGamePlayer102>();

    public override void Awake()
    {
        base.Awake();

        if (Config.buildType == BuildType.REMOTE_SERVER)
        {
            Connections = new List<UnityNetworkConnection>();
            NetworkServer.RegisterHandler<ReceiveAuthenticateMessage>(OnRecieveAuthenticate);
        }
    }

    private void OnRecieveAuthenticate(NetworkConnection _conn, ReceiveAuthenticateMessage msgType)
    {
        var conn = Connections.Find(c => c.ConnectionId == _conn.connectionId);
        if (conn != null)
        {
            conn.PlayFabId = msgType.PlayFabId;
            conn.IsAuthenticated = true;
            OnPlayerAdded?.Invoke(msgType.PlayFabId);
        }
    }

    public override void OnStartServer()
    {
        //spawnPrefabs.Clear();
        //spawnPrefabs = Resources.LoadAll<GameObject>("SpawnablePrefabs").ToList();

        //new test to see if works
        RoomPlayers.Clear();
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

        OnClientConnected?.Invoke(conn);
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);

        SceneManager.LoadScene(offlineScene);

        OnClientDisconnected?.Invoke(conn);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        //if (numPlayers >= maxConnections)
        //{
        //    conn.Disconnect();
        //    return;
        //}

        //if (SceneManager.GetActiveScene().name != menuScene)
        //{
        //    conn.Disconnect();
        //    return;
        //}

        var uconn = Connections.Find(c => c.ConnectionId == conn.connectionId);
        if (uconn == null)
        {
            Connections.Add(new UnityNetworkConnection()
            {
                Connection = conn,
                ConnectionId = conn.connectionId,
                LobbyId = PlayFabMultiplayerAgentAPI.SessionConfig.SessionId
            });
        }
    }

    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        if (SceneManager.GetActiveScene().name == menuScene)
        {
            bool isLeader = RoomPlayers.Count == 0;
            Debug.Log("isLeader är: " + isLeader);

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

            if(player is NetworkRoomPlayer102 roomPlayer)
            {
                RoomPlayers.Remove(roomPlayer);

                NotifyPlayersOfReadyState();
            }
            else if(player is NetworkGamePlayer102 gamePlayer)
            {
                GamePlayers.Remove(gamePlayer);
            }
        }

        var uconn = Connections.Find(c => c.ConnectionId == conn.connectionId);
        if (uconn != null)
        {
            if (!string.IsNullOrEmpty(uconn.PlayFabId))
            {
                OnPlayerRemoved?.Invoke(uconn.PlayFabId);
            }
            Connections.Remove(uconn);
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
        Debug.Log("NotifyPlayersOfReadyState Startad");
        int antal = 0;
        foreach (var player in RoomPlayers)
        {
            player.HandleReadyToStart(IsReadyToStart());
            Debug.Log("NotifyPlayersOfReadyState har körts: " + antal);
            antal++;
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
        Debug.Log("IsReadyToStart Startad");
        if (RoomPlayers.Count < minPlayers)
        {
            Debug.Log("IsReadyToStart är falskt - fastnat på antal spelare");
            return false;
        }
        foreach (var player in RoomPlayers)
        {
            if (!player.IsReady)
            {
                Debug.Log("IsReadyToStart är falskt - fastnat på att spelare inte är redo");
                return false;
            }
        }
        Debug.Log("IsReadyToStart är sant");
        return true;
    }

    private bool IsReadyToStartGame()
    {
        if (RoomPlayers.Count < minPlayers)
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
        if(SceneManager.GetActiveScene().name == menuScene)
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
