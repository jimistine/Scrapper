using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class testCountingScript : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CountUp());
    }

    // Update is called once per frame
    void Update()
    {
    }

    public Slider slider;
    public IEnumerator CountUp(){

        while(slider.value < slider.maxValue){
            slider.value++;
            yield return new WaitForSeconds(1);
        }
    }
}
