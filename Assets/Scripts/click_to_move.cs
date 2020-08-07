using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_to_move : MonoBehaviour
{

public Vector3 pointClicked;

public float speed;
public float startTime;
public float journeyLength;
public float fractionOfJourney;

public float lerpTime = 1f;
public float currentLerpTime;

// public bool isMoving;

    // Start is called before the first frame update
    void Start()
    {       
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0)){
            //Debug.Log("clicked");
            startTime = Time.time;
            //isMoving = true;
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = Camera.main.nearClipPlane;
            pointClicked = Camera.main.ScreenToWorldPoint(mousePos);
            Debug.Log("Point Clicked: " + pointClicked);
            journeyLength = Vector3.Distance(transform.position, pointClicked);
        }

        //if(isMoving){
            
    //Easeing
            if (Input.GetMouseButtonDown(0)) {
                currentLerpTime = 0f;
            }
 
            //increment timer once per frame
            currentLerpTime += Time.deltaTime;
            if (currentLerpTime > lerpTime) {
                currentLerpTime = lerpTime;
            }
            float t = currentLerpTime / lerpTime;

        // Ease out
            //t = Mathf.Sin(t * Mathf.PI * speed);

        // Ease in
            //t = 1f - Mathf.Cos(t * Mathf.PI * 0.5f);
            
            //transform.position = Vector3.Lerp(transform.position, pointClicked, t);

    // Constant Speed
            //transform.position = Vector3.MoveTowards(transform.position, pointClicked, Time.deltaTime * speed); 

    //Classic Lerp
            float distCovered = (Time.time - startTime) * speed;
            fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, pointClicked, fractionOfJourney);

        
            if(Vector3.Distance(transform.position, pointClicked) < .1f){
                Debug.Log(Vector3.Distance(transform.position, pointClicked));
               // isMoving = false;
                
            }
        //}
    }
}
