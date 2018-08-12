using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using FarrokhGames.Shared;
using Yarn.Unity;

[RequireComponent(typeof(InventoryRenderer))]
public class LudumInventory : MonoBehaviour
{
	[SerializeField] private ThoughtItem[] _definitions;
	[SerializeField] private Font font;

	[Header("Gameplay Options")]
	public GameSettings gameSettings;

	InventoryManager inventory;
	InventoryController controller;

	Text[] gridText;

	[SerializeField] Text totalScoreText;

	public int[,] currentScores;

	private Pool<Text> _textPool;

	public Heatmap heatmap;
	public bool empty { get { return inventory.AllItems.Count == 0 && !controller.dragging;} } 

	// Use this for initialization
	void Start ()
	{
		inventory = new InventoryManager(gameSettings.startWidth, gameSettings.startHeight);

		// initalise scores
		currentScores = new int[inventory.Width, inventory.Height];

		// Fill inventory with random items
		/*
		var tries = (inventory.Width * inventory.Height) / 3;
		for (var i = 0; i < tries; i++)
		{
			inventory.Add(_definitions[Random.Range(0, _definitions.Length)].CreateInstance());
		}
		*/

		controller = GetComponent<InventoryController>();
		// Sets the renderers's inventory to trigger drawing
		InventoryRenderer inventoryRenderer = GetComponent<InventoryRenderer>();
		GetComponent<InventoryRenderer>().SetInventory(inventory);

		var textContainer = new GameObject("Text Pool").AddComponent<RectTransform>();
		textContainer.transform.SetParent(transform);
		textContainer.transform.localPosition = Vector3.zero;
		textContainer.transform.localScale = Vector3.one;

		// Create pool of images
		_textPool = new Pool<Text>(
			delegate
			{
			var text = new GameObject("Text").AddComponent<Text>();
			text.transform.SetParent(textContainer);
			text.transform.localScale = Vector3.one;
			return text;
		},
		0,
		true);

		Vector2 CellSize = inventoryRenderer.CellSize;

		// Render new grid
		var containerSize = new Vector2(CellSize.x * inventory.Width, CellSize.y * inventory.Height);
		var topLeft = new Vector3(-containerSize.x / 2, -containerSize.y / 2, 0); // Calculate topleft corner
		var halfCellSize = new Vector3(CellSize.x / 2, CellSize.y / 2, 0); // Calulcate cells half-size

		// Spawn grid images
		gridText = new Text[inventory.Width * inventory.Height];
		var c = 0;
		for (int y = 0; y < inventory.Height; y++)
		{
			for (int x = 0; x < inventory.Width; x++)
			{
				Text text = CreateText();
				text.text = "0";
				text.color = Color.white;
				text.font = font;
				text.alignment = TextAnchor.MiddleCenter;
				text.gameObject.name = "text " + c;
				text.rectTransform.SetAsFirstSibling();
				text.rectTransform.localPosition = topLeft + new Vector3(CellSize.x * ((inventory.Width - 1) - x), CellSize.y * y, 0) + halfCellSize;
				text.rectTransform.sizeDelta = CellSize;
				gridText[c] = text;
				c++;
			}
		}

		Evaluate();
		inventory.OnItemAdded += (item) =>
		{
			ThoughtItem thoughtItem = (ThoughtItem) item;
			GetComponent<AudioSource>().PlayOneShot(thoughtItem.dropSound);
			Evaluate();
		};
	}

	/*
         * Create an image with given sprite and settings
         */
	private Text CreateText()
	{
		var text = _textPool.Take();
		text.gameObject.SetActive(true);
		text.transform.SetAsLastSibling();
		return text;
	}

	public void Evaluate()
	{
		//Debug.Log("Evaluate");
		int total = 0;
		for(int w = 0; w < inventory.Width; w++)
		{
			for(int h = 0; h < inventory.Height; h++)
			{
				int currentCellScore = GetCellScore(w,h);
				total+= currentCellScore;
				currentScores[w,h] = currentCellScore;
				var index = h * inventory.Width + ((inventory.Width - 1) - w);
				gridText[index].text = currentScores[w,h].ToString();
			}
		}

		if (totalScoreText){
			totalScoreText.text = total.ToString();
		}
		
		if (heatmap){

			heatmap.heatmapRefresh = true;
		}


	}

	int GetCellScore(int gridX, int gridY)
	{
		HashSet<ThoughtItem> neighbourItems = new HashSet<ThoughtItem>();

		int score = 0;
		IInventoryItem item = inventory.GetAtPoint(new Vector2Int(gridX,gridY));
		for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX ++)
		{
			for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY ++)
			{
				if (neighbourX >= 0 && neighbourX < inventory.Width && neighbourY >= 0 && neighbourY < inventory.Height)
				{// If we're inside the grid
					if (neighbourX != gridX || neighbourY != gridY)
					{// If we're not the original cell
						IInventoryItem neighbourItem = inventory.GetAtPoint(new Vector2Int(neighbourX,neighbourY));
						if(neighbourItem != null)
						{
							ThoughtItem thoughtItem = (ThoughtItem) neighbourItem;
							if(gameSettings.itemsOnlyAffectCellsOnce == false)
							{
								if(gameSettings.scoreOnlyFromOthers != true || (gameSettings.scoreOnlyFromOthers == true && item != neighbourItem))
								{
									score = score + thoughtItem.score;
								}
							}
							else
							{
								neighbourItems.Add(thoughtItem);
							}
						}
					}
				}
				else
				{// If we're outside the grid
				}
			}
		}

		if(gameSettings.itemsOnlyAffectCellsOnce == true)
		{
			Debug.Log(gridX + ", " + gridY + " affected by " + neighbourItems.Count);
			foreach(ThoughtItem thoughtItem in neighbourItems)
			{
				score = score + thoughtItem.score;
			}
		}
		return score;
	}

	[YarnCommand("addItem")]
	public void AddItem(string itemName)
	{
		foreach(ThoughtItem item in _definitions)
		{
			if(item.Name == itemName)
			{
				inventory.Add(item.CreateInstance());
			}
		}
	}
}
