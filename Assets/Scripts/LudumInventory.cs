using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;
using FarrokhGames.Shared;

[RequireComponent(typeof(InventoryRenderer))]
public class LudumInventory : MonoBehaviour
{
	[SerializeField] private int _width = 8;
	[SerializeField] private int _height = 4;
	[SerializeField] private ThoughtItem[] _definitions;
	[SerializeField] private Font font;

	[Header("Gameplay Options")]

	[Tooltip("Multi-cell items are only affected by other items, not by the other cells of the item.")]
	[SerializeField] private bool scoreOnlyFromOthers = false;

	[Tooltip("The score for a cell is only affected by each surrounding object once, even if multiple cells for that object are nearby")]
	[SerializeField] private bool itemsOnlyAffectCellsOnce = false;

	InventoryManager inventory;

	Text[] gridText;

	[SerializeField] int[,] currentScores;

	private Pool<Text> _textPool;

	// Use this for initialization
	void Start ()
	{
		inventory = new InventoryManager(_width, _height);

		// initalise scores
		currentScores = new int[_width,_height];

		// Fill inventory with random items
		var tries = (_width * _height) / 3;
		for (var i = 0; i < tries; i++)
		{
			inventory.Add(_definitions[Random.Range(0, _definitions.Length)].CreateInstance());
		}

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
		Debug.Log("Evaluate");
		for(int w = 0; w < _width; w++)
		{
			for(int h = 0; h < _height; h++)
			{
				currentScores[w,h] = GetCellScore(w,h);
				var index = h * inventory.Width + ((inventory.Width - 1) - w);
				gridText[index].text = currentScores[w,h].ToString();
			}
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
				if (neighbourX >= 0 && neighbourX < _width && neighbourY >= 0 && neighbourY < _height)
				{// If we're inside the grid
					if (neighbourX != gridX || neighbourY != gridY)
					{// If we're not the original cell
						IInventoryItem neighbourItem = inventory.GetAtPoint(new Vector2Int(neighbourX,neighbourY));
						if(neighbourItem != null)
						{
							ThoughtItem thoughtItem = (ThoughtItem) neighbourItem;
							if(itemsOnlyAffectCellsOnce == false)
							{
								if(scoreOnlyFromOthers != true || (scoreOnlyFromOthers == true && item != thoughtItem))
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

		if(itemsOnlyAffectCellsOnce == true)
		{
			Debug.Log(gridX + ", " + gridY + " affected by " + neighbourItems.Count);
			foreach(ThoughtItem thoughtItem in neighbourItems)
			{
				score = score + thoughtItem.score;
			}
		}
		return score;
	}
}
