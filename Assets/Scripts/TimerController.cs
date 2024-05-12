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
    }

    private void OnDestroy()
    {
        leftPose.WhenSelected -= LeftPose_WhenSelected;
        leftPose.WhenUnselected -= LeftPose_WhenUnselected;
        rightPose.WhenSelected -= RightPose_WhenSelected;
        rightPose.WhenUnselected -= RightPose_WhenUnselected;
    }

    private void RightPose_WhenUnselected()
    {
        rightHandUnselectEvent?.Invoke();
        //DeleteTimer(1);
    }


    private void RightPose_WhenSelected()
    {
        rightHandSelectEvent?.Invoke();
        CreateTimer(1);
    }


    private void LeftPose_WhenUnselected()
    {
        leftHandUnselectEvent?.Invoke();
        //DeleteTimer(0);
    }


    private void LeftPose_WhenSelected()
    {
        leftHandSelectEvent?.Invoke();
        CreateTimer(0);
    }
    

    public void CreateTimer(int handSign)
    {
        //_ = Instantiate(PFB_Timer, handSign == 0 ? leftHandWatchAnchor.transform : rightHandWatchAnchor.transform);
        CreateTimerInSet(handSign);
    }

    public void CreateTimerInSet(int handSign) {
        GameObject timer = Instantiate(PFB_Timer, handSign == 0 ? leftTimerSetAnchor.transform : rightTimerSetAnchor.transform);
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

    
}
