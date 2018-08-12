using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity.Example;

public class GameManager : MonoBehaviour {

	[Header("Introduction")]
	[SerializeField] Text titleFadeOut;

	[SerializeField] float waitForDialogueStart;

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

		//StartCoroutine(StartDialogue());
		StartCoroutine(FadeTitleOut());
		
	}

	IEnumerator FadeTitleOut()
	{
		Color visible = new Color (titleFadeOut.color.r, titleFadeOut.color.g, titleFadeOut.color.b, titleFadeOut.color.a);
		Color invisible = new Color (titleFadeOut.color.r, titleFadeOut.color.g, titleFadeOut.color.b, 0);
		float progress = 0;
		while(progress <= 1)
		{
			titleFadeOut.color = Color.Lerp (visible, invisible, progress);
			progress = progress + 0.1f;
			yield return new WaitForSeconds (0.1f);
		}
		titleFadeOut.color = new Color (titleFadeOut.color.r, titleFadeOut.color.g, titleFadeOut.color.b, 0);
		Debug.Log ("Fade Finished ");
		yield return new WaitForSeconds(waitForDialogueStart);
		dialogueRunnerScript.StartDialogue();
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
