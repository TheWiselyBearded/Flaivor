using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeFullUI : MonoBehaviour
{
    public GameObject contentViewParent;
    public GameObject PFB_InstructionStep;
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI recipeDescription;
    public TextMeshProUGUI ingredientDescription;
    public List<InstructionUI> instructionUIs;

    public RawImage recipeImg;

    public event Action<string> OnChooseDishFullReceived;

    private void Start()
    {
        instructionUIs = new List<InstructionUI>();
    }

    public void SetRecipeImg(RawImage rawImg) { recipeImg.texture = rawImg.texture; }

    /// <summary>
    /// wired via unity gui
    /// </summary>
    public void ChooseDishButtonPress()
    {
        OnChooseDishFullReceived?.Invoke(recipeName.text);
    }

    public void SetRecipeUI(string _recipeName, string _recipeDescription, string _ingredientDescription)
    {
        recipeName.text = _recipeName;
        recipeDescription.text = _recipeDescription;
        ingredientDescription.text = _ingredientDescription;
    }

    public void SetInstructionsUI(List<Instruction> _instructions)
    {
        foreach (Instruction _instruction in _instructions)
        {
            Debug.Log($"Creating instruction {_instruction}");            
            instructionUIs.Add(CreateInstructionStep(_instruction)); // append to ui element
        }
    }

    public InstructionUI CreateInstructionStep(Instruction _instruction)
    {
        // Instantiate the prefab as a child of uiParent
        GameObject instructionStep = Instantiate(PFB_InstructionStep, contentViewParent.transform);
        InstructionStepUI instructionStepUI = instructionStep.GetComponent<InstructionStepUI>();
        InstructionUI instructionUI = new InstructionUI(_instruction.StepNumber.ToString() + _instruction.Description,  instructionStepUI.instructionName, _instruction.SubSteps, instructionStepUI.instructionDescription);

        return instructionUI;
    }
}

[System.Serializable]
public class InstructionUI
{
    public TextMeshProUGUI instructionName;
    public TextMeshProUGUI instructionDescription;

    public InstructionUI(TextMeshProUGUI instructionName, TextMeshProUGUI instructionDescription)
    {
        this.instructionName = instructionName;
        this.instructionDescription = instructionDescription;
    }

    public InstructionUI(string _instructionName, TextMeshProUGUI gui_InstructionName, List<string> _instructionDescription, TextMeshProUGUI gui_instructionDescription)
    {
        instructionName = gui_InstructionName;
        instructionDescription = gui_instructionDescription;
        instructionName.text = _instructionName;
        instructionDescription.text = string.Join("\n", _instructionDescription);
    }

    public InstructionUI(string instructionName, List<string> instructionSubsteps)
    {
        Debug.LogFormat($"Instruction name {instructionName} and substep count {instructionSubsteps.Count}");
        //this.instructionName.text = instructionName;
        //this.instructionDescription.text = string.Join("\n", instructionSubsteps);
    }

    public InstructionUI(TextMeshProUGUI instructionName, List<string> instructionSubsteps)
    {
        Debug.LogFormat($"Instruction name {instructionName} and substep count {instructionSubsteps.Count}");
        this.instructionName = instructionName;
        this.instructionDescription.text = string.Join("\n", instructionSubsteps);
    }
}
