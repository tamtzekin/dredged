using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Yarn.Unity;
using Yarn.Unity.Example;
using FarrokhGames.Inventory;

public class GameManager : MonoBehaviour {

	[Header("Introduction")]
	[SerializeField] Text titleFadeOut;

	[SerializeField] float waitForDialogueStart;

	Slider timerSlider;

	[Header("Timer")]
	[SerializeField] int maxTime;

	Coroutine timerCoroutine = null;

	[Header("Gameplay")]
	[SerializeField] int maxScore;

	GameObject dialogueObject;
	Yarn.Unity.DialogueRunner dialogueRunnerScript;
	LudumDialogueUI ludumDialogueUI;

	LudumInventory inventoryScript;

	LudumInventory dispenserInventory;
	// Use this for initialization
	void Awake () {
		dialogueObject = GameObject.Find("Dialogue");
		dialogueRunnerScript = dialogueObject.GetComponent<Yarn.Unity.DialogueRunner>();
		ludumDialogueUI = (LudumDialogueUI) dialogueRunnerScript.dialogueUI;

		inventoryScript = GameObject.Find("Inventory UI").GetComponent<LudumInventory>();
		inventoryScript.OnScoreUpdate+= EvaluateScore;

		dispenserInventory = GameObject.Find("DispenserInventory").GetComponent<LudumInventory>();

		timerSlider = GameObject.Find("TimerSlider").GetComponent<Slider>();
		timerSlider.gameObject.SetActive(false);
		timerSlider.maxValue = maxTime;
		//StartCoroutine(StartDialogue());
	}

	public void StartGame()
	{
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

	IEnumerator RunTimer()
	{
		Debug.Log ("Running timer");
		timerSlider.value = timerSlider.maxValue;
		while(timerSlider.value > 0)
		{
			yield return new WaitForSeconds (0.25f);
			timerSlider.value -= 0.25f;
		}
		inventoryScript.AddItem("Lie_No");
		timerSlider.gameObject.SetActive(false);
		timerCoroutine = null;
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

	public void EvaluateScore(int newScore)
	{
		Debug.Log ("Score updated to " + newScore);
		if(newScore > maxScore)
		{
			Debug.Log ("Game Over");
			dialogueRunnerScript.startNode = "GameOver";
			dialogueRunnerScript.StartDialogue();
		}
	}

	[YarnCommand("showInventories")]
	public void ShowInventories()
	{
		inventoryScript.gameObject.SetActive(true);
		dispenserInventory.inventory.OnItemAdded += (item) =>
		{
			if(timerCoroutine == null)
			{
				timerSlider.gameObject.SetActive(true);
				timerCoroutine = StartCoroutine(RunTimer ());
			}
		};
		inventoryScript.inventory.OnItemAdded += (item) =>
		{
			timerSlider.value = timerSlider.maxValue;
			if(dispenserInventory.inventory.AllItems.Count == 0 &&
			   (dispenserInventory.controller.itemBeingDragged == null ||
			   dispenserInventory.controller.itemBeingDragged == item))
			{
				timerSlider.gameObject.SetActive(false);
				if(timerCoroutine != null)
				{
					StopCoroutine(timerCoroutine);
					timerCoroutine = null;
				}
				dispenserInventory.gameObject.SetActive(false);
			}
		};
	}

	[YarnCommand("addItem")]
	public void AddItem(string itemName)
	{
		dispenserInventory.gameObject.SetActive(true);
		IInventoryItem newItem = dispenserInventory.AddItem(itemName);
		if(!inventoryScript.inventory.CanAdd(newItem))
		{
			Debug.Log ("Outta room!!!");
			dialogueRunnerScript.startNode = "GameOver";
			dialogueRunnerScript.StartDialogue();
		}
	}

	[YarnCommand("GameOver")]
	public void GameOver()
	{
		SceneManager.LoadScene(0);
	}
}
