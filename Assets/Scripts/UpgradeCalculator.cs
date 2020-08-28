using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeCalculator : MonoBehaviour
{

    [Header("Upgrades")]
    [Space(5)]
    public List<Upgrade> upgrades = new List<Upgrade>();

    public UIManager UIM;

    public float upgradePriceBottom;
    public float upgradePriceTop;
    public float upgradePriceModifier;
    public float upgradePriceOffered;
    public float effectRangeBottom;
    public float effectRangeTop;
    public float effectOffered;
    public float effectModifier;
    public float statAffected;
    public int upgradeLevel;

   
    // Start is called before the first frame update
    void Start()
    {
        UIM = UIManager.UIM;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    // player picks an upgrade to buy
    // we do our calculations on numbers taken from the UI?
        // or we do have a set of variables for each upgrade, but they get assigned to the generics
        //   and the generics pass it back to the specifics?
    // send the output back to the UI in question

    public Upgrade CalculateUpgrade(Upgrade upgrade){
        if(upgrade.upgradeName == "Engine"){
            

        }
        if(upgrade.upgradeName == "Reactor"){
            
        }
        
        statAffected += effectOffered;
        // take their money
        //PlayerManager.playerCredits -= enginePriceOffered;
        // keep track of upgrade progressoin with int?
        upgradeLevel++;
        // every time it's divisible by 5, the increments are bigger
        if (upgradeLevel % 5 == 0){
            upgradePriceOffered += upgradePriceModifier * (upgradePriceOffered * Random.Range(upgradePriceBottom, upgradePriceTop));
            effectOffered += effectModifier * (effectOffered * Random.Range(effectRangeBottom, effectRangeTop));
        }
        else{
            // increment price per level
            upgradePriceOffered += upgradePriceOffered * Random.Range(upgradePriceBottom, upgradePriceTop);
            // increment effect per level
            effectOffered += effectOffered * Random.Range(effectRangeBottom, effectRangeTop);
        }
        // merchant says something about it and the name + desc. change
        //UIManager.BoughtEngineUpgrade();

        return null;
    }
}
