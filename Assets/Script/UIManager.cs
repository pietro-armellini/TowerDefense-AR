using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    private GameManager gameManager; 
	public TextMeshProUGUI enemies;
	public TextMeshProUGUI roundNumber;
	public TextMeshProUGUI coins;
	public TextMeshProUGUI gameStatus;
	public Button play;

	private void Awake() {
		gameManager = GetComponent<GameManager>();
	}
    public void setEnemies(int n){
		enemies.text = n.ToString();
	}

	public void setCoins(int n){
		coins.text = n.ToString();
	}

	public void setRoundNumber(int n){
		roundNumber.text = n.ToString();
	}
	public void setGameStatus(GameManager.GameState state){
		switch(state){
			case GameManager.GameState.SelectingPlane:
				gameStatus.text = "Select the game plane";
				break;
			case GameManager.GameState.PlacingMainTower:
				gameStatus.text = "Place the main tower";
				break;
			case GameManager.GameState.Editing:
				gameStatus.text = "Editing the scene";
				break;
			case GameManager.GameState.Playing:
				gameStatus.text = "In game";
				break;
		}

	}
	public void playPressed(){
		
		gameManager.nextRound();
	}

	public void setPlayVisible(bool value){
		play.gameObject.SetActive(value);
	}

}
