using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class scrap : MonoBehaviour
{
    public string scrapName;
    public string description;
    public int image;
    public int size;
    public int value;
    public string material;
    public float zoneA_rarity;
    public float zoneB_rarity;
    public float zoneC_rarity;
    public float zoneD_rarity;
    public bool carriesComponents;
    public bool isBuried;
}
