using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class Director : MonoBehaviour
{

    public static Director Dir;

    public DialogueManager DialogueManager;
    public ScrapDialogueUI ScrapDialogueUI;
    public DialogueRunner DialogueRunner;
    public SceneController SceneController;
    public AudioManager AudioManager;
    public UIManager UIManager;
    public Image screenCover;

    public bool gameStarted;
    public bool waitingToEnterTown;
    public float fadeDuration;

    public Color coverColor;
    public Color fadeColor;


    void Awake(){
        Dir = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        DialogueRunner.onDialogueComplete.AddListener(LoadTownOnDialogueEnd);

        coverColor = screenCover.color;
        fadeColor = new Color(coverColor.r, coverColor.g , coverColor.b, 0);
    }

    public void StartGame(){
        gameStarted = true;
        StartCoroutine("StartGameRoutine");
    }

    IEnumerator StartGameRoutine(){
        Debug.Log("Starting game");
        AudioManager.TransitionToOverworld();

        float elapsedTime = 0.0f;
        while(elapsedTime < fadeDuration){
            elapsedTime += Time.deltaTime;
            screenCover.color = Color.Lerp(coverColor, fadeColor, (elapsedTime/fadeDuration));
            
            yield return null;
        }
        screenCover.gameObject.SetActive(false);
        yield return new WaitForSeconds(3);
        DialogueManager.RunNode("intro");

    }

    public void StartWaitToEnterTown(){
        Debug.Log("Starting wait to enter town");
        //StartCoroutine("WaitToEnterTown");
        waitingToEnterTown = true;
        PlayerManager.PM.SetPlayerMovement(false);
        StartCoroutine(FadeCanvasGroup(UIManager.gameObject, "out", .5f));
        // now we're waiting for dialogue to end which will trigger the town to actually load
    }
    // IEnumerator WaitToEnterTown(){
    //     waitingToEnterTown = true;
    //     float elapsedTime = 0.0f;
    //     float uiFadeTime = 1.0f;
    //     while(elapsedTime < uiFadeTime){
    //         elapsedTime += Time.deltaTime;
    //         UIManager.gameObject.GetComponent<CanvasGroup>().alpha -= 0.1f;
    //         Debug.Log("fading");
    //         yield return null;
    //     }
    // }
    public void LoadTownOnDialogueEnd(){
        Debug.Log("Load town on dialogue end called");
        if(waitingToEnterTown){
        Debug.Log("entering town");
            SceneController.StartCoroutine("LoadTown");
            StartCoroutine(FadeCanvasGroup(UIManager.gameObject, "in", 1f));
            UIManager.EnterTown();
            waitingToEnterTown = false;
        }
    }

    // public void FadeElement(GameObject element){

    //     float elapsedTime = 0.0f;
    //     float fadeTime = 1.0f;
    //     while(elapsedTime < fadeTime){
    //         elapsedTime += Time.deltaTime;
    //         UIManager.gameObject.GetComponent<CanvasGroup>().alpha -= 0.1f;
    //         Debug.Log("fading");
    //         yield return null;
    //     }
    // }
    public void StartFadeCanvasGroup(GameObject element, string targetVisibility, float fadeTime){
        StartCoroutine(FadeCanvasGroup( element,  targetVisibility, fadeTime));
    }
    public IEnumerator FadeCanvasGroup(GameObject element, string targetVisibility, float fadeTime){
        float elapsedTime = 0.0f;
        float alphaStart;
        float alphaEnd;
        bool isGroupActive;
        bool lerping = true;

        if(targetVisibility == "in"){
            alphaStart = 0;
            alphaEnd = 1;
            isGroupActive = true;
            element.SetActive(true);    
        }
        else{
            alphaStart = 1;
            alphaEnd = 0;
            isGroupActive = false;    
        }

        while(lerping){
            
            element.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(alphaStart, alphaEnd, elapsedTime/fadeTime);

            if(fadeTime < elapsedTime){
                lerping = false;
            }

            elapsedTime += Time.deltaTime;

            //element.GetComponent<CanvasGroup>().alpha = Mathf.Lerp(alphaStart, alphaEnd, elapsedTime/fadeTime);

            Debug.Log("fading " + element + " " + targetVisibility);
            yield return null;
        }
        element.SetActive(isGroupActive);    
        // while( element.GetComponent<CanvasGroup>().alpha > alphaEnd){
            // element.GetComponent<CanvasGroup>().alpha += fadeSpeed
            //}
       
    }
}
