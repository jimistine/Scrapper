using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    [Header("Upgrades")]
    [Space(5)]
    public List<Upgrade> upgrades = new List<Upgrade>();

    public UIManager UIM;
    public PlayerManager PlayerManager;

    public float priceModApplied;
    public float effectModApplied; 
    // public float upgradePriceTop;
    // public float upgradePriceModifier;
    // public float upgradePriceOffered;
    // public float effectRangeBottom;
    // public float effectRangeTop;
    // public float effectOffered;
    // public float effectModifier;
    // public float statAffected;
    // public int upgradeLevel;

    /*
        - we need dictionaries for each upgrade type with name and desc as entries
        - if we keep a dictionary of dictionaries it would be upgrade type and the relevant dic
        - then we look at the main dic, find the matching type, and apply the name and desc from the
            index according to the upgrade level

        or each upgrade has dictionary with names and descs
        then we match the upgrade level to the index of that dic and go from there
    */

   
    void Start()
    {
        UIM = UIManager.UIM;
        PlayerManager = PlayerManager.PM;
    }

    public Upgrade CalculateUpgrade(Upgrade upgrade){
        if(upgrade.type == "engine"){
            PlayerManager.gameObject.GetComponent<clickToMove>().speed += upgrade.effectOffered;
            // maybe the UI update gets called here too?
        }
        if(upgrade.type == "reactor"){
            PlayerManager.gameObject.GetComponent<fuel>().maxFuel += upgrade.effectOffered;
        }
        
        upgrade.statAffected += upgrade.effectOffered;
        upgrade.upgradeLevel++;
        // every time it's divisible by 5, the increments are bigger
        if (upgrade.upgradeLevel % 5 == 0){
            priceModApplied = upgrade.priceModifier;
            effectModApplied = upgrade.effectModifier;
        }
        else{
            priceModApplied = 1; effectModApplied = 1;
        }
        // increment price per level
        upgrade.priceOffered += priceModApplied * (upgrade.priceOffered * Random.Range(upgrade.priceBottom, upgrade.priceTop));
        // increment effect per level
        upgrade.effectOffered += effectModApplied * (upgrade.effectOffered * Random.Range(upgrade.effectRangeBottom, upgrade.effectRangeTop));
        // merchant says something about it and the name + desc. change
        UIM.BoughtUpgrade(upgrade);

        return null;
    }
}
