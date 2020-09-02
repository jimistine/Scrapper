using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RigManager : MonoBehaviour
{

    /*
        What might belong to the Rig?
        - Search radius
        - Speed
        - Fuel
        - Health
        - Map scale
    */

public Camera droneCamera;
public int zoomIndex;
public List<float> zoomLevels = new List<float>();
public static RigManager RM;


    // Start is called before the first frame update
    void Awake(){
        RM = this;
    }
    void Start()
    {
        droneCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ChangeZoom(float currentZoom){
        Debug.Log("Curent Zoom index: " + zoomIndex);
        if((zoomLevels.FindIndex(x => x == currentZoom) + 1) >= zoomLevels.Count){
            zoomIndex = 0;
        }
        else{
            zoomIndex += 1;
        }
        droneCamera.orthographicSize = zoomLevels[zoomIndex];
        UIManager.UIM.UpdateZoomLevel(droneCamera.orthographicSize);
    }
}
