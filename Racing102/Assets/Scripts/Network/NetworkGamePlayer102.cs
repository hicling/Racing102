using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System.Linq;

public class NetworkGamePlayer102 : NetworkBehaviour
{
    [Header("UI")]
    [SerializeField] private GameObject finnishMenu = null;
    [SerializeField] private Text[] playerNameTexts = new Text[6];
    [SerializeField] private Text[] playerReadyTexts = new Text[6];
    [SerializeField] private Text[] playerPositionTexts = new Text[6];
    [SerializeField] private Image[] playerBackgrounds = new Image[6];
    [SerializeField] private Button startGameButton = null;
    [SerializeField] GameObject pauseMenu;

    [SyncVar]
    public string DisplayName = "Loading...";
    [SyncVar(hook = nameof(HandlePositionChanged))]
    public int Position;
    [SyncVar(hook = nameof(HandleReadyStatusChanged))]
    public bool IsReady = false;

    private bool isLeader;
    public bool IsLeader
    {
        set
        {
            isLeader = value;
            startGameButton.gameObject.SetActive(value);
        }
    }

    private NetworkManager102 room;
    private NetworkManager102 Room
    {
        get
        {
            if (room != null) { return room; }
            return room = NetworkManager.singleton as NetworkManager102;
        }
    }

    private void Start()
    {
        PauseMenu.IsOn = false;
    }

    public override void OnStartClient()
    {
        DontDestroyOnLoad(gameObject);

        Room.GamePlayers.Add(this);
        NetworkManager102.OnSceneChange += HandleSceneChange;
        PlayerMp.OnFinnished += HandlePlayerFinnish;
        Position = 999;
    }

    public override void OnStopClient()
    {
        Room.GamePlayers.Remove(this);
        NetworkManager102.OnSceneChange -= HandleSceneChange;
        PlayerMp.OnFinnished -= HandlePlayerFinnish;
    }

    private void Update()
    {
        if (Position != 999) { return; }

        if (InputManager.Controls.Player.Pause.triggered)
        {
            TogglePauseMenu();
        }
    }

    public void HandleReadyStatusChanged(bool oldValue, bool newValue) => UpdateDisplay();
    public void HandlePositionChanged(int oldValue, int newValue) => UpdateDisplay();

    public void HandleSceneChange()
    {
        foreach (var player in Room.GamePlayers)
        {
            player.Position = 999;
            IsReady = false;
            if (player.isLeader)
            {
                startGameButton.interactable = false;
            }
        }
        RpcDisableUI();
    }

    private void UpdateDisplay()
    {
        var gamePlayers = Room.GamePlayers.OrderBy(o => o.Position).ToList();
        if (!hasAuthority)
        {
            foreach (var player in gamePlayers)
            {
                if (player.hasAuthority)
                {
                    player.UpdateDisplay();
                    break;
                }
            }

            return;
        }

        for (int i = 0; i < playerNameTexts.Length; i++)
        {
            playerNameTexts[i].text = string.Empty;
            playerReadyTexts[i].text = string.Empty;
            playerPositionTexts[i].text = string.Empty;
            playerBackgrounds[i].enabled = false;
        }

        for (int i = 0; i < gamePlayers.Count; i++)
        {
            if (gamePlayers[i].Position != 999)
            {
                playerBackgrounds[i].enabled = true;
                playerNameTexts[i].text = gamePlayers[i].DisplayName;
                playerReadyTexts[i].text = gamePlayers[i].IsReady ?
                    "<color=green>READY</color>" :
                    "<color=red>NOT READY</color>";
                playerPositionTexts[i].text = gamePlayers[i].Position.ToString();
                if (gamePlayers[i].Position == 1)
                {
                    playerBackgrounds[i].color = new Color32(255, 215, 0, 255);
                }
                else if (gamePlayers[i].Position == 2)
                {
                    playerBackgrounds[i].color = new Color32(192, 192, 192, 255);
                }
                else if (gamePlayers[i].Position == 3)
                {
                    playerBackgrounds[i].color = new Color32(205, 127, 50, 255);
                }
            }            
        }
    }

    public void HandleReadyToStart(bool readyToStart)
    {
        if (!isLeader)
        {
            return;
        }

        startGameButton.interactable = readyToStart;
    }

    [Server]
    public void SetDisplayName(string displayName)
    {
        this.DisplayName = displayName;
    }

    [Server]
    public void SetFinalPosition(int finalPosition)
    {
        Position = finalPosition;
    }

    private void HandlePlayerFinnish(PlayerMp player, bool finnished)
    {
        if (!hasAuthority) { return; }

        if (this.netId == player.OwnderId) 
        {
            finnishMenu.SetActive(true);
            pauseMenu.SetActive(false);
        }        
    }

    [ClientRpc]
    public void RpcDisableUI()
    {
        finnishMenu.SetActive(false);
    }

    [Command]
    public void CmdReadyUp()
    {
        IsReady = !IsReady;

        Room.NotifyPlayersOfReadyStateGame();
    }

    [Command]
    public void CmdStartGame()
    {
        if (!isServer) { return; }

        Room.RestartRound();
    }

    public void OpenMainMenu()
    {
        if (isServer)
        {
            RpcOpenMainMenu();
            Room.StopHost();
            Destroy(Room.gameObject);
        }
        else
        {
            Room.StopClient();
            Destroy(Room.gameObject);
        }
    }

    public void QuitGame()
    {
        if (isServer)
        {
            RpcOpenMainMenu();
            Room.StopHost();
            Application.Quit();
        }
        else
        {
            Room.StopClient();
            Application.Quit();
        }
    }

    void TogglePauseMenu()
    {
        pauseMenu.SetActive(!pauseMenu.activeSelf);
        PauseMenu.IsOn = pauseMenu.activeSelf;
    }

    [ClientRpc]
    public void RpcOpenMainMenu()
    {
        Room.StopClient();
        Destroy(Room.gameObject);
    }
}
