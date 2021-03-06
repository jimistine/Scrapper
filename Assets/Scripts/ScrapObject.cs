﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrapObject : MonoBehaviour
{
    public string scrapName;
    public string description;
    public int image;
    public float size;
    public int value;
    public string material;
    public int zoneA_rarity;
    public int zoneB_rarity;
    public int zoneC_rarity;
    public float ID;
    public bool carriesComponents;
    public bool isBuried;


    public ScrapObject(string name, string desc, int img, float siz, int val, string mat, 
                        int rarityA, int rarityB, int rarityC, float id, bool components, bool buried)
        {
            scrapName = name;
            description = desc;
            image = img;
            size = siz;
            value = val;
            material = mat;
            zoneA_rarity = rarityA;
            zoneB_rarity = rarityB;
            zoneC_rarity = rarityC;
            ID = id;
            carriesComponents = components;
            isBuried = buried;
        }
}
