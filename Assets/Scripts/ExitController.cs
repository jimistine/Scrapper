using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitController : MonoBehaviour
{

    // This manages what happens when you try to leave the game world
    
    public GameObject invisibleWall;   
    
    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name == "Player"){
            if(Director.Dir.ticketsPurchased == 2 || Director.Dir.Timer.day >= 10){
                invisibleWall.SetActive(false);
                Director.Dir.StartExitCanyon();
            }
            else{
                DialogueManager.DM.RunNode("cant-leave");
            }
        }
    }
}
