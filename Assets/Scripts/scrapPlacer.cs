﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

[System.Serializable]

public class scrapPlacer : MonoBehaviour
{
    /*
    Ok what the hell am I doing?
    [x] Look at the list of scrap from the json
    [x] instantiate prefabs for each scrap on the list
    [x] scatter the scrap around the map
    [x] spawn each piece more than .25 units away from any other scrap
    [x] spawn multiples of each scrap to cover all of map
    [x] pick a random piece of scrap to spawn each time we instantiate
    [x] move all that mess in start to a function
    [x] spawn based on rarity
    [] spawn based on seed
    [] distribute according to zone rarity
    [x] don't place scrap anywhere the player can't get to
    */


   public JsonReader JsonReader;

   public GameObject sampleScrap;
   public GameObject rareScrap;
   public GameObject scrapGlow;

   public int totalSpawnableScrap;
   public int totalLiveScrap;
   public float averageScrapValuePerM3;
   public float scrapRespawnBuffer;
   public float minDistance;
   public float spawningBoundX;
   public float spawningBoundY;
   public Object[] scrapShadows;
   public float scrapShadowScaler;
   public GameObject[] placedScrap;

   Rect spawningRect;
   int counter = 0;
   float totalSpawningWeight;
   float randomScrapPick;
   Scrap incomingScrap;
   Random shuffleRandom;
   float totalScrapValue;
   float totalScrapVolume;
   public GameObject[] currentLiveScrap;
   
   RaycastHit2D scrapPosRay;
   Vector3 position;
   bool generatingPosition;
   bool spawnedPlacedScrap;

    // Start is called before the first frame update
    public static scrapPlacer SP;
    void Awake(){
        SP = this;
    }
    
    void Start(){
        scrapShadows = Resources.LoadAll("Sprites/Blobs", typeof(Sprite));
        SpawnScrap(totalSpawnableScrap);
        SpawnPlacedScrap();
        spawningRect = gameObject.GetComponent<RectTransform>().rect;
        spawningBoundX = Mathf.RoundToInt(spawningRect.xMax);
        spawningBoundY = Mathf.RoundToInt(spawningRect.yMax);
        Debug.Log("Spawning bounds X: " + spawningRect.xMin + ", " + spawningRect.xMax + " | Y: " + spawningRect.yMin + ", " + spawningRect.yMax);
    }
    void Update(){
        currentLiveScrap = GameObject.FindGameObjectsWithTag("Scrap");
        if(currentLiveScrap.Count() < (totalSpawnableScrap * scrapRespawnBuffer)){
            int scrapToRespawn = totalSpawnableScrap - currentLiveScrap.Count();
            SpawnScrap(scrapToRespawn);
            UpdateAverageScrapValue();
        }

        totalLiveScrap = currentLiveScrap.Count();
    }
    
    public void UpdateAverageScrapValue(){
        foreach(GameObject scrap in currentLiveScrap){
            totalScrapValue += scrap.GetComponent<ScrapObject>().value;
            totalScrapVolume += scrap.GetComponent<ScrapObject>().size;
        }
        averageScrapValuePerM3 = totalScrapValue / totalScrapVolume;
    }

    float GenerateScrapID(){
        float newID = Random.Range(0f,100000f);
        foreach(GameObject scrap in currentLiveScrap){
            if(newID == scrap.GetComponent<ScrapObject>().ID){
                while(newID == scrap.GetComponent<ScrapObject>().ID){
                    newID = Random.Range(0f,100000f);  
                }
            }
        }
        return newID;
    }

