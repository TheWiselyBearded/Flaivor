using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using DG.Tweening;

public class LoadingPanel : MonoBehaviour
{
    public string[] cookingFax;

    [SerializeField] private float factTime = 10f;
    [SerializeField] private TMP_Text factText;

    [SerializeField] private AgentController agentController;

    [SerializeField] private GameObject panelObj;


    private void Awake()
    {
        panelObj.SetActive(false);

        StartCoroutine(SwapFax());

        agentController.OnAgentStateChanged.AddListener(OnAgentStateChanged);
    }

    private void OnAgentStateChanged(AgentController.AgentState newState)
    {
        if (agentController.agentState == AgentController.AgentState.Thinking && agentController.thinkingMode == AgentController.ThinkingMode.Recipes)
        {
            panelObj.SetActive(true);
        }
        else
        {
            panelObj.SetActive(false);
        }
    }

    private IEnumerator SwapFax()
    {
        while (true)
        {
            for (int i = 0; i < cookingFax.Length; i++)
            {
                yield return new WaitForSeconds(factTime);
                factText.text = "<b>Did you know?</b>\n" + cookingFax[i];
            }
        }
    }
}
