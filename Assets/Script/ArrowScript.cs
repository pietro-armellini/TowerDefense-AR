using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script for managing the arrow and driving it to the target
//It's attached to the arrow prefab
public class ArrowScript : MonoBehaviour
{
	private GameObject target;
    private float moveSpeed;

	//method to call once the arrow has been instantiate for setting the target and the speed
	 public void Initialize (GameObject target, float moveSpeed)
    {
        this.target = target;
        this.moveSpeed = moveSpeed;
	}

	void Update ()
    {
        if(target != null)
        {
			//move the arrow towards the target
            transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
			//set the right direction of the arrow
            transform.LookAt(target.transform);
        }
    }
}
