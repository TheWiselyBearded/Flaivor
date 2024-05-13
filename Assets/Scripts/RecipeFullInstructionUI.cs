using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RecipeFullInstructionUI : MonoBehaviour
{
    public GameObject contentViewParent;
    public GameObject PFB_InstructionStep;
    public TextMeshProUGUI recipeName;
    public TextMeshProUGUI ingedients;
    public List<InstructionUI> instructionUIs;

    private void Start() {
        instructionUIs = new List<InstructionUI>();
    }

    public void SetRecipeInfo(string _recipeName, string _ingedientDescription) {
        recipeName.text = _recipeName;
        ingedients.text = _ingedientDescription;
    }

    public void SetInstructionsUI(List<Instruction> _instructions) {
        foreach (Instruction _instruction in _instructions) {
            Debug.Log($"Creating instruction {_instruction}");
            instructionUIs.Add(CreateInstructionStep(_instruction)); // append to ui element
        }
    }

    public InstructionUI CreateInstructionStep(Instruction _instruction) {
        // Instantiate the prefab as a child of uiParent
        GameObject instructionStep = Instantiate(PFB_InstructionStep, contentViewParent.transform);
        InstructionStepUI instructionStepUI = instructionStep.GetComponent<InstructionStepUI>();
        InstructionUI instructionUI = new InstructionUI(_instruction.StepNumber.ToString() + " " + _instruction.Description, instructionStepUI.instructionName, _instruction.SubSteps, instructionStepUI.instructionDescription);

        return instructionUI;
    }
}
