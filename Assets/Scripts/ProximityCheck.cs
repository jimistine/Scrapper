using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityCheck : MonoBehaviour
{
    public bool interactable;
    
    public void IsInRange(bool inRange){
        if(inRange){
            interactable = true;
        }
        else{
            interactable = false;
        }
    }
}
