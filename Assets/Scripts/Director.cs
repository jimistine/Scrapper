using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Director : MonoBehaviour
{

    public static Director Dir;

    public DialogueManager DialogueManager;
    public SceneController SceneController;
    public AudioManager AudioManager;
    public UIManager UIManager;
    public Image screenCover;

    public bool gameStarted;
    public float fadeDuration;

    public Color coverColor;
    public Color fadeColor;


    void Awake(){
        Dir = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        coverColor = screenCover.color;
        fadeColor = new Color(coverColor.r, coverColor.b , coverColor.g, 0);
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
        yield return new WaitForSeconds(3);
        DialogueManager.RunNode("intro");

    }

    public void StartWaitToEnterTown(){
        StartCoroutine("WaitToEnterTown");
    }
    IEnumerator WaitToEnterTown(){
        float elapsedTime = 0.0f;
        float uiFadeTime = 1.0f;
        // fade ui / fade to black?
        while(elapsedTime < uiFadeTime){
            elapsedTime += Time.deltaTime;
            UIManager.gameObject.GetComponent<CanvasGroup>().alpha = elapsedTime/uiFadeTime;
            Debug.Log("elapsedtime: " + elapsedTime);
            yield return null;
        }
        // prevent movement
        PlayerManager.PM.SetPlayerMovement(false);
        // enter town on dialogue end
            // probably make a separate meathod that listens to the on dialogue end event
                //if waiting to enter town -> star the load town
                //else do nothing
        SceneController.StartCoroutine("LoadTown");
    }
}
