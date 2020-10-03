using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Timer : MonoBehaviour
{
    public TextMeshProUGUI timerText;
    public float time;
    public TextMeshProUGUI dayText;
    public float day = 1;
 
     void Update() {
         time += Time.deltaTime;
 
         var minutes = time / 60; //Divide the guiTime by sixty to get the minutes.
         //var days = Mathf.RoundToInt(minutes / 10); //Divide the guiTime by sixty to get the minutes.
         var seconds = time % 60;//Use the euclidean division for the seconds.
         var fraction = (time * 100) % 100;
 
         //update the label value
         if (gameObject.tag == "UI"){
            timerText.text = string.Format ("{0:00} : {1:00}", minutes, seconds);
            dayText.text = ("Day " + day.ToString());
         }
     }
}

