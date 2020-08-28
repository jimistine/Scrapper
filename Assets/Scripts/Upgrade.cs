using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public string type;
    public string uName;
    public string desc;
    public float priceBottom;
    public float priceTop;
    public float priceModifier;
    public float priceOffered;
    public float effectRangeBottom;
    public float effectRangeTop;
    public float effectOffered;
    public float effectModifier;
    public string statAffected;
    public int upgradeLevel;
    [System.Serializable]
    public struct FlavorText{
        public string flavorName;
        public string flavorDesc;
    }
    public FlavorText[] flavorTexts;

    public Upgrade(string upgradeType, string upgradeName, string upgradeDesc, 
                   FlavorText[] fTexts,
                   float uPriceBottom, float uPriceTop, float uPriceModifier, float uPriceOffered, 
                   float eRangeBottom, float eRangeTop, float eOffered, float eModifier, 
                   string stAffected, int uLevel)
    {
        type = upgradeType;
        uName = upgradeName;
        desc = upgradeDesc;
        flavorTexts = fTexts;
        priceBottom = uPriceBottom;
        priceTop = uPriceTop;
        priceModifier = uPriceModifier;
        priceOffered = uPriceOffered;
        effectRangeBottom = eRangeBottom;
        effectRangeTop = eRangeTop;
        effectOffered = eOffered;
        effectModifier = eModifier;
        statAffected = stAffected;
        upgradeLevel = uLevel;
    }
}
