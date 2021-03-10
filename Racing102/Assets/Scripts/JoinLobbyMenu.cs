using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class JoinLobbyMenu : MonoBehaviour
{
    //[SerializeField] private NetworkManager102 networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject LandingPagePanel = null;
    [SerializeField] private InputField ipAddressInputField = null;
    [SerializeField] private Button joinButton = null;

    private NetworkManager102 room;
    private NetworkManager102 Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManager102;
        }
    }

    private void OnEnable()
    {
        NetworkManager102.OnClientConnected += HandleClientConnected;
        NetworkManager102.OnClientDisconnected += HandleClientDisconnected;
    }

    private void OnDisable()
    {
        NetworkManager102.OnClientConnected -= HandleClientConnected;
        NetworkManager102.OnClientDisconnected -= HandleClientDisconnected;
    }

    public void JoinLobby()
    {
        string ipAddress = ipAddressInputField.text;

        Room.networkAddress = ipAddress;
        Room.StartClient();

        joinButton.interactable = false;
    }

    private void HandleClientConnected()
    {
        joinButton.interactable = true;

        gameObject.SetActive(false);
        LandingPagePanel.SetActive(false);
    }

    private void HandleClientDisconnected()
    {
        joinButton.interactable = true;
        LandingPagePanel.SetActive(true);
    }
}
