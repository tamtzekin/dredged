using UnityEngine;
using FarrokhGames.Inventory;
using FarrokhGames.Inventory.Examples;

[RequireComponent(typeof(InventoryRenderer))]
public class LudumInventory : MonoBehaviour
{
	[SerializeField] private int _width = 8;
	[SerializeField] private int _height = 4;
	[SerializeField] private ItemDefinition[] _definitions;

	InventoryManager inventory;

	// Use this for initialization
	void Start ()
	{
		inventory = new InventoryManager(_width, _height);

		// Fill inventory with random items
		var tries = (_width * _height) / 3;
		for (var i = 0; i < tries; i++)
		{
			inventory.Add(_definitions[Random.Range(0, _definitions.Length)].CreateInstance());
		}

		// Sets the renderers's inventory to trigger drawing
		GetComponent<InventoryRenderer>().SetInventory(inventory);

		inventory.OnItemAdded += (item) =>
		{
			Evaluate();
		};
	}

	public void Evaluate()
	{
		Debug.Log("Evaluate");
		for(int w = 0; w < _width; w++)
		{
			for(int h = 0; h < _height; h++)
			{
				IInventoryItem item = inventory.GetAtPoint(new Vector2Int(w,h));
				if(item != null)
				{
					Debug.Log(w+","+h+": " + GetCellScore(w,h));
				}
			}
		}
	}

	int GetCellScore(int gridX, int gridY)
	{
		int score = 0;
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
							if(neighbourItem.Name.Contains("Lie"))
							{
								score--;
							}
							else
							{
								score++;
							}
						}
					}
				}
				else
				{// If we're outside the grid
				}
			}
		}
		return score;
	}
}
