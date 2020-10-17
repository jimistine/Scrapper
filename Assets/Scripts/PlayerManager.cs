﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerManager : MonoBehaviour
{

    public static PlayerManager PM;

    [Header("Scripts")]
    [Space(5)]
    public fuel fuelManager;
    public ClickDrag ClickDrag;
    public UIManager UIManager;
    AudioManager AudioManager;

    [Header("Rig Stats")]
    [Space(5)]
    public CircleCollider2D searchRadius;

    [Header("Player Inventory")]
    [Space(5)]
    public float currentHaul;
    public float maxHaul;
    public float playerCredits;
    public bool scannerActive;
    public List<ScrapObject> playerScrap = new List<ScrapObject>();

    [Header("UI")]
    [Space(5)]
    public GameObject hasronCallout;
    public GameObject chipCallout;
    public int tickReadoutIndex;

    // Tutorial bools
    public bool firstScrapFound;
    public bool firstScrapTaken;

    // Start is called before the first frame update
    void Awake()
    {
        PM = this;
    }
    void Start(){
        UIManager = UIManager.UIM;
        AudioManager = AudioManager.AM;
        fuelManager = gameObject.GetComponent<fuel>();
        ClickDrag = gameObject.GetComponent<ClickDrag>();
    }

    // Update is called once per frame
    void Update()
    {        
// INTERACTIONS
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            // 3. If player clicked something, and that something was scrap, and it's in range, show the readout panel
            if(hit.collider != null){
                if (hit.collider.tag == "Scrap" && hit.collider.gameObject.GetComponent<ProximityCheck>().interactable == true) {
                    ScrapObject newScrap = hit.collider.GetComponent<ScrapObject>();
                    //UIManager.ShowReadout(newScrap);
                    UIManager.ShowReadout();
                }
            }    
        }
        if(Input.GetKeyDown(KeyCode.Q) && UIManager.tickReadout.activeSelf){
            Debug.Log("GetKey scrap index: "+ tickReadoutIndex);
            AudioManager.PlayPlayerClip("drop scrap");
            DropScrap(tickReadoutIndex);
        }
        if(Input.GetMouseButtonDown(1)){
            if(RigManager.RM.zoomLevels.Count > 1){
                RigManager.RM.ChangeZoom(Camera.main.orthographicSize);
                AudioManager.ChangeZoom();
            }
        }
        if(Input.GetKeyDown(KeyCode.F)){
            AudioManager.ToggleHeadlights();
            if(RigManager.RM.rigLights.activeSelf == true){
                RigManager.RM.rigLights.SetActive(false);
            }
            else{
                RigManager.RM.rigLights.SetActive(true);
            }
        }
        if(Input.GetKeyDown(KeyCode.P)){
            Debug.Log("pressed p");
            UIManager.TogglePause();
            if(Director.Dir.gameStarted == false){
                Debug.Log("calling dir start game");
                Director.Dir.StartGame();
            }
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            DialogueManager.DM.ContinueDialogue();
        }
        if(Input.GetKeyDown(KeyCode.R)){
            Debug.Log("restarting");
            SceneController.SC.RestartGame();
        }
        if(Input.GetKeyDown(KeyCode.Semicolon)){
            //Debug.Log("restarting");
            DialogueManager.DM.RunNode("left-town-ogden");
        }
    }
    
    void OnTriggerEnter2D(Collider2D other){
    // 1. Pop goes the scrap!  If our search radius hits the small EdgeCollider on the scrap, it pops
        if(other.gameObject.tag == "Scrap" && other.GetType() == typeof(EdgeCollider2D) && scannerActive){
            ScrapObject newScrap = other.gameObject.GetComponent<ScrapObject>();
            other.gameObject.GetComponent<ProximityCheck>().IsInRange(true);
            if(other.gameObject.GetComponent<SpriteRenderer>().enabled == false){
                AudioManager.PlayPlayerClip("found scrap");
                if(newScrap.scrapName == "Land speeder (unknown)"){
                    DialogueManager.DM.RunNode("land-speeder");
                }
                else if(newScrap.scrapName == "Chunk of raw cordonite"){
                    DialogueManager.DM.RunNode("chunk-of-raw-cordonite");
                }
                else if(newScrap.scrapName == "Sha'ak-ji Holospace Generator"){
                    DialogueManager.DM.RunNode("holospace-generator");
                }
                else if(firstScrapFound == false && Director.Dir.introCompleted){
                    DialogueManager.DM.RunNode("tutorial-find-scrap");
                    firstScrapFound = true;
                }
                else{
                    DialogueManager.DM.RunNode("scrap-find");
                }
                Debug.Log("Found scrap");
            }
            UIManager.ShowScrap(newScrap);
        }
        if(other.gameObject.name == "Town" && OverworldManager.OM.towRig.activeSelf == false){
            UIManager.ActivateTownButton(true);
        }
    }
    void OnTriggerExit2D(Collider2D other){ // Only consider out of range when we exit the larger collider on scrap
        if(other.gameObject.tag == "Scrap" && other.GetType() == typeof(CircleCollider2D) && scannerActive){
            ScrapObject newScrap = other.gameObject.GetComponent<ScrapObject>();
            other.gameObject.GetComponent<ProximityCheck>().IsInRange(false);
            UIManager.OutOfRangeScrap(newScrap);
        }
        if(other.gameObject.name == "Town"){
            UIManager.ActivateTownButton(false);
        }
    }
   
    // 5. If they clicked Take (UIM) and they could fit it, take it
    public void TakeScrap(ScrapObject takenScrap){
        takenScrap.gameObject.SetActive(false);
        playerScrap.Add(takenScrap);
        UpdateCurrentHaul();
        foreach(ScrapObject scrap in playerScrap){
            Debug.Log("Player has: " + scrap.GetComponent<ScrapObject>().scrapName);
        }
        if(firstScrapTaken == false && Director.Dir.introCompleted){
            DialogueManager.DM.RunNode("tutorial-take-scrap");
            firstScrapTaken = true;
        }
    }
    public void DropScrap(int tickScrapIndex){
        //Debug.Log("Index Called: " + tickScrapIndex);
        Debug.Log("Object at index on player: " + playerScrap[tickScrapIndex].scrapName);
        scrapPlacer.SP.SpawnDroppedScrap(playerScrap[tickScrapIndex]);
        playerScrap.RemoveAt(tickScrapIndex);
        GameObject.Destroy(UIManager.scrapTickSlots.transform.GetChild(tickScrapIndex).gameObject);
        UpdateCurrentHaul();
        UIManager.tickReadout.SetActive(false);
    }
// UTILITY
    // For whenever we need to stop em in their tracks
    public void SetPlayerMovement(bool letPlayerMove){
        ClickDrag.moveEnabled = letPlayerMove;
    }

    public void UpdateCurrentHaul(){
        currentHaul = 0;
        foreach(ScrapObject scrap in playerScrap){
            currentHaul += scrap.size;
        }
    }

    public float evaluateCurrentHaul(){
        float currentHaulValaue = 0;
        foreach(ScrapObject scrap in playerScrap){                
                currentHaulValaue += scrap.value;
            }
        return currentHaulValaue;
    }

    public void dropHeavyScrap(){
        foreach(ScrapObject scrap in playerScrap){
            if(scrap.size >= 10){
                scrapPlacer.SP.SpawnDroppedScrap(scrap);
            }
        }
        playerScrap.RemoveAll(t => t.size >= 10);
        UpdateCurrentHaul();
        UIManager.UpdateScrapTicks();
        AudioManager.PlayPlayerClip("drop scrap");
    }
}
