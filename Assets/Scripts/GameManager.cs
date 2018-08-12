using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity.Example;

public class GameManager : MonoBehaviour {
	GameObject dialogueObject;
	Yarn.Unity.DialogueRunner dialogueRunnerScript;
	LudumDialogueUI ludumDialogueUI;

	LudumInventory inventoryScript;
	// Use this for initialization
	void Awake () {
		dialogueObject = GameObject.Find("Dialogue");
		dialogueRunnerScript = dialogueObject.GetComponent<Yarn.Unity.DialogueRunner>();
		ludumDialogueUI = (LudumDialogueUI) dialogueRunnerScript.dialogueUI;

		inventoryScript = GameObject.Find("Inventory UI").GetComponent<LudumInventory>();

		StartCoroutine(StartDialogue());
		
	}

	IEnumerator StartDialogue(){

		// Have to wait for dialogue runner to finish it's starter method
		yield return new WaitForSeconds(0.1f);
		dialogueRunnerScript.StartDialogue();
		yield return null;

	}

	void Update()
	{
		if(ludumDialogueUI.optionButtons[0].IsActive())
		{
			if(Input.GetKeyDown(KeyCode.Alpha1))
			{
				ludumDialogueUI.optionButtons[0].onClick.Invoke();
			}
			if(ludumDialogueUI.optionButtons[1].IsActive())
			{
				if(Input.GetKeyDown(KeyCode.Alpha2))
				{
					ludumDialogueUI.optionButtons[1].onClick.Invoke();
				}
			}
		}
	}
}
