using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToMove : MonoBehaviour
{

public Vector3 startPos;
public Vector3 pointClicked;

public float speed;
public float currentSpeed;
public float startTime;
public float journeyLength;
public float fractionOfJourney;
public float distanceToEnd;

public float lerpTime = 1f;
public float currentLerpTime;
float distCovered;

[Range(0.0f, 0.1f)]
public float outOfFuelSlowRate;

public bool isMoving;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)){           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if(hit.collider != null){
                // if they clicked on something navigable, let em go!
                if(hit.collider.tag == "Terrain"){
                    Debug.Log("Clicked: " + hit.collider.transform.name);
                    startPos = transform.position;
                    startTime = Time.time;
                    pointClicked = mousePos;
                    pointClicked.z = 0;
                    journeyLength = Vector3.Distance(transform.position, pointClicked);
                    StartCoroutine("MovePlayer");
                }
            }            
        }  
    }

    IEnumerator MovePlayer(){
        //speed /= 10000;
        isMoving = true;
    //Classic Lerp
        while(isMoving){
            distCovered = (Time.time - startTime) * speed;
            fractionOfJourney = distCovered / journeyLength;
            // move is here
            transform.position = Vector3.Lerp(transform.position, pointClicked, fractionOfJourney);

            currentSpeed = Vector3.Distance(startPos, transform.position) / (Time.time - startTime);
            distanceToEnd = Vector3.Distance(transform.position, pointClicked);
            if(distanceToEnd < .1f){
                isMoving = false;
                //speed *= 1000;
            }
            yield return null;
        }
    }

    public IEnumerator OutOfFuel(){
        // once they run out, slow them down for ~drama~
        for (float i = speed; speed > 0; speed -=.00005f){
            Debug.Log("Running out!");
            yield return new WaitForSeconds(outOfFuelSlowRate);
            continue;
        }
        isMoving = false;
        //this.enabled = false;
        OverworldManager.OM.SetUpTowRig();
    }
}
//Easeing
            // if (Input.GetMouseButtonDown(0)) {
            //     currentLerpTime = 0f;
            // }
 
            // //increment timer once per frame
            // currentLerpTime += Time.deltaTime;
            // if (currentLerpTime > lerpTime) {
            //     currentLerpTime = lerpTime;
            // }
            // float t = currentLerpTime / lerpTime;

        // Ease out
            //t = Mathf.Sin(t * Mathf.PI * speed);

        // Ease in
            //t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            
            //transform.position = Vector3.Lerp(transform.position, pointClicked, t);

    // Constant Speed
        //transform.position = Vector3.MoveTowards(transform.position, pointClicked, Time.deltaTime * speed); 

