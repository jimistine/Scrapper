using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class ProximityCheck : MonoBehaviour
{
    public bool interactable;
    public bool fading;
    public float minDistance;
    public float waitToMarkOutOfRangeTime;
    public float lightFadeDuration;
    public float fadeOutDuration;
    public float maxLightIntensity;
    public Color inRangeColor;
    public Color outOfRangeColor;
    public bool scanned;
    public bool playerFound;
    public bool inspected;

    Light2D scrapLight;
    
    // public void IsInRange(bool inRange){
    //     if(inRange){
    //         interactable = true;
    //     }
    //     else{
    //         interactable = false;
    //         if(!fading && gameObject.activeSelf){
    //             StartCoroutine(FadeScrap());
    //         }
    //     }
    // }
    void Start(){
        scrapLight = gameObject.GetComponent<Light2D>();
    }
    void Update(){
        if(interactable){
            StopCoroutine(FadeScrap());
            gameObject.GetComponent<SpriteRenderer>().color = inRangeColor;
        }
        if(Vector3.Distance(PlayerManager.PM.gameObject.transform.position, gameObject.transform.position) <= GetComponent<CircleCollider2D>().radius){
            interactable = true;
            //Debug.Log("Distance from interactable scrap: " + Vector3.Distance(PlayerManager.PM.gameObject.transform.position, gameObject.transform.position));
        }
        else{
            interactable = false;
        }
    }

    IEnumerator LightAndFadeScrap(){
        //scrapLight.enabled = true;
        scrapLight.intensity = maxLightIntensity;

        //SpriteRenderer scrapShadow = gameObject.GetComponentInChildren<SpriteRenderer>();

        float elapsedTime = 0;
        while(elapsedTime < lightFadeDuration){
            scrapLight.intensity = Mathf.Lerp(maxLightIntensity, 0, elapsedTime/lightFadeDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        //scrapLight.enabled = false;
    }

    IEnumerator FadeScrap(){
        fading = true;
        float elapsedTime = 0;
        yield return new WaitForSeconds(waitToMarkOutOfRangeTime);
        while(elapsedTime < fadeOutDuration){
            gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(inRangeColor, outOfRangeColor, elapsedTime/fadeOutDuration); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }
    void OnTriggerEnter2D(Collider2D other){
        if(PlayerManager.PM.scannerActive && (other.gameObject.name == "Player" || other.gameObject.name == "Scanner")){

            // What do we do if the scanner or the player found the scrap?
            gameObject.GetComponent<SpriteRenderer>().color = inRangeColor;

            // What do we do when the player gets close?
            if(other.gameObject.name == "Player"){
                Debug.Log("Player trigger found: " + gameObject.name);
                interactable = true;
                UIManager.UIM.ShowScrap(gameObject.GetComponent<ScrapObject>());

                if(playerFound == false){
                    if(gameObject.name == "Land speeder (unknown)"){
                        DialogueManager.DM.RunNode("land-speeder");
                    }
                    else if(gameObject.name == "Chunk of raw cordonite"){
                        DialogueManager.DM.RunNode("chunk-of-raw-cordonite");
                    }
                    else if(gameObject.name == "Sha'ak-ji Holospace Generator"){
                        DialogueManager.DM.RunNode("holospace-generator");
                    }
                    else if(PlayerManager.PM.firstScrapFound == false && Director.Dir.introCompleted && !DialogueManager.DM.isDialogueRunner1Running){
                        DialogueManager.DM.RunNode("tutorial-find-scrap");
                        PlayerManager.PM.firstScrapFound = true;
                    }
                    else{
                        DialogueManager.DM.RunNode("scrap-find");
                    }   
                }
                playerFound = true;
            }
            else if(other.gameObject.name == "Scanner"){// What do we do when the scanner finds the scrap?
                AudioManager.AM.PlayPlayerClip("found scrap");
                Debug.Log("Scanner trigger entered.");
                scanned = true;
                StopCoroutine(LightAndFadeScrap());
                StartCoroutine(LightAndFadeScrap());
            }
            // This also happens no matter what.
            scanned = true;
        }
    }
    void OnTriggerExit2D(Collider2D other){
        // Only once the player's smaller collider has left
        if(other.GetType()==typeof(EdgeCollider2D) && interactable == true){
            interactable = false;
           // UIManager.UIM.OutOfRangeScrap(gameObject.GetComponent<ScrapObject>());
        }
        if(!fading && gameObject.activeSelf){
            StopCoroutine(FadeScrap());
            StartCoroutine(FadeScrap());
        }
    }
}
