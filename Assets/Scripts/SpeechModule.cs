using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ElevenLabs;
using System.Linq;
using ElevenLabs.Voices;

public class SpeechModule : MonoBehaviour
{
    [SerializeField] private ThinkerModule thinkerModule;
    [SerializeField] private Voice voice;
    [SerializeField] private AudioSource audioSource;

    private void Awake()
    {
        thinkerModule.OnChatGPTHelpInputReceived += OnHelpCompletionReceived;
    }

    private async void OnHelpCompletionReceived(string completion)
    {
        var api = new ElevenLabsClient();
        var text = completion;
        // var voice = (await api.VoicesEndpoint.GetAllVoicesAsync()).FirstOrDefault();
        var defaultVoiceSettings = await api.VoicesEndpoint.GetDefaultVoiceSettingsAsync();
        var voiceClip = await api.TextToSpeechEndpoint.TextToSpeechAsync(text, voice, defaultVoiceSettings);
        audioSource.PlayOneShot(voiceClip.AudioClip);
    }

    private void OnDestroy()
    {
        thinkerModule.OnChatGPTHelpInputReceived -= OnHelpCompletionReceived;
    }
}
