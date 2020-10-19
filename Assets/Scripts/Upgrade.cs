using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    //public float priceIncreasePercentBottom;
    //public float priceIncreasePercentTop;
    //public float priceModifier;
    //public float priceOffered;
    //public float effectRangeBottom;
    //public float effectRangeTop;
    //public float effectOffered;
    //public float effectModifier;
    public string type;
    public string statAffected;
    public int upgradeLevel;
    public int upgradeLevelMax;
    [System.Serializable]
    public struct UpgradeItem{
        public string flavorName;
        public string flavorDesc;
        public float effectValue;
        public float price;
    }
    public UpgradeItem[] upgradeItemValues;

    public Upgrade(string upgradeType,  
                   UpgradeItem[] uItemValues,
                   /*float uPriceBottom, float uPriceTop, float uPriceModifier, float uPriceOffered, 
                   float eRangeBottom, float eRangeTop, float eOffered, float eModifier,*/
                   string stAffected, int uLevel, int uLevelMax)
    {
        type = upgradeType;
        upgradeItemValues = uItemValues;
        //priceIncreasePercentBottom = uPriceBottom;
        //priceIncreasePercentTop = uPriceTop;
        //priceModifier = uPriceModifier;
        //priceOffered = uPriceOffered;
        //effectRangeBottom = eRangeBottom;
        //effectRangeTop = eRangeTop;
        //effectOffered = eOffered;
        //effectModifier = eModifier;
        statAffected = stAffected;
        upgradeLevel = uLevel;
        upgradeLevelMax = uLevelMax;
    }
}
