using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MerchantManager : MonoBehaviour
{
    /*
        How to make a shop that sell upgrades and items for the player to buy and use?
        
        Sell Incremental Upgrades
        - Each upgrade progresses through 3-5 tiers at moderate price and upgradiness,
          followed by a big jump in both effect and to an even greater extent, price.
        - Price increases within a random range of a % of Base Upgrade Price for that upgrade
        - Upgradiness increases within a random range of a % of current value for that upgrade
            
            All currently sold by Scrap Buyer
            - Max Fuel
                - Most expensive
                - What do they even use for fuel???
                - Each upgrade is much more expensive than the last
            - Speed
                - Second most expensive
                - Churn Industries Mover 6-7 Mark I, II, III, IV, V
                - Significant increase in price with significantly varrying, but mostly small, increases in speed
            - Haul Size
                - Third most expensive
                - moderate increases in price and size
            - Search radius
                - Fifth most expensive
                - Very small increments in price and size
            - Map size
                - Only ~4 levels of zoom are available
                - Each are very expensive
                - Fist is base at base price, subsequent versions become available over time
                  and cost a significant %, if not always more, than what they player has in their wallet
            
            Sold by Fuel Merchant
            - Fuel Efficency
                - each new engine upgrade can be tweaked to improve efficiency once
                - base is 0.5, lower is more efficent


        Notes on Game Economy
        https://www.gamasutra.com/blogs/DarinaEmelyantseva/20190418/340987/5_Basic_Steps_in_Creating_Balanced_InGame_Economy.php
            
            - Try to get an idea of total time to beat, and base most values off of that
                -  How long is a session? Make sure they can experince a variety of emotions within that unit of time
            - Try to make each segment of the game end up with the player at 0
                - They had just enough to get what they needed
    */

    public PlayerManager PlayerManager;
    public UIManager UIManager;
    public UpgradeManager UpgradeManager;


    [Header("Scrap Buyer")]
    [Space(5)]
    public float scrapValue;
    public List<ScrapObject> SoldScrap;


    [Header("Fuel Merchant")]
    [Space(5)]
    public fuel fuelManager;
    public float fuelPrice;
    public float fuelToAdd;
    public float creditsToTakeFuel;
    public float towPriceModifier;
    public bool ogdenVisited;


    void Start()
    {
        PlayerManager = PlayerManager.PM;
        UIManager = UIManager.UIM;
        UpgradeManager = this.GetComponent<UpgradeManager>();
        fuelManager = PlayerManager.GetComponent<fuel>();
    }

    void Update(){
       
    }

// SCRAP BUYER
    public void SellAllScrap(){
        if(PlayerManager.playerScrap.Count != 0){
            foreach(ScrapObject scrap in PlayerManager.playerScrap){
                scrapValue += scrap.value;
                SoldScrap.Add(scrap);
                Debug.Log("Sold a: " + scrap.scrapName);
            }
            PlayerManager.playerCredits += scrapValue;
            PlayerManager.playerScrap.Clear();
            PlayerManager.UpdateCurrentHaul();
            Debug.Log("cleared inventory");
            UIManager.SoldScrap();
            scrapValue = 0;
        }
        else{
            UIManager.cantSellScrap();
        }
    }

// Upgrades
    // u.1 button passes name of upgrade as string
    public void BuyUpgrade(string upgrade){ 
        // upgrade values are in a list of specific upgrade items on Upgrade Manager, edit Updgrades starter in inspector 
        //    and we populate the rest as we go.
        // Check progress of upgrades as childeren of Merchant Manager object
        Upgrade upgradeToCalculate = UpgradeManager.upgrades.Find(x => x.type == upgrade);
        if(PlayerManager.playerCredits >= upgradeToCalculate.upgradeItemValues[upgradeToCalculate.upgradeLevel].price){
            if(upgradeToCalculate.upgradeLevel == upgradeToCalculate.upgradeLevelMax){
                AudioManager.AM.PlayMiscUIClip("reject");
                //UIManager.UpgradeAlreadyMaxed(upgradeToCalculate);
            }
            else{
                PlayerManager.playerCredits -= upgradeToCalculate.upgradeItemValues[upgradeToCalculate.upgradeLevel].price;
                UpgradeManager.CalculateUpgrade(upgradeToCalculate);
                AudioManager.AM.PlayRandomUpgrade();
            }
        }
        else{
            UIManager.CantAffordUpgrade(upgradeToCalculate);
            AudioManager.AM.PlayMiscUIClip("reject");
        }
        if(upgradeToCalculate.upgradeLevel == upgradeToCalculate.upgradeLevelMax){
            UIManager.UIM.UpgradeAlreadyMaxed(upgradeToCalculate);
        }
    }

// FUEL MERCHANT
    public void EnterFuelMerchant(){
        // put the price on the button
        UpdateFuelPrice();
        
        if(PlayerManager.GetComponent<fuel>().currentFuelUnits <= 0 ){
            DialogueManager.DM.RunNode("ogden-towed");
            UIManager.fuelMerchantReadout.text += "\n<size=75%>A service fee of "+ (PlayerManager.playerCredits*towPriceModifier).ToString("#,#") + " credits has been detucted from your account</size>";
            PlayerManager.playerCredits -= PlayerManager.playerCredits*towPriceModifier;
        }
    }
    public void UpdateFuelPrice(){
        fuelToAdd = PlayerManager.fuelManager.maxFuel - PlayerManager.fuelManager.currentFuelUnits;
        creditsToTakeFuel = fuelToAdd * fuelPrice;
        UIManager.fillFuelButtonText.text = creditsToTakeFuel.ToString("#,#") + " cr.";
    }
    public void FillFuel(){
        // How much is it to fill up?
        // Can they afford a fill?
        if(PlayerManager.playerCredits >= creditsToTakeFuel){
            if (PlayerManager.fuelManager.currentFuelUnits == PlayerManager.fuelManager.maxFuel){
                UIManager.FuelAlreadyFull();
                AudioManager.AM.PlayMiscUIClip("reject");
            }
            else{
                PlayerManager.playerCredits -= creditsToTakeFuel;
                PlayerManager.fuelManager.currentFuelUnits = PlayerManager.fuelManager.maxFuel;
                UpdateFuelPrice();
                fuelManager.UpdateFuelPercent();
                UIManager.BoughtFuel();
                AudioManager.AM.FillFuel();
            }
        }
        else{
            fuelToAdd = PlayerManager.playerCredits / fuelPrice;
            PlayerManager.fuelManager.currentFuelUnits += fuelToAdd;
            if(PlayerManager.playerCredits == 0){
                AudioManager.AM.PlayMiscUIClip("reject");
            }
            else{
                AudioManager.AM.PlayMiscUIClip("fill fuel");
            }
            PlayerManager.playerCredits = 0;
            UpdateFuelPrice();
            fuelManager.UpdateFuelPercent();
            UIManager.CantAffordFill();
        }
    }
    public void TweakReactor(){
        UIManager.TweakedReactor();
    }

}
