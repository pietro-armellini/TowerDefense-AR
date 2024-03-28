using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;

//Class to manage the input
public class InputManager : MonoBehaviour
{
    [SerializeField] 
	public ARRaycastManager aRRaycastManager;
	private ARPlaneManager aRPlaneManager;
	private GameManager gameManager;
	private PlacementSystem placementSystem;
	private List<ARRaycastHit> hits = new List<ARRaycastHit>();
	private void Awake() {
		aRRaycastManager = GetComponent<ARRaycastManager>();
		aRPlaneManager = GetComponent<ARPlaneManager>();
		gameManager = GetComponent<GameManager>();
		placementSystem = GetComponent<PlacementSystem>();
	}
	

	private void OnEnable() {
		EnhancedTouch.TouchSimulation.Enable();
		EnhancedTouch.EnhancedTouchSupport.Enable();

		//register method to the event
		EnhancedTouch.Touch.onFingerDown += fingerDown;
	}

	private void OnDisable() {
		EnhancedTouch.TouchSimulation.Disable();
		EnhancedTouch.EnhancedTouchSupport.Disable();
	}

	private void fingerDown(EnhancedTouch.Finger finger){
		//disable multitouch
		if(finger.index != 0) return;
		
		//check if pressing on the screen i pressed a UI object
		//Here for checking if the ones in the main canvas
		RaycastHit2D hit2d = Physics2D.Raycast(finger.currentTouch.screenPosition, Vector2.zero);
		if(hit2d.collider!=null) return;

		//here to check the ones floting above the towers
		RaycastHit hit3d;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		
		if (Physics.Raycast(ray, out hit3d))
            {
				if (hit3d.transform.gameObject.layer==LayerMask.NameToLayer("UI")){
					return;
				}
				else if(hit3d.transform.gameObject.layer==LayerMask.NameToLayer("Tower")){
					GameObject tower = hit3d.transform.gameObject;
					TowerManager towerManager = tower.GetComponent<TowerManager>();
					towerManager.towerPressed();
					return;
				}
			}

		//if nothing has been hit (no tower or UI) there are two possibilities:
		//trying to place a tower (main or defensive) or trying to select the game plane, depending on the gameStatus
		if(aRRaycastManager.Raycast(finger.currentTouch.screenPosition, hits, TrackableType.PlaneWithinPolygon)){
			Pose pose = hits[0].pose;

			//if i have just started i'm trying to select the plane
			if(gameManager.gameState==GameManager.GameState.SelectingPlane){
				ARPlane gamePlane = aRPlaneManager.GetPlane(hits[0].trackableId);

				//change the color of the line to show it is selected
				LineRenderer lineRenderer = gamePlane.GetComponent<LineRenderer>();
				lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
				lineRenderer.startColor = Color.green;
				lineRenderer.endColor = Color.green;

				//disable the ar to identify new planes and delete the ones not selected
				aRPlaneManager.enabled = false;
				foreach (var plane in aRPlaneManager.trackables){
					if(plane.trackableId!=gamePlane.trackableId){
						GameObject.Destroy(plane.gameObject);
					}
				}
				gameManager.setGamePlane(gamePlane);
				Debug.Log("Plane Selected");
			}
			//call the placing methog if i'm trying to place a tower
			else if(gameManager.gameState==GameManager.GameState.PlacingMainTower){
				placementSystem.placeMainTower(pose);
			}
			else if(gameManager.gameState==GameManager.GameState.Editing){
				placementSystem.placeDefensiveTower(pose);
			}
		}
		
		
	}


}
