using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuel : MonoBehaviour
{

public clickToMove ClickToMove;

public float currentSpeed;
public float currentFuelLevel;
public float fuelEfficiency;

    void Start()
    {
        
    }

    void Update()
    {
        currentSpeed = ClickToMove.currentSpeed;
        float fuelLossRate = fuelEfficiency * currentSpeed;

        if(ClickToMove.isMoving && float.IsNaN(fuelLossRate) == false){
            currentFuelLevel -= fuelLossRate * .01f;
        }
    }
}
