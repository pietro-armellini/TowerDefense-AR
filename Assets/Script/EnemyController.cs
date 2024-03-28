using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;

//Script for managing the enemy
public class EnemyController : MonoBehaviour
{
	private NavMeshAgent agent;
	private EnemyManager enemyManager;
	Transform target = null;
	public HealthBar healthBar;
	public int startingHealth = 100;
	private int health;
	private Animator enemyAnimator;

	//how long a pickaxe hit takes
	private float interval = 1;
	private float timer;

	private void Awake()
	{
		agent = GetComponent<NavMeshAgent>();
		enemyAnimator = GetComponent<Animator>();
	}

	public void SetTarget(Transform target, float stoppingDistance){
		this.target = target;
		//set the stopping distance to the NavMesh Agent
		agent.stoppingDistance = stoppingDistance;
	}

	private void Start() {
		health = startingHealth;
		timer = interval;
	}

	public void setUp(EnemyManager enemyManager){
		this.enemyManager = enemyManager;
	}

	private void Update() {
		//check if he has a target
		if(target!=null){
			//set the navmeshagent destination
			agent.SetDestination(target.position);
			//start walking animation
			enemyAnimator.SetTrigger("Walking");
			gameObject.transform.LookAt(target);
			timer -= Time.deltaTime;
			if(timer > 0) return;
			
			//check if the enemy has reached the target
			if(agent.remainingDistance<=agent.stoppingDistance){
				//start the hitting animation
				enemyAnimator.SetTrigger("Picking");
				timer = interval;
				
				//make the tower react
				TowerManager towerManager = target.gameObject.GetComponent<TowerManager>();
				towerManager.react();
			}
		}
	}

	public void react(){
		//reduce health and update the healthbar
		health -= 10;
		healthBar.updateHealth((float)health/(float)startingHealth);

		//if enemy is out of health it dies
		if(health<=0){
			enemyManager.removeEnemy(gameObject);
			//start dying animation
			enemyAnimator.SetTrigger("Dead");
		}
		
	}

	//method triggered when an arrow enters the collider
	private void OnTriggerEnter(Collider other) {
		if(other.tag == "Arrow"){
			react();
			//destroy the arrow
			Destroy(other.gameObject);
		}
	}



}
