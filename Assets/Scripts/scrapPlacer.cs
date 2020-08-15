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

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //for(int j = 0; j < totalSpawnableScrap; j++){
        if(counter < totalSpawnableScrap){
            // pick a random number within the total number of scrap in our list
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
            // if distance between position and any other piece is < .5, generate a new position
            // iterate through all scrap positions
                for(int i = 0; i < spawnedScrap.Length; i++){
                    // if it's less than the min distance away from any piece,
                    // generate a new position and start checking from the top again
                    if(Vector3.Distance(position, spawnedScrap[i].transform.position) <= minDistance){
                        position = new Vector3(Random.Range(-spawningBoundX, spawningBoundX), Random.Range(-spawningBoundY, spawningBoundY), 0);
                        i = 0;
                    }   
                }
            // spawn that $hit
            GameObject copiedScrap = (Instantiate(sampleScrap, position, Quaternion.identity));
            copiedScrap.transform.parent = gameObject.transform;
            counter++;
        }
    }
 }
