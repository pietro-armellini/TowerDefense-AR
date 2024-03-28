using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using EnhancedTouch = UnityEngine.InputSystem.EnhancedTouch;
using UnityEngine.SceneManagement;

//Class to manage the input in the main menu
public class InputManager_menu : MonoBehaviour
{
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

		//check if the player touches the screen
		RaycastHit hit3d;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

		
		if (Physics.Raycast(ray, out hit3d))
            {
				if (hit3d.transform.gameObject.layer==LayerMask.NameToLayer("UI_menu")){
					//Load the game scene
					SceneManager.LoadScene(1);
				}
			}

		
	}


}