    void SpawnScrap(int scrapToSpawn){
        for(int j = 0; j < scrapToSpawn; j++){

        // to gaurentee at least one of each piece is spawned
            // Count all the unique scrap items
            int numUniqueScrap = JsonReader.scrapInJson.allScrap.Count();
            // If the count is <= that number, spawn the scrap at the index equal to the current count
            if (j < numUniqueScrap){
                incomingScrap = JsonReader.scrapInJson.allScrap[j];
            }
            // else, do the weighted spawn thing like normal
            else{
                // find the highest weight of all scrap, that is, the most common
                int maxWeight = JsonReader.scrapInJson.allScrap.Max(x => x.zoneA_rarity);
                // pick a number somwhere inbetween that max and 0
                randomScrapPick = Random.Range(0, maxWeight);
                // shuffle the array so everyone gets a chance
                RandomExtensions.Shuffle(shuffleRandom, JsonReader.scrapInJson.allScrap);
                // iterate through all the scrap in our list and pick the first one that's weighted higher than 
                //   our random value and exit the loop.
                foreach(Scrap scrap in JsonReader.scrapInJson.allScrap){
                    if(scrap.zoneA_rarity >= randomScrapPick){
                        incomingScrap = scrap;
                        break;
                    }
                }
            }
            //Debug.Log("Spawning scrap: " + incomingScrap.scrapName);
            // transfer that sweet sweet data to prefab
            sampleScrap.GetComponent<ScrapObject>().scrapName = incomingScrap.scrapName;
            sampleScrap.GetComponent<ScrapObject>().description = incomingScrap.description;
            sampleScrap.GetComponent<ScrapObject>().image = incomingScrap.image;
            sampleScrap.GetComponent<ScrapObject>().size = incomingScrap.size;
            sampleScrap.GetComponent<ScrapObject>().value = incomingScrap.value;
            sampleScrap.GetComponent<ScrapObject>().material = incomingScrap.material;
            sampleScrap.GetComponent<ScrapObject>().zoneA_rarity = incomingScrap.zoneA_rarity;
            sampleScrap.GetComponent<ScrapObject>().zoneB_rarity = incomingScrap.zoneB_rarity;
            sampleScrap.GetComponent<ScrapObject>().zoneC_rarity = incomingScrap.zoneC_rarity;
            sampleScrap.GetComponent<ScrapObject>().ID = incomingScrap.ID;
            sampleScrap.GetComponent<ScrapObject>().carriesComponents = incomingScrap.carriesComponents;
            sampleScrap.GetComponent<ScrapObject>().isBuried = incomingScrap.isBuried;

            // generate a position within the bounds of the map in a spawnable area
            generatingPosition = true;
            while(generatingPosition){
                position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
                Vector2 pos2D = new Vector2(position.x, position.y);
                scrapPosRay = Physics2D.Raycast(pos2D, Vector2.zero);
                if(scrapPosRay.collider == null){
                    generatingPosition = false;
                }
                else if(scrapPosRay.collider.tag == "ob" || scrapPosRay.collider.tag == "Non-spawnable"){
                    generatingPosition = true;
                }
            }
            // get location of everything with tag scrap
            GameObject[] spawnedScrap = GameObject.FindGameObjectsWithTag("Scrap");
            // if distance between position and any other scrap < minimum distance, generate a new position
            for(int i = 0; i < spawnedScrap.Length; i++){
                if(Vector3.Distance(position, spawnedScrap[i].transform.position) <= minDistance){
                    position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
                   
                    i = 0;
                }   
            }
            // spawn that $hit
            GameObject copiedScrap = (Instantiate(sampleScrap, position, Quaternion.Euler(0, 0, Random.Range(0f, 360f))));
            copiedScrap.transform.parent = gameObject.transform;
            copiedScrap.name = copiedScrap.GetComponent<ScrapObject>().scrapName;
            copiedScrap.GetComponent<ScrapObject>().ID = GenerateScrapID();
            // scale and swap the shadow sprite
            float scrapScale = copiedScrap.GetComponent<ScrapObject>().size/scrapShadowScaler;
            if(scrapScale > 5){scrapScale = 5;}
            copiedScrap.transform.localScale = new Vector3(scrapScale, scrapScale, scrapScale);
            int blobIndex = Random.Range(0, scrapShadows.Count());
            copiedScrap.GetComponent<SpriteRenderer>().sprite = (Sprite)scrapShadows[blobIndex];
            //copiedScrap.GetComponentsInChildren<SpriteRenderer>()[1].sprite = (Sprite)scrapShadows[blobIndex];
            //Debug.Log("Scrap child name: "+ copiedScrap.GetComponentsInChildren<SpriteRenderer>()[1].gameObject.name);
            //double value if we want
            if(Director.Dir.doubleScrapValue){
                copiedScrap.GetComponent<ScrapObject>().value *= 2; 
            }
            // and add a glow if it's this specific one.
            if(copiedScrap.name == "Chunk of raw cordonite"){
                GameObject glowToAttach = (GameObject) Instantiate(scrapGlow, copiedScrap.transform.position, copiedScrap.transform.rotation);
                glowToAttach.transform.parent = copiedScrap.transform;
            }
        /*
            //check if it has components
            if (copiedScrap.GetComponent<ScrapObject>().carriesComponents){
                // iterate through all components, check their rarity and roll the dice
                // if it has a component, mark that component's presence as true
                // does this mean we have to add every component's rarity and wheather or not it spawns
                  // for every scrap? That's 10 extra variables...
            }
        */    
            counter++;
        }
    }

