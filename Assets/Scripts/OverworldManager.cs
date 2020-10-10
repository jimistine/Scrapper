using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverworldManager : MonoBehaviour
{

    public static OverworldManager OM;

    [Header("Environment")]
    [Space(5)]
    public DayNight DayNight;
    public GameObject worldLights;
    [Header("Tow Rig")]
    [Space(5)]
    public GameObject towRig;
    public GameObject town;
    public bool isMoving;
    public bool goingOut = true;
    public float distCovered;
    public Vector3 startPos;
    public float speed;
    public float startTime;
    public float journeyLength;
    public float fractionOfJourney;
    public float distanceToEnd;
    // [Header("UI")]
    // [Space(5)]
    // public GameObject onScrapPanel;
    
    void Awake()
    {
        OM = this;
    }

    void Start(){
        DayNight = GameObject.Find("Celestial").GetComponent<DayNight>();
    }
    void Update()
    {
        if(DayNight.isDay){
            worldLights.SetActive(false);
        }
        else{
            worldLights.SetActive(true);
        }
        //Debug.Log("Distance is: " + Vector3.Distance(PlayerManager.PM.gameObject.transform.position, town.transform.position));
        if(Vector3.Distance(PlayerManager.PM.gameObject.transform.position, town.transform.position) > 1.15f){
            UIManager.UIM.ActivateTownButton(false);
        }
    }

    public void SetUpTowRig(){ // called from fuel
        startPos = towRig.gameObject.transform.position;
        startTime = Time.time;
        isMoving = true;

        if(goingOut){
            Debug.Log("Going out");
            towRig.SetActive(true);
            journeyLength = Vector3.Distance(towRig.transform.position, PlayerManager.PM.gameObject.transform.position);
            StartCoroutine(SendTowRig());
        }
        else{
            Debug.Log("Coming back");
            journeyLength = Vector3.Distance(towRig.transform.position, town.transform.position);
            StartCoroutine(BringTowRigBack());
        }
    }
    public IEnumerator SendTowRig(){
        Debug.Log("Tow rig sent");
        while(isMoving){
            distCovered = (Time.time - startTime) * speed;
            fractionOfJourney = distCovered / journeyLength;
            // move is here
            towRig.transform.position = Vector3.Lerp(towRig.transform.position, PlayerManager.PM.gameObject.transform.position, fractionOfJourney);
            distanceToEnd = Vector3.Distance(towRig.transform.position, PlayerManager.PM.gameObject.transform.position);
            if(distanceToEnd < .1f){
                isMoving = false;
            }
            yield return null;
        }
        yield return new WaitForSeconds(2);
        goingOut = false;
        PlayerManager.PM.GetComponentInChildren<EdgeCollider2D>().enabled = false;
        SetUpTowRig();
    }
    public IEnumerator BringTowRigBack(){
            //float elapsedTime = 0;
        while(isMoving){
            //elapsedTime += Time.deltaTime;
            distCovered = (Time.time - startTime) * speed;
            fractionOfJourney = distCovered / journeyLength;
            // move is here
            towRig.transform.position = Vector3.Lerp(towRig.transform.position, town.transform.position, fractionOfJourney);
            // take player too
            PlayerManager.PM.transform.position = Vector3.Lerp(PlayerManager.PM.transform.position, town.transform.position, fractionOfJourney);
            distanceToEnd = Vector3.Distance(towRig.transform.position, town.transform.position);
            if(distanceToEnd < .1f){
                isMoving = false;
            }
            yield return null;
        }
        goingOut = true;
        PlayerManager.PM.GetComponentInChildren<EdgeCollider2D>().enabled = true;
        towRig.SetActive(false);
        SceneController.SC.StartLoadTown();
    }
}
