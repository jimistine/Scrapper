﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetClickedOn : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

        if (Input.GetMouseButtonDown(0)){           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);
            //RaycastHit2D hit = Physics2D.GetRayIntersection(ray);
            if(hit.collider != null){
                Debug.Log("Clicked: " + hit.collider.transform.name);         
            }
        }  
    }
}
