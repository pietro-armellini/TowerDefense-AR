using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class to keep the towers UI looking at the camera
public class HealthBarRotation : MonoBehaviour
{
    void Update()
    {
        transform.LookAt(Camera.main.transform);
    }
}
