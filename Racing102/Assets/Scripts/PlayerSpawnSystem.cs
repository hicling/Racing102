using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System.Linq;

public class PlayerSpawnSystem : NetworkBehaviour
{
    [SerializeField] private GameObject playerPrefab = null;

    private static List<Transform> spawnPoints = new List<Transform>();

    private int nextIndex = 0;

    public static void AddSpawnPoint(Transform transform)
    {
        spawnPoints.Add(transform);

        spawnPoints = spawnPoints.OrderBy(x => x.GetSiblingIndex()).ToList();
    }

    public static void RemoveSpawnPoint(Transform transform) => spawnPoints.Remove(transform);

    public override void OnStartServer() => NetworkManager102.OnServerReadied += SpawnPlayer;

    public override void OnStartClient()
    {
        InputManager.Add(ActionMapNames.Player);
        InputManager.Controls.Player.Pause.Enable();
    }
    [ServerCallback]
    private void OnDestroy() => NetworkManager102.OnServerReadied -= SpawnPlayer;

    [Server]
    public void SpawnPlayer(NetworkConnection conn)
    {
        Transform spawnPoint = spawnPoints.ElementAtOrDefault(nextIndex);

        if (spawnPoint = null)
        {
            Debug.LogError($"Missing spawn point for player {nextIndex}");
            return;
        }
        var gamePlayer = conn.identity.GetComponent<NetworkGamePlayer102>();

        var playerInstance = Instantiate(playerPrefab, spawnPoints[nextIndex].position, spawnPoints[nextIndex].rotation);
        var playerMp = playerInstance.GetComponent<PlayerMp>();
        playerMp.SetOwner(gamePlayer.netId);
        NetworkServer.Spawn(playerInstance, conn);

        nextIndex++;
    }

    public override void OnStopServer()
    {
        NetworkManager102.OnServerReadied -= SpawnPlayer;
    }
}
