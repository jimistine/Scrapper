using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{

    public TextMeshProUGUI haulText;
    public PlayerManager PlayerManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public ScrapObject ShowScrap(ScrapObject newScrap){
        Debug.Log("Found: " + newScrap.scrapName);
        newScrap.GetComponent<SpriteRenderer>().enabled = true;
        haulText.text = "Current Haul: " + PlayerManager.currentHaul.ToString() + " m<sup>3</sup>";
        Destroy(newScrap.gameObject);
        return newScrap;
    }
    
}
