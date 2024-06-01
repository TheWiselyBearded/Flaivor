using Oculus.Interaction.Samples;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimerCountdown : MonoBehaviour
{
    public GameObject vrPanel;

    public GameObject root;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI timerTitle;
    public GameObject StartButton;
    public GameObject DestroyButton;
    public GameObject TimerControlButtons;
    public Image background;
    public Color32 bgColorAlarm;

    public AudioSource alarmSound;

    public GameObject anchorReference;
    public TimerCountdown anchorReferenceTC;

    //public int minutes = 5;
    //public int seconds = 0;
    public bool timeSet;
    public bool timeFinished;
    public float countdownTime;
    private int secondsOnesPlace;
    private int secondsTenthsPlace;

    private float lastDisabledTime; // Store the time when the script was last disabled

    public bool isFloater = false;
    private bool firstGrab = false;

    public float rotationSpeed = 10f;

    public static event Action<TimerCountdown> OnTimerCountdownDuplicate;

    private void Start()
    {
        if (vrPanel == null) vrPanel = transform.GetChild(0).gameObject;
        if (timerText == null) timerText = GetComponent<TextMeshProUGUI>();
        //countdownTime = minutes * 60 + seconds;
    }

    public void SetParent() {
        if (!firstGrab) {
            OnTimerCountdownDuplicate?.Invoke(this);
            isFloater = true;
        }
        firstGrab = true;
        //root.transform.parent = null;
    }

    private void OnDisable() {
        // Save the current time when the script is disabled
        lastDisabledTime = Time.time;
    }

    private void OnEnable() {
        if (!isFloater) {
            // Subtract the time difference from countdownTime
            float timeDifference = Time.time - lastDisabledTime;
            countdownTime -= timeDifference;
        }
        //if (isFloater) isFloater = false;
    }


    public void SetTimer(int min, int sec) {
        countdownTime = min * 60 + sec;
    }


    public void SetTimer(int targetTime)
    {
        countdownTime = targetTime;
        timerText.text = targetTime.ToString();
    }

    public void SetTimerTitle(string title) {
        timerTitle.text = title;
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
        TimerControlButtons.SetActive(false);
    }
    public void DestroyTimer() {
        if (timeFinished) {
            Destroy(this.gameObject);
        }
    }
    

    // Update timer text to display the current countdown time
    void UpdateTimerText()
    {
        int minutes = Mathf.FloorToInt(countdownTime / 60);
        int seconds = Mathf.FloorToInt(countdownTime % 60);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }
    //public void SetMinutes(int min) => minutes = min;
    //public void SetSeconds(int sec) => seconds = sec;

    void Update() {
        if (!timeSet) return;

        if (!timeFinished) {
            // Decrease countdown by deltaTime
            countdownTime -= Time.deltaTime;

            // Calculate remaining minutes and seconds
            int remainingMinutes = Mathf.FloorToInt(countdownTime / 60);
            int remainingSeconds = Mathf.FloorToInt(countdownTime % 60);

            // Update text
            timerText.text = string.Format("{0:00}:{1:00}", remainingMinutes, remainingSeconds);
            if (anchorReference != null && anchorReference.activeInHierarchy) {
                if (anchorReferenceTC == null) anchorReferenceTC = anchorReference.GetComponent<TimerCountdown>();
                if (anchorReferenceTC != null) anchorReferenceTC.timerText.text = timerText.text;
            }
            // Check if countdown is finished
            if (countdownTime <= 0 && firstGrab) {
                // Do something when timer reaches 0
                Debug.Log("Timer Finished!");
                DestroyButton.SetActive(true);
                timeFinished = true;
                SetTimerOff();
                //enabled = false; // Disable this script to stop updating the timer
            } 
        }
        if (countdownTime <=0 && !isFloater) {
            Destroy(this.gameObject);
        }

        // Check if rotation is enabled
        if (timeFinished) {
            // Calculate rotation angles
            float leftRotationAngle = Mathf.Sin(Time.time * rotationSpeed) * 2f;
            float rightRotationAngle = -leftRotationAngle;

            // Rotate the object
            vrPanel.transform.localEulerAngles = new Vector3(vrPanel.transform.localEulerAngles.x, vrPanel.transform.localEulerAngles.y, leftRotationAngle);

            // Uncomment below line if you want the object to rotate back and forth
            //transform.rotation = Quaternion.Euler(0f, leftRotationAngle, 0f);
        }
    }

    public void SetTimerOff() {
        if (isFloater) {
            background.color = bgColorAlarm;
            timerText.text = "00:00";
            DestroyButton.SetActive(true);
            alarmSound.Play();
        }
        // else play audio
    }
}
