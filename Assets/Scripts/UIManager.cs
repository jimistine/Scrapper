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
    public SceneController SceneController;
    public fuel fuleManager;
    public MerchantManager MerchantManager;

    [Header("Statbar")]
    [Space(5)]
    public GameObject statBar;
    public TextMeshProUGUI haulText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI creditText;
    public GameObject scrapTick;
    public GameObject scrapTickBackup;
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
    
    [Header("Town")]
    [Space(5)]
    public GameObject TownUI;
    public Button enterTownButton;
    public TextMeshProUGUI scrapMerchantWelcome;
    public TextMeshProUGUI scrapMerchantReadout;
    public TextMeshProUGUI fuelMerchantWelcome;
    public TextMeshProUGUI fuelMerchantReadout;

    //public Button exitTownButton;


    [Header("Other")]
    [Space(5)]
    ScrapObject scrapToTake;
    

    void Awake(){
        UIM = this;
    }

    void Start()
    {
        fuleManager = PlayerManager.PM.GetComponent<fuel>();
        PlayerManager = PlayerManager.PM;
        SceneController = SceneController.SC;
        scrapTickBackup = scrapTick;
    }

// OVERWORLD AND GAMEPLAY
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
    public int OnTickHover(int tickIndex){
        ScrapObject tickScrap = PlayerManager.playerScrap[tickIndex];
        readoutName.text = tickScrap.scrapName;
        readoutDesc.text = tickScrap.description;
        readoutSize.text = string.Format("Size: {0:#,#}", tickScrap.size + " m<sup>3</sup>");
        readoutValue.text = tickScrap.value.ToString("Value: " + "#,#" + " cr.");
        readoutPanel.SetActive(true);
        return tickIndex;
    }
    public void OnTickHoverExit(){
        readoutPanel.SetActive(false);
    }

// TOWN AND MERCHANTS
    public void OfferTown(){
        enterTownButton.gameObject.SetActive(true);
    }
    public void EnterTown(){
        enterTownButton.gameObject.SetActive(false);
        haulText.color = (Color.white);
        fuelText.color = (Color.white);
        creditText.color = (Color.white);
        TownUI.SetActive(true);
    }
    public void LeaveTown(){
        haulText.color = (Color.black);
        fuelText.color = (Color.black);
        creditText.color = (Color.black);
        TownUI.SetActive(false);
        scrapMerchantReadout.text = "\"Back again?\"";
    }

    // SCRAP BUYER
    public void cantSellScrap(){
        scrapMerchantReadout.text = "\"You two ah, don't have any scrap.\"";
    }
    public void SoldScrap(){
        scrapMerchantReadout.text = "\"It's yours\"\nYou made: " + 
            "<color=#D44900>" + MerchantManager.scrapValue.ToString("#,#") + " credits.";
        creditText.text = "Credits: " + PlayerManager.playerCredits.ToString("#,#");
        haulText.text = "Current Haul: " + "0" + " m<sup>3</sup>";
        for(int i = scrapTickSlots.transform.childCount - 1; i >= 0; i--){
            GameObject.Destroy(scrapTickSlots.transform.GetChild(i).gameObject);
        }
        scrapTick = scrapTickBackup;
    }

    // FUEL MERCHANT
    public void BoughtFuel(){
        fuelMerchantReadout.text = "\"That'll be " + MerchantManager.creditsToTakeFuel.ToString("#,#") + " credits.\"";
        creditText.text = "Credits: " + PlayerManager.playerCredits.ToString("#,#");
    }
    public void CantAffordFill(){
        fuelMerchantReadout.text = "\"I am sorry, friend, but that is only enough for a partial fuel refill. I will provide for you what I can.\"";
        creditText.text = "Credits: 0 cr.";
    }
    
}
