using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;


public class LightPulse : MonoBehaviour
{
    public float speed;
    public float maxIntensity;
    public float minIntensity;
    Light2D thisLight;
    float startIntensity;
    // Start is called before the first frame update
    void Start()
    {
        thisLight = gameObject.GetComponent<Light2D>();
        startIntensity = thisLight.intensity;
    }

    // Update is called once per frame
    void Update()
    {
        // float intensity = Mathf.Sin(Time.timeSinceLevelLoad/speed);
        // if(intensity < 0){
        //     intensity = .15f;
        // }
        // light.intensity = startIntensity * intensity;

        thisLight.intensity = Mathf.PingPong((Time.time)*speed, maxIntensity)+ minIntensity;
    }
}
