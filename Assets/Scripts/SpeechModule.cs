using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElevenLabs;
using System.Linq;
using ElevenLabs.Voices;
using TMPro;
using Newtonsoft.Json.Linq;

public class SpeechModule : MonoBehaviour
{
    [SerializeField] private ThinkerModule thinkerModule;
    [SerializeField] private Voice voice;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        ThinkerModule.OnChatGPTHelpInputReceived += OnHelpCompletionReceived;
    }

    private async void OnHelpCompletionReceived(string completion)
    {
        string response = "";
        // parse response from json
        try
        {
            var json = JObject.Parse(completion);
            response = json["response"].ToString();
        }
        catch (Exception e)
        {
            // unable to get response from json, so let's just use the completion
            response = completion;

            Debug.LogError(e);

        }


        var api = new ElevenLabsClient();
        var text = response;
        // var voice = (await api.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
        var defaultVoiceSettings = await api.VoicesEndpoint.GetDefaultVoiceSettingsAsync();
        var voiceClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(text, voice, defaultVoiceSettings);
        if (audioSource != null) audioSource.PlayOneShot(voiceClip.AudioClip);
    }

    private void OnDestroy()
    {
        ThinkerModule.OnChatGPTHelpInputReceived -= OnHelpCompletionReceived;
    }
}
