using Oculus.Interaction.Samples;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerCountdown : MonoBehaviour
{
    public GameObject root;
    public TextMeshProUGUI timerText;
    public GameObject StartButton;
    public GameObject DestroyButton;

    //public int minutes = 5;
    //public int seconds = 0;
    public bool timeSet;
    private bool timeFinished;
    private float countdownTime;
    private int secondsOnesPlace;
    private int secondsTenthsPlace;

    private void Start()
    {
        if (timerText == null) timerText = GetComponent<TextMeshProUGUI>();
        //countdownTime = minutes * 60 + seconds;
    }

    public void SetParent() => root.transform.parent = null;
    

    public void SetTimer(float targetTime)
    {
        timerText.text = targetTime.ToString();
    }

    // Method to increase minutes
    public void IncreaseMinutes()
    {
        countdownTime += 60f; // Increase by 1 minute (60 seconds)
        UpdateTimerText();
    }

    // Method to increase tenths of seconds
    public void IncreaseTenthsSeconds()
    {
        countdownTime += 10f; // Increase by 0.1 seconds
        UpdateTimerText();
    }

    // Method to increase ones place seconds
    public void IncreaseOnesSeconds()
    {
        countdownTime += 1f; // Increase by 1 second
        UpdateTimerText();
    }

    // Method to increase minutes
    public void DecreaseMinutes()
    {
        countdownTime -= 60f; // Increase by 1 minute (60 seconds)
        UpdateTimerText();
    }

    // Method to increase tenths of seconds
    public void DecreaseTenthsSeconds()
    {
        countdownTime -= 10f; // Increase by 0.1 seconds
        UpdateTimerText();
    }

    // Method to increase ones place seconds
    public void DecreaseOnesSeconds()
    {
        countdownTime -= 1f; // Increase by 1 second
        UpdateTimerText();
    }

    public void StartTimer()
    {
        timeSet = true;
        StartButton.SetActive(false);
    }
    public void DestroyTimer() => Destroy(this.gameObject);
    

    // Update timer text to display the current countdown time
    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    //public void SetMinutes(int min) => minutes = min;
    //public void SetSeconds(int sec) => seconds = sec;

    void Update()
    {
        if (!timeSet || timeFinished) return;
        // Decrease countdown by deltaTime
        countdownTime -= Time.deltaTime;

        // Calculate remaining minutes and seconds
        int remainingMinutes = Mathf.FloorToInt(countdownTime / 60);
        int remainingSeconds = Mathf.FloorToInt(countdownTime % 60);

        // Update text
        timerText.text = string.Format("{0:00}:{1:00}", remainingMinutes, remainingSeconds);

        // Check if countdown is finished
        if (countdownTime <= 0)
        {
            // Do something when timer reaches 0
            Debug.Log("Timer Finished!");
            DestroyButton.SetActive(true);
            timeFinished = true;
            //enabled = false; // Disable this script to stop updating the timer
        }
    }
}
