using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fuel : MonoBehaviour
{

public click_to_move click_To_Move;

public float currentSpeed;
public float currentFuelLevel;
public float fuelEfficiency;

    void Start()
    {
        
    }

    void Update()
    {
        currentSpeed = click_To_Move.currentSpeed;
        float fuelLossRate = fuelEfficiency * currentSpeed;

        if(click_To_Move.isMoving && float.IsNaN(fuelLossRate) == false){
            currentFuelLevel -= fuelLossRate * .01f;
        }
    }
}
