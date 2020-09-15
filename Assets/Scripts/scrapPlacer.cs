using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [] don't place scrap anywhere the player can't get to
    */


   public JsonReader JsonReader;
   public bool spawning = true;
   public bool placing = true;
   public GameObject sampleScrap;
   public int totalSpawnableScrap;
   public int totalLiveScrap;
   public float averageScrapValuePerM3;
   public float scrapRespawnBuffer;
   public float minDistance;
   public float spawningBoundX;
   public float spawningBoundY;
   Rect spawningRect;
   int counter = 0;
   float totalSpawningWeight;
   float randomScrapPick;
   Scrap incomingScrap;
   Random shuffleRandom;
   float totalScrapValue;
   float totalScrapVolume;
   GameObject[] currentLiveScrap;
   RaycastHit2D scrapPosRay;
   Vector3 position;
   bool generatingPosition;

    // Start is called before the first frame update
    public static scrapPlacer SP;
    void Awake(){
        SP = this;
    }
    
    void Start(){
        SpawnScrap(totalSpawnableScrap);
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
    
    void UpdateAverageScrapValue(){
        foreach(GameObject scrap in currentLiveScrap){
            totalScrapValue += scrap.GetComponent<ScrapObject>().value;
            totalScrapVolume += scrap.GetComponent<ScrapObject>().size;
        }
        averageScrapValuePerM3 = totalScrapValue / totalScrapVolume;
    }

    void SpawnScrap(int scrapToSpawn){
        for(int j = 0; j < scrapToSpawn; j++){
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
                    //Debug.Log("Spawning scrap: " + incomingScrap.scrapName);
                    break;
                }
            }
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
            sampleScrap.GetComponent<ScrapObject>().zoneD_rarity = incomingScrap.zoneD_rarity;
            sampleScrap.GetComponent<ScrapObject>().carriesComponents = incomingScrap.carriesComponents;
            sampleScrap.GetComponent<ScrapObject>().isBuried = incomingScrap.isBuried;
            // generate a position within the bounds of the map
            generatingPosition = true;
            while(generatingPosition){
                position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
                // position = new Vector3(Random.Range(Mathf.RoundToInt(spawningRect.xMin), Mathf.RoundToInt(spawningRect.xMax)), 
                //                        Random.Range(Mathf.RoundToInt(spawningRect.yMin), Mathf.RoundToInt(spawningRect.yMax)), 0);
                Vector2 pos2D = new Vector2(position.x, position.y);
                scrapPosRay = Physics2D.Raycast(pos2D, Vector2.zero);
                if(scrapPosRay.collider == null){
                    generatingPosition = false;
                }
                else if(scrapPosRay.collider.tag == "ob"){
                    generatingPosition = true;
                }
            }
            // get location of everything with tag scrap
            GameObject[] spawnedScrap = GameObject.FindGameObjectsWithTag("Scrap");
            // if distance between position and any other scrap < minimum distance, generate a new position
            for(int i = 0; i < spawnedScrap.Length; i++){
                if(Vector3.Distance(position, spawnedScrap[i].transform.position) <= minDistance){
                    position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
                    // position = new Vector3(Random.Range(Mathf.RoundToInt(spawningRect.xMin), Mathf.RoundToInt(spawningRect.xMax)), 
                    //                        Random.Range(Mathf.RoundToInt(spawningRect.yMin), Mathf.RoundToInt(spawningRect.yMax)), 0);
                    i = 0;
                }   
            }
            // spawn that $hit
            //Debug.Log("Spawning scrap on " + scrapPosRay.collider.tag);
            GameObject copiedScrap = (Instantiate(sampleScrap, position, Quaternion.identity));
            copiedScrap.transform.parent = gameObject.transform;
            copiedScrap.name = copiedScrap.GetComponent<ScrapObject>().scrapName;
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
            droppedScrapObj.GetComponent<ScrapObject>().zoneD_rarity = droppedScrap.zoneD_rarity;
            droppedScrapObj.GetComponent<ScrapObject>().carriesComponents = droppedScrap.carriesComponents;
            droppedScrapObj.GetComponent<ScrapObject>().isBuried = droppedScrap.isBuried;
            droppedScrapObj.SetActive(true);
            Instantiate(droppedScrapObj, PlayerManager.PM.gameObject.transform.position, Quaternion.identity);
            droppedScrap.transform.parent = PlayerManager.PM.gameObject.transform;
    }
 }
