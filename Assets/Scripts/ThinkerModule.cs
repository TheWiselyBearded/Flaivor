using OpenAI;
using OpenAI.Chat;
using OpenAI.Models;
using OVR.OpenVR;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class ThinkerModule : MonoBehaviour
{
    public RawImage inputImage;
    private static bool isChatPending;


    private OpenAIClient api;
    public bool preprompt;
    public PersonalityType Personality;
    private readonly List<Message> chatMessages = new List<Message>();


    public event Action<string> OnChatGPTInputReceived;


    private void Start()
    {
        api = new OpenAIClient();
    }

    /// <summary>
    /// entry point from other script to submit api request
    /// </summary>
    /// <param name="input"></param>
    public void GenerateResponse(string input) => SubmitChat(input);

    public string PrepromptQueryText(string text)
    {
        string complete = String.Empty;
        string preprompt = "Briefly and uniquely, as shortly as possible, please use no more than 4 sentences to respond to the following statement/question: ";
        complete = preprompt + text;
        return complete;
    }

    public async void SubmitChat(string userInput)
    {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        if (preprompt) userInput = PrepromptQueryText(userInput);
        var userMessage = new Message(Role.User, userInput);
        chatMessages.Add(userMessage);

        try
        {
            var chatRequest = new ChatRequest(chatMessages, Model.GPT3_5_Turbo);
            var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);
            var response = result.ToString();

            Debug.Log(response);
            OnChatGPTInputReceived?.Invoke(response);
        }
        catch (Exception e)
        {
            Debug.LogError(e);
        }
        finally
        {
            //if (lifetimeCancellationTokenSource != null) {}
            isChatPending = false;
        }
    }

    public void SubmitChatRequest()
    {
        StartChatAsync();
    }

    public Texture2D ConvertRawImageToTexture2D()
    {
        if (inputImage == null || inputImage.texture == null)
        {
            Debug.LogError("RawImage or its texture is null.");
            return null;
        }

        Texture2D texture = new Texture2D(inputImage.texture.width, inputImage.texture.height);
        texture.SetPixels(((Texture2D)inputImage.texture).GetPixels());
        texture.Apply();
        return texture;
    }

    public async void StartChatAsync()
    {
        if (api == null)
        {
            Debug.LogError("OpenAIClient is not initialized.");
            return;
        }

        var messages = new List<Message>
        {
            new Message(Role.System, "You are a helpful chef assistant."),
            new Message(Role.User, new List<Content>
            {
                "From all the food you see in this picture, what kind of foods can I make?",
                ConvertRawImageToTexture2D()
            })
        };
        var chatRequest = new ChatRequest(messages, model: Model.GPT4_Turbo);
        var result = await api.ChatEndpoint.GetCompletionAsync(chatRequest);

        Debug.Log($"{result.FirstChoice.Message.Role}: {result.FirstChoice} | Finish Reason: {result.FirstChoice.FinishDetails}");
    }
}


[System.Serializable]
public class PersonalityType
{
    public string Name;
    public string Description;

    public PersonalityType(string name, string description)
    {
        Name = name;
        Description = description;
    }
}