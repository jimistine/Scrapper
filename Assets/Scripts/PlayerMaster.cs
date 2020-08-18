using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerMaster : MonoBehaviour
{

    public fuel Fuel;
    public clickToMove ClickToMove;

    public float currentSpeed;
    public float fuelLevel;

    // Invetory
    public float currentHaul;
    public float maxHaul;
    public float playerCredits;
    public List<ScrapObject> playerScrap = new List<ScrapObject>();
    public TextMeshProUGUI haulText;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        fuelLevel = Fuel.currentFuelLevel;
        currentSpeed = ClickToMove.currentSpeed;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Scrap"){
            ScrapObject newScrap = other.GetComponent<ScrapObject>();
            Debug.Log("Found: " + newScrap.scrapName);
            other.GetComponent<SpriteRenderer>().enabled = true;
            playerScrap.Add(newScrap);
            currentHaul += newScrap.size;
            haulText.text = "Current Haul: " + currentHaul.ToString() + " m<sup>3</sup>";
            Destroy(newScrap.gameObject);
        }
    }
}
