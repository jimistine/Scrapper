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


    [Header("Fuel Merchant")]
    [Space(5)]
    public float fuelPrice;
    public float fuelToAdd;
    public float creditsToTakeFuel;
    public float towPriceModifier;


    void Start()
    {
        PlayerManager = PlayerManager.PM;
        UIManager = UIManager.UIM;
        UpgradeManager = this.GetComponent<UpgradeManager>();
    }

    void Update(){
       
    }

// SCRAP BUYER
    public void SellAllScrap(){
        if(PlayerManager.playerScrap.Count != 0){
            foreach(ScrapObject scrap in PlayerManager.playerScrap){
                scrapValue += scrap.value;
                Debug.Log("Sold a: " + scrap.scrapName);
            }
            PlayerManager.playerCredits += scrapValue;
            PlayerManager.playerScrap.Clear();
            PlayerManager.UpdateCurrentHaul();
            Debug.Log("cleared inventory");
            UIManager.SoldScrap();
        }
        else{
            UIManager.cantSellScrap();
        }
    }

    // Upgrades
    public void BuyUpgrade(string upgrade){ // button passes name of upgrade as string
        // upgrade values are in a list on Upgrade Manager, edit Updgrades starter in inspector 
        //    and we populate the rest as we go.
        // Check progress of upgrades as childered of Merchant Manager object
        Upgrade upgradeToCalculate = UpgradeManager.upgrades.Find(x => x.type == upgrade);
        if(upgradeToCalculate.upgradeLevel == upgradeToCalculate.upgradeLevelMax){
            UIManager.UpgradeAlreadyMaxed(upgradeToCalculate);
        }
        else{
            PlayerManager.playerCredits -= upgradeToCalculate.priceOffered;
            UpgradeManager.CalculateUpgrade(upgradeToCalculate);
        }
    }

// FUEL MERCHANT
    public void EnterFuelMerchant(){
        // put the price on the button
        fuelToAdd = PlayerManager.fuelManager.maxFuel - PlayerManager.fuelManager.currentFuelUnits;
        creditsToTakeFuel = fuelToAdd * fuelPrice;
        UIManager.fillFuelButtonText.text = creditsToTakeFuel.ToString("#,#") + " cr.";
        if(PlayerManager.fuelLevel <= 0 ){
            UIManager.fuelMerchantReadout.text = "\"I am sorry to have retrieved you, but I am glad glad to see that you are unharmed. The fee is appreciated as always." 
                +"\nPlease, buy your fill of what deuterium I have. \""
                +"\n<sub>A service fee of "+ (PlayerManager.playerCredits*towPriceModifier).ToString("#,#") + " has been detucted from your account</sub>";
                PlayerManager.playerCredits -= PlayerManager.playerCredits*towPriceModifier;
        }
        else{
        // this should be pulling from a list of welcomes
            UIManager.fuelMerchantReadout.text = "\"Welcome to my establishment, gentlepersons.\"";
        }
    }
    public void FillFuel(){
        // How much is it to fill up?
        // Can they afford a fill?
        if(PlayerManager.playerCredits >= creditsToTakeFuel){
            if (PlayerManager.fuelManager.currentFuelUnits == PlayerManager.fuelManager.maxFuel){
                UIManager.FuelAlreadyFull();
            }
            else{
            PlayerManager.playerCredits -= creditsToTakeFuel;
            PlayerManager.fuelManager.currentFuelUnits = PlayerManager.fuelManager.maxFuel;
            UIManager.BoughtFuel();
            }
        }
        else{
            fuelToAdd = PlayerManager.playerCredits / fuelPrice;
            PlayerManager.fuelManager.currentFuelUnits += fuelToAdd;
            PlayerManager.playerCredits = 0;
            UIManager.CantAffordFill();
        }
    }
    public void TweakReactor(){
        UIManager.TweakedReactor();
    }

}
