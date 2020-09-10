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
        SceneManager.LoadScene("Town", LoadSceneMode.Additive);
        PlayerManager.SetPlayerMovement(false);
        PlayerManager.gameObject.GetComponent<ClickDrag>().currentSpeed = 0;
        UIManager.EnterTown();
        OverworldCamera.SetActive(false);
        yield return null;
    }
    public void StartLeaveTown(){
        StartCoroutine("LeaveTown");
    }
    public IEnumerator LeaveTown(){
        PlayerManager.SetPlayerMovement(true);
        SceneManager.UnloadSceneAsync("Town");
        OverworldCamera.SetActive(true);
        UIManager.LeaveTown();
        yield return new WaitForSeconds(0.05f);
    }

    public void RestartGame(){
        SceneManager.LoadScene("ManagerScene");
    }
}
