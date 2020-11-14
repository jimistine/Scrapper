using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Experimental.Rendering.Universal;



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
    public Color pulseColor;
    public Color pulseColorFadeTarget;
    //public CircleCollider2D pulseScanner;
    public GameObject pulseScanner;
    public Light2D pulseLight;
    public float maxPulse;
    public float pulseDuration;
    public float pulseFadeDuration;

    [Header("Player Inventory")]
    [Space(5)]
    public float currentHaul;
    public float currentHaulPercent;
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
    public bool nearTown;
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
        currentHaulPercent = (currentHaul/maxHaul) * 100;
// INTERACTIONS
        // if (Input.GetMouseButtonDown(0)) {
        //     Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        //     Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
        //     RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

        //     //3. If player clicked something, and that something was scrap, and it's in range, show the readout panel
        //     if(hit.collider != null){
        //         if (hit.collider.tag == "Scrap" && hit.collider.gameObject.GetComponent<ProximityCheck>().interactable == true) {
        //             GameObject newScrap = hit.collider.gameObject;
        //             UIManager.ShowReadout(newScrap);
        //             //UIManager.ShowReadout();
        //         }
        //     }    
        // }
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
        if(Input.GetKeyDown(KeyCode.LeftShift)||Input.GetKeyDown(KeyCode.RightShift)){
            if(scannerActive){
                StartCoroutine(ScannerPulse());
                if(Director.Dir.tip_2.activeSelf){
                    Director.Dir.StartFadeCanvasGroup(Director.Dir.tip_2, "out", 1);
                    Director.Dir.showTip_2 = false;
                }
                //Debug.Log("scanning");
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
        if(Input.GetKeyDown(KeyCode.P) || Input.GetKeyDown(KeyCode.Escape)){
            Debug.Log("pressed p");
            UIManager.TogglePause();
        }
        if(Input.GetKeyDown(KeyCode.I)||Input.GetKeyDown(KeyCode.E)){
            Debug.Log("pressed i");
            UIManager.OpenHUDDetails();
        }
        if(Input.GetKeyDown(KeyCode.Space)){
            DialogueManager.DM.ContinueDialogue();
        }
        if(Input.GetKeyDown(KeyCode.Backslash)){
            Debug.Log("restarting");
            SceneController.SC.RestartGame();
        }
        if(Input.GetKeyDown(KeyCode.Semicolon)){
            //Debug.Log("restarting");
            //DialogueManager.DM.RunNode("left-town-ogden");
            scrapPlacer.SP.UpdateAverageScrapValue();
            //Debug.Log(Random.Range(0,3));
        }
    }
    IEnumerator ScannerPulse(){
        pulseScanner.SetActive(true);
        AudioManager.PlayPlayerClip("scanner pulse");
        Vector3 startScale = new Vector3(0.001f, 0.001f, 0.001f);
        //Debug.Log("start scale is: " + startScale.x);

        float elapsedTime = 0;
        Vector3 maxPulseVector = new Vector3( maxPulse,maxPulse, maxPulse);
        while(elapsedTime < pulseDuration){
            float t = elapsedTime/pulseDuration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f);
            pulseLight.pointLightOuterRadius = Mathf.Lerp(0, maxPulse * 2.5f, t);
            pulseLight.intensity = Mathf.Lerp(1f, 0f, t);
            pulseScanner.transform.localScale = Vector3.Lerp(startScale, maxPulseVector, t);
            pulseScanner.GetComponent<SpriteRenderer>().color = Color.Lerp(pulseColor, pulseColorFadeTarget, elapsedTime/pulseFadeDuration); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        //Debug.Log("end scale is: " + pulseScanner.transform.localScale.x);
        pulseScanner.GetComponent<SpriteRenderer>().color = pulseColor;
        pulseScanner.transform.localScale = new Vector3 (startScale.x, startScale.y, startScale.z);
        pulseScanner.SetActive(false);
    }
    void OnTriggerEnter2D(Collider2D other){
    
    }
    void OnCollisionEnter2D(Collision2D other){
        if(other.gameObject.tag == "ob"){
            AudioManager.AM.PlayRandomRigHit();
        }
    }
    void OnTriggerExit2D(Collider2D other){ 
    }
   
   
    // 5. If they clicked Take (UIM) and they could fit it, take it
    public void TakeScrap(ScrapObject takenScrap){
        takenScrap.gameObject.SetActive(false);
        playerScrap.Add(takenScrap);
        UpdateCurrentHaul();
        foreach(ScrapObject scrap in playerScrap){
//            Debug.Log("Player has: " + scrap.GetComponent<ScrapObject>().scrapName);
        }
        if(firstScrapTaken == false && Director.Dir.introCompleted && !DialogueManager.DM.isDialogueRunner1Running){
            DialogueManager.DM.RunNode("tutorial-take-scrap");
            firstScrapTaken = true;
        }
    }
    public void DropScrap(int tickScrapIndex){
        //Debug.Log("Index Called: " + tickScrapIndex);
        //Debug.Log("Object at index on player: " + playerScrap[tickScrapIndex].scrapName);
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
