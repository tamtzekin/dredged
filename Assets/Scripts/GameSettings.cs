using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game Settings", menuName = "Inventory/Game Settings", order = 2)]

public class GameSettings : ScriptableObject
{
	[Tooltip("Multi-cell items are only affected by other items, not by the other cells of the item.")]
	public bool scoreOnlyFromOthers = false;
	
	[Tooltip("The score for a cell is only affected by each surrounding object once, even if multiple cells for that object are nearby")]
	public bool itemsOnlyAffectCellsOnce = false;

	public int startWidth;

	public int startHeight;
}