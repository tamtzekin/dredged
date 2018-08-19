using System;
using UnityEngine;

public class MenuManager : MonoBehaviour
{
	[SerializeField] GameManager gameManager;

	public void Begin()
	{
		// hide menu
		gameObject.SetActive(false);
		gameManager.StartGame();
	}

	public void Quit()
	{
		Application.Quit();
	}
}
