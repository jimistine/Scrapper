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
    public float wasdTurnSpeed;
    public float fuelModifier = 1;
    public bool moveEnabled = true;
    public bool recovering;
    public bool accelerating;
    public string moveType = "PointerSteer";
    Rigidbody2D  PlayerRB;
    Vector2 direction;
    Vector2 lastDirection;
    float dragStored;
    fuel fuel;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRB = gameObject.GetComponent<Rigidbody2D>(); 
        fuel = gameObject.GetComponent<fuel>();
    }


    void Update(){
        if(Input.GetKeyDown(KeyCode.Alpha1)){
            moveType = "PointerSteer";
        }
        if(Input.GetKeyDown(KeyCode.Alpha2)){
            moveType = "WASD";
        }
                                                             //Pointer
        if(moveEnabled && moveType == "PointerSteer"){
            if (currentSpeed < 0){           
                currentSpeed = 0;
            }
            if(Input.GetKeyDown(KeyCode.W)){
                accelerating = true;
                //AudioManager.AM.PlayRigStart();
                StartCoroutine("Accelerate");
                StartCoroutine("UpdateRotation");
            }
            if(Input.GetKeyUp(KeyCode.W)){
                accelerating = false;
                //AudioManager.AM.PlayRigStop();
                StopCoroutine("Accelerate");
                StopCoroutine("UpdateRotation");
            }
            if(Input.GetKeyDown(KeyCode.S)){
                dragStored = PlayerRB.drag;
                StartCoroutine("Break");
            }
            if(Input.GetKeyUp(KeyCode.S)){
                PlayerRB.drag = dragStored;
                StopCoroutine("Break");
            }
            if(turnIntensity < .99f && recovering == false){
                StartCoroutine("SteeringDampener");
            }
        }                                                   // WASD
        else if(moveEnabled && moveType == "WASD"){
            if (currentSpeed < 0){           
                currentSpeed = 0;
            }
            if(Input.GetKeyDown(KeyCode.W)){
                accelerating = true;
                AudioManager.AM.PlayRigStart();
                StartCoroutine("Accelerate");
            }
            if(Input.GetKeyUp(KeyCode.W)){
                StopCoroutine("Accelerate");
                AudioManager.AM.PlayRigStop();
                accelerating = false;
            }
            if(Input.GetKeyDown(KeyCode.S)){
                dragStored = PlayerRB.drag;
                StartCoroutine("Break");
            }
            if(Input.GetKeyUp(KeyCode.S)){
                PlayerRB.drag = dragStored;
                StopCoroutine("Break");
            }
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        currentSpeedActual = PlayerRB.velocity.magnitude;
        if(currentSpeedActual < 0.001){currentSpeedActual = 0.0f;}
        if(moveType == "PointerSteer"){
            if(moveEnabled && Input.GetKey(KeyCode.W)){
                PlayerRB.AddForce((direction.normalized * -1) * currentSpeed * turnIntensity * fuelModifier);
                float driftForce = Vector2.Dot(PlayerRB.velocity, PlayerRB.GetRelativeVector(Vector2.left)) * 2.0f;
                Vector2 relativeForce = Vector2.right * driftForce;
                PlayerRB.AddForce(PlayerRB.GetRelativeVector(relativeForce));
            }
        }
        if(moveType == "WASD"){
            if(moveEnabled && Input.GetKey(KeyCode.W)){
                PlayerRB.AddForce(gameObject.transform.up * currentSpeed * fuelModifier);
                float driftForce = Vector2.Dot(PlayerRB.velocity, PlayerRB.GetRelativeVector(Vector2.left)) * 2.0f;
                Vector2 relativeForce = Vector2.right * driftForce;
                //Debug.Log("Adding force");
                PlayerRB.AddForce(PlayerRB.GetRelativeVector(relativeForce));
            }
            if(Input.GetKey(KeyCode.A)){
                transform.Rotate(0,0, wasdTurnSpeed);
            }
            if(Input.GetKey(KeyCode.D)){
                transform.Rotate(0,0,(wasdTurnSpeed * -1));
            }      
        }
    }

    public IEnumerator Accelerate(){
        currentSpeed = 0;
        while(currentSpeed < topSpeed){
            currentSpeed += acceleration;
            yield return new WaitForSeconds(.01f);
        }
    }
    public IEnumerator Break(){
        PlayerRB.drag = breakingPower * 50;
        while(currentSpeed > 0 && Input.GetKey(KeyCode.S)){
            //Debug.Log("Breaking");
            currentSpeed -= breakingPower;
            yield return new WaitForSeconds(.01f);
         }
    }

    public IEnumerator UpdateRotation(){

        while(currentSpeed > 0){
            if(moveType == "PointerSteer"){
                Vector3 pos = Camera.main.WorldToScreenPoint(transform.position);
                Vector3 dir = Input.mousePosition - pos;
                float angle = (Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg) - 90;
                transform.rotation = Quaternion.AngleAxis(angle, Vector3.forward);
            
                Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                direction = gameObject.transform.position - mousePos;
                turnIntensity = Vector2.Dot(direction.normalized, lastDirection.normalized);
                lastDirection = direction;
            }
            yield return new WaitForSeconds(0.025f);  
        }
    }
    public IEnumerator SteeringDampener(){
        recovering = true;
        turnIntensity *= turnIntensityModifier;
        for(float intensity = turnIntensityModifier; intensity < 1; intensity+=.01f){
            //Debug.Log("Recovering");
            turnIntensity = intensity;
            yield return new WaitForSeconds(recoveryTime);
        }
        recovering = false;
    }
    public IEnumerator OutOfFuel(){
        // once they run out, slow them down for ~drama~
        UIManager.UIM.Callout("OutOfFuel");
        fuelModifier = fuel.noFuelSpeedModifier;
        moveEnabled = false;
        for (float i = currentSpeed; i > 0; i -= fuel.outOfFuelSlowRate){
            Debug.Log("Running out!");
            yield return new WaitForSeconds(.01f);
        }
        OverworldManager.OM.SetUpTowRig();
    }
}
