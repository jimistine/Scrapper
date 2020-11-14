using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownDistanceChecker : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.name == "Player"){
            UIManager.UIM.ActivateTownButton(true);
            Debug.Log("not near town");
            PlayerManager.PM.nearTown = true;
        }
    }
    void OnTriggerExit2D(Collider2D other){
        if(other.gameObject.name == "Player"){
            UIManager.UIM.ActivateTownButton(false);
            Debug.Log("not near town");
            PlayerManager.PM.nearTown = false;
        }
    }
}
