using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class PlayerMp : NetworkBehaviour
{
    public enum ControlType { HumanInput, AI }
    public ControlType controlType = ControlType.HumanInput;

    public float BestLapTime { get; private set; } = Mathf.Infinity;
    public float LastLapTime { get; private set; } = 0;
    public float CurrentLapTime { get; private set; } = 0;
    public int CurrentLap { get; private set; } = 0;
    public float TotalTime { get; private set; } = 0;
    public float SpeedKph { get; private set; } = 0;

    private float lapTimeTimestamp;
    private float firstLapStart;
    private int lastCheckpointPassed = 0;

    private Transform checkpointsParent;
    private int checkpointCount;
    private int checkpointLayer;

    private int totalLaps;

    private GameMenuController gameMenuController;

    private CarController carController;

    public override void OnStartServer()
    {
        base.OnStartServer();
        Debug.Log("Server stardet!");
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<CameraFollowMp>().setTarget(carController.transform);
    }

    void Awake()
    {
        checkpointsParent = GameObject.Find("Checkpoints").transform;
        checkpointCount = checkpointsParent.childCount;
        checkpointLayer = LayerMask.NameToLayer("Checkpoint");
        carController = GetComponentInChildren<CarController>();
        //gameMenuController = GameObject.FindGameObjectWithTag("Canvas").GetComponent<GameMenuController>();
        totalLaps = Selection.numberOfLaps;
    }

    void StartLap()
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

    void Endlap()
    {
        LastLapTime = Time.time - lapTimeTimestamp;
        BestLapTime = Mathf.Min(LastLapTime, BestLapTime);
        Debug.Log("EndLap - din tid: " + LastLapTime + " sekunder");
        if (CurrentLap == totalLaps)
        {
            TotalTime = Time.time - firstLapStart;
            gameMenuController.Finnish();
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

    void Update()
    {
        CurrentLapTime = lapTimeTimestamp > 0 ? Time.time - lapTimeTimestamp : 0;
        SpeedKph = carController.Speed;
        if (controlType == ControlType.HumanInput && isLocalPlayer)
        {
            carController.Steer = Input.GetAxis("Horizontal");
            carController.Throttle = Input.GetAxis("Right Trigger") - Input.GetAxis("Submit");
            carController.Brake = Input.GetAxis("Submit");
            carController.Reset = Input.GetAxis("Reset");
        }
    }
}
