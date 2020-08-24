using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    public PlayerManager PlayerManager;
    public UIManager UIManager;

    [Header("Scrap Buyer")]
    [Space(5)]
    public float scrapValue;

    [Header("Fuel Merchant")]
    [Space(5)]
    public float fuelPrice;
    public float fuelToAdd;
    public float creditsToTakeFuel;

    void Awake(){
        
    }

    void Start()
    {
        PlayerManager = PlayerManager.PM;
        UIManager = UIManager.UIM;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
}
