using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMp : NetworkBehaviour
{
    [SerializeField] Behaviour[] componentsToDisable;
    [SerializeField] GameObject playerUIPrefab;

    private GameObject playerUIInstance;

    public float BestLapTime { get; private set; } = Mathf.Infinity;
    public float LastLapTime { get; private set; } = 0;
    public float CurrentLapTime { get; private set; } = 0;
    public int CurrentLap { get; private set; } = 0;
    public float TotalTime { get; private set; } = 0;
    public float SpeedKph { get; private set; } = 0;

    private float lapTimeTimestamp;
    private float firstLapStart;
    public int lastCheckpointPassed { get; set; } = 0;

    private int totalLaps;

    private CarControllerMp carController;

    private Transform checkpointsParent;
    private int checkpointCount;
    private int checkpointLayer;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server stardet!");
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<CameraFollowMp>().setTarget(carController.transform);
        
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();
        PlayerMp _player = GetComponent<PlayerMp>();
        GameManager.RegisterPlayer(_netID, _player);

        //carController = GetComponentInChildren<CarControllerMp>();
        carController = GetComponent<CarControllerMp>();
        totalLaps = Selection.numberOfLaps;

    }

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
        }
        else
        {
            playerUIInstance = Instantiate(playerUIPrefab);
            playerUIInstance.name = playerUIPrefab.name;

            PlayerUIController ui = playerUIInstance.GetComponent<PlayerUIController>();
            if (ui == null)
                Debug.LogError("No Ui on playerUI prefab");
            ui.SetController(GetComponent<PlayerMp>());

            checkpointsParent = GameObject.Find("Checkpoints").transform;
            checkpointCount = checkpointsParent.childCount;
            Debug.Log(checkpointCount);
            checkpointLayer = LayerMask.NameToLayer("Checkpoint");
        }

    }

    public void StartLap()
    {
        Debug.Log("StartLap");
        CurrentLap++;
        lastCheckpointPassed = 1;
        lapTimeTimestamp = Time.time;
        if (CurrentLap == 1)
        {
            firstLapStart = Time.time;
        }
    }

    public void Endlap()
    {
        LastLapTime = Time.time - lapTimeTimestamp;
        BestLapTime = Mathf.Min(LastLapTime, BestLapTime);
        Debug.Log("EndLap - din tid: " + LastLapTime + " sekunder");
        if (CurrentLap == totalLaps)
        {
            TotalTime = Time.time - firstLapStart;
        }
    }

    void Update()
    {
        CurrentLapTime = lapTimeTimestamp > 0 ? Time.time - lapTimeTimestamp : 0;
        SpeedKph = carController.Speed;
        if (hasAuthority)
        {
            carController.Steer = Input.GetAxis("Horizontal");
            carController.Throttle = Input.GetAxis("Right Trigger") - Input.GetAxis("Submit");
            carController.Brake = Input.GetAxis("Submit");
            carController.Reset = Input.GetAxis("Reset");
        }
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer != checkpointLayer)
        {
            return;
        }

        if (collider.gameObject.name == "1")
        {
            if (lastCheckpointPassed == checkpointCount)
            {
                Endlap();
            }

            if (CurrentLap == 0 || lastCheckpointPassed == checkpointCount)
            {
                StartLap();
            }
            return;
        }

        if (collider.gameObject.name == (lastCheckpointPassed + 1).ToString())
        {
            lastCheckpointPassed++;
        }

    }

    private void OnDisable()
    {
        Destroy(playerUIInstance);
        GameManager.UnRegisterPlayer(transform.name);
    }

    void DisableComponents()
    {
        for (int i = 0; i < componentsToDisable.Length; i++)
        {
            componentsToDisable[i].enabled = false;
        }
    }
}
