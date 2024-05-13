using Meta.WitAi.Dictation;
using Meta.WitAi.Requests;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PalmChefController : MonoBehaviour
{
    public DictationService witDictation;
    public Sprite talkToChef;
    public Sprite listening;
    public Sprite analyzing;
    public Sprite sent;

    public Image palmUI;

    public void SetTalkChef() => palmUI.sprite = talkToChef;
    public void SetListening() => palmUI.sprite = listening; /// invoked via init button press on palm
    public void SetAnalyzing() => palmUI.sprite = analyzing;
    public void SetSent() => palmUI.sprite = sent;
    private void ThinkerModule_OnChatGPTHelpInputReceived(string obj) {
        StartCoroutine(ProcessThought());
    }

    private IEnumerator ProcessThought() {
        SetSent(); // Set the "sent" state immediately

        yield return new WaitForSeconds(2.5f); // Wait for 2.5 seconds

        SetTalkChef(); // Set the "talk to chef" state after 2.5 seconds
    }

    private void OnEnable() {
        SetTalkChef();
        witDictation.DictationEvents.OnFullTranscription.AddListener(OnDictationCompletePalm);
        ThinkerModule.OnChatGPTHelpInputReceived += ThinkerModule_OnChatGPTHelpInputReceived;
    }


    private void OnDictationCompletePalm(string arg0) {
        SetAnalyzing();
    }


    private void OnDisable() {
        witDictation.DictationEvents.OnFullTranscription.RemoveListener(OnDictationCompletePalm);
        ThinkerModule.OnChatGPTHelpInputReceived -= ThinkerModule_OnChatGPTHelpInputReceived;
    }

}
