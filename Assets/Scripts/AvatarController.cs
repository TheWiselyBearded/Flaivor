using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AvatarController : MonoBehaviour
{
    private Animator animator;

    [SerializeField] private AgentController agentController;


    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        agentController.OnAgentStateChanged.AddListener(AgentStateChanged);
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
