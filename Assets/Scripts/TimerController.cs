using GLTFast.Schema;
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

    public GameObject PFB_Timer;
    public GameObject leftHandWatchAnchor;
    public GameObject leftTimerSetAnchor;
    public GameObject rightHandWatchAnchor;
    public GameObject rightTimerSetAnchor;

    public List<GameObject> timerSet;
    public List<TimerCountdown> timerSetTimerCountdownComponent;

    public int timerIndex = 0;
    public Vector3 verticalOffset;
    public Vector3 horizontalOffset;
    public int horizontalIndex = 0;
    public int verticalIndex = 0;
    private const int HORIZONTAL_MAX = 3;
    private const int VERTICAL_MAX = 3;

    private void Awake() {
        timerSet = new List<GameObject>();
        timerSetTimerCountdownComponent = new List<TimerCountdown>();
    }

    void Start()
    {
        leftPose.WhenSelected += LeftPose_WhenSelected;
        leftPose.WhenUnselected += LeftPose_WhenUnselected;
        rightPose.WhenSelected += RightPose_WhenSelected;
        rightPose.WhenUnselected += RightPose_WhenUnselected;
        TimerCountdown.OnTimerCountdownDuplicate += TimerCountdown_OnTimerCountdownDuplicate;
    }

    private void TimerCountdown_OnTimerCountdownDuplicate(TimerCountdown obj) {
        // Clone the object
        GameObject clone = Instantiate(obj.root);

        // Set the position of the clone to be the same as the original
        clone.transform.position = obj.root.transform.position;

        // Set the rotation of the clone to be the same as the original
        clone.transform.rotation = obj.root.transform.rotation;

        // Set the scale of the clone to be the same as the original
        clone.transform.localScale = obj.root.transform.localScale;

        // Set the parent of the clone to be the same as the original
        clone.transform.parent = obj.root.transform.parent;
        obj.root.transform.parent = null;

        int removalIndex = 0;
        // Iterate over timerSet to find and replace the original object with the clone
        for (int i = 0; i < timerSet.Count; i++) {
            if (timerSet[i] == obj.root) {
                // Remove the original object from the list
                timerSet.RemoveAt(i);
                // Add the clone to the list
                timerSet.Add(clone);
                break; // Exit loop since the original object is found and replaced
            }
        }
        //timerSet.RemoveAt(removalIndex);
    }

    private void OnDestroy()
    {
        leftPose.WhenSelected -= LeftPose_WhenSelected;
        leftPose.WhenUnselected -= LeftPose_WhenUnselected;
        rightPose.WhenSelected -= RightPose_WhenSelected;
        rightPose.WhenUnselected -= RightPose_WhenUnselected;
        TimerCountdown.OnTimerCountdownDuplicate -= TimerCountdown_OnTimerCountdownDuplicate;
    }

    private void RightPose_WhenUnselected()
    {
        rightHandUnselectEvent?.Invoke();
        ToggleTimers(1, false);
    }


    private void RightPose_WhenSelected()
    {
        ToggleTimers(1, true);
        rightHandSelectEvent?.Invoke();
        CreateTimer(1);
    }


    private void LeftPose_WhenUnselected()
    {
        leftHandUnselectEvent?.Invoke();
        ToggleTimers(0, false);
    }


    private void LeftPose_WhenSelected()
    {
        ToggleTimers(1, true);
        leftHandSelectEvent?.Invoke();
        CreateTimer(0);
    }
    

    public void CreateTimer(int handSign)
    {
        //_ = Instantiate(PFB_Timer, handSign == 0 ? leftHandWatchAnchor.transform : rightHandWatchAnchor.transform);
        if (horizontalIndex >= HORIZONTAL_MAX) {
            verticalIndex++;
            horizontalIndex = 0;
        } 
        CreateTimerInSet(handSign);
        horizontalIndex++;
    }

    public void CreateTimerInSet(int handSign) {
        //Transform anchor = handSign == 0 ? leftTimerSetAnchor.transform : rightTimerSetAnchor.transform;

        Vector3 spawnPosition = new Vector3(0, 0, 0);
        GameObject timer = Instantiate(PFB_Timer,
            spawnPosition,
            new Quaternion(0,0,0,0),
            handSign == 0 ? leftTimerSetAnchor.transform : rightTimerSetAnchor.transform);
        timer.transform.localPosition = spawnPosition + (horizontalOffset * (horizontalIndex)) + (verticalOffset * verticalIndex);
        timerSet.Add(timer);
        timerSetTimerCountdownComponent.Add(timer.GetComponent<TimerCountdown>());
        timerIndex++;
    }

    public void CreateTimerInSet(int handSign, int min, int sec) {
        GameObject timer = Instantiate(PFB_Timer, handSign == 0 ? leftTimerSetAnchor.transform : rightTimerSetAnchor.transform);
        timerSet.Add(timer);
        timerSetTimerCountdownComponent.Add(timer.GetComponent<TimerCountdown>());
        SetTimerTime(timerIndex, min, sec);
        timerIndex++;
    }

    public void SetTimerTime(int timerIndex, int min, int sec) {
        timerSetTimerCountdownComponent[timerIndex].SetTimer(min, sec);
    }

    public void DeleteTimer(int handSign)
    {
        foreach (Transform child in handSign == 0 ? leftHandWatchAnchor.transform : rightHandWatchAnchor.transform)
        {
            Destroy(child.gameObject);
        }
    }

    public void ToggleTimers(int handSign, bool status) {
        //foreach (Transform child in handSign == 0 ? leftTimerSetAnchor.transform : rightTimerSetAnchor.transform) {
        //    child.gameObject.SetActive(status);
        //}
        foreach (GameObject timer in timerSet) {
            timer.SetActive(status);
        }
    }
}
