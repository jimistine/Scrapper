using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrapTickOnHover : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    UIManager UIManager = UIManager.UIM;
    AudioManager AudioManager = AudioManager.AM;
    int tickIndex;

    public void OnPointerEnter(PointerEventData eventData){
        Debug.Log("Mouse is over GameObject.");
        tickIndex = transform.GetSiblingIndex();
        UIManager.OnTickHover(tickIndex);
        AudioManager.OnHover();
        PlayerManager.PM.tickReadoutIndex = tickIndex;
    }

    public void OnPointerExit(PointerEventData eventData){
        Debug.Log("Mouse is no longer on GameObject.");
        UIManager.OnTickHoverExit();
    }
    
}
