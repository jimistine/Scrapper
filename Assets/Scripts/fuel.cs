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


    void Start()
    {
        ClickDrag = PlayerManager.PM.gameObject.GetComponent<ClickDrag>();
    }

    void Update()
    {
        if(currentFuelUnits > 0 && ClickDrag.moveEnabled){
            ClickDrag.fuelModifier = 1;
            currentFuelUnits -= fuelEfficiency * ClickDrag.currentSpeedActual;
            currentFuelPercent = (currentFuelUnits/maxFuel) * 100;
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
