using Oculus.Interaction;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class TimerController : MonoBehaviour
{
    public UnityEvent leftHandSelectEvent;
    public UnityEvent leftHandUnselectEvent;
    public UnityEvent rightHandSelectEvent;
    public UnityEvent rightHandUnselectEvent;

    public ActiveStateSelector leftPose;
    public ActiveStateSelector rightPose;


    public TextMeshProUGUI textMesh;

    public int minutes = 5;
    public int seconds = 0;
    public bool timeSet;

    private float countdown;

    void Start()
    {
        if (textMesh  == null) textMesh = GetComponent<TextMeshProUGUI>();
        countdown = minutes * 60 + seconds;

        leftPose.WhenSelected += LeftPose_WhenSelected;
        leftPose.WhenUnselected += LeftPose_WhenUnselected;
        rightPose.WhenSelected += RightPose_WhenSelected;
        rightPose.WhenUnselected += RightPose_WhenUnselected;
    }

    private void RightPose_WhenUnselected()
    {
        rightHandUnselectEvent?.Invoke();
    }

    private void RightPose_WhenSelected()
    {
        rightHandSelectEvent?.Invoke();
    }

    private void LeftPose_WhenUnselected()
    {
        leftHandUnselectEvent?.Invoke(); 
    }

    private void LeftPose_WhenSelected()
    {
        leftHandSelectEvent?.Invoke();
    }

    public void SetTimer(float targetTime)
    {
        textMesh.text = targetTime.ToString();
    }

    public void SetMinutes(int min) => minutes = min;
    public void SetSeconds(int sec) => seconds = sec;

    void Update()
    {
        if (!timeSet) return;
        // Decrease countdown by deltaTime
        countdown -= Time.deltaTime;

        // Calculate remaining minutes and seconds
        int remainingMinutes = Mathf.FloorToInt(countdown / 60);
        int remainingSeconds = Mathf.FloorToInt(countdown % 60);

        // Update text
        textMesh.text = string.Format("{0:00}:{1:00}", remainingMinutes, remainingSeconds);

        // Check if countdown is finished
        if (countdown <= 0)
        {
            // Do something when timer reaches 0
            Debug.Log("Timer Finished!");
            enabled = false; // Disable this script to stop updating the timer
        }
    }
}
