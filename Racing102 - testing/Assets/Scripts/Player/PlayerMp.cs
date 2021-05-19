using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using TMPro;

public class PlayerMp : MonoBehaviour
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

    private bool finnished = false;
    private uint ownerId;

    public bool Finnished => finnished;
    public uint OwnderId => ownerId;

    private GameObject playerUIInstance;

    private void HandlePlayerFinnish(bool odlvalue, bool newValue)
    {
        OnFinnished?.Invoke(this, finnished);
    }

    private void HandleOwnerSet(uint oldValue, uint newValue)
    {
        OnPlayerSpawned?.Invoke(this);

    }

    public void SetFinnish(bool finnish)
    {
        this.finnished = finnish;        
    }

    public void SetOwner(uint ownerId)
    {
        this.ownerId = ownerId;
    }





    void Start()
    {
        
        playerUIInstance = Instantiate(playerUIPrefab);
        PlayerUIController ui = playerUIInstance.GetComponent<PlayerUIController>();
        if (ui == null)
            Debug.LogError("No Ui on playerUI prefab");
        ui.SetController(GetComponentInChildren<CarLapController>());
        playerUIInstance.SetActive(true);
    }
    
    void Update()
    {
        
        if (carLapController.Finnished) { return; }

        //if (PauseMenu.IsOn) { return; }

        carController.Steer = InputManager.Controls.Player.Steer.ReadValue<float>();
        carController.Throttle = InputManager.Controls.Player.Throttle.ReadValue<float>() - InputManager.Controls.Player.Brake.ReadValue<float>();
        carController.Brake = InputManager.Controls.Player.Brake.ReadValue<float>();
        carController.Reset = InputManager.Controls.Player.Reset.ReadValue<float>();
    }


    private void OnDestroy()
    {
        OnPlayerDespawned?.Invoke(this);
    }

}
