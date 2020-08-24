using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    // Next is disabling scene parent in overworkd during town scene
    // how do we store everything in town scene so that it is the same when you go back?
    // Rough skeleton of town with vendors and things to buy
    public static SceneController SC;
    public PlayerManager PlayerManager;
    public UIManager UIManager;

    public GameObject OverworldParent;
    public GameObject OverworldCamera;
    public GameObject TownCamera;


    void Awake(){
        SC = this;
        SceneManager.LoadScene("OverWorldScene", LoadSceneMode.Additive);
    }

    void Start(){
        PlayerManager = PlayerManager.PM;
        UIManager = UIManager.UIM;
        OverworldCamera = GameObject.Find("Overworld Camera");
        OverworldParent = GameObject.FindWithTag("OverworldParent");
    }
    public void StartLoadTown(){
        StartCoroutine("LoadTown");
    }
    public IEnumerator LoadTown(){
        //OverworldParent.SetActive(false);
        //SceneManager.UnloadSceneAsync("OverworldScene");

        SceneManager.LoadScene("Town", LoadSceneMode.Additive);
        OverworldCamera.SetActive(false);
        PlayerManager.TogglePlayerMovement();
        UIManager.EnterTown();
        yield return null;
    }
    public void StartLeaveTown(){
        StartCoroutine("LeaveTown");
    }
    public IEnumerator LeaveTown(){
        SceneManager.UnloadSceneAsync("Town");
        OverworldCamera.SetActive(true);
        UIManager.LeaveTown();
        yield return new WaitForSeconds(0.05f);
        PlayerManager.TogglePlayerMovement();
        //SceneManager.LoadScene("OverworldScene", LoadSceneMode.Additive);
    }
}
