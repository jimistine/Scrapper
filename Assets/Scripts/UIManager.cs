using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text;
//using System;
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
    public Timer clockTimer;
    public DayNight DayNight;

    [Header("Minimal")]
    [Header("New HUD")]
    public Slider fuelSlider;
    public Slider haulSlider;
    public TextMeshProUGUI haulText;
    public TextMeshProUGUI haulTextRemaining;
    [Header("Details")]
    public GameObject detailsPanel;
    public GameObject detailsButt;
    public TextMeshProUGUI reactorNameTag;
    public GameObject cassetteParent;
    public GameObject cassettePrefab;
    public List<GameObject> cassettes;
    float numCassettes;
    public GameObject statsParent;
    public TextMeshProUGUI creditText;
    public TextMeshProUGUI storageBayNameTag;
    public TextMeshProUGUI maxHaulText;
    public GameObject scrapTick;
    public GameObject scrapTickBackup;
    public GameObject scrapTickSlots;

    [Header("Old HUD")]
    [Space(5)]
    public GameObject statBar;
    public TextMeshProUGUI fuelText;

    [Header("Overworld Readouts")]
    [Space(10)]
    //public GameObject onScrapPanel;
    public GameObject onScrapPanelPrefab;
    public GameObject onScrapPanelParent;
    public GameObject recentScrap;
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

    [Header("Pause Screen")]
    [Space(5)]
    public GameObject pauseScreen;
    public GameObject scrapLog;
    public GameObject scrapLogItemPrefab;

    
    [Header("Town")]
    [Space(5)]
    public GameObject OverworldUI;
    public GameObject TownUI;
    public GameObject townHub;
    public Button enterTownButton;
    public string playerLocation = "overworld";
    //public TMP_InputField inputField;

    [Header("Scrap Buyer")]
    [Space(10)]
    public TextMeshProUGUI scrapBuyerCredits;
    public GameObject sellScrapButt;
    public GameObject scrapBuyer;
    public ScrapObject scrapToTake;
    [Space(5)]
    public List<GameObject> upgradePanels = new List<GameObject>();
    public int panelIndex;
    public string effectReadout;
    [Space(5)]
    public List<GameObject> upgradeButtons = new List<GameObject>();
    [Header("Fuel Merchant")]
    [Space(10)]
    public TextMeshProUGUI fuelMerchantCredits;
    public GameObject fuelMerchant;
    public Button fillFuelButt;
    public TextMeshProUGUI fuelMerchantReadout;
    public TextMeshProUGUI fillFuelButtonText;

    public UnityEngine.Events.UnityEvent leftScrap;

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
        maxHaulText.text = PlayerManager.maxHaul.ToString("#,#") + " m<sup>3</sup>";
        InitUpgradePanels();
        InitCassettes();
    }

