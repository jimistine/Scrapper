using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDrag : MonoBehaviour
{
    public float currentSpeed;
    public float currentSpeedActual;
    public float topSpeed;
    public float acceleration;
    public float breakingPower;
    public float recoveryTime;
    public float turnIntensityModifier;
    public float turnIntensity;
    public bool recovering;
    Rigidbody2D  PlayerRB;
    Vector2 direction;
    Vector2 lastDirection;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRB = gameObject.GetComponent<Rigidbody2D>(); 
    }


    void Update(){
        if (currentSpeed < 0){           
            currentSpeed = 0;
        }
        if(Input.GetKeyDown(KeyCode.W)){
            StartCoroutine("Accelerate");
            StartCoroutine("UpdateRotation");
        }
        if(Input.GetKeyUp(KeyCode.W)){
            StopCoroutine("Accelerate");
        }
        if(Input.GetKeyDown(KeyCode.S)){
            StartCoroutine("Break");
        }
        if(Input.GetKeyUp(KeyCode.S)){
            StopCoroutine("Break");
        }

        if(turnIntensity < .99f && recovering == false){
            StartCoroutine("SteeringDampener");
        }
        // else{
        //     turnIntensity = 1;
        // }

    }
    // Update is called once per frame
    void FixedUpdate()
    {
        currentSpeedActual = PlayerRB.velocity.magnitude;
        PlayerRB.AddForce((direction.normalized * -1) * currentSpeed * turnIntensity);

        float driftForce = Vector2.Dot(PlayerRB.velocity, PlayerRB.GetRelativeVector(Vector2.left)) * 2.0f;
        Vector2 relativeForce = Vector2.right * driftForce;
        Debug.DrawLine(PlayerRB.position, PlayerRB.GetRelativePoint(relativeForce), Color.green);
        PlayerRB.AddForce(PlayerRB.GetRelativeVector(relativeForce));

    }

    public IEnumerator Accelerate(){
        while(currentSpeed < topSpeed){
            currentSpeed += acceleration;
            yield return new WaitForSeconds(.01f);
        }
    }
    public IEnumerator Break(){
        while(currentSpeed > 0){
            Debug.Log("Breaking");
            currentSpeed -= breakingPower;
            yield return new WaitForSeconds(.01f);
         }
    }

    public IEnumerator UpdateRotation(){
        while(currentSpeed > 0){
            
            Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 dir = Input.mousePosition - pos;
            float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
            transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            direction = gameObject.transform.position - mousePos;
            turnIntensity = Vector2.Dot(direction.normalized, lastDirection.normalized);
            lastDirection = direction;
            yield return new WaitForSeconds(0.025f);
               
        }
    }
    public IEnumerator SteeringDampener(){
        recovering = true;
        turnIntensity *= turnIntensityModifier;
        for(float intensity = turnIntensityModifier; intensity < 1; intensity+=.01f){
            Debug.Log("Recovering");
            turnIntensity = intensity;
            yield return new WaitForSeconds(recoveryTime);
        }
        recovering = false;
    }
}