    public void SpawnPlacedScrap(){
        placedScrap = GameObject.FindGameObjectsWithTag("placed");

        foreach(GameObject placedObj in placedScrap){

            if(placedObj.name == "Chunk of raw cordonite"){
                incomingScrap = System.Array.Find(JsonReader.scrapInJson.allScrap, x => x.scrapName == "Chunk of raw cordonite");
                Debug.Log("Found chunk of cordonite.");
            }
            else{
                // find the highest weight of all scrap, that is, the most common
                int maxWeight = JsonReader.scrapInJson.allScrap.Max(x => x.zoneA_rarity);
                // pick a number somwhere inbetween max and 0
                randomScrapPick = Random.Range(0, maxWeight);
                // shuffle the array so everyone gets a chance
                RandomExtensions.Shuffle(shuffleRandom, JsonReader.scrapInJson.allScrap);
                // iterate through all the scrap in our list and pick the first one that's weighted higher than 
                //   our random value and our rarity threshold of 45 and exit the loop.
                foreach(Scrap scrap in JsonReader.scrapInJson.allScrap){
                    if(scrap.zoneA_rarity >= randomScrapPick && scrap.zoneA_rarity <= 45){
                        incomingScrap = scrap;
                        Debug.Log("Spawning placed scrap: " + incomingScrap.scrapName);
                        break;
                    }
                }
            }
            // transfer that sweet sweet data to prefab
            rareScrap.GetComponent<ScrapObject>().scrapName = incomingScrap.scrapName;
            rareScrap.GetComponent<ScrapObject>().description = incomingScrap.description;
            rareScrap.GetComponent<ScrapObject>().image = incomingScrap.image;
            rareScrap.GetComponent<ScrapObject>().size = incomingScrap.size;
            rareScrap.GetComponent<ScrapObject>().value = incomingScrap.value;
            rareScrap.GetComponent<ScrapObject>().material = incomingScrap.material;
            rareScrap.GetComponent<ScrapObject>().zoneA_rarity = incomingScrap.zoneA_rarity;
            rareScrap.GetComponent<ScrapObject>().zoneB_rarity = incomingScrap.zoneB_rarity;
            rareScrap.GetComponent<ScrapObject>().zoneC_rarity = incomingScrap.zoneC_rarity;
            rareScrap.GetComponent<ScrapObject>().ID = incomingScrap.ID;
            rareScrap.GetComponent<ScrapObject>().carriesComponents = incomingScrap.carriesComponents;
            rareScrap.GetComponent<ScrapObject>().isBuried = incomingScrap.isBuried;

            GameObject placedScrap = (Instantiate(rareScrap, placedObj.transform.position, Quaternion.identity));
            placedScrap.transform.parent = gameObject.transform;
            placedScrap.name = placedScrap.GetComponent<ScrapObject>().scrapName;
            placedScrap.GetComponent<ScrapObject>().ID = GenerateScrapID();
            // scale and swap the shadow sprite
            float scrapScale = placedScrap.GetComponent<ScrapObject>().size/scrapShadowScaler;
            if(scrapScale > 5){scrapScale = 5;}
            placedScrap.transform.localScale = new Vector3(scrapScale, scrapScale, scrapScale);
            int blobIndex = Random.Range(0, scrapShadows.Count());
            placedScrap.GetComponent<SpriteRenderer>().sprite = (Sprite)scrapShadows[blobIndex];
            
            if(placedScrap.name == "Chunk of raw cordonite"){
                GameObject glowToAttach = (GameObject) Instantiate(scrapGlow, placedScrap.transform.position, placedScrap.transform.rotation);
                glowToAttach.transform.parent = placedScrap.transform;
            }
        }
    }

    public void SpawnDroppedScrap(ScrapObject droppedScrap){
            Debug.Log("Dropping a " + droppedScrap.scrapName);
            GameObject droppedScrapObj = sampleScrap;
            droppedScrapObj.GetComponent<ScrapObject>().scrapName = droppedScrap.scrapName;
            droppedScrapObj.GetComponent<ScrapObject>().scrapName = droppedScrap.scrapName;
            droppedScrapObj.GetComponent<ScrapObject>().description = droppedScrap.description;
            droppedScrapObj.GetComponent<ScrapObject>().image = droppedScrap.image;
            droppedScrapObj.GetComponent<ScrapObject>().size = droppedScrap.size;
            droppedScrapObj.GetComponent<ScrapObject>().value = droppedScrap.value;
            droppedScrapObj.GetComponent<ScrapObject>().material = droppedScrap.material;
            droppedScrapObj.GetComponent<ScrapObject>().zoneA_rarity = droppedScrap.zoneA_rarity;
            droppedScrapObj.GetComponent<ScrapObject>().zoneB_rarity = droppedScrap.zoneB_rarity;
            droppedScrapObj.GetComponent<ScrapObject>().zoneC_rarity = droppedScrap.zoneC_rarity;
            droppedScrapObj.GetComponent<ScrapObject>().ID = droppedScrap.ID;
            droppedScrapObj.GetComponent<ScrapObject>().carriesComponents = droppedScrap.carriesComponents;
            droppedScrapObj.GetComponent<ScrapObject>().isBuried = false;
            droppedScrapObj.SetActive(true);
            Instantiate(droppedScrapObj, PlayerManager.PM.gameObject.transform.position, Quaternion.identity);
            //droppedScrap.transform.parent = PlayerManager.PM.gameObject.transform;
            droppedScrap.transform.parent = gameObject.transform;

            droppedScrap.gameObject.name = droppedScrap.GetComponent<ScrapObject>().scrapName;
            droppedScrap.ID = GenerateScrapID();
            // scale and swap the shadow sprite
            float scrapScale = droppedScrap.size/scrapShadowScaler;
            if(scrapScale > 5){scrapScale = 5;}
            droppedScrap.gameObject.transform.localScale = new Vector3(scrapScale, scrapScale, scrapScale);
            int blobIndex = Random.Range(0, scrapShadows.Count());
            droppedScrap.gameObject.GetComponent<SpriteRenderer>().sprite = (Sprite)scrapShadows[blobIndex];
    }
 }
