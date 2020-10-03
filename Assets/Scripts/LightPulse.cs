using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class LightPulse : MonoBehaviour
{
    public float speed;
    public float maxIntensity;
    public float minIntensity;
    Light2D light;
    float startIntensity;
    // Start is called before the first frame update
    void Start()
    {
        light = gameObject.GetComponent<Light2D>();
        startIntensity = light.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // float intensity = Mathf.Sin(Time.timeSinceLevelLoad/speed);
        // if(intensity < 0){
        //     intensity = .15f;
        // }
        // light.intensity = startIntensity * intensity;

        light.intensity = Mathf.PingPong((Time.time)*speed, maxIntensity)+ minIntensity;
    }
}
