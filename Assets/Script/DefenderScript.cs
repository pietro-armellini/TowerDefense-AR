using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

//Script for managing the defender (characther on the towers)
public class DefenderScript : MonoBehaviour
{
	public EnemyManager enemyManager;
	public GameObject arrowPrefab;
	
	//how long the arrow draw takes
	public float interval = 5;
	private float timer;
	public float arrowSpeed;
	//how far the defender can shoot (the idea is that defenders placed on lower spots can shoot shorter than the ones upper)
	public float shootingDistance = 0.1f;
	private Animator defenderAnimator;
	public Transform arrowSpawningPoint;

	private void Awake() {
		defenderAnimator = GetComponent<Animator>();
	}
	public void setUp(EnemyManager enemyManager){
		this.enemyManager = enemyManager;
	}

	private void Start() {
		timer = interval;
	}

	private void Update() {
		//decrese the timer
		timer -= Time.deltaTime;
		
		defenderAnimator.SetTrigger("Idle");
		
		//return if 5 seconds have not been passed since the last draw
		if(timer > 0) return;
		timer = interval;

		//set the target as closer enemy (inside the shootingDistance)
		GameObject target = enemyManager.getCloserEnemy(transform, shootingDistance);
		if(target!=null){
			//Start shooting coroutine
			StartCoroutine(shoot(target));
		}
	}

	//Coroutine for synchronize te draw animation and the arrow spawn 
	IEnumerator shoot(GameObject target)
    {
		//define the target position as the one the defender has to look at, but keeping the y the same as the defender
		// this way he will not rotate on the y axix
		Vector3 lookTarget = new Vector3(target.transform.position.x, transform.position.y, target.transform.position.z);
		gameObject.transform.LookAt(lookTarget);

		//start draw animation
		defenderAnimator.SetTrigger("Draw");

		//wait one second
		yield return new WaitForSeconds(1);

		//check if the target is stil alive
		if(target!=null){
			//instantiate the arrow
			GameObject arrow = Instantiate(arrowPrefab, arrowSpawningPoint.position, arrowSpawningPoint.rotation) as GameObject;
			ArrowScript arrowScript = arrow.GetComponent<ArrowScript>();
			arrowScript.Initialize(target, arrowSpeed);
			//destroy after 5 seconds if it doesn't get the enemy
			Destroy(arrow, 5f);

			//draw sound
			enemyManager.gameManager.soundManager.playClip(0, GetComponent<AudioSource>());
		}
    }

	
}
