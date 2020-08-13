using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[System.Serializable]

public class scrapPlacer : MonoBehaviour
{

   public ScrapObject[] spawnedScrap;
   public JsonReader JsonReader;

    // Start is called before the first frame update
    void Start()
    {
        foreach (Scrap scrap in JsonReader.scrapInJson.allScrap){
            Debug.Log("Found scrap: " + scrap.scrapName + ". With description: " + scrap.description);
        }
        
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
