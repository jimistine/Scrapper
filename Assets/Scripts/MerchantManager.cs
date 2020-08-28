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
    public UpgradeCalculator UpgradeCalculator;

    [Header("Scrap Buyer")]
    [Space(5)]
    public float scrapValue;

    // for slightly randomizeing the price and effectiveness the upgrades
    public float enginePriceBottom;
    public float enginePriceTop;
    public float enginePriceModifier;
    public float enginePriceOffered;
    public float speedRangeBottom;
    public float speedRangeTop;
    public float speedUpgradeOffered;
    public float speedUpgradeModifier;
    public int engineUpgradeLevel = 0;

    public float fuelTankPriceBottom;
    public float fuelTankPriceTop;
    public float fuelTankPriceModifier;
    public float fuelTankPriceOffered;
    public float maxFuelRangeBottom;
    public float maxFuelRangeTop;
    public float maxFuelUpgradeOffered;
    public float maxFuelUpgradeModifier;
    public int fuelTankUpgradeLevel = 0;


    [Header("Fuel Merchant")]
    [Space(5)]
    public float fuelPrice;
    public float fuelToAdd;
    public float creditsToTakeFuel;


    void Start()
    {
        PlayerManager = PlayerManager.PM;
        UIManager = UIManager.UIM;
        UpgradeCalculator = this.GetComponent<UpgradeCalculator>();
    }

    void Update(){
        if (Input.GetMouseButtonDown(0)){
            BuyUpgrade(UpgradeCalculator.upgrades.Find(x => x.upgradeName == EventSystem.current.currentSelectedGameObject.name));
        }
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
            Debug.Log("cleared inventory");
            UIManager.SoldScrap();
        }
        else{
            UIManager.cantSellScrap();
        }
    }

    // Upgrades
    public Upgrade BuyUpgrade(Upgrade upgrade){
        PlayerManager.playerCredits -= upgrade.upgradePriceOffered;
        UpgradeCalculator.CalculateUpgrade(upgrade);
        return null;
    }

    public void BuyEngineUpgrade(){
        // initial values for speed and price offered are entered by jimi, maybe calculated on average scrap selling price
        // increase speed
        PlayerManager.clickToMove.speed += speedUpgradeOffered;
        // take their money
        PlayerManager.playerCredits -= enginePriceOffered;
        // keep track of upgrade progressoin with int?
        engineUpgradeLevel++;
        // every time it's divisible by 5, the increments are bigger
        if (engineUpgradeLevel % 5 == 0){
            enginePriceOffered += enginePriceModifier * (enginePriceOffered * Random.Range(enginePriceBottom, enginePriceTop));
            speedUpgradeOffered += speedUpgradeModifier * (speedUpgradeOffered * Random.Range(speedRangeBottom, speedRangeTop));
        }
        else{
            // increment price per level
            enginePriceOffered += enginePriceOffered * Random.Range(enginePriceBottom, enginePriceTop);
            // increment effect per level
            speedUpgradeOffered += speedUpgradeOffered * Random.Range(speedRangeBottom, speedRangeTop);
        }
        // merchant says something about it and the engine name changes
        UIManager.BoughtEngineUpgrade();
    }
    public void BuyFuelUpgrade(){
        PlayerManager.fuelManager.maxFuel += maxFuelUpgradeOffered;
        PlayerManager.playerCredits -= fuelTankPriceOffered;
        fuelTankUpgradeLevel++;
        if (fuelTankUpgradeLevel % 5 == 0){
            fuelTankPriceOffered += fuelTankPriceModifier * (fuelTankPriceOffered * Random.Range(fuelTankPriceBottom, fuelTankPriceTop));
            maxFuelUpgradeOffered += maxFuelUpgradeModifier * (maxFuelUpgradeOffered * Random.Range(maxFuelRangeBottom, maxFuelRangeTop));
        }
        else{
            fuelTankPriceOffered += fuelTankPriceOffered * Random.Range(fuelTankPriceBottom, fuelTankPriceTop);
            maxFuelUpgradeOffered += maxFuelUpgradeOffered * Random.Range(maxFuelRangeBottom, maxFuelRangeTop);
        }
        UIManager.BoughtFuelTankUpgrade();
    }

    public void BuyHaulSizeUpgrade(){
        
    }
    public void BuySearchRadiusUpgrade(){
        
    }
    public void BuyMapScaleUpgrade(){
        
    }

// FUEL MERCHANT
    public void FillFuel(){
        // How much is it to fill up?
        fuelToAdd = PlayerManager.fuelManager.maxFuel - PlayerManager.fuelManager.currentFuelUnits;
        creditsToTakeFuel = fuelToAdd * fuelPrice;
        // Can they afford a fill?
        if(PlayerManager.playerCredits >= creditsToTakeFuel){
            PlayerManager.playerCredits -= creditsToTakeFuel;
            PlayerManager.fuelManager.currentFuelUnits = PlayerManager.fuelManager.maxFuel;
            UIManager.BoughtFuel();
        }
        else{
            fuelToAdd = PlayerManager.playerCredits / fuelPrice;
            PlayerManager.fuelManager.currentFuelUnits += fuelToAdd;
            PlayerManager.playerCredits = 0;
            UIManager.CantAffordFill();
        }
    }
    public void UpgradeFuelEfficiency(){
        
    }
}
