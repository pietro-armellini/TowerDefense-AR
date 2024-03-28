using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARFoundation.VisualScripting;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

public class EnemyManager : MonoBehaviour
{
	[SerializeField] 
	public GameManager gameManager;
	private PlacementSystem placementSystem;
	//the prefab of the enemy to spawn
	public GameObject enemyPrefab;
	//List of the spawning points on the place
	private List<Vector3> spawningPoints = new List<Vector3>();
	public ARPlane supportPlane;
	//List of the alive enemies present on the scene
	private List<GameObject> enemiesInstances = new List<GameObject>();
	private int remaingEnemies;
	//how long to wait between spawning enemies
	public int spawningInterval = 1;

	//difficulty level
	public int difficulty = 10;

	private void Awake() {
		gameManager = GetComponent<GameManager>();
		placementSystem = GetComponent<PlacementSystem>();
	}

	//this method is needed to identify the spawning points of the enemies
	//the points will be the points on the line renderer of the game plane (LineRenderer.GetPositions())
	public void setUp(){
		ARPlane gamePlane = gameManager.getGamePlane();
		Vector3[] tmp = new Vector3[gamePlane.GetComponent<LineRenderer>().positionCount];
		
		gamePlane.GetComponent<LineRenderer>().GetPositions(tmp);
		//move the support plane to the same y of the gameplane
		supportPlane.transform.position = new Vector3(supportPlane.transform.position.x,gamePlane.transform.position.y,supportPlane.transform.position.z);
		
		//transform local coordinated to world space coordinates
		foreach(Vector3 vector in tmp){
			float x = gameManager.getGamePlane().transform.position.x;
			float y = gameManager.getGamePlane().transform.position.y;
			float z = gameManager.getGamePlane().transform.position.z;
			Vector3 v = new Vector3(x+vector.x,gameManager.navMeshSurface.transform.position.y,z+vector.z);
			spawningPoints.Add(v);
		}
	}

	//method called for beginning a round
	//n is the round number
	public void round(int n){
		//setting the amount of enemies
		int enemyNumbers = n*difficulty;

		//Call the coroutine to generate the enemies
		//spawningInterval is how long to wait between spawing enemies
		StartCoroutine(enemiesGenerator(enemyNumbers, spawningInterval));
		remaingEnemies = enemyNumbers;
		gameManager.uIManager.setEnemies(remaingEnemies);

		//Set active false for the interactables UI (repair and upgrade button above the towers)
		placementSystem.deactivateInteractables();
	}
	
	IEnumerator enemiesGenerator(int enemyNumbers, float interval)
    {
		//choosing random the spawning position in the List
		System.Random rnd = new System.Random();
		for(int j = 0; j < enemyNumbers; j++)
		{
			//random number between the List indexes
			int pos = rnd.Next(0, spawningPoints.Count);
			//spawn the enemy
			GameObject obj = Instantiate(enemyPrefab, spawningPoints[pos], Quaternion.identity);	
			EnemyController enemy =  obj.GetComponent<EnemyController>();
			NavMeshAgent agent = obj.GetComponent<NavMeshAgent>();
			//set up the enemy and the navmeshagent
			enemy.setUp(this);
			agent.enabled = true;
			//add the enemy to the enemieslist
			enemiesInstances.Add(obj);
			//wait interval
			yield return new WaitForSeconds(interval);
		}
    }

	//method to remove an enemy (when he dies)
	public void removeEnemy(GameObject enemy){
		if(enemiesInstances.Contains(enemy)){
			enemiesInstances.Remove(enemy);
			//delete the enemy but keep it for the dead animation
			StartCoroutine(destroyEnemy(enemy));
			remaingEnemies--;
			//update the UI
			gameManager.uIManager.setEnemies(remaingEnemies);
			//add 20 coins
			gameManager.setCoins(gameManager.coins+20);
			gameManager.soundManager.playClip(1);
			//if there are no more enemies finish the round
			if(getRemainingEnemies()==0){
				gameManager.roundFinished();
			}
		}
	}

	//method used by the defenders for spotting the closer enemy to attach, inside the shooting distance
	public GameObject getCloserEnemy(Transform defenderTransform, float shootingDistance){
		//check if the enemies list is empty
		if(enemiesInstances.Count!=0){
			
			//standard algorithm for finding a minimum value in a list using the Distance between the defender and the enemy as value
			//and the enemy as gameobject to find and return
			GameObject closerEnemy = enemiesInstances[0];
			float min = Vector3.Distance(enemiesInstances[0].transform.position, defenderTransform.position);

			foreach(GameObject enemy in enemiesInstances){
				if(Vector3.Distance(enemy.transform.position, defenderTransform.position)<min){
					min = Vector3.Distance(enemy.transform.position, defenderTransform.position);
					closerEnemy = enemy;
				}
			}
			//check if the closer enemy is inside the distance the defender can reach with the arrow
			if(min>shootingDistance) return null;
			return closerEnemy;
		}
		return null;
	}

	public int getRemainingEnemies(){
		return enemiesInstances.Count;
	}

	private void Update()
	{
		//iterate over all the enemies and update the target of the enemies
		//because some other enemies may have detroyed the tower in the meanwhile one enemy is walking towards it
		foreach(GameObject enemy in enemiesInstances){
			EnemyController enemyController = enemy.GetComponent<EnemyController>();
			GameObject targetTower = placementSystem.getCloserTower(enemy.transform);

			//change the stopping distance depending on what kind of tower the enemy is going to
			float stoppingDistance = 0.25f;
			if(targetTower!=placementSystem.mainTower){
				stoppingDistance = 0.11f; 
			}
			enemyController.SetTarget(targetTower.transform, stoppingDistance);
		}
	}

	//method for destroying the enemy gameobject some seconds later the dead
	//for showing the dead animation
	IEnumerator destroyEnemy(GameObject enemy)
    {
		Rigidbody rigidbody = enemy.GetComponent<Rigidbody>();
		rigidbody.useGravity = true;
		//disable the healthbar
		GameObject canva = enemy.transform.Find("Canvas").gameObject;
		canva.SetActive(false);

		//wait and detroy the enemy
		yield return new WaitForSeconds(4);
		Destroy(enemy);
    }

}
