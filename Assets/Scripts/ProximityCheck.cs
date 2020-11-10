using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityCheck : MonoBehaviour
{
    public bool interactable;
    public bool fading;
    public float waitToMarkOutOfRangeTime;
    public float fadeDuration;
    public Color inRangeColor;
    public Color outOfRangeColor;
    
    public void IsInRange(bool inRange){
        if(inRange){
            interactable = true;
        }
        else{
            interactable = false;
            if(!fading && gameObject.activeSelf){
                StartCoroutine(FadeScrap());
            }
        }
    }
    IEnumerator FadeScrap(){
        fading = true;
        float elapsedTime = 0;
        yield return new WaitForSeconds(waitToMarkOutOfRangeTime);
        while(elapsedTime < fadeDuration){
            gameObject.GetComponent<SpriteRenderer>().color = Color.Lerp(inRangeColor, outOfRangeColor, elapsedTime/fadeDuration); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        fading = false;
    }
}
