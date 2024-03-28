using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TowerManager : MonoBehaviour
{
    public int startingHealth = 500;
	private int health;
	public HealthBar healthBar;
	//reference to the canvas object you can press (repair and upgrade buttons)
	public GameObject interactable;
	private PlacementSystem placementSystem;
	//List of tower defenders
	public List<GameObject> defenders;
	//tower level
	public int level = 1;

	private void Start() {
		health = startingHealth;
	}
	public void setUp(PlacementSystem placementSystem){
		this.placementSystem = placementSystem;
	}

	//method to be used when an enemy hits a tower
	public void react(){
		health -= 10;
		healthBar.updateHealth((float)health/(float)startingHealth);
		if(health<=0){
			placementSystem.removeTower(gameObject);
		}
	}

	//method triggered when player touches the repair button above the tower
	public void repair(){
		//check if the player has enough coins
		if(placementSystem.gameManager.repair()){
			health = startingHealth;
			placementSystem.gameManager.soundManager.playClip(5, GetComponent<AudioSource>());
			healthBar.updateHealth((float)health/(float)startingHealth);
		}
	}

	//method triggered when player touches the upgrade button above the tower
	public void upgrade(){
		placementSystem.upgradeTower(gameObject);
	}

	//method triggered when player touches a tower
	//it enables and disable the interactables
	public void towerPressed(){
		interactable.SetActive(!interactable.activeSelf);
	}
	public void deactivateInteractables(){
		interactable.SetActive(false);
	}
}