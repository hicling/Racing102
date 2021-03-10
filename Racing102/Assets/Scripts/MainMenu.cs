using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class MainMenu : MonoBehaviour
{
    //private NetworkManager102 networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject LandingPagePanel = null;

    private NetworkManager102 room;
    private NetworkManager102 Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManager102;
        }
    }

    public void HostLobby()
    {
        Room.StartHost();

        LandingPagePanel.SetActive(false);
    }
}
