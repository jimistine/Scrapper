using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class jsonReader : MonoBehaviour
{
    public TextAsset jsonFile;

    // Start is called before the first frame update
    void Start()
    {
        scrapAll scrapInJson = JsonUtility.FromJson<scrapAll>(jsonFile.text);

        foreach (scrap scrap in scrapInJson.allScrap){
            Debug.Log("Found scrap: " + scrap.name + ". With description " + scrap.description);
        }
    }

}
