using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using OpenAI;
using UnityEngine.UI;

public class ResponseWindow : MonoBehaviour
{

    [SerializeField]
    private RectTransform contentArea;

    [SerializeField]
    private ScrollRect scrollView;

    public string debugString;

    public void Update() {
        if (Input.GetKeyDown(KeyCode.U)) SetResponseText(debugString);
    }

    public void SetResponseText(string text) {
        //responseText.text = text;
        var messageContent = AddNewTextMessageContent();
        messageContent.text = text;
        scrollView.verticalNormalizedPosition = 0f;
    }

    private TextMeshProUGUI AddNewTextMessageContent() {
        var textObject = new GameObject($"{contentArea.childCount + 1}");
        textObject.transform.SetParent(contentArea, false);
        var textMesh = textObject.AddComponent<TextMeshProUGUI>();
        textMesh.fontSize = 24;
#if UNITY_2023_1_OR_NEWER
            textMesh.textWrappingMode = TextWrappingModes.Normal;
#else
        textMesh.enableWordWrapping = true;
#endif
        return textMesh;
    }
}
