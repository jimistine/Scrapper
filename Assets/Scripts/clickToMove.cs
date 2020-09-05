using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class clickToMove : MonoBehaviour
{

public Vector3 startPos;
public Vector3 pointClicked;

public float speed;
public float storedSpeed;
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
public bool speedSlowed;

    void Update()
    {
        if(gameObject.GetComponent<fuel>().hasFuel == true){
            speedSlowed = false;
        }

        if (Input.GetMouseButtonDown(0) && !isMoving){           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            //RaycastHit2D hit = Physics2D.GetRayIntersection(ray);


            if(hit.collider != null){
                // if they clicked on something navigable, let em go!
                if(hit.collider.tag == "Terrain" || hit.collider.tag == "Player"){
                    Debug.Log("Clicked: " + hit.collider.transform.name);
                    ResetMove();
                    startPos = transform.position;
                    startTime = Time.time;
                    pointClicked = mousePos;
                    pointClicked.z = 0;
                    journeyLength = Vector3.Distance(transform.position, pointClicked);
                    StartCoroutine(MovePlayer(pointClicked));
                }
            }            
        }  
    }
    public void MoveToCustomPoint(GameObject objToMove, Vector3 destination){
        if(!isMoving){
            ResetMove();
            Vector2 destination2D = new Vector2(destination.x, destination.y);
            startPos = transform.position;
            startTime = Time.time;
            destination.z = 0;
            journeyLength = Vector3.Distance(transform.position, destination);
            //StopCoroutine("MovePlayer");
            StartCoroutine(MovePlayer(destination));
        }
    }

    IEnumerator MovePlayer(Vector3 destination){
        isMoving = true;
    //Classic Lerp
        while(isMoving){
            if(speedSlowed){
                distCovered = (Time.time - startTime) * .00025f;
            }
            else{
                distCovered = (Time.time - startTime) * speed;
            }
            fractionOfJourney = distCovered / journeyLength;
            // move is here
            transform.position = Vector3.Lerp(transform.position, destination, fractionOfJourney);

            currentSpeed = Vector3.Distance(startPos, transform.position) / (Time.time - startTime);
            distanceToEnd = Vector3.Distance(transform.position, destination);
            if(distanceToEnd < .025f){
                isMoving = false;
            }
            yield return null;
        }
    }

    public IEnumerator OutOfFuel(){
        // once they run out, slow them down for ~drama~
        storedSpeed = speed;
        for (float i = speed; speed > 0.001; speed -=.00005f){
            Debug.Log("Running out!");
            yield return new WaitForSeconds(outOfFuelSlowRate);
            continue;
        }
        isMoving = false;
        speedSlowed = true;
        PlayerManager.PM.SetPlayerMovement(false);
        speed = storedSpeed;
        //this.enabled = false;
        OverworldManager.OM.SetUpTowRig();
    }

    public void ResetMove(){
        isMoving = false;
        startPos = transform.position;
        pointClicked = transform.position;
        currentSpeed = 0;
        journeyLength = 0;
        fractionOfJourney = 0;
        distanceToEnd = 0;
        distCovered = 0;
        //Debug.Log("Move resest");
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

