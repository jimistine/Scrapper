﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.Events;


public class OnScrapPanelController : MonoBehaviour
{
    public GameObject panelScrapGO;
    public ScrapObject panelScrap;
    public GameObject panelContainer;
    public bool isActive;
    public float ID;



    void Start(){
        UIManager.UIM.leftScrap.AddListener(StartActivate);
        panelContainer = gameObject.transform.Find("Panel Object").gameObject;
    }

    void Update(){
        if(gameObject.activeSelf){
            Vector3 scrapPos = Camera.main.WorldToScreenPoint(panelScrapGO.transform.position);
            gameObject.transform.position = scrapPos;
        }
        if(panelScrapGO.GetComponent<ProximityCheck>().interactable == true && isActive == false){
            StartCoroutine(Activate());
            isActive = true;
        }
        if(panelScrapGO.GetComponent<ProximityCheck>().interactable == false && isActive == true){
            StartCoroutine(Deactivate(1.5f));
            Debug.Log("Deactivated via update");
            isActive = false;
        }
        if(panelScrapGO.activeSelf == false){
            Destroy(this.gameObject);
        }
    }

    public void CallShowReadoutButton(){
        UIManager.UIM.ShowReadoutButton(panelScrapGO);
        Debug.Log("Deactivated via show readout");
        StartCoroutine(Deactivate(0f));
    }
    public void StartActivate(){
        if(panelScrapGO.GetComponent<ProximityCheck>().interactable == true){
            StartCoroutine(Activate());
        }
    }
    IEnumerator Activate(){
        Director.Dir.StartFadeCanvasGroup(panelContainer, "in", 0.15f);
        if(panelScrap.isBuried == true){ // they have not yet inspected this piece
            //panelScrap.isBuried = false;
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = "?";
            //Debug.Log("Showing: " + panelScrap.scrapName);
        }
        else{  // they have already inspected it
            Director.Dir.StartFadeCanvasGroup(panelContainer, "in", 0.15f);
            gameObject.GetComponentInChildren<TextMeshProUGUI>().text = 
                panelScrap.scrapName + "<size=60%><color=#798478> | <color=#FF752A>" + panelScrap.size.ToString("#,#") + " m<sup>3</sup></size>";
        }
        yield return null;
    }
    IEnumerator Deactivate(float timeToWait){
        yield return new WaitForSeconds(timeToWait);
        Director.Dir.StartFadeCanvasGroup(panelContainer, "out", 0.15f);
        
    }
}
