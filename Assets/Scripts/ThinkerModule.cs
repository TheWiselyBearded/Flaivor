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
            var chatRequest = new ChatRequest(chatMessages, Model.GPT3_5_Turbo_16K);
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

    public async void SubmitChatJSON(string userInput)
    {
        if (isChatPending || string.IsNullOrWhiteSpace(userInput)) { return; }
        isChatPending = true;

        var messages = new List<Message>
        {
            new Message(Role.System, "I will give you a photo or list of ingredients and you will ALWAYS generate 3 recipes based on the given ingredients. For each recipe please include a Title, a short description of the dish, list of all ingredients, and a detailed list of instructions for making the dish. The instructions for the dish should be separated into numbered sections with smaller detailed steps for the section underneath. All of this should be formatted as a JSON file. make sure the code is complete and includes 3 recipes.\n\nExample output:\n[\n{\n  \"recipe_name\": \"Spaghetti and Meatballs\",\n  \"ingredients\": {\n    \"ground_beef\": \"1 pound\",\n    \"breadcrumbs\": \"1/2 cup\",\n    \"grated_Parmesan_cheese\": \"1/4 cup\",\n    \"fresh_parsley\": \"1/4 cup\",\n    \"garlic_cloves\": \"6\",\n    \"large_egg\": \"1\",\n    \"salt\": \"1 teaspoon\",\n    \"black_pepper\": \"1/2 teaspoon\",\n    \"dried_oregano\": \"1 1/4 teaspoons\",\n    \"dried_basil\": \"3/4 teaspoon\",\n    \"olive_oil\": \"1 tablespoon + for frying\",\n    \"small_onion\": \"1\",\n    \"crushed_tomatoes\": \"1 (28-ounce) can\",\n    \"sugar\": \"pinch (optional)\",\n    \"spaghetti\": \"12 ounces\",\n    \"grated_Parmesan_cheese_garnish\": \"for garnish\",\n    \"chopped_fresh_basil_or_parsley_garnish\": \"for garnish\"\n  },\n  \"instructions\": [\n    {\n      \"step_number\": 1,\n      \"description\": \"Prepare the Meatballs:\",\n      \"sub_steps\": [\n        \"In a large mixing bowl, combine the ground beef, breadcrumbs, grated Parmesan cheese, minced parsley, minced garlic, egg, salt, black pepper, dried oregano, and dried basil. Mix until well combined.\",\n        \"Shape the mixture into meatballs, about 1 to 1.5 inches in diameter.\",\n        \"Heat olive oil in a large skillet over medium heat. Cook the meatballs in batches until browned on all sides and cooked through, about 8-10 minutes. Transfer cooked meatballs to a plate and set aside.\"\n      ]\n    },\n    ...\n  ]\n},\n...\n]"),
            new Message(Role.User, "Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk"),
        };

        foreach(var message in messages) chatMessages.Add(message);
        Debug.Log("Added all messages");

        try
        {
            // example query: I have the following ingredients with me, Eggs, spices, green onions, steak, potatoes, tortilla, bread, sugar, and milk
            //var chatRequest = new ChatRequest(chatMessages, Model.GPT4_Turbo, responseFormat: ChatResponseFormat.Json, temperature: 1, maxTokens: 14421, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
            var chatRequest = new ChatRequest(chatMessages, Model.GPT4_Turbo, responseFormat: ChatResponseFormat.Json, temperature: 1, topP: 1, frequencyPenalty: 0, presencePenalty: 0);
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