using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrapTickOnHover : MonoBehaviour
    , IPointerEnterHandler
    , IPointerExitHandler
{
    UIManager UIManager = UIManager.UIM;
    int tickIndex;
    void Update()
    {

    }

    public void OnPointerEnter(PointerEventData eventData){
        Debug.Log("Mouse is over GameObject.");
       // GameObject tickSlots = gameObject.transform.parent.gameObject;
        tickIndex = transform.GetSiblingIndex();
        UIManager.OnTickHover(tickIndex);
        PlayerManager.PM.tickReadoutIndex = tickIndex;
    }

    public void OnPointerExit(PointerEventData eventData){
        Debug.Log("Mouse is no longer on GameObject.");
        UIManager.OnTickHoverExit();
    }
    
}
