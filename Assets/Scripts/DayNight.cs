using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

public class DayNight : MonoBehaviour
{

    [Header("Time")]
    [Space(5)]
    public Timer clock;
    public float clockTime;
    public float dayLength;
    public float transitionLength;
    public bool isNight;
    public bool rising;
    public bool isDay;
    public bool setting;
    [Header("Lights")]
    [Space(5)]
    public Light2D globalLight;
    public Light2D daru;

    float t;
    bool startRise;
    bool startSet;


    // Start is called before the first frame update
    void Start()
    {
       // clock = GameObject.Find("Timer").GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {/*             0m          5m          10m
                        ttt             ttt
                      2m - 3m         7m - 8m
                        2.5m            7.5m
                    nnnnn-ddddddddddddddd-nnnnn
                    12am        12pm        12am
                    |    |      |        |    |
        Global i   0.1  .5      1       .5   0.1

                Mathf.Lerp(a, b, t);
        // t needs to be between 0 and 1
        // when t = 0.5, you're halfway there.

        float t;
        float elapsedTime = 0;
        float transitionLength = 60;
        float globalLightIntensity = 0.1;

        void Update(){
            if(elapsedTime <= transitionLength){
                globalLightIntensity = Mathf.Lerp(0.1f, 1, elapsedTime/transitionLength);
                elapsedTime += Time.deltaTime;
            }
            else{
                globalLightIntensity = 1;
            }
        }
        // at eTime = 0, etime/length = 0
        //    eTime = 1, etime/length = .016  -> 1.6% of the way to 1
        //    eTime = 2, etime/length = .03   -> 3.0%
        //      ...
        //    eTime = 30, etime/length = .5   -> 50%
        //      ...
        //    eTime = 60, etime/length = 1   -> 100%


        // --------------------------------------------------------- //

        void Start(){
            StartCoroutine(Sunrise());
        }

        IEnumerator Sunrise(){
            elapsedTime = 0;
            while(elapsedTime < transitionLength){
                globalLightIntensity = Mathf.Lerp(0.1f, 1, elapsedTime/transitionLength);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            globalLightIntensity = 1;
        }
        IEnumerator Sunset(){
            elapsedTime = 0;
            while(elapsedTime < transitionLength){
                globalLightIntensity = Mathf.Lerp(1, 0.1f, elapsedTime/transitionLength);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            globalLightIntensity = 0.1f;
        }

        // --------------------------------------------------------- //

        // SmoothDamp -> try using this for the WASD turns
        // maybe RotateTowards for the rig lights

    */
    // GLOBAL LIGHT

        clockTime = clock.time;

        // RISES between 2m and 3m
        if(clockTime >= 120 && clockTime <= (120 + transitionLength)){
            if(rising == false){
                rising = true;
                StartCoroutine(Sunrise());
            }
        }
        else {
            rising = false;
        }

        // becomes DAY at 2.5m, stops halfway through sunset
        if(clockTime >= 120 + (transitionLength/2) && clockTime <= 420 + (transitionLength/2)){
            isDay = true;
            //startRise = false; // reset the coroutine bool gate for rise
            //rising = false; // reset the coroutine bool gate for rise
        }
        else{
            isDay = false;
        }

        // SETS between 7m and 8m 
        if(clockTime >= 420 && clockTime <= (420 + transitionLength)){
            if(setting == false){
                setting = true;
                StartCoroutine(Sunset());
            }
        }
        else{
            setting = false;
        }

        // becomes NIGHT at 7.5m - stops halfway through the sunrise
        if(clockTime >= 420 + (transitionLength/2) || clockTime <= (120 + transitionLength/2))
            isNight = true;
        else{
            isNight = false;
        }
        // reset at midnight, add a day
        if(clockTime >= dayLength){
            clockTime = 0;
            clock.day += 1;
        }

      // while rising, ping 
       
        // if(rising){
        //     //t = Mathf.PingPong(Time.time, transitionLength)/transitionLength;
        //     //t = 0;
        //     t += Time.deltaTime;
        //     globalLight.intensity = Mathf.Lerp(0.1f, 1, (t/transitionLength));
        // }
        // if(t > transitionLength){
        //     rising = false;
        //     t = 0;
        // }

      // while setting, pong
        // if(setting){
        //     t = Mathf.PingPong(Time.time, transitionLength)/transitionLength;
        //     globalLight.intensity = Mathf.Lerp(1f, 0.1f, t);
        // }


        // DARU, THE SUN
        // XOLA, THE FIRST MOON OF TALACAN
        // ROPTSU, THE SECOND MOON OF TALACAN
    }
    IEnumerator Sunrise(){
        float elapsedTime = 0;
        Debug.Log("Sunrise starting");
        while(elapsedTime < transitionLength){
            globalLight.intensity = Mathf.Lerp(0.1f, 1, elapsedTime/transitionLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Sunrise complete");
        globalLight.intensity = 1;
    }
    IEnumerator Sunset(){
        float elapsedTime = 0;
        Debug.Log("Sunset starting");
        while(elapsedTime < transitionLength){
            globalLight.intensity = Mathf.Lerp(1f, 0.1f, elapsedTime/transitionLength);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Sunset complete");
        globalLight.intensity = 0.1f;
    }
}
