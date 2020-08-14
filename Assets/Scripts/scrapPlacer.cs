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
     [] scatter the scrap around the map
     [] pick a random piece of scrap to spawn each time we instantiate
     [] distribute according to zone rarity

    */


   public JsonReader JsonReader;
   public bool spawning = true;
   public GameObject sampleScrap;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(spawning){
            foreach (Scrap scrap in JsonReader.scrapInJson.allScrap){

                sampleScrap.GetComponent<ScrapObject>().scrapName = scrap.scrapName;
                sampleScrap.GetComponent<ScrapObject>().description = scrap.description;
                sampleScrap.GetComponent<ScrapObject>().image = scrap.image;
                sampleScrap.GetComponent<ScrapObject>().size = scrap.size;
                sampleScrap.GetComponent<ScrapObject>().value = scrap.value;
                sampleScrap.GetComponent<ScrapObject>().material = scrap.material;
                sampleScrap.GetComponent<ScrapObject>().zoneA_rarity = scrap.zoneA_rarity;
                sampleScrap.GetComponent<ScrapObject>().zoneB_rarity = scrap.zoneB_rarity;
                sampleScrap.GetComponent<ScrapObject>().zoneC_rarity = scrap.zoneC_rarity;
                sampleScrap.GetComponent<ScrapObject>().zoneD_rarity = scrap.zoneD_rarity;
                sampleScrap.GetComponent<ScrapObject>().carriesComponents = scrap.carriesComponents;
                sampleScrap.GetComponent<ScrapObject>().isBuried = scrap.isBuried;
                
                GameObject copiedScrap = (Instantiate(sampleScrap));
                copiedScrap.transform.parent = gameObject.transform;
            }  
            spawning = false;
        }
    }
 }