// OVERWORLD AND GAMEPLAY
    void Update()
    {   // updating stats at runtime
        //fuelText.text = "Fuel: " + fuelManager.currentFuelPercent.ToString("N0") + "%";
        fuelSlider.value = fuelManager.currentFuelPercent/100;
        haulSlider.value = PlayerManager.currentHaulPercent/100;

        if(PlayerManager.playerCredits == 0){
            creditText.text = "CREDITS           <color=#777777>————</color>  0 cr.";
            if(TownUI != null){
                scrapBuyerCredits.text = "Current Credits\n<b>0 cr.</b>";
                fuelMerchantCredits.text = "Current Credits\n<b>0 cr.</b>";
            }
        }
        else{
            creditText.text = "CREDITS           <color=#777777>————</color>  " + PlayerManager.playerCredits.ToString("#,#") + " cr.";
            if(TownUI != null){
                scrapBuyerCredits.text = "Current Credits\n<b>" + PlayerManager.playerCredits.ToString("#,#") + " cr.</b>";
                fuelMerchantCredits.text = "Current Credits\n<b>" + PlayerManager.playerCredits.ToString("#,#") + " cr.</b>";
            }
            //creditText.text = "Credits: " + PlayerManager.playerCredits.ToString("#,#") + " cr.";
        }
        if(PlayerManager.currentHaul != 0){
            haulText.text = PlayerManager.currentHaul.ToString("#,#") + "/" + PlayerManager.maxHaul.ToString("#,#") + "<size=75%>m</size><sup>3</sup>";
            haulTextRemaining.text = (PlayerManager.maxHaul - PlayerManager.currentHaul).ToString("#,#") + "<size=70%>m</size><sup>3</sup>";
        }
        else if(PlayerManager.currentHaul == PlayerManager.maxHaul){
            haulTextRemaining.text = "0<size=70%>m</size><sup>3</sup>";
        }
        else{
            haulText.text = "0/" + PlayerManager.maxHaul.ToString("#,#") + "<size=75%>m</size><sup>3</sup>";
            //haulText.text = "Space left: " + PlayerManager.maxHaul+ " m<sup>3</sup>";
        }
        if(fuelManager.currentFuelUnits == fuelManager.maxFuel){
            fillFuelButtonText.text = "Cassets full.";
        }
        // if(onScrapPanel.activeSelf){
        //     Vector3 scrapPos = Camera.main.WorldToScreenPoint(recentScrap.transform.position);
        //     onScrapPanel.transform.position = scrapPos;
        // }
    }
    // HUD
    public void OpenHUDDetails(){
        if(detailsPanel != null){
            Animator animator = detailsPanel.GetComponent<Animator>();
            if(animator != null){
                bool isOpen = animator.GetBool("openDetails");
                if(isOpen){
                    AudioManager.AM.PlayMiscUIClip("hud close");
                }
                else{
                    AudioManager.AM.PlayMiscUIClip("hud open");
                }
                animator.SetBool("openDetails", !isOpen);
            }
        }
    }

    // 2. PlayerManager has found scrap 
    public ScrapObject ShowScrap(ScrapObject newScrap){
        //Debug.Log("Showing: " + newScrap.scrapName);
        newScrap.GetComponent<SpriteRenderer>().enabled = true;
        newScrap.GetComponent<SpriteRenderer>().color = new Vector4 (0.5764706f,0.2431373f,0,1);
        scrapToTake = newScrap;

        
        if(newScrap.isBuried == true){ // they have not yet scanned this piece
            //newScrap.isBuried = false;
            Vector3 scrapPos = Camera.main.WorldToScreenPoint(newScrap.transform.position);
            GameObject onScrapPanel = (Instantiate(onScrapPanelPrefab, scrapPos, Quaternion.identity));
            onScrapPanel.transform.SetParent(onScrapPanelParent.transform, false);
            //onScrapPanel.transform.parent = newScrap.transform;
            onScrapPanel.GetComponent<OnScrapPanelController>().panelScrap = newScrap;
            onScrapPanel.GetComponent<OnScrapPanelController>().panelScrapGO = newScrap.gameObject;
            // onScrapPanel.GetComponentInChildren<TextMeshProUGUI>().text = "?";
            // Director.Dir.StartFadeCanvasGroup(onScrapPanel, "in", 0.1f);
            Debug.Log("made panel");
        }
        // else{  // they have already scanned it
        //     Director.Dir.StartFadeCanvasGroup(newScrap.transform.Find("onScrapPanel(Clone)").gameObject, "in", 0.1f);
        //     newScrap.transform.Find("onScrapPanel(Clone)").GetComponentInChildren<TextMeshProUGUI>().text = 
        //         newScrap.scrapName + "<size=60%><color=#798478> | <color=#FF752A>" + newScrap.size.ToString("#,#") + " m<sup>3</sup></size>";
        // }
        recentScrap = newScrap.gameObject;
        recentScrap.transform.position = newScrap.transform.position;
        return newScrap;
    }
    public void OutOfRangeScrap(ScrapObject newScrap){
        //Debug.Log(newScrap.scrapName + " is now out of range");
        //newScrap.GetComponent<SpriteRenderer>().color = new Vector4 (0.6509434f,0.5183763f,0.4216803f,1);
        Director.Dir.StartFadeCanvasGroup(readoutPanel, "out", 0.15f);
    }

    // 4. Player has clicked on scrap
    public void ShowReadout(GameObject scrap = default(GameObject)){ // this one is for clicking on the scrap itself
        //Director.Dir.StartFadeCanvasGroup(scrap.transform.Find("onScrapPanel(Clone)").gameObject, "out", 0.1f);
        if(scrap != null){
            recentScrap = scrap;
        }
        ReadoutManager.UpdateReadout(recentScrap.GetComponent<ScrapObject>());
        Director.Dir.StartFadeCanvasGroup(readoutPanel, "in", 0.05f);
        AudioManager.AM.PlayMiscUIClip("inspect");
        recentScrap.GetComponent<ScrapObject>().isBuried = false;
       
    }
    public void ShowReadoutButton(GameObject scrap = default(GameObject)){ // and this one is for the on scrap panel. i hate it.
        //Director.Dir.StartFadeCanvasGroup(recentScrap.transform.Find("onScrapPanel(Clone)").gameObject, "out", 0.1f);
        ReadoutManager.UpdateReadout(scrap.GetComponent<ScrapObject>());
        Director.Dir.StartFadeCanvasGroup(readoutPanel, "in", 0.05f);
        AudioManager.AM.PlayMiscUIClip("inspect");
        
        recentScrap.GetComponent<ScrapObject>().isBuried = false;
     
    }

    // 5. Player clicks "take" or "leave" and we do what they tell us
    public void TakeScrap(){
        if((PlayerManager.currentHaul + scrapToTake.size) <= PlayerManager.maxHaul){
                PlayerManager.TakeScrap(scrapToTake);
                UpdateScrapTicks();
                Director.Dir.StartFadeCanvasGroup(readoutPanel, "out", 0.05f);
                AudioManager.AM.PlayPlayerClip("pick up scrap");
                DialogueManager.DM.RunNode("scrap-take");
        }
        else{
            DialogueManager.DM.RunNode("scrap-wont-fit");
            AudioManager.AM.PlayMiscUIClip("reject");
        }
    }
    public void UpdateScrapTicks(){
        foreach(Transform tick in scrapTickSlots.transform){
            GameObject.Destroy(tick.gameObject);
        }
        foreach(ScrapObject scrap in PlayerManager.playerScrap){
            GameObject newTick = Instantiate(scrapTick) as GameObject;
            newTick.transform.SetParent(scrapTickSlots.transform, false);
        }
    }
    public void LeaveScrap(){
        // recentScrap.transform.Find("onScrapPanel(Clone)").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = 
        //     recentScrap.GetComponent<ScrapObject>().scrapName + "<size=60%><color=#798478> | <color=#FF752A>" + recentScrap.GetComponent<ScrapObject>().size.ToString("#,#") + " m<sup>3</sup></size>";
        // Director.Dir.StartFadeCanvasGroup(recentScrap.transform.Find("onScrapPanel(Clone)").gameObject, "in", 0.1f);
        leftScrap?.Invoke();      
        Director.Dir.StartFadeCanvasGroup(readoutPanel, "out", 0.05f);
        AudioManager.AM.PlayMiscUIClip("dismiss");
        DialogueManager.DM.RunNode("scrap-leave");
    }
    public void AddScrapToLog(ScrapObject scrapToAdd){
        GameObject newLogItem = Instantiate(scrapLogItemPrefab) as GameObject;
        newLogItem.transform.SetParent(scrapLog.transform, false);
        ReadoutManager.FillScrapLogItem(newLogItem, scrapToAdd);
    }
    public void Callout(string callout){
        characterToTalk = Random.Range(1,3);
        Debug.Log("Character int: " + characterToTalk);
        StartCoroutine(callout);
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

    bool paused = false;
    public void TogglePause(){
        AudioManager.AM.TogglePausedSnapshot();
        if(paused == false){ // they pause the game
            Director.Dir.StartFadeCanvasGroup(pauseScreen, "in", .15f);
            paused = true;
            AudioManager.AM.PlayMiscUIClip("inspect");
            Director.Dir.gamePaused = true;
        }
        else{                // they unpause the game
            Director.Dir.StartFadeCanvasGroup(pauseScreen, "out", .15f);
            paused = false;
            AudioManager.AM.PlayMiscUIClip("dismiss");
            Director.Dir.gamePaused = false;
        }
    }

// TOWN AND MERCHANTS
    public void ActivateTownButton(bool isActive){
        if(isActive == true){
            Director.Dir.StartFadeCanvasGroup(enterTownButton.gameObject,"in", 0.25f);
            enterTownButton.enabled = true;
        }
        else if(isActive == false){
            Debug.Log("town butt false");
            Director.Dir.StartFadeCanvasGroup(enterTownButton.gameObject,"out", 0.25f);
            enterTownButton.enabled = false;
        }
        //enterTownButton.gameObject.SetActive(isActive);
    }
    public void EnterTown(){
        ActivateTownButton(false);
        playerLocation = "town hub";
        TownUI.SetActive(true);
        OverworldUI.SetActive(false);
    }
    public void LeaveTown(){
        ActivateTownButton(true);
        playerLocation = "overworld";
        TownUI.SetActive(false);
        OverworldUI.SetActive(true);
    }
    public void BackToHub(){
        if(playerLocation == "scrap buyer"){
            scrapBuyer.SetActive(false);
        }
        if(playerLocation == "fuel merchant"){
            fuelMerchant.SetActive(false);
        }
        townHub.SetActive(true);
        AudioManager.AM.TransitionToTownExterior();
        playerLocation = "town hub";
        DialogueManager.DM.ConversationEnded();
    }
    // for setting the time to a specific time in town, not used, current configuration uses set day/night on DayNight.cs
    public void Wait(){
        // if(inputField.gameObject.activeSelf == false){
        //     inputField.gameObject.SetActive(true);
        // }
        // else{
        //     // each hour in game is 0.42 minutes irl
        //     // each minute irl is 2.4 hours in-game
        //     string waitTimeInput = inputField.text;
        //     int waitTimeHours = System.Convert.ToInt32(waitTimeInput);
        //     float secondsToWait = (waitTimeHours * 0.42f) * 60;
        //     DayNight.clockTime += secondsToWait;
        //     inputField.gameObject.SetActive(false);
        //     Debug.Log(secondsToWait);
        //     SceneController.StartLeaveTown();
        // }
    }
    // u.3 called from UpgradeManager
    public void BoughtUpgrade(Upgrade upgrade){
        // cant afford?
        if(upgrade.type == "engine"){ // more upgrade specific stuff than I would like...
            panelIndex = 0;
            effectReadout = (PlayerManager.gameObject.GetComponent<ClickDrag>().topSpeed * 100).ToString("#,#") + " →" + (upgrade.upgradeItemValues[upgrade.upgradeLevel].effectValue * 100).ToString("#,#") + " kph top speed";
            DialogueManager.DM.RunNode("engine");
            statsParent.transform.Find("Top Speed").GetComponent<TextMeshProUGUI>().text = "Top Speed        <color=#777777>————</color>  " + (upgrade.upgradeItemValues[upgrade.upgradeLevel - 1].effectValue * 100).ToString("#,#") + "kph";
        }
        if(upgrade.type == "reactor"){
            panelIndex = 1;
            effectReadout = (PlayerManager.gameObject.GetComponent<fuel>().maxFuel/100).ToString("#,#") + " →" + (upgrade.upgradeItemValues[upgrade.upgradeLevel].effectValue/100).ToString("#,#") + " deuterium cassets";
            reactorNameTag.text = upgrade.upgradeItemValues[upgrade.upgradeLevel - 1].flavorName + "\n<size=50%>cassettes: " + numCassettes + "</size>";
            DialogueManager.DM.RunNode("reactor");
        }
        if(upgrade.type == "storage-bay"){
            panelIndex = 2;
            effectReadout = PlayerManager.maxHaul.ToString("#,#") + " →" + upgrade.upgradeItemValues[upgrade.upgradeLevel].effectValue.ToString("#,#") + " m<sup>3</sup> max haul";
            DialogueManager.DM.RunNode("storage-bay");
            maxHaulText.text = PlayerManager.maxHaul.ToString("#,#") + " m<sup>3</sup>";
            storageBayNameTag.text = upgrade.upgradeItemValues[upgrade.upgradeLevel - 1].flavorName;
            statsParent.transform.Find("Max Haul").GetComponent<TextMeshProUGUI>().text = "Max Haul         <color=#777777>————</color>  " + upgrade.upgradeItemValues[upgrade.upgradeLevel - 1].effectValue.ToString("#,#") + "m<sup>3</sup>";
        }
        if(upgrade.type == "scanner"){
            panelIndex = 3;
            effectReadout = (PlayerManager.maxPulse * 10).ToString("F") + " →" + (upgrade.upgradeItemValues[upgrade.upgradeLevel].effectValue * 10).ToString("F") + " meter radius";
            DialogueManager.DM.RunNode("scanner");
            statsParent.transform.Find("Search Radius").GetComponent<TextMeshProUGUI>().text = "Search Radius  <color=#777777>————</color>  " + (upgrade.upgradeItemValues[upgrade.upgradeLevel - 1].effectValue * 10).ToString("F") + "m";
        }
        if(upgrade.type == "drone"){
            panelIndex = 4;
            DialogueManager.DM.RunNode("drone");
            zoomLevels = "";
            int i = 1;
            foreach(float zoomLevel in PlayerManager.gameObject.GetComponentInChildren<RigManager>().zoomLevels){
                if(i == PlayerManager.gameObject.GetComponentInChildren<RigManager>().zoomLevels.Count){
                    zoomLevels += zoomLevel.ToString("N1") + "x";
                }
                else{
                    zoomLevels += zoomLevel.ToString("N1") + "x, ";
                }
                i++;
            }
            effectReadout = zoomLevels + " → +" + upgrade.upgradeItemValues[upgrade.upgradeLevel].effectValue.ToString("N1") + "x zoom";
            statsParent.transform.Find("Drone Lenses").GetComponent<TextMeshProUGUI>().text = "Drone Lenses   <color=#777777>————</color>  " + zoomLevels;
        }

        Debug.Log("Panel index: " + panelIndex);  // Which panel do we change?
        if(upgradePanels[panelIndex] == null){
            return;
        }
        else{
            upgradePanels[panelIndex].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = upgrade.upgradeItemValues[upgrade.upgradeLevel].flavorName;
            upgradePanels[panelIndex].transform.Find("Desc.").GetComponent<TextMeshProUGUI>().text = upgrade.upgradeItemValues[upgrade.upgradeLevel].flavorDesc;
            upgradePanels[panelIndex].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = "Price: " + upgrade.upgradeItemValues[upgrade.upgradeLevel].price.ToString("#,#") + " cr.";
            upgradePanels[panelIndex].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effectReadout;
        }
    }
    public void UpgradeAlreadyMaxed(Upgrade upgrade){
        upgradePanels[panelIndex].gameObject.GetComponent<Image>().color = Color.gray;
        DialogueManager.DM.RunNode(upgrade.type);
    }
    public void CantAffordUpgrade(Upgrade upgrade){
        if(playerLocation == "scrap buyer"){
            DialogueManager.DM.RunNode("chundr-cant-afford");
        }
        else if(playerLocation == "fuel merchant"){
            DialogueManager.DM.RunNode("ogden-cant-afford");
        }
    }

    // SCRAP BUYER
    public void enterScrapBuyer(){
        AudioManager.AM.TransitionToTownInterior();
        townHub.SetActive(false);
        scrapBuyer.SetActive(true);
        playerLocation = "scrap buyer";
        DialogueManager.DM.RunNode("chundr");
        UpdateSellScrapButtText();
    }
    public void cantSellScrap(){
        DialogueManager.DM.RunNode("cant-sell-scrap");
        AudioManager.AM.PlayMiscUIClip("reject");
    }
    public void SoldScrap(){  // Called from Merchant Manager
        AudioManager.AM.PlayMiscUIClip("sold scrap");
        UpdateSellScrapButtText();
        DialogueManager.DM.RunNode("sold-all-scrap");
        for(int i = scrapTickSlots.transform.childCount - 1; i >= 0; i--){
            GameObject.Destroy(scrapTickSlots.transform.GetChild(i).gameObject);
        }
        scrapTick = scrapTickBackup;
    }
    public void UpdateSellScrapButtText(){
        if(PlayerManager.playerScrap.Count != 0){
            sellScrapButt.GetComponentInChildren<TextMeshProUGUI>().text = "Sell all scrap for " + PlayerManager.evaluateCurrentHaul().ToString("#,#") + " credits";
        }
        else{
            sellScrapButt.GetComponentInChildren<TextMeshProUGUI>().text = "No scrap to sell";
        }
    }
    
    public void UpdateCassettes(){
        foreach(Transform cassette in cassetteParent.transform){
            Destroy(cassette.gameObject);
            cassettes.Clear();
        }
        numCassettes = fuelManager.maxFuel/100;
        for(float i = 1; i <= numCassettes; i++){
            GameObject newCassette = Instantiate(cassettePrefab) as GameObject;
            newCassette.transform.SetParent(cassetteParent.transform, false);
            newCassette.GetComponent<Slider>().value = 1;
            newCassette.GetComponentInChildren<TextMeshProUGUI>().text = i.ToString();
            cassettes.Add(newCassette);
        }
        Debug.Log("Updated cassettes");
    }

    // FUEL MERCHANT
    public void enterFuelMerchant(){
        AudioManager.AM.TransitionToTownInterior();
        townHub.SetActive(false);
        fuelMerchant.SetActive(true);
        playerLocation = "fuel merchant";
        DialogueManager.DM.RunNode("ogden-intro");
        MerchantManager.EnterFuelMerchant();
    }
    public void BoughtFuel(){
        DialogueManager.DM.RunNode("bought-fuel");
        fuelManager.UpdateFuelPercent();
        Debug.Log("Fuel text" + fuelText.text);
    }
    
    public void TweakedReactor(){
        DialogueManager.DM.RunNode("tweaked-reactor");
    }

// SETUP
    string zoomLevels;
    public void InitUpgradePanels(){
        for(int i = 0; i < upgradePanels.Count; i++){
            if(UpgradeManager.upgradesStarter[i].type == "engine"){ // more upgrade specific stuff than I would like...
                effectReadout = (PlayerManager.gameObject.GetComponent<ClickDrag>().topSpeed * 100).ToString("#,#") + " → " + (UpgradeManager.upgradesStarter[i].upgradeItemValues[0].effectValue * 100).ToString("#,#") + " kph top speed";
            }
            else if(UpgradeManager.upgradesStarter[i].type == "reactor"){
                effectReadout = (PlayerManager.GetComponent<fuel>().maxFuel/100).ToString("#,#") + " → " + (UpgradeManager.upgradesStarter[i].upgradeItemValues[0].effectValue/100).ToString("#,#") + " deuterium cassets";
            }
            else if(UpgradeManager.upgradesStarter[i].type == "storage-bay"){
                effectReadout = PlayerManager.maxHaul.ToString("#,#") + " → " + UpgradeManager.upgradesStarter[i].upgradeItemValues[0].effectValue.ToString("#,#") + "m<sup>3</sup> max haul";
            }
            else if(UpgradeManager.upgradesStarter[i].type == "scanner"){
                effectReadout = (PlayerManager.maxPulse * 10).ToString("F") + " → " + (UpgradeManager.upgradesStarter[i].upgradeItemValues[0].effectValue * 10).ToString("F") + " meter radius";
            }
            else if(UpgradeManager.upgradesStarter[i].type == "drone"){
                zoomLevels = PlayerManager.gameObject.GetComponentInChildren<RigManager>().zoomLevels[0].ToString("N1") + "x ";
                effectReadout = zoomLevels + "→ +" + UpgradeManager.upgradesStarter[i].upgradeItemValues[0].effectValue.ToString("N1") + "x zoom"; 
            }
            upgradePanels[i].transform.Find("Name").GetComponent<TextMeshProUGUI>().text = UpgradeManager.upgradesStarter[i].upgradeItemValues[0].flavorName;
            upgradePanels[i].transform.Find("Desc.").GetComponent<TextMeshProUGUI>().text = UpgradeManager.upgradesStarter[i].upgradeItemValues[0].flavorDesc;
            upgradePanels[i].transform.Find("Price").GetComponent<TextMeshProUGUI>().text = "Price: " + UpgradeManager.upgradesStarter[i].upgradeItemValues[0].price.ToString("#,#") + " cr.";
            upgradePanels[i].transform.Find("Effect").GetComponent<TextMeshProUGUI>().text = effectReadout;
        }
    }
    public void InitCassettes(){
        foreach(Transform cassette in cassetteParent.transform){
            cassettes.Add(cassette.gameObject);
        }
    }
}
