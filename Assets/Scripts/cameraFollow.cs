using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class cameraFollow : MonoBehaviour
{
     public float dampTime = 0.15f;
     private Vector3 velocity = Vector3.zero;
     public Transform player;
    // public Vector3 followOffset;
     public bool adjusted;
 
    void Start(){
        player = PlayerManager.PM.gameObject.transform;
        //transform.position += followOffset;
    }


     void Update () 
     {
         if (player)
         {
             if(this.tag == "MainCamera"){
                Vector3 point = GetComponent<Camera>().WorldToViewportPoint(player.position);
                Vector3 delta = player.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z));
                Vector3 destination = transform.position + delta;
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
             }
             else if(this.tag == "UI"){
                Vector3 point = player.position;
                Vector3 delta = (player.position) - (transform.position);
                Vector3 destination = transform.position + delta;
                transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
             }
         }

     
     }
}
