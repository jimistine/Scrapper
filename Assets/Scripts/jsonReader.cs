using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JsonReader : MonoBehaviour
{
    public TextAsset jsonFile;
    public AllScrap scrapInJson;

    // Start is called before the first frame update
    void Start()
    {
       scrapInJson = JsonUtility.FromJson<AllScrap>(jsonFile.text);

        // foreach (Scrap scrap in scrapInJson.allScrap){
        //     Debug.Log("Found scrap: " + scrap.scrapName + ". With description: " + scrap.description);
        //}
    }

}
