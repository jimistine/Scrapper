using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;



public class TitleScreenManager : MonoBehaviour
{
    public AudioMixerSnapshot fadedOut;
    public AudioMixerSnapshot defaultSnapshot;
    public float transitionTime;
    public Image screenCover;
    // Start is called before the first frame update
    void Start()
    {
        defaultSnapshot.TransitionTo(2);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void StartGameFromTitle(){
        StartCoroutine(StartGameRoutine());
    }
    IEnumerator StartGameRoutine(){
        Debug.Log("Starting game.");

        fadedOut.TransitionTo(transitionTime + 1);

        float elapsedTime = 0;
        Color screenCoverStart = screenCover.color;
        screenCover.gameObject.SetActive(true);
        while(elapsedTime < transitionTime){
            screenCover.color = Color.Lerp(screenCoverStart, Color.black, elapsedTime/transitionTime); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return null;
        SceneManager.LoadScene("ManagerScene");
        SceneManager.UnloadSceneAsync("TitleScreen");

    }
}
