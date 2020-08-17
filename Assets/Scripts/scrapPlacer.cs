using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    [] spawn based on seed and rarity
    [] distribute according to zone rarity
    [] don't place scrap anywhere the player can't get to
    */


   public JsonReader JsonReader;
   public bool spawning = true;
   public bool placing = true;
   public GameObject sampleScrap;
   public int totalSpawnableScrap;
   public float minDistance;
   public float spawningBoundX;
   public float spawningBoundY;
   int counter = 0;
   float totalSpawningWeight;
   float randomScrapPick;

    // Start is called before the first frame update
    void Start()
    {
        for(int j = 0; j < totalSpawnableScrap; j++){
            // pick a random number within the total number of scrap in our list
            // ** This is where we look at all their weights first and then picked based on that
            //    Or do we want to bring all the scrap in as GameObjects first before we do any of this?
            foreach(Scrap scrap in JsonReader.scrapInJson.allScrap){
                totalSpawningWeight += scrap.zoneD_rarity;
            }

            randomScrapPick = Random.Range(0, Mathf.Max(JsonReader.scrapInJson.allScrap.zoneD_rarity));

            foreach(Scrap scrap in JsonReader.scrapInJson.allScrap){

            }



            int scrapToPick = Random.Range(0, JsonReader.scrapInJson.allScrap.Length);
            // grab the scrap at that position 
            Scrap incomingScrap = JsonReader.scrapInJson.allScrap[scrapToPick];
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
            Vector3 position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
            // get location of everything with tag scrap
            GameObject[] spawnedScrap = GameObject.FindGameObjectsWithTag("Scrap");
            // if distance between position and any other piece is < minimum distance, generate a new position
                for(int i = 0; i < spawnedScrap.Length; i++){
                    if(Vector3.Distance(position, spawnedScrap[i].transform.position) <= minDistance){
                        position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
                        i = 0;
                    }   
                }
            // spawn that $hit
            GameObject copiedScrap = (Instantiate(sampleScrap, position, Quaternion.identity));
            copiedScrap.transform.parent = gameObject.transform;
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

    // Update is called once per frame
    void Update()
    {
        // //for(int j = 0; j < totalSpawnableScrap; j++){
        // if(counter < totalSpawnableScrap){
        //     // pick a random number within the total number of scrap in our list
        //     int scrapToPick = Random.Range(0, JsonReader.scrapInJson.allScrap.Length);
        //     // grab the scrap at that position 
        //     Scrap incomingScrap = JsonReader.scrapInJson.allScrap[scrapToPick];
        //     // transfer that sweet sweet data to prefab
        //     sampleScrap.GetComponent<ScrapObject>().scrapName = incomingScrap.scrapName;
        //     sampleScrap.GetComponent<ScrapObject>().description = incomingScrap.description;
        //     sampleScrap.GetComponent<ScrapObject>().image = incomingScrap.image;
        //     sampleScrap.GetComponent<ScrapObject>().size = incomingScrap.size;
        //     sampleScrap.GetComponent<ScrapObject>().value = incomingScrap.value;
        //     sampleScrap.GetComponent<ScrapObject>().material = incomingScrap.material;
        //     sampleScrap.GetComponent<ScrapObject>().zoneA_rarity = incomingScrap.zoneA_rarity;
        //     sampleScrap.GetComponent<ScrapObject>().zoneB_rarity = incomingScrap.zoneB_rarity;
        //     sampleScrap.GetComponent<ScrapObject>().zoneC_rarity = incomingScrap.zoneC_rarity;
        //     sampleScrap.GetComponent<ScrapObject>().zoneD_rarity = incomingScrap.zoneD_rarity;
        //     sampleScrap.GetComponent<ScrapObject>().carriesComponents = incomingScrap.carriesComponents;
        //     sampleScrap.GetComponent<ScrapObject>().isBuried = incomingScrap.isBuried;

        //     // generate a position within the bounds of the map
        //     Vector3 position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
        //     // get location of everything with tag scrap
        //     GameObject[] spawnedScrap = GameObject.FindGameObjectsWithTag("Scrap");
        //     // if distance between position and any other piece is < minimum distance, generate a new position
        //         for(int i = 0; i < spawnedScrap.Length; i++){
        //             if(Vector3.Distance(position, spawnedScrap[i].transform.position) <= minDistance){
        //                 position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
        //                 i = 0;
        //             }   
        //         }
        //     // spawn that $hit
        //     GameObject copiedScrap = (Instantiate(sampleScrap, position, Quaternion.identity));
        //     copiedScrap.transform.parent = gameObject.transform;
        // /*
        //     //check if it has components
        //     if (copiedScrap.GetComponent<ScrapObject>().carriesComponents){
        //         // iterate through all components, check their rarity and roll the dice
        //         // if it has a component, mark that component's presence as true
        //         // does this mean we have to add every component's rarity and wheather or not it spawns
        //           // for every scrap? That's 10 extra variables...
        //     }
        // */    
        //     counter++;
        // }
    }
 }
