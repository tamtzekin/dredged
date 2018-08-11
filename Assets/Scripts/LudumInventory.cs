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
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (Input.GetKeyDown(KeyCode.Space))
		{
			for(int w = 0; w < _width; w++)
			{
				for(int h = 0; h < _height; h++)
				{
					IInventoryItem item = inventory.GetAtPoint(new Vector2Int(w,h));
					if(item != null)
					{
						Debug.Log(w+","+h+": " + item.Name);
					}
				}
			}
		}
	}
}
