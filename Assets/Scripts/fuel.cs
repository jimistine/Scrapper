using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuel : MonoBehaviour
{

public clickToMove clickToMove;

public float maxFuel;
public float currentFuelUnits;
public float currentFuelPercent;
public float fuelEfficiency;
public float currentSpeed;

public bool hasFuel;


    void Start()
    {
        
    }

    void Update()
    {
        currentSpeed = clickToMove.currentSpeed;
        float fuelLossRate = fuelEfficiency * currentSpeed;

        if(clickToMove.isMoving && float.IsNaN(fuelLossRate) == false && hasFuel){
            currentFuelUnits -= fuelLossRate * .01f;
            currentFuelPercent = (currentFuelUnits/maxFuel) * 100;
        }
        if(currentFuelUnits <= 0 && hasFuel){
            hasFuel = false;
            clickToMove.StartCoroutine("OutOfFuel");
            
        }
        

    }
}
