using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public string type;
    public float priceIncreasePercentBottom;
    public float priceIncreasePercentTop;
    public float priceModifier;
    public float priceOffered;
    public float effectRangeBottom;
    public float effectRangeTop;
    public float effectOffered;
    public float effectModifier;
    public string statAffected;
    public int upgradeLevel;
    public int upgradeLevelMax;
    [System.Serializable]
    public struct FlavorText{
        public string flavorName;
        public string flavorDesc;
    }
    public FlavorText[] flavorTexts;

    public Upgrade(string upgradeType,  
                   FlavorText[] fTexts,
                   float uPriceBottom, float uPriceTop, float uPriceModifier, float uPriceOffered, 
                   float eRangeBottom, float eRangeTop, float eOffered, float eModifier, 
                   string stAffected, int uLevel, int uLevelMax)
    {
        type = upgradeType;
        flavorTexts = fTexts;
        priceIncreasePercentBottom = uPriceBottom;
        priceIncreasePercentTop = uPriceTop;
        priceModifier = uPriceModifier;
        priceOffered = uPriceOffered;
        effectRangeBottom = eRangeBottom;
        effectRangeTop = eRangeTop;
        effectOffered = eOffered;
        effectModifier = eModifier;
        statAffected = stAffected;
        upgradeLevel = uLevel;
        upgradeLevelMax = uLevelMax;
    }
}
