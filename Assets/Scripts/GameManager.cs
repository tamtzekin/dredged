using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {
	GameObject dialogueObject;
	Yarn.Unity.DialogueRunner dialogueRunnerScript;

	LudumInventory inventoryScript;
	// Use this for initialization
	void Awake () {
		dialogueObject = GameObject.Find("Dialogue");
		dialogueRunnerScript = dialogueObject.GetComponent<Yarn.Unity.DialogueRunner>();

		inventoryScript = GameObject.Find("Inventory UI").GetComponent<LudumInventory>();

		StartCoroutine(StartDialogue());
		
	}

	IEnumerator StartDialogue(){

		// Have to wait for dialogue runner to finish it's starter method
		yield return new WaitForSeconds(0.1f);
		dialogueRunnerScript.StartDialogue();
		yield return null;

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
