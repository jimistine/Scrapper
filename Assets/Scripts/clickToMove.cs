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

public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {       

    }

   

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            if(hit.collider != null){
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
        isMoving = true;
    //Classic Lerp
        while(isMoving){
            distCovered = (Time.time - startTime) * speed;
            fractionOfJourney = distCovered / journeyLength;

            transform.position = Vector3.Lerp(transform.position, pointClicked, fractionOfJourney);

            currentSpeed = Vector3.Distance(startPos, transform.position) / (Time.time - startTime);
            distanceToEnd = Vector3.Distance(transform.position, pointClicked);
            if(distanceToEnd < .1f){
                isMoving = false;
            }
            yield return null;
        }
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

