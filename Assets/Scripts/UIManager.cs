using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class UIManager : MonoBehaviour
{
    public static UIManager UIM;

    [Header("Scripts")]
    [Space(5)]
    public PlayerManager PlayerManager;
    public SceneController SceneController;
    public fuel fuelManager;
    public MerchantManager MerchantManager;
    public UpgradeManager UpgradeManager;
    public ReadoutManager ReadoutManager;

    [Header("Statbar")]
    [Space(5)]
    public GameObject statBar;
    public TextMeshProUGUI haulText;
    public TextMeshProUGUI maxHaulText;
    public TextMeshProUGUI fuelText;
    public TextMeshProUGUI creditText;
    public GameObject scrapTick;
    public GameObject scrapTickBackup;
    public GameObject scrapTickSlots;

    [Header("Overworld Readouts")]
    [Space(5)]
    public GameObject readoutPanel;
    public TextMeshProUGUI readoutName;
    public TextMeshProUGUI readoutMat;
    public TextMeshProUGUI readoutDesc;
    public TextMeshProUGUI readoutSize;
    public TextMeshProUGUI readoutValue;
    public Image readoutImage;
    public GameObject tickReadout;
    public ScrapObject tickScrap;
    public TextMeshProUGUI zoomLevelReadout;

    
    [Header("Town")]
    [Space(5)]
    public GameObject OverworldUI;
    public GameObject TownUI;
    public GameObject townHub;
    public Button enterTownButton;
    public string playerLocation;

    [Header("Scrap Buyer")]
    [Space(10)]
    public GameObject scrapBuyer;
    public TextMeshProUGUI scrapBuyerReadout;
    public ScrapObject scrapToTake;
    [Space(5)]
    public List<GameObject> upgradePanels = new List<GameObject>();
    public int panelIndex;
    public string effectSuffix;
    [Header("Fuel Merchant")]
    [Space(10)]
    public GameObject fuelMerchant;
    public TextMeshProUGUI fuelMerchantReadout;
    public TextMeshProUGUI fillFuelButtonText;
    int characterToTalk;

    void Awake(){
        UIM = this;
    }

    void Start()
    {
        fuelManager = PlayerManager.PM.GetComponent<fuel>();
        PlayerManager = PlayerManager.PM;
        SceneController = SceneController.SC;
        scrapTickBackup = scrapTick;
        ReadoutManager = this.GetComponent<ReadoutManager>();
        maxHaulText.text = "max: " + PlayerManager.maxHaul.ToString("#,#") + " m<sup>3</sup>";
        InitUpgradePanels();
    }

// OVERWORLD AND GAMEPLAY
    void Update()
    {   // updating stats at runtime
        fuelText.text = "Fuel: " + fuelManager.currentFuelPercent.ToString("N0") + "%";
        if(PlayerManager.playerCredits == 0){
            creditText.text = "Credits: 0 cr.";
        }
        else{
            creditText.text = "Credits: " + PlayerManager.playerCredits.ToString("#,#") + " cr.";
        }
        if(PlayerManager.currentHaul != 0){
            haulText.text = "Space left: " + (PlayerManager.maxHaul - PlayerManager.currentHaul).ToString("#,#") + " m<sup>3</sup>";
        }
        else{
            haulText.text = "Space left: " + PlayerManager.maxHaul;
        }
        if(fuelManager.currentFuelUnits == fuelManager.maxFuel){
            fillFuelButtonText.text = "Cassets full.";
        }
    }

    // 2. PlayerManager has found scrap 
    public ScrapObject ShowScrap(ScrapObject newScrap){
        Debug.Log("Showing: " + newScrap.scrapName);
        newScrap.GetComponent<SpriteRenderer>().enabled = true;
        newScrap.GetComponent<SpriteRenderer>().color = new Vector4 (0.5764706f,0.2431373f,0,1);
        scrapToTake = newScrap;
        return newScrap;
    }
    public ScrapObject OutOfRangeScrap(ScrapObject newScrap){
        Debug.Log(newScrap.scrapName + " is now out of range");
        newScrap.GetComponent<SpriteRenderer>().color = new Vector4 (0.6509434f,0.5183763f,0.4216803f,1);
        return newScrap;
    }

    // 4. Player has clicked on scrap
    public ScrapObject ShowReadout(ScrapObject newScrap){
        ReadoutManager.UpdateReadout(newScrap);
        readoutPanel.SetActive(true);
        //PlayerManager.SetPlayerMovement(false); // stop the player from moving because how the hell do you actually get UI to block a raycast???
        return newScrap;
    }

    // 5. Player clicks "take" or "leave" and we do what they tell us
    public void TakeScrap(){
        if((PlayerManager.currentHaul + scrapToTake.size) < PlayerManager.maxHaul){
                PlayerManager.TakeScrap(scrapToTake);
                GameObject newTick = Instantiate(scrapTick) as GameObject;
                newTick.transform.SetParent(scrapTickSlots.transform, false);
                readoutPanel.SetActive(false);
                //PlayerManager.SetPlayerMovement(true);
        }
        else{
            Callout("CantFitScrap");
        }
    }
    public void LeaveScrap(){
        readoutPanel.SetActive(false);
        //PlayerManager.SetPlayerMovement(true);
    }

    public void Callout(string callout){
        characterToTalk = Random.Range(1,3);
        Debug.Log("Character int: " + characterToTalk);
        StartCoroutine(callout);
    }
    public IEnumerator CantFitScrap(){
        if(characterToTalk == 1){
            PlayerManager.hasronCallout.SetActive(true);
            PlayerManager.hasronCallout.GetComponentInChildren<TextMeshProUGUI>().text = "\"Doesn't look like that's gonna fit...\"";
            yield return new WaitForSeconds(3);
            PlayerManager.hasronCallout.SetActive(false);
        }
        else{
            PlayerManager.chipCallout.SetActive(true);
            PlayerManager.chipCallout.GetComponentInChildren<TextMeshProUGUI>().text = "\"No way we're taking that right now.\"";
            yield return new WaitForSeconds(3);
            PlayerManager.chipCallout.SetActive(false);
        }
    }
    public IEnumerator OutOfFuel(){
        Debug.Log("Character int: " + characterToTalk);
        if(characterToTalk == 1){
            PlayerManager.hasronCallout.SetActive(true);
            PlayerManager.hasronCallout.GetComponentInChildren<TextMeshProUGUI>().text = "\"Not this again.\"";
            yield return new WaitForSeconds(3);
            PlayerManager.hasronCallout.SetActive(false);
        }
        else{
            PlayerManager.chipCallout.SetActive(true);
            PlayerManager.chipCallout.GetComponentInChildren<TextMeshProUGUI>().text = "\"How do they even know we're out?\"";
            yield return new WaitForSeconds(3);
            PlayerManager.chipCallout.SetActive(false);
        }
    }

    public int OnTickHover(int tickIndex){
        tickScrap = PlayerManager.playerScrap[tickIndex];
        PlayerManager.tickReadoutIndex = tickIndex;
        ReadoutManager.ShowTickReadout(tickScrap);
        return tickIndex;
    }
    public void OnTickHoverExit(){
        tickReadout.SetActive(false);
        PlayerManager.tickReadoutIndex = PlayerManager.playerScrap.Count + 1;
    }
    public void UpdateZoomLevel(float currentZoom){
        zoomLevelReadout.text = currentZoom + "x";
    }


// TOWN AND MERCHANTS
    public void ActivateTownButton(bool isActive){
        enterTownButton.gameObject.SetActive(isActive);
    }
    public void EnterTown(){
        ActivateTownButton(false);
        haulText.color = (Color.white);
        maxHaulText.color = (Color.white);
        fuelText.color = (Color.white);
        creditText.color = (Color.white);
        playerLocation = "town hub";
        TownUI.SetActive(true);
        OverworldUI.SetActive(false);
    }
    public void LeaveTown(){
        ActivateTownButton(true);
        haulText.color = (Color.black);
        maxHaulText.color = (Color.black);
        fuelText.color = (Color.black);
        creditText.color = (Color.black);
        TownUI.SetActive(false);
        OverworldUI.SetActive(true);
        scrapBuyerReadout.text = "\"Back again?\"";
    }
    public void BackToHub(){
        if(playerLocation == "scrap buyer"){
            scrapBuyer.SetActive(false);
            townHub.SetActive(true);
        }
        if(playerLocation == "fuel merchant"){
            fuelMerchant.SetActive(false);
            townHub.SetActive(true);
        }
        playerLocation = "town hub";
    }
    public void BoughtUpgrade(Upgrade upgrade){
        // cant afford?
        if(upgrade.type == "engine"){ // more upgrade specific stuff than I would like...
            panelIndex = 0;
            effectSuffix = " kph";
            scrapBuyerReadout.text = "\"Careful installing that thing, ok?\"";
        }
        if(upgrade.type == "reactor"){
            panelIndex = 1;
            effectSuffix = " deuterium cassets";
            scrapBuyerReadout.text = "\"Be sure to go see Ogden to have that thing tuned.\"";
        }
        if(upgrade.type == "storage bay"){
            panelIndex = 2;
            effectSuffix = " m<sup>3</sup>";
            scrapBuyerReadout.text = "\"Gonna fit a lot of scrap in there for me?\"";
            maxHaulText.text = "max: " + PlayerManager.maxHaul.ToString("#,#") + " m<sup>3</sup>";
        }
        if(upgrade.type == "scanner"){
            panelIndex = 3;
            effectSuffix = " m";
            scrapBuyerReadout.text = "\"That should help you two find some real good scrap out there.\"";
        }
        if(upgrade.type == "drone"){
            panelIndex = 4;
            effectSuffix = "x zoom";
            scrapBuyerReadout.text = "\"Got a big eye up there, huh?\"\n <sub>right click to change zoom level</sub>";
        }
        Debug.Log("Panel index: " + panelIndex);  // Which panel do we change?
        upgradePanels[panelIndex].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgrade.flavorTexts[upgrade.upgradeLevel].flavorName;
        upgradePanels[panelIndex].transform.Find("Desc.").GetComponent<TextMeshProUGUI>().text = upgrade.flavorTexts[upgrade.upgradeLevel].flavorDesc;
        upgradePanels[panelIndex].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = upgrade.priceOffered.ToString("#,#") + " cr.";
        if(upgrade.type == "engine"){
            upgrade.effectOffered *= 1000; // this is some stupid shit
            upgradePanels[panelIndex].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "+" + upgrade.effectOffered.ToString("F") + effectSuffix;
            upgrade.effectOffered /= 1000;
        }
        else{
            upgradePanels[panelIndex].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "+" + upgrade.effectOffered.ToString("F") + effectSuffix;
        }
    }
    public void UpgradeAlreadyMaxed(Upgrade upgrade){
        scrapBuyerReadout.text = "\"Look, you've bought all I got on that one. Maybe try the next town over, ha.\"";
        upgradePanels[panelIndex].gameObject.GetComponent<Image>().color = Color.black;
    }
    public void CantAffordUpgrade(Upgrade upgrade){
        scrapBuyerReadout.text = "\"As much as I'd like to, I can't sell that " + upgrade.type + " for anything less.\"";
    }

    // SCRAP BUYER
    public void enterScrapBuyer(){
        townHub.SetActive(false);
        scrapBuyer.SetActive(true);
        playerLocation = "scrap buyer";
        // this should be pulling from a list of welcomes
        scrapBuyerReadout.text = "\"How are you all doing?\"";
    }
    public void cantSellScrap(){
        scrapBuyerReadout.text = "\"You two ah, don't have any scrap out there.\"";
    }
    public void SoldScrap(){  // Called from Merchant Manager
        scrapBuyerReadout.text = "\"It's yours\"\nYou made: " + 
            "<color=#D44900>" + MerchantManager.scrapValue.ToString("#,#") + " credits.";
        for(int i = scrapTickSlots.transform.childCount - 1; i >= 0; i--){
            GameObject.Destroy(scrapTickSlots.transform.GetChild(i).gameObject);
        }
        scrapTick = scrapTickBackup;
    }
    
    // FUEL MERCHANT
    public void enterFuelMerchant(){
        townHub.SetActive(false);
        fuelMerchant.SetActive(true);
        playerLocation = "fuel merchant";
        MerchantManager.EnterFuelMerchant();
    }
    public void BoughtFuel(){
        fuelMerchantReadout.text = "\"Thank you for the credits. I assure you it will be put to good use.\"";
        fuelManager.UpdateFuelPercent();
        Debug.Log("Fuel text" + fuelText.text);
    }
    public void FuelAlreadyFull(){
        fuelMerchantReadout.text = "\"It seems that your reactor is already at maximum capacity.\"";
    }
    public void CantAffordFill(){
        fuelMerchantReadout.text = "\"I am sorry, friend, but that is only enough for a partial fuel refill. I will provide for you what I can.\"";
    }
    public void TweakedReactor(){
        fuelMerchantReadout.text = "\"I am not capable of adjusting your reactor, yet.\"";
    }

// SETUP
    public void InitUpgradePanels(){
        for(int i = 0; i < upgradePanels.Count; i++){
            if(UpgradeManager.upgradesStarter[i].type == "engine"){ // more upgrade specific stuff than I would like...
                effectSuffix = " kph";
            }
            if(UpgradeManager.upgradesStarter[i].type == "reactor"){
                effectSuffix = " deuterium cassets";
            }
            if(UpgradeManager.upgradesStarter[i].type == "storage bay"){
                effectSuffix = " m<sup>3</sup>";
            }
            if(UpgradeManager.upgradesStarter[i].type == "scanner"){
                effectSuffix = " m";
            }
            if(UpgradeManager.upgradesStarter[i].type == "drone"){
                effectSuffix = "x zoom";
            }

            upgradePanels[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = UpgradeManager.upgradesStarter[i].flavorTexts[0].flavorName;
            upgradePanels[i].transform.Find("Desc.").GetComponent<TextMeshProUGUI>().text = UpgradeManager.upgradesStarter[i].flavorTexts[0].flavorDesc;
            upgradePanels[i].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = UpgradeManager.upgradesStarter[i].priceOffered.ToString("#,#") + " cr.";
            if(UpgradeManager.upgradesStarter[i].type == "engine"){
                UpgradeManager.upgradesStarter[i].effectOffered *= 1000; // this is still some stupid shit
                upgradePanels[i].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "+" + UpgradeManager.upgradesStarter[i].effectOffered.ToString("F") + effectSuffix;
                UpgradeManager.upgradesStarter[i].effectOffered /= 1000;
            }
            else{
                upgradePanels[i].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = "+" + UpgradeManager.upgradesStarter[i].effectOffered.ToString("F") + effectSuffix;
            }
        }
    }
    
    bool paused;
    public void TogglePause(){
        GameObject pauseMenu = gameObject.transform.Find("IntroPanel").gameObject;
        if(paused == false){
            pauseMenu.SetActive(true);
            paused = true;
        }
        else{
            pauseMenu.SetActive(false);
            paused = false;
        }
    }


    
}
