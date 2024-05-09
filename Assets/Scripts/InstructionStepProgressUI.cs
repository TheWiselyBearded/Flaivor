using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InstructionStepProgressUI : MonoBehaviour
{
    public TextMeshProUGUI RecipeName;
    public TextMeshProUGUI InstructionStepTitle;
    public GameObject contentViewParent;
    public GameObject PFB_InstructionSubstepDescription;
    public List<GameObject> instructionStepUIs;

    public event Action<string> OnProgressStepReceived;

    public void SetInstructionStepUI(string InstructionStep, List<string> _instructionSteps)
    {
        InstructionStepTitle.text = InstructionStep;
        foreach (string _instruction in _instructionSteps)
        {
            Debug.Log($"Creating instruction {_instruction}");
            instructionStepUIs.Add(CreateInstructionStepUI(_instruction)); // append to ui element
        }
    }

    public GameObject CreateInstructionStepUI(string _instruction)
    {
        // Instantiate the prefab as a child of uiParent
        GameObject instructionStep = Instantiate(PFB_InstructionSubstepDescription, contentViewParent.transform);
        TextMeshProUGUI instructionStepText = instructionStep.GetComponent<TextMeshProUGUI>();
        instructionStepText.text = _instruction;

        return instructionStep;
    }

    public void InstructionStepProgressSubmit()
    {
        OnProgressStepReceived?.Invoke("step progress"); // TODO: populate with proper progressor for controllers
    }

}
