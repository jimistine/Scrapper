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
    public Color moonLight;
    public Color daylight;
    public Light2D daru;
    public Vector3 daruStart;// = new Vector3;(-80f, 2.8f,-3f);
    public Vector3 daruNoon;// = new Vector3(0f, 2.8f,-3f);
    public Vector3 daruEnd;// = new Vector3(80f, 2.8f,-3f);

    float t;
    bool startRise;
    bool startSet;
    bool struckMidnight;


    // Start is called before the first frame update
    void Start()
    {
       // clock = GameObject.Find("Timer").GetComponent<Timer>();
    }

    // Update is called once per frame
    void Update()
    {/*                      TIMELINE
                    0m          5m          10m
                        ttt             ttt
                      2m - 3m         7m - 8m
                        2.5m            7.5m
                    nnnnn-ddddddddddddddd-nnnnn
                    12am        12pm        12am
                    |    |      |        |    |
        Global i   0.1  .5      1       .5   0.1
    */
    // GLOBAL LIGHT

        clockTime = clock.time;

        // RISES between 2m and 3m
        if(clockTime >= 120 && clockTime <= (120 + transitionLength)){
            if(rising == false){
                rising = true;
                struckMidnight = false;
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
        if(clockTime >= 420 + (transitionLength/2) || clockTime <= (120 + transitionLength/2)){
            isNight = true;
        }
        else{
            isNight = false;
        }
        // reset at midnight, add a day
        if(clockTime >= dayLength && struckMidnight == false){
            struckMidnight = true;
            clockTime = 0;
            clock.day += 1;

            daru.transform.position = daruStart;
        }
    // DARU, THE SUN
    }

    IEnumerator Sunrise(){
        float elapsedTime = 0;
        Debug.Log("Sunrise starting");
        while(elapsedTime < transitionLength){
            globalLight.intensity = Mathf.Lerp(0.15f, 1, elapsedTime/transitionLength);
            globalLight.color = Color.Lerp(moonLight, daylight, elapsedTime/transitionLength);
           
            daru.intensity = Mathf.Lerp(0.15f, 1, elapsedTime/transitionLength);
            daru.color = Color.Lerp(moonLight, daylight, elapsedTime/transitionLength);
            daru.transform.position = Vector3.Lerp(daruStart, daruNoon, elapsedTime/transitionLength);
           
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Sunrise complete");
        globalLight.intensity = 1;
        globalLight.color = daylight;
        daru.intensity = 1;
        daru.transform.position = daruNoon;
        daru.color = daylight;
    }
    IEnumerator Sunset(){
        float elapsedTime = 0;
        Debug.Log("Sunset starting");
        while(elapsedTime < transitionLength){
            globalLight.intensity = Mathf.Lerp(1f, 0.15f, elapsedTime/transitionLength);
            globalLight.color = Color.Lerp(daylight, moonLight, elapsedTime/transitionLength);

            daru.intensity = Mathf.Lerp(1, 0.15f, elapsedTime/transitionLength);
            daru.color = Color.Lerp(daylight, moonLight, elapsedTime/transitionLength);
            daru.transform.position = Vector3.Lerp(daruNoon, daruEnd, elapsedTime/transitionLength);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        Debug.Log("Sunset complete");
        globalLight.intensity = 0.1f;
        globalLight.color = moonLight;
        daru.intensity = 1;
        daru.transform.position = daruEnd;
        daru.color = moonLight;
    }

        // XOLA, THE FIRST MOON OF TALACAN
        // ROPTSU, THE SECOND MOON OF TALACAN

}
