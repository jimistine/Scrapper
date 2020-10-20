using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public static SceneController SC;
    public PlayerManager PlayerManager;
    public UIManager UIManager;

    public GameObject OverworldParent;
    public GameObject OverworldCamera;
    public GameObject TownCamera;

    public UnityEngine.Events.UnityEvent overworldLoaded;
    public UnityEngine.Events.UnityEvent initCharacters;


    void Awake(){
        SC = this;

        // int countLoaded = SceneManager.sceneCount;
        // Scene[] loadedScenes = new Scene[countLoaded];
        // for (int i = 0; i < countLoaded; i++){
        //     loadedScenes[i] = SceneManager.GetSceneAt(i);
        // }
        // foreach (Scene scene in loadedScenes){
        //     Debug.Log("Scene name: " + scene.name);
        //     if(scene.name == "OverworldScene"){
        //         Debug.Log("Unloading: " + scene.name);
        //         SceneManager.UnloadSceneAsync("OverworldScene");
        //     }
        // }
     
        SceneManager.LoadScene("OverWorldScene", LoadSceneMode.Additive);
    
    }

    void Start(){
        PlayerManager = PlayerManager.PM;
        UIManager = UIManager.UIM;
        OverworldCamera = GameObject.Find("Overworld Camera");
        OverworldParent = GameObject.FindWithTag("OverworldParent");

        initCharacters?.Invoke();
        overworldLoaded?.Invoke();
    }

    public void LoadOverworld(){
        SceneManager.LoadScene("OverWorldScene", LoadSceneMode.Additive);
        AudioManager.AM.TransitionToOverworld();
        overworldLoaded?.Invoke();
    }
    public void StartLoadTown(){
        if (DialogueManager.DM.isDialogueRunner1Running){
            Director.Dir.StartWaitToEnterTown();
        }
        else{
            StartCoroutine("LoadTown");
        }
    }
    public IEnumerator LoadTown(){
        SceneManager.LoadScene("Town", LoadSceneMode.Additive);
        yield return null;
    }
    public void StartLeaveTown(){
        StartCoroutine("LeaveTown");
        overworldLoaded?.Invoke();
    }
    public IEnumerator LeaveTown(){
        SceneManager.UnloadSceneAsync("Town");
        yield return null;
    }
    public void RestartGame(){
        Debug.Log("Restarting game");
        SceneManager.LoadScene("ManagerScene");
    }
}
