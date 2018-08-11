using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FarrokhGames.Inventory;

[CreateAssetMenu(fileName = "Item", menuName = "Inventory/Thought Item", order = 1)]
public class ThoughtItem : ScriptableObject, IInventoryItem
{
	[SerializeField] private Sprite _sprite;
	[SerializeField] private InventoryShape _shape;
	
	public string Name { get { return this.name; } }
	public Sprite Sprite { get { return _sprite; } }
	public InventoryShape Shape { get { return _shape; } }
	
	public int score;
	
	/// <summary>
	/// Creates a copy if this scriptable object
	/// </summary>
	public IInventoryItem CreateInstance()
	{
		return ScriptableObject.Instantiate(this);
	}
}
