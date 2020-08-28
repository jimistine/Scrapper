using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Upgrade : MonoBehaviour
{
    public string upgradeName;
    public float upgradePriceBottom;
    public float upgradePriceTop;
    public float upgradePriceModifier;
    public float upgradePriceOffered;
    public float effectRangeBottom;
    public float effectRangeTop;
    public float effectOffered;
    public float effectModifier;
    public string statAffected;
    public int upgradeLevel;

    public Upgrade(string uName, 
                   float uPriceBottom, float uPriceTop, float uPriceModifier, float uPriceOffered, 
                   float eRangeBottom, float eRangeTop, float eOffered, float eModifier, 
                   string stAffected, int uLevel)
    {
        upgradeName = uName;
        upgradePriceBottom = uPriceBottom;
        upgradePriceTop = uPriceTop;
        upgradePriceModifier = uPriceModifier;
        upgradePriceOffered = uPriceOffered;
        effectRangeBottom = eRangeBottom;
        effectRangeTop = eRangeTop;
        effectOffered = eOffered;
        effectModifier = eModifier;
        statAffected = stAffected;
        upgradeLevel = uLevel;
    }
}
