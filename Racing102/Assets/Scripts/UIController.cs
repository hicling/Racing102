using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    [SerializeField] GameObject UIRacePanel;
    [SerializeField] Text UITextCurrentLap;
    [SerializeField] Text UITextCurrentTime;
    [SerializeField] Text UITextLastLapTime;
    [SerializeField] Text UITextBestLapTime;

    [SerializeField] Text UIFinnishTextBestLapTime;
    [SerializeField] Text UIFinnishTextTotalTime;
    [SerializeField] Text UITextSpeedKph;

    private Player UpdateUIForPlayer;

    private int currentLap = -1;
    private int totalLaps;
    private float currentLapTime;
    private float lastLapTime;
    private float bestLapTime;
    private float totalTime;
    private float speedKph;

    private void Start()
    {
        UpdateUIForPlayer = GameObject.FindGameObjectWithTag("PlayerCar").GetComponent<Player>();
        totalLaps = Selection.numberOfLaps;
    }
    void Update()
    {
        if (UpdateUIForPlayer == null)
            return;
        
        if (UpdateUIForPlayer.SpeedKph != speedKph)
        {
            speedKph = UpdateUIForPlayer.SpeedKph;
            speedKph = Mathf.Abs(speedKph);
            UITextSpeedKph.text = $"{speedKph.ToString("f0")} km/h";
        }

        if (UpdateUIForPlayer.CurrentLap != currentLap)
        {
            currentLap = UpdateUIForPlayer.CurrentLap;
            UITextCurrentLap.text = $"LAP: {currentLap}/{totalLaps}";
        }

        if(UpdateUIForPlayer.CurrentLapTime != currentLapTime)
        {
            currentLapTime = UpdateUIForPlayer.CurrentLapTime;
            UITextCurrentTime.text = $"TIME: {(int)currentLapTime / 60}:{(currentLapTime) % 60:00.000}";
        }

        if(UpdateUIForPlayer.LastLapTime != lastLapTime)
        {
            lastLapTime = UpdateUIForPlayer.LastLapTime;
            UITextLastLapTime.text = $"LAST: {(int)lastLapTime / 60}:{(lastLapTime) % 60:00.000}";
        }

        if(UpdateUIForPlayer.BestLapTime != bestLapTime)
        {
            bestLapTime = UpdateUIForPlayer.BestLapTime;
            UITextBestLapTime.text = bestLapTime < 10000 ?$"BEST: {(int)bestLapTime / 60}:{(bestLapTime) % 60:00.000}" : "BEST: NONE";
            UIFinnishTextBestLapTime.text = bestLapTime < 10000 ? $"BEST LAP: {(int)bestLapTime / 60}:{(bestLapTime) % 60:00.000}" : "BEST LAP: NONE";
        }

        if (UpdateUIForPlayer.TotalTime != totalTime)
        {
            totalTime = UpdateUIForPlayer.TotalTime;
            UIFinnishTextTotalTime.text = $"TOTAL TIME: {(int)totalTime / 60}:{(totalTime) % 60:00.000}";
        }
    }
}
