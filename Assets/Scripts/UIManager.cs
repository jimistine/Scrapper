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
    public ReadoutManager ReadoutManager;

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
    public TextMeshProUGUI readoutMat;
    public TextMeshProUGUI readoutDesc;
    public TextMeshProUGUI readoutSize;
    public TextMeshProUGUI readoutValue;
    public Image readoutImage;
    public Button readoutTakeButt;
    public Button readoutLeaveButt;
    
    [Header("Town")]
    [Space(5)]
    public GameObject TownUI;
    public Button enterTownButton;
    public TextMeshProUGUI fuelMerchantReadout;

    [Header("Scrap Buyer")]
    [Space(5)]
    public TextMeshProUGUI scrapMerchantReadout;
    public TextMeshProUGUI scrapMerchantWelcome;
    public TextMeshProUGUI fuelMerchantWelcome;
    [Space(10)]
    public TextMeshProUGUI engineName;
    public TextMeshProUGUI engineDesc;
    public TextMeshProUGUI engineStat;
    public TextMeshProUGUI enginePrice;
    public Button upgradeEngineButt;
    [Space(10)]
    public TextMeshProUGUI reactorName;
    public TextMeshProUGUI reactorDesc;
    public TextMeshProUGUI reactorStat;
    public TextMeshProUGUI reactorPrice;
    public Button upgradeReactorButt;
    [Space(10)]
    public TextMeshProUGUI haulerName;
    public TextMeshProUGUI haulerDesc;
    public TextMeshProUGUI haulerStat;
    public TextMeshProUGUI haulerPrice;
    public Button upgradeHaulerButt;
    [Space(10)]
    public TextMeshProUGUI searchName;
    public TextMeshProUGUI searchDesc;
    public TextMeshProUGUI searcgStat;
    public TextMeshProUGUI searchPrice;
    public Button upgradeSearchButt;
    [Space(10)]
    public TextMeshProUGUI droneName;
    public TextMeshProUGUI droneDesc;
    public TextMeshProUGUI droneStat;
    public TextMeshProUGUI dronePrice;
    public Button upgradeDroneButt;
    [Space(10)]
    public List<GameObject> upgradePanels = new List<GameObject>();
    public int panelIndex;
    public string effectSuffix;

    //public Button exitTownButton;


    [Header("Other")]
    [Space(5)]
    public ScrapObject scrapToTake;
    

    void Awake(){
        UIM = this;
    }

    void Start()
    {
        fuleManager = PlayerManager.PM.GetComponent<fuel>();
        PlayerManager = PlayerManager.PM;
        SceneController = SceneController.SC;
        scrapTickBackup = scrapTick;
        ReadoutManager = this.GetComponent<ReadoutManager>();
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
        ReadoutManager.UpdateReadout(newScrap);
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
    public void enterScrapBuyer(){
        // turn off the rest of town
        // turn on scrap buyer stuff
    }
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
    public void BoughtUpgrade(Upgrade upgrade){
        if(upgrade.type == "engine"){
            panelIndex = 0;
            effectSuffix = " kph";
            scrapMerchantReadout.text = "You bought a nice new engine, kiddo!";
        }
        if(upgrade.type == "reactor"){
            panelIndex = 1;
            effectSuffix = " deuterium cassets";
            scrapMerchantReadout.text = "You bought a nice new fusion reactor, kiddo!";
        }
        upgradePanels[panelIndex].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgrade.flavorTexts[upgrade.upgradeLevel].flavorName;
        upgradePanels[panelIndex].transform.Find("Desc.").GetComponent<TextMeshProUGUI>().text = upgrade.flavorTexts[upgrade.upgradeLevel].flavorDesc;
        upgradePanels[panelIndex].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = upgrade.priceOffered.ToString("#,#") + " cr.";
        upgradePanels[panelIndex].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "+" + upgrade.effectOffered.ToString() + effectSuffix;
    }
    

    // FUEL MERCHANT
    public void enterFuelMerchant(){

    }
    public void BoughtFuel(){
        fuelMerchantReadout.text = "\"That'll be " + MerchantManager.creditsToTakeFuel.ToString("#,#") + " credits.\"";
        creditText.text = "Credits: " + PlayerManager.playerCredits.ToString("#,#");
    }
    public void CantAffordFill(){
        fuelMerchantReadout.text = "\"I am sorry, friend, but that is only enough for a partial fuel refill. I will provide for you what I can.\"";
        creditText.text = "Credits: 0 cr.";
    }
    
}
