using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Temp : MonoBehaviour
{
    public TimerCountdown tc;

    private void Update() {
        if (Input.GetKey(KeyCode.Space)) SetTimerOff();
    }

    public void SetTimerOff() {
        tc.isFloater = true;
        tc.timeSet = true;
        tc.timeFinished = true;
        tc.SetTimerOff();
    }
}
