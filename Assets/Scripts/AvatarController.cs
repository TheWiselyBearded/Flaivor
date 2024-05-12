using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    [SerializeField] private Animator animator;

    [SerializeField] private AgentController agentController;

    [SerializeField] private Renderer[] cutoffRenderers;
    [SerializeField] private Transform avatarDisc;

    private void Start()
    {
        agentController.OnAgentStateChanged.AddListener(AgentStateChanged);
    }

    private void Update()
    {

        Vector3 cutoffPoint = avatarDisc.position;
        Vector3 cutoffPlane = avatarDisc.up;

        foreach (var renderer in cutoffRenderers)
        {
            Material material = renderer.material;
            material.SetVector("_SectionPoint", cutoffPoint);
            material.SetVector("_SectionPlane", cutoffPlane);
        }

    }

    private void AgentStateChanged(AgentController.AgentState newState)
    {
        switch (newState)
        {
            case AgentController.AgentState.Listening:
                animator.SetTrigger("Listening");
                break;
            case AgentController.AgentState.Thinking:
                animator.SetTrigger("Thinking");
                break;
            case AgentController.AgentState.Speaking:
                animator.SetTrigger("Speaking");
                break;
        }
    }
}
