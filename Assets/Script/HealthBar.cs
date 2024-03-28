using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public Image healthBar;
	public void updateHealth(float value){
		healthBar.fillAmount = value;
	}
}
