using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private NetworkManager102 networkManager = null;

    [Header("UI")]
    [SerializeField] private GameObject LandingPagePanel = null;

    public void HostLobby()
    {
        networkManager.StartHost();

        LandingPagePanel.SetActive(false);
    }
}
