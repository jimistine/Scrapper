using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{

    [Header("Upgrades")]
    [Space(5)]
    public List<Upgrade> upgradesStarter = new List<Upgrade>();
    public List<Upgrade> upgrades = new List<Upgrade>();

    public UIManager UIM;
    public PlayerManager PlayerManager;

    public float priceModApplied;
    public float effectModApplied; 
   
    void Start()
    {
        UIM = UIManager.UIM;
        PlayerManager = PlayerManager.PM;
        for(int i = 0; i < upgradesStarter.Count; i++){
            upgrades.Add(Instantiate(upgradesStarter[i], this.transform));
        }
    }

    public Upgrade CalculateUpgrade(Upgrade upgrade){
        if(upgrade.type == "engine"){
            PlayerManager.gameObject.GetComponent<clickToMove>().speed += upgrade.effectOffered;
        }
        if(upgrade.type == "reactor"){
            PlayerManager.gameObject.GetComponent<fuel>().maxFuel += upgrade.effectOffered;
        }
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
        upgrade.priceOffered += priceModApplied * (upgrade.priceOffered * Random.Range(upgrade.priceIncreasePercentBottom, upgrade.priceIncreasePercentTop));
        // increment effect per level
        upgrade.effectOffered += effectModApplied * (upgrade.effectOffered * Random.Range(upgrade.effectRangeBottom, upgrade.effectRangeTop));
        // merchant says something about it and the name + desc. change
        UIM.BoughtUpgrade(upgrade);

        return null;
    }
}
