using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private const string PLAYER_ID_PREFIX = "Player ";

    public static Dictionary<string, PlayerMp> players = new Dictionary<string, PlayerMp>();
    public static GameManager Instance { get; private set; }
    public InputController InputController { get; private set; }

    void Awake()
    {
        Instance = this;
        InputController = GetComponentInChildren<InputController>();
    }

    public static void RegisterPlayer(string _netID, PlayerMp _player)
    {
        string _playerID = PLAYER_ID_PREFIX + _netID;
        players.Add(_playerID, _player);
        _player.transform.name = _playerID;
    }

    public static void UnRegisterPlayer(string _playerID)
    {
        players.Remove(_playerID);
    }

    public static PlayerMp GetPlayer (string _playerID)
    {
        return players[_playerID];
    }

}
