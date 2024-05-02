using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Threading.Tasks;
using OpenAI.Chat;
using OpenAI.Models;
using OpenAI;

public class GPTInterfacer : MonoBehaviour
{
    [SerializeField] private Button chatRequestButton = default!;
    public RawImage rawImage;
    public TMP_InputField inputField;

    private OpenAIClient api;

    private void Awake()
    {
        chatRequestButton.onClick.AddListener(SubmitChatRequest);
    }

    private void Start()
    {
        api = new OpenAIClient();
    }

    public void SubmitChatRequest()
    {
        StartChatAsync();
    }

    public Texture2D ConvertRawImageToTexture2D()
    {
        if (rawImage == null || rawImage.texture == null)
        {
            Debug.LogError("RawImage or its texture is null.");
            return null;
        }

        Texture2D texture = new Texture2D(rawImage.texture.width, rawImage.texture.height);
        texture.SetPixels(((Texture2D)rawImage.texture).GetPixels());
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
        inputField.text = result.FirstChoice.ToString();
    }
}
