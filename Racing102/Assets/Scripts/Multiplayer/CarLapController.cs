using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CarLapController : NetworkBehaviour
{
    [SerializeField] private CarControllerMp carController;
    public float BestLapTime { get; private set; } = Mathf.Infinity;
    public float LastLapTime { get; private set; } = 0;
    public float CurrentLapTime { get; private set; } = 0;
    public int CurrentLap { get; private set; } = 0;
    public float TotalTime { get; private set; } = 0;
    public float SpeedKph { get; private set; } = 0;
    public int lastCheckpointPassed { get; set; } = 0;

    private PositionSystem room;
    

    private void Awake()
    {
        room = GameObject.FindGameObjectWithTag("PositionSystem").GetComponent<PositionSystem>();
    }

    private float lapTimeTimestamp;
    private float firstLapStart;
    

    private int totalLaps;

    private Transform checkpointsParent;
    private int checkpointCount;
    private int checkpointLayer;

    private void Start()
    {
        checkpointsParent = GameObject.Find("Checkpoints").transform;
        checkpointCount = checkpointsParent.childCount;
        Debug.Log(checkpointCount);
        checkpointLayer = LayerMask.NameToLayer("Checkpoint");
        room.CarLapPlayers.Add(this);
    }

    public override void OnStopClient()
    {
        room.CarLapPlayers.Remove(this);
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

    private void Update()
    {
        CurrentLapTime = lapTimeTimestamp > 0 ? Time.time - lapTimeTimestamp : 0;
        SpeedKph = carController.Speed;
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
}
