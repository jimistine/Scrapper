using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MerchantManager : MonoBehaviour
{
    public PlayerManager PlayerManager;
    public UIManager UIManager;
    public float scrapValue;

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
}
