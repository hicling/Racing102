using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarLapController : MonoBehaviour
{
    [SerializeField] private CarController carController;
    public float BestLapTime { get; private set; } = Mathf.Infinity;
    public float LastLapTime { get; private set; } = 0;
    public float CurrentLapTime { get; private set; } = 0;
    public int CurrentLap { get; private set; } = 0;
    public float TotalTime { get; private set; } = 0;
    public float SpeedKph { get; private set; } = 0;
    public int LastCheckpointPassed { get; set; } = 0;
    public int Position { get; set; }
    public int numberOfPlayers { get; set; }
    public float DistanceToNextCheckpoint { get; private set; }
    public bool Finnished { get; private set; } = false;

    private int finnishedPosition;
    public int FinnishedPosition => finnishedPosition;

    private PositionSystem positionSystem;

    private float lapTimeTimestamp;
    private float firstLapStart;

    private int totalLaps;
    public int TotalLaps { get { return totalLaps; } set { totalLaps = value; } }

    private Transform checkpointsParent;
    private int checkpointCount;
    private int checkpointLayer;

    private void Awake()
    {
        positionSystem = GameObject.FindGameObjectWithTag("PositionSystem").GetComponent<PositionSystem>();
    }    

    private void Start()
    {
        checkpointsParent = GameObject.Find("Checkpoints").transform;
        checkpointCount = checkpointsParent.childCount;
        checkpointLayer = LayerMask.NameToLayer("Checkpoint");
        positionSystem.CarLapPlayers.Add(this);
        totalLaps = positionSystem.TotalLaps;
    }

    public void OnDestroy()
    {
        positionSystem.CarLapPlayers.Remove(this);
    }
    public void StartLap()
    {
        if(CurrentLap == totalLaps) { return; }
        CurrentLap++;
        LastCheckpointPassed = 1;
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
            finnishedPosition = Position;
            TotalTime = Time.time - firstLapStart;
            Finnished = true;
        }
    }

    private void Update()
    {
        if (!Finnished)
        {
            CurrentLapTime = lapTimeTimestamp > 0 ? Time.time - lapTimeTimestamp : 0;
        }else
        {
            CurrentLapTime = LastLapTime;
        }
        SpeedKph = carController.Speed;
        FindDistanceToNextCheckpoint();
    }

    private void FindDistanceToNextCheckpoint()
    {
        int index = LastCheckpointPassed;
        if (index == 7) { index = 0; }
        DistanceToNextCheckpoint = Vector3.Distance(checkpointsParent.GetChild(index).transform.position, this.transform.position);
    }

    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.layer != checkpointLayer)
        {
            return;
        }

        if (collider.gameObject.name == "1")
        {
            if (LastCheckpointPassed == checkpointCount)
            {
                Endlap();
            }

            if (CurrentLap == 0 || LastCheckpointPassed == checkpointCount)
            {
                StartLap();
            }
            return;
        }

        if (collider.gameObject.name == (LastCheckpointPassed + 1).ToString())
        {
            LastCheckpointPassed++;
        }

    }
}
