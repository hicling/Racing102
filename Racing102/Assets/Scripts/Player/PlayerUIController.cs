using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] Text UITextCurrentPos;
    [SerializeField] Text UITextCurrentLap;
    [SerializeField] Text UITextCurrentTime;
    [SerializeField] Text UITextLastLapTime;
    [SerializeField] Text UITextBestLapTime;
    [SerializeField] Text UITextSpeedKph;
    [SerializeField] GameObject racingPanel;

    private CarLapController controller;

    private int currentLap = -1;
    private int totalLaps;
    private float currentLapTime;
    private float lastLapTime;
    private float bestLapTime;
    private float speedKph;
    private int position = -1;
    private int numberOfPlayers = -1;

    private void Start()
    {
        totalLaps = controller.TotalLaps;
        
    }

    void Update()
    {
        if (controller == null)
            return;

        if (controller.SpeedKph != speedKph)
        {
            speedKph = controller.SpeedKph;
            speedKph = Mathf.Abs(speedKph);
            UITextSpeedKph.text = $"{speedKph.ToString("f0")} km/h";
        }

        if (controller.CurrentLap != currentLap)
        {
            currentLap = controller.CurrentLap;
            UITextCurrentLap.text = $"LAP: {currentLap}/{totalLaps}";
        }

        if (controller.CurrentLapTime != currentLapTime)
        {
            currentLapTime = controller.CurrentLapTime;
            UITextCurrentTime.text = $"TIME: {(int)currentLapTime / 60}:{(currentLapTime) % 60:00.000}";
        }

        if (controller.LastLapTime != lastLapTime)
        {
            lastLapTime = controller.LastLapTime;
            UITextLastLapTime.text = $"LAST: {(int)lastLapTime / 60}:{(lastLapTime) % 60:00.000}";
        }

        if (controller.BestLapTime != bestLapTime)
        {
            bestLapTime = controller.BestLapTime;
            UITextBestLapTime.text = bestLapTime < 10000 ? $"BEST: {(int)bestLapTime / 60}:{(bestLapTime) % 60:00.000}" : "BEST: NONE";
        }

        position = controller.Position;
        numberOfPlayers = controller.numberOfPlayers;
        UITextCurrentPos.text = $"POS: {position}/{numberOfPlayers}";

        if (PauseMenu.IsOn)
        {
            racingPanel.SetActive(false);
        }
        else
        {
            racingPanel.SetActive(true);
        }
        
    }

    public void SetController(CarLapController _controller)
    {
        controller = _controller;
    }
}
