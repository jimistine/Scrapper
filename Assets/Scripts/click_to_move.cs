using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class click_to_move : MonoBehaviour
{

public GameObject player;
public Vector3 pointClicked;
private Transform target;

public float speed;
public float startTime;
public float journeyLength;
public float fractionOfJourney;

public bool isMoving;

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
            isMoving = true;
            pointClicked = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            journeyLength = Vector3.Distance(transform.position, pointClicked);
        }

        if(isMoving){
            float distCovered = (Time.time - startTime) * speed;
            fractionOfJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, pointClicked, fractionOfJourney);
            if(Vector3.Distance(transform.position, pointClicked) < 1f){
                Debug.Log(Vector3.Distance(transform.position, pointClicked));
                isMoving = false;
            }
        }
    }
}
