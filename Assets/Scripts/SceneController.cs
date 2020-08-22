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


    void Awake(){
        SC = this;
        SceneManager.LoadScene("OverWorldScene", LoadSceneMode.Additive);
    }

    void Start(){
        
    }

    public void LoadTown(){
        SceneManager.LoadScene("Town", LoadSceneMode.Additive);
    }
    public void LeaveTown(){
        SceneManager.LoadScene("OverworldScene");
    }
}
