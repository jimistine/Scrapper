using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class UIManager : MonoBehaviour
{

    /*
        Bugs
        [] Player moves when clicking on UI
            - move all interactions to player
            - rout all clicks accordingly
            - only run click to move if the player clicks on object with tag terrain
            - click to move lives in a coroutine?
            ^^ did all this and none of it works! Ha! Haha!
               current fix stops player from moving at all while readout is open
        [x] Scrap spawns behind the map...
        [x] Player spawns behind the map 
        [] Need better memory of different scrap instead of treating every scrap like it's the one you found
            most recently
            - 
    
    */

    public static UIManager UIM;

    [Header("Scripts")]
    [Space(5)]
    public PlayerManager PlayerManager;
    public fuel fuleManager;

    [Header("Statbar")]
    [Space(5)]
    public TextMeshProUGUI haulText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI creditText;
    public GameObject scrapTick;
    public GameObject scrapTickSlots;

    [Header("Readout")]
    [Space(5)]
    public GameObject readoutPanel;
    public TextMeshProUGUI readoutName;
    public TextMeshProUGUI readoutDesc;
    public TextMeshProUGUI readoutSize;
    public TextMeshProUGUI readoutValue;
    public Button readoutTakeButt;
    public Button readoutLeaveButt;
    
    [Header("Other")]
    [Space(5)]
    public Button townButton;


    ScrapObject scrapToTake;
    
    void Awake(){
        UIM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        fuleManager = PlayerManager.PM.GetComponent<fuel>();
        PlayerManager = PlayerManager.PM;
    }

    // Update is called once per frame
    void Update()
    {
        fuelText.text = "Fuel: " + fuleManager.currentFuelPercent.ToString("N0") + "%";
    }

    // 1. PlayerManager has found scrap 
    public ScrapObject ShowScrap(ScrapObject newScrap){
        Debug.Log("Showing: " + newScrap.scrapName);
        newScrap.GetComponent<SpriteRenderer>().enabled = true;
        scrapToTake = newScrap;
        return newScrap;
    }

    // 2. Player has clicked on scrap
    public ScrapObject ShowReadout(ScrapObject newScrap){
        scrapToTake = newScrap;
        readoutName.text = newScrap.scrapName;
        readoutDesc.text = newScrap.description;
        readoutSize.text = string.Format("Size: {0:#,#}", newScrap.size + " m<sup>3</sup>");
        readoutValue.text = newScrap.value.ToString("Value: " + "#,#" + " cr.");
        readoutPanel.SetActive(true);
        // stop the player from moving because how the hell do you actually get UI to block a raycast???
        PlayerManager.GetComponentInParent<clickToMove>().enabled = false;
        return newScrap;
    }

    // 3. Player clicks "take" or "leave" and we do what they tell us
    public void TakeScrap(){
        PlayerManager.TakeScrap(scrapToTake);
        haulText.text = "Current Haul: " + PlayerManager.currentHaul.ToString("#,#") + " m<sup>3</sup>";
        scrapTick = Instantiate(scrapTick) as GameObject;
        scrapTick.transform.SetParent(scrapTickSlots.transform, false);
        readoutPanel.SetActive(false);
        PlayerManager.GetComponentInParent<clickToMove>().enabled = true;
    }
    public void LeaveScrap(){
        readoutPanel.SetActive(false);
        PlayerManager.GetComponentInParent<clickToMove>().enabled = true;

    }

    public void OfferTown(){
        townButton.gameObject.SetActive(true);
    }
    public void EnterTown(){
        townButton.gameObject.SetActive(false);
    }
    
}
