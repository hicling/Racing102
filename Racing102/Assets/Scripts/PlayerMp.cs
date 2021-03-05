using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using System;
using UnityEngine.UI;
using TMPro;

public class PlayerMp : NetworkBehaviour
{
    [SerializeField] private GameObject playerUIPrefab;
    [SerializeField] private CarController carController;
    [SerializeField] private CarLapController carLapController;
    [Header("PlayerName")]
    [SerializeField] private Canvas canvasNameTag;
    [SerializeField] private TMP_Text playerNametext;

    public static event Action<PlayerMp> OnPlayerSpawned;
    public static event Action<PlayerMp> OnPlayerDespawned;
    public static event Action<PlayerMp, bool> OnFinnished;

    [SyncVar(hook = nameof(HandlePlayerFinnish))]
    private bool finnished = false;
    [SyncVar(hook = nameof(HandleOwnerSet))]
    private uint ownerId;

    public bool Finnished => finnished;
    public uint OwnderId => ownerId;

    private GameObject playerUIInstance;

    private void HandlePlayerFinnish(bool odlvalue, bool newValue)
    {
        OnFinnished?.Invoke(this, finnished);
        StartCoroutine(DisablePlayerUI());
    }

    IEnumerator DisablePlayerUI()
    {

        yield return new WaitForSecondsRealtime(2);
        //playerUIInstance.SetActive(false);
    }

    private void HandleOwnerSet(uint oldValue, uint newValue)
    {
        OnPlayerSpawned?.Invoke(this);

        var gamePlayer = NetworkIdentity.spawned[ownerId].GetComponent<NetworkGamePlayer102>();

        playerNametext.text = gamePlayer.DisplayName;
    }

    [Server]
    public void SetFinnish(bool finnish)
    {
        this.finnished = finnish;        

        if (!isServerOnly) { return; }
    }

    [Server]
    public void SetOwner(uint ownerId)
    {
        this.ownerId = ownerId;

        if (!isServerOnly) { return; }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
    }

    public override void OnStartAuthority()
    {
        canvasNameTag.gameObject.SetActive(false);
        enabled = true;
    }

    public override void OnStartClient()
    {
        base.OnStartClient();
    }

    void Start()
    {
        if (!hasAuthority) { return; }
        
        playerUIInstance = Instantiate(playerUIPrefab);
        PlayerUIController ui = playerUIInstance.GetComponent<PlayerUIController>();
        if (ui == null)
            Debug.LogError("No Ui on playerUI prefab");
        ui.SetController(GetComponentInChildren<CarLapController>());
        playerUIInstance.SetActive(true);
    }
    
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; }
        
        if (carLapController.Finnished) { return; }
        carController.Steer = InputManager.Controls.Player.Steer.ReadValue<float>();
        carController.Throttle = InputManager.Controls.Player.Throttle.ReadValue<float>() - InputManager.Controls.Player.Brake.ReadValue<float>();
        carController.Brake = InputManager.Controls.Player.Brake.ReadValue<float>();
        carController.Reset = InputManager.Controls.Player.Reset.ReadValue<float>();
    }

    [ClientCallback]
    private void OnDisable()
    {
        Destroy(playerUIInstance);
    }

    private void OnDestroy()
    {
        OnPlayerDespawned?.Invoke(this);
    }

}
