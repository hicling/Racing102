using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMp : NetworkBehaviour
{
    [SerializeField] GameObject playerUIPrefab;
    [SerializeField] CarControllerMp carController;

    private GameObject playerUIInstance;

    private float previusThrottle;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server stardet!");
    }

    public override void OnStartAuthority()
    {
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
        playerUIInstance.name = playerUIPrefab.name;

        PlayerUIController ui = playerUIInstance.GetComponent<PlayerUIController>();
        if (ui == null)
            Debug.LogError("No Ui on playerUI prefab");
        ui.SetController(GetComponentInChildren<CarLapController>());
        
    }
    
    [ClientCallback]
    void Update()
    {
        if (!hasAuthority) { return; }

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

}
