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

    [SerializeField] private Transform avatarHead;
    Transform mainCamera;
    Transform lookAtTarget;
    [SerializeField] private float lookAtSpeed = 5f;
    [SerializeField] private float maxAngle = 45f;


    private void Start()
    {
        agentController.OnAgentStateChanged.AddListener(AgentStateChanged);

        mainCamera = Camera.main.transform;

        lookAtTarget = new GameObject().transform;
        lookAtTarget.name = "LookAtTarget";
        lookAtTarget.position = mainCamera.position;
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

    private Vector3 lastValidForward;

    private void LateUpdate()
    {
        // Calculate the position the head should look at
        Vector3 targetPosition = mainCamera.position;

        // Calculate the direction from the head to the target
        Vector3 directionToTarget = targetPosition - avatarHead.position;

        // Calculate the rotation needed to look at the target
        Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

        // Calculate the angle between the current forward direction of the head and the direction towards the target
        float angle = Quaternion.Angle(avatarHead.rotation, targetRotation);

        // If the angle exceeds the maximum neck rotation angle, limit the rotation
        if (angle > maxAngle)
        {
            targetRotation = Quaternion.RotateTowards(avatarHead.rotation, targetRotation, maxAngle);
        }

        // Apply rotation to the head
        avatarHead.rotation = targetRotation;
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
