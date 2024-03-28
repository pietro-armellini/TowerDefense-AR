using System.Collections.Generic;
using UnityEngine;

public class PlacementSystem : MonoBehaviour
{
	[SerializeField]
	public GameManager gameManager;

	//prefabs of the tower and their upgrades
	public GameObject MainTowerPrefab_1, MainTowerPrefab_2, MainTowerPrefab_3;
	public GameObject DefensiveTowerPrefab_1, DefensiveTowerPrefab_2, DefensiveTowerPrefab_3;

	//reference to the main tower gameobject in the scene
	public GameObject mainTower;
	//references to the defensive tower gameobjects in the scene
	public List<GameObject> defensiveTowers = new List<GameObject>();

    private void Awake() {
		gameManager = GetComponent<GameManager>();
	}

	//this method is used by the imput manager, it place the main tower where the player touches
	public void placeMainTower(Pose pose){
		this.mainTower = Instantiate(MainTowerPrefab_1, pose.position, pose.rotation);
		setUpTower(mainTower);
		//start the game
		gameManager.startPlaying();
		gameManager.soundManager.playClip(3);
	}

	//this method is used by the imput manager, it place a defensive tower where the player touches
	public void placeDefensiveTower(Pose pose){
		if(gameManager.placeDefensiveTower()){
			this.defensiveTowers.Add(Instantiate(DefensiveTowerPrefab_1, pose.position, pose.rotation));
			setUpTower(defensiveTowers[defensiveTowers.Count-1]);
			gameManager.soundManager.playClip(3);
		}
	}

	//this method setup a new tower after pressed
	private void setUpTower(GameObject tower){
		TowerManager towerManager = tower.GetComponent<TowerManager>();
		towerManager.setUp(this);

		//setting up all the defenders of the tower
		foreach(GameObject defender in towerManager.defenders){
			DefenderScript defenderScript = defender.GetComponent<DefenderScript>();
			defenderScript.setUp(gameManager.enemyManager);
		}
	}

	//when a tower gets destroyed it has to be removed
	public void removeTower(GameObject tower){
		//if the tower is a defensive tower remove and destroy it
		if(defensiveTowers.Contains(tower)){
			this.defensiveTowers.Remove(tower);
			GameObject.Destroy(tower);
		}
		//if the tower is the main one the game is over
		else{
			gameManager.gameOver();
		}
	}

	//method used by the towers when the play upgrades them (press on upgrade button)
	public bool upgradeTower(GameObject tower){
		//call two different methods depending on the type of tower
		if(gameManager.upgrade()){
			if (tower==mainTower){
				return upgradeMainTower(tower);
			}
			else if (defensiveTowers.Contains(tower)){
				return upgradeDefensiveTower(tower);
			}
		}
		return false;
	}

	//method to upgrade a defensive tower
	private bool upgradeDefensiveTower(GameObject tower){
		TowerManager towerManager = tower.GetComponent<TowerManager>();
		GameObject tmpPrefab;
		//select the right prefab depending on the tower level
		if(towerManager.level==1){
			tmpPrefab = DefensiveTowerPrefab_2;
		}
		else{
			tmpPrefab = DefensiveTowerPrefab_3;
		}

		//create the new tower
		GameObject newTower = Instantiate(tmpPrefab, tower.transform.position, tower.transform.rotation);
		setUpTower(newTower);
		//remove the old one
		defensiveTowers.Remove(tower);
		Destroy(tower);
		defensiveTowers.Add(newTower);
		return true;
	}

	//method to upgrade the main tower
	private bool upgradeMainTower(GameObject tower) {
		TowerManager towerManager = tower.GetComponent<TowerManager>();
		GameObject tmpPrefab;
		//select the right prefab depending on the tower level
		if(towerManager.level==1){
			tmpPrefab = MainTowerPrefab_2;
		}
		else{
			tmpPrefab = MainTowerPrefab_3;
		}
		
		//create the new tower
		GameObject newTower = Instantiate(tmpPrefab, tower.transform.position, tower.transform.rotation);
		mainTower = newTower;
		setUpTower(newTower);
		//remove the old one
		Destroy(tower);
		return true;
	}
	//method used by the defenders for spotting the closer tower to attack
	//standard algorithm for finding a minimum value in a list using the Distance between the enemy and the tower as value
	//and the tower as gameobject to find and return
	public GameObject getCloserTower(Transform enemyTransform){
		GameObject closerTower = mainTower;
		float min = Vector3.Distance(mainTower.transform.position, enemyTransform.position);

		foreach(GameObject tower in defensiveTowers){
			if(Vector3.Distance(tower.transform.position, enemyTransform.position)<min){
				min = Vector3.Distance(tower.transform.position, enemyTransform.position);
				closerTower = tower;
			}
		}
		return closerTower;
	}

	//method for disabling the tower interactables (UI floating above the tower you can interact with)
	public void deactivateInteractables(){
		foreach(GameObject tower in defensiveTowers){
			tower.GetComponent<TowerManager>().deactivateInteractables();
		}
		mainTower.GetComponent<TowerManager>().deactivateInteractables();
	}
}
