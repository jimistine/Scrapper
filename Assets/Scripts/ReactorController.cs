using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class ReactorController : MonoBehaviour
{
    // Start is called before the first frame update
    void Awake()
    {
        
    }
    public GameObject currentCassette;
    // Update is called once per frame
    void Update()
    {
        // Fuel details
        // max units / number of cassets = max value per slider
        //float cassetMaxValue = PlayerManager.fuelManager.maxFuel/PlayerManager.fuelManager.numCassets;
        // number of sliders is based on max fuel and updated from the Upgrade Manager when a reactor is bought
        // go through all sliders, when current slider is 0, go to next
        // subtract from the current slider along with the total units
        
        foreach(GameObject cassette in UIManager.UIM.cassettes){
            currentCassette = cassette;
            if(cassette.GetComponent<Slider>() != null){
                Slider currentSlider = cassette.GetComponent<Slider>();
                while (currentSlider.value > 0){
                    return;
                } 
            }
        }
    }
}
