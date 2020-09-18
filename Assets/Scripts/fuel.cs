using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuel : MonoBehaviour
{

//public clickToMove clickToMove;
ClickDrag ClickDrag;

public float maxFuel;
public float currentFuelUnits;
public float currentFuelPercent;
public float fuelEfficiency;
public float outOfFuelSlowRate;
public float noFuelSpeedModifier;

//public bool hasFuel;
public bool canBeTowed = true;
bool lowFuel;


    void Start()
    {
        ClickDrag = PlayerManager.PM.gameObject.GetComponent<ClickDrag>();
    }

    void FixedUpdate()
    {
        if(currentFuelUnits > 0 && ClickDrag.moveEnabled && ClickDrag.accelerating){
            ClickDrag.fuelModifier = 1;
            currentFuelUnits -= fuelEfficiency * ClickDrag.currentSpeedActual;
            currentFuelPercent = (currentFuelUnits/maxFuel) * 100;
        }
        if(currentFuelPercent <= 10f && lowFuel == false){
            lowFuel = true;
            AudioManager.AM.PlayPlayerClip("low fuel"); // calllout here
        }
        if( currentFuelPercent > 10f){
            lowFuel = false;
        }
        if(currentFuelUnits <= 0 && canBeTowed){
            canBeTowed = false;
            ClickDrag.StartCoroutine("OutOfFuel");
        }
        else if(currentFuelUnits > 0){
            canBeTowed = true;
        }
    }
    public void UpdateFuelPercent(){
        currentFuelPercent = (currentFuelUnits/maxFuel) * 100;
    }
}
