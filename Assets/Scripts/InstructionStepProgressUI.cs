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


    public static event Action<string> OnProgressStepReceived;

    /// <summary>
    /// Creates all substeps that are part of the main ui canvas
    /// </summary>
    /// <param name="InstructionStep"></param>
    /// <param name="_instructionSteps"></param>
    public void SetInstructionStepUI(string InstructionStep, List<string> _instructionSteps)
    {
        // clear out existing substep info if present
        ClearOutPreviousStep();

        InstructionStepTitle.text = InstructionStep;
        foreach (string _instruction in _instructionSteps)
        {
            Debug.Log($"Creating instruction {_instruction}");
            instructionStepUIs.Add(CreateInstructionStepUI(_instruction)); // append to ui element
        }
    }

    public void SetInstructionSubstep(string stepDescription, string substepDescription) {
        Debug.Log($"Step Description {stepDescription}, Substep Description {substepDescription}");
        contentViewParent.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = stepDescription;
        contentViewParent.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = substepDescription;
        //Debug.Log("Setting next substep");
    }

    protected void ClearOutPreviousStep() {
        // Iterate over each child object of the parentObject
        foreach (Transform substepObject in contentViewParent.transform) {
            // Access the child GameObject
            GameObject childObject = substepObject.gameObject;
            Destroy(childObject);
        }
    }

    protected GameObject CreateInstructionStepUI(string _instruction)
    {
        // Instantiate the prefab as a child of uiParent
        GameObject instructionStep = Instantiate(PFB_InstructionSubstepDescription, contentViewParent.transform);
        // set step title
        // set substep description
        TextMeshProUGUI instructionStepText = instructionStep.transform.GetChild(1).GetComponent<TextMeshProUGUI>();
        instructionStepText.text = _instruction;

        return instructionStep;
    }

    public void InstructionStepProgressSubmit()
    {
        OnProgressStepReceived?.Invoke("step progress"); // TODO: populate with proper progressor for controllers
    }

}
