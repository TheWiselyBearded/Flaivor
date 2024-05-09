using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerCountdown : MonoBehaviour
{
    public GameObject root;
    public TextMeshProUGUI textMesh;

    public int minutes = 5;
    public int seconds = 0;
    public bool timeSet;
    private float countdown;

    private void Start()
    {
        if (textMesh == null) textMesh = GetComponent<TextMeshProUGUI>();
        countdown = minutes * 60 + seconds;
    }

    public void SetParent() => root.transform.parent = null;
    

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
