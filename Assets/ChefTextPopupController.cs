using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using NUnit.Framework;
public class ChefTextPopupController : MonoBehaviour
{
    public bool StaticActivationTime=false;
    public float ActivationTime =5.0f;
    public GameObject chefTextWindow;
    public TextMeshProUGUI TMP_ChefText;

    private void Start() {
        chefTextWindow.SetActive(false);
    }

    public void OnEnable() {
        SpeechModule.OnSpeechInputReceived += ActivateTextWindow;
    }

    private void OnDisable() {
        SpeechModule.OnSpeechInputReceived -= ActivateTextWindow;
    }

    public void ActivateTextWindow(string text, float timeActive) => StartCoroutine(ActivateChefTextPopUp(text, timeActive));

    public IEnumerator ActivateChefTextPopUp(string text, float timeActive) {
        chefTextWindow.SetActive(true);
        TMP_ChefText.text = text;
        if (!StaticActivationTime) ActivationTime = timeActive;
        yield return new WaitForSeconds(ActivationTime);
        chefTextWindow.SetActive(false);
    }

}
