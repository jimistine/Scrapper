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
    public Timer Timer;
    public Image screenCover;
    public bool waitingToEnterTown;
    public float fadeDuration;
    [Header("Colors")]
    [Space(10)]
    public Color coverColor;
    public Color myBlack;
    [Header("General")]
    [Space(10)]
    public bool gamePaused;
    [Header("Tutorial")]
    [Space(10)]
    public bool gameStarted;
    public bool waitingForAcceleration;
    public bool introCompleted;
    public bool ogdenVisited;
    public bool chundrVisited;
    public bool outOfFuelCompleted;
    public bool showTip_1;
    [Header("Debugging")]
    [Space(10)]
    public bool skipIntroDialogue;
    public bool lotsOfCredits;
    public bool scannerActive;
    public bool lowFuel;
    public bool nearTown;
    public float totalCreditsEarned;
    public float totalCreditsSpent;
    [System.Serializable]
    public struct Sale{
        public float amount;
        public float day;
    }
    public List<Sale> Sales;
    


    void Awake(){
        Dir = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        StartGame();
        DialogueRunner.onDialogueComplete.AddListener(LoadTownOnDialogueEnd);

        coverColor = screenCover.color;

//Debugging
        if(lotsOfCredits){
            PlayerManager.PM.playerCredits = 1000000;
        }
        else{
            PlayerManager.PM.playerCredits = 497;
        }
        if(lowFuel){
            PlayerManager.PM.GetComponent<fuel>().currentFuelUnits = 10;
        }
        else{
            PlayerManager.PM.GetComponent<fuel>().currentFuelUnits = PlayerManager.PM.GetComponent<fuel>().maxFuel;
        }
        if(nearTown){
            PlayerManager.PM.gameObject.transform.position = new Vector3(0, -2, 0);
        }
        else{
            PlayerManager.PM.gameObject.transform.position = new Vector3(19, -44, 0);
        }
        if(scannerActive){
            PlayerManager.PM.scannerActive = true;
        }
        else{
            PlayerManager.PM.scannerActive = false;
        }
    }
    public void AddSale(float amount){
        Sale newSale;
        newSale.day = Timer.day;
        newSale.amount = amount;
        totalCreditsEarned += amount;
        Sales.Add(newSale);
    }

    public void StartGame(){
        gameStarted = true;
        StartCoroutine("StartGameRoutine");
    }

    IEnumerator StartGameRoutine(){
        Debug.Log("Starting game");
        AudioManager.AM.InitTowTigAudio();
        AudioManager.TransitionToOverworld();
        StartFadeCanvasGroup(screenCover.gameObject,"out", fadeDuration);
        yield return new WaitForSeconds(3);
        //yield return new WaitForSeconds(fadeDuration + 3);
        if(skipIntroDialogue){
            yield return null;;
        }
        else{
            DialogueManager.RunNode("intro-2-1");
            Debug.Log("sending intro node");
        }
    }

    public void StartWaitToEnterTown(){
        Debug.Log("Starting wait to enter town");
        waitingToEnterTown = true;
        PlayerManager.PM.SetPlayerMovement(false);
        StartCoroutine(FadeCanvasGroup(UIManager.gameObject, "out", .5f));
        // now we're waiting for dialogue to end which will trigger the town to actually load
    }
    public void LoadTownOnDialogueEnd(){
        //Debug.Log("Load town on dialogue end called");
        if(waitingToEnterTown){
            waitingToEnterTown = false;
            Debug.Log("entering town");
            StartCoroutine(EnterTown());
        }
    }
    public void StartEnterTown(){
        if(DialogueManager.DM.isDialogueRunner1Running){
            StartWaitToEnterTown();
        }
        else{
            StartCoroutine(EnterTown());
        }
    }
    IEnumerator EnterTown(){
        screenCover.color = myBlack;
        StartFadeCanvasGroup(screenCover.gameObject, "in", 0.5f);
        yield return new WaitForSeconds(0.75f);
        UIManager.gameObject.SetActive(true);
        UIManager.gameObject.GetComponent<CanvasGroup>().alpha = 1;
        AudioManager.AM.TransitionToTownExterior();
        PlayerManager.PM.SetPlayerMovement(false);
        PlayerManager.PM.gameObject.GetComponent<ClickDrag>().currentSpeed = 0;
        OverworldManager.OM.overworldCamera.SetActive(false);
        UIManager.EnterTown();
        if(PlayerManager.PM.GetComponent<fuel>().currentFuelUnits <= 0){
            UIManager.UIM.enterFuelMerchant();
        }
        SceneController.StartLoadTown();
        StartFadeCanvasGroup(screenCover.gameObject, "out", 0.5f);
    }
    
    public void StartLeaveTown(){
        if(chundrVisited == false){
            DialogueManager.DM.RunNode("leaving-town-check");
        }
        else{
            StartCoroutine(LeaveTown());
        }
    }
    IEnumerator LeaveTown(){
        screenCover.color = myBlack;
        StartFadeCanvasGroup(screenCover.gameObject, "in", 0.5f);
        DialogueManager.DM.ConversationEnded();
        yield return new WaitForSeconds(0.5f);
        AudioManager.AM.TransitionToOverworld();
        SceneController.StartLeaveTown();
        PlayerManager.PM.SetPlayerMovement(true);
        OverworldManager.OM.overworldCamera.SetActive(true);
        UIManager.LeaveTown();
        StartFadeCanvasGroup(screenCover.gameObject, "out", 0.5f);
        float timeToWait = Random.Range(2, 5);
        yield return new WaitForSeconds(timeToWait);
        if(ogdenVisited){
            DialogueManager.DM.RunNode("left-town-ogden");
        }
    }

    
    public void StartFadeDip(float timeDown, float timeBottom, float timeUp){
        StartCoroutine(FadeDip(timeDown, timeBottom, timeUp));
    }
    IEnumerator FadeDip(float timeDown, float timeBottom, float timeUp){
        screenCover.color = myBlack;
        Debug.Log("time down = " + timeDown);
        StartFadeCanvasGroup(screenCover.gameObject, "in", timeDown);
        yield return new WaitForSeconds(timeDown + 0.2f);
        yield return new WaitForSeconds(timeBottom);
        StartFadeCanvasGroup(screenCover.gameObject, "out", timeUp);
    }

    public void StartFadeCanvasGroup(GameObject element, string targetVisibility, float fadeTime){
        StartCoroutine(FadeCanvasGroup( element,  targetVisibility, fadeTime));
    }
    public void StartFadeCanvasGroup(GameObject element, string targetVisibility, float delayTime, float fadeTime){
        StartCoroutine(FadeCanvasGroup( element,  targetVisibility, delayTime, fadeTime));
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

            //Debug.Log("fading " + element + " " + targetVisibility);
            yield return null;
        }
        element.SetActive(isGroupActive);    
    }
    public IEnumerator FadeCanvasGroup(GameObject element, string targetVisibility, float delayTime, float fadeTime){
        yield return new WaitForSeconds(delayTime);
        float elapsedTime = 0.0f;
        float alphaStart;
        float alphaEnd;
        bool isGroupActive;
        bool lerping = true;
        //Debug.Log("Fading: " + element.name);

        if(targetVisibility == "in"){
            alphaStart = 0;
            alphaEnd = 1;
            isGroupActive = true;
            if(element != null){
                element.SetActive(true);    
            }
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

            //Debug.Log("fading " + element + " " + targetVisibility);
            yield return null;
        }
        element.SetActive(isGroupActive);    
    }
    public void QuitGame(){
        Application.Quit();
    }
    public void ToggleFullscreen(bool setting){
        if(setting == true){
            OverworldManager.OM.overworldCamera.GetComponent<AspectRatioControllerScalable>().enabled = false;
            Screen.fullScreen = true;
        }
        if(setting == false){
            OverworldManager.OM.overworldCamera.GetComponent<AspectRatioControllerScalable>().enabled = true;
            Screen.fullScreen = false;
        }
    }
}
