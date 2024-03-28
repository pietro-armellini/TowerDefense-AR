using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	[SerializeField]
	public EnemyManager enemyManager;
	public UIManager uIManager;
	public SoundManager soundManager;

	//Enumerator class for managing the game status
	public enum GameState {SelectingPlane, PlacingMainTower, Playing, Editing};
	private ARPlane gamePlane = null;
	public GameState gameState;

	public NavMeshSurface navMeshSurface;
	public int coins = 0;
	public int round = 1;

	private void Start() {
		//setup the UI
		changeGameStatus(GameState.SelectingPlane);
		uIManager.setCoins(coins);
		uIManager.setRoundNumber(round);

	}

	private void Awake(){
		enemyManager = GetComponent<EnemyManager>();
		uIManager = GetComponent<UIManager>();
		soundManager = GetComponent<SoundManager>();
	}

	//method called once the player has selected the plane
	public void setGamePlane(ARPlane gamePlane){
		this.gamePlane=gamePlane;
		changeGameStatus(GameState.PlacingMainTower);
		navMeshSurface = gamePlane.GetComponent<NavMeshSurface>();
	}
	//gameplane getter
	public ARPlane getGamePlane(){
		return gamePlane;
	}
	
	//when the plane is selected and the main tower it's place this method gets triggered
	public void startPlaying(){
		changeGameStatus(GameState.Playing);
		navMeshSurface.BuildNavMesh();
		//set up and launch the first round
		enemyManager.setUp();
		enemyManager.round(this.round);
		//update UI
		uIManager.setRoundNumber(round);
		//play start sound
		soundManager.playClip(2);
	}

	//to be called when "next round" button gets pressed
	public void nextRound(){
		//start next round
		enemyManager.round(this.round);
		//disable play button
		uIManager.setPlayVisible(false);
		changeGameStatus(GameState.Playing);
		soundManager.playClip(2);
	}

	//method used by the placingSystem to check and remove the coins 
	//return true if there are enough coins to build a new tower false otherwise
	public bool placeDefensiveTower(){
		if (coins>=100){
			setCoins(coins-100);
			return true;
		}
		else{
			return false;
		}
	}

	//same as placeDefensiveTower()
	public bool repair(){
		if(coins>=100){
			setCoins(coins-100);
			return true;
		}
		return false;
	}

	//same as placeDefensiveTower()
	public bool upgrade(){
		if(coins>=200){
			setCoins(coins-200);
			return true;
		}
		return false;
	}

	//to be called once all enemies are dead
	public void roundFinished(){
		changeGameStatus(GameState.Editing);
		round++;
		//activate the play button
		uIManager.setPlayVisible(true);
		uIManager.setRoundNumber(round);
	}

	public void gameOver(){
		SceneManager.LoadScene(0);
	}

	//to update in only one place the coins UI
	public void setCoins(int coins){
		uIManager.setCoins(coins);
		this.coins = coins;
	}

	//to update in only one place the GameStatus UI
	public void changeGameStatus(GameState state){
		this.gameState = state;
		uIManager.setGameStatus(state);
	}

}
