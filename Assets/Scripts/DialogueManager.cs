using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;
using System.Text.RegularExpressions;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    
    public static DialogueManager DM;

    public ScrapDialogueUI ui;
    public DialogueRunner DR;
    public MerchantManager MerchantManager;
    //public DialogueRunner DialogueRunner_2;

    //public bool isDialogueRunner2Running;
    public SceneController SceneController;
    [Space(5)]
    public bool isDialogueRunner1Running;

    public System.Action testAction;
    
    [System.Serializable]
    public struct Character{
        public string characterName;
        public GameObject characterPanel;
    }

    public List<Character> characters = new List<Character>();

    public List<Yarn.Line> lineQueue = new List<Yarn.Line>();
    public List<string> lineQueueIDs = new List<string>();
    public Yarn.Line queuedLine;
    [Header("Images")]
    [Space(10)]
    public Sprite continueIcon;
    public Sprite finishedIcon;

    GameObject activeSpeakerPanel;
    string speakerNameLast;
    Dictionary<string, int> randomLinePulls = new Dictionary<string, int>();




    void Awake(){
        DM = this;
        SceneController.initCharacters.AddListener(InitCharacters);
        SceneController.overworldLoaded.AddListener(RunOverwolrdDialogue);
        testAction += scrapOnComplete;
    }
    
    void Start(){
        

        //ui.onDialogueStart.AddListener(InturruptCheck);
        ui.onLineStart.AddListener(LineStarted);
        ui.onLineUpdate.AddListener(LineUpdate);
        ui.onLineEnd.AddListener(LineEnd);
        ui.onLineFinishDisplaying.AddListener(FinishedDisplayingText);
        DR.onDialogueComplete.AddListener(ConversationEnded);
        
        DR = GameObject.Find("YarnManager").GetComponent<DialogueRunner>();
        ui = GameObject.Find("YarnManager").GetComponent<ScrapDialogueUI>();

// Functions
    // when we need unity to tell yarn something in the middle of a conversation
        // adds the node requesting the random to a dictionary to make sure
        //   we don't play the same random line for that node twice in a row
        DR.AddFunction("random", 2, delegate(Yarn.Value[] parameters){
            var requestingNode = parameters[0];
            var maxNum = parameters[1];
            int numberToReturn = (int)Random.Range(0, Mathf.Round(maxNum.AsNumber));

            if(!randomLinePulls.ContainsKey(requestingNode.AsString)){
                randomLinePulls.Add(requestingNode.AsString, numberToReturn);
            }
            else if(randomLinePulls[requestingNode.AsString] == numberToReturn){
                while(randomLinePulls[requestingNode.AsString] == numberToReturn){
                    numberToReturn = (int)Random.Range(0, Mathf.Round(maxNum.AsNumber));
                }
            }
            Debug.Log("Random line number is: " + numberToReturn);
            return numberToReturn;
        });
        

    }

    void Update(){
        isDialogueRunner1Running = DR.IsDialogueRunning;
    //    isDialogueRunner2Running = DialogueRunner_2.IsDialogueRunning;
    }
    
    public void InitCharacters(){
        //Debug.Log("Initializing characters");
        Character Hasron = new Character();
        Hasron.characterName = "Hasron";
        Hasron.characterPanel = GameObject.Find("Hasron Callout");
        Hasron.characterPanel.SetActive(false);
        characters.Add(Hasron);

        Character Chip = new Character();
        Chip.characterName = "CH1-P";
        Chip.characterPanel = GameObject.Find("CH1-P Callout");
        Chip.characterPanel.SetActive(false);
        characters.Add(Chip);
    }

    public void LineStarted(){   // if the speaker of this line is different from the last line, swap active panels
        if(activeSpeakerPanel != null){
            if(activeSpeakerPanel.tag == "merchant"){
                ui.textSpeed = 0.05f;
            }
            else{
                ui.textSpeed = 0.025f;
            }
        }
        if(speakerNameLast != ui.speakerName || speakerNameLast == null){
            if(activeSpeakerPanel != null && activeSpeakerPanel.tag != "merchant"){
                activeSpeakerPanel.SetActive(false);
            } 
            activeSpeakerPanel = characters.Find(x => x.characterName == ui.speakerName).characterPanel;
            if(activeSpeakerPanel.tag != "merchant"){
                Director.Dir.StartFadeCanvasGroup(activeSpeakerPanel,"in", 0.1f);
            }
        }

        speakerNameLast = ui.speakerName;
        //prefill the dynamic textbox with invisible text
        string prefit = DR.strings[ui.currentLine.ID];
        prefit = Regex.Replace(prefit, ui.speakerName + ": ", "");
        activeSpeakerPanel.GetComponentsInChildren<TextMeshProUGUI>()[1].text = prefit;
    }
    private void LineUpdate(string line){
        activeSpeakerPanel.GetComponentsInChildren<TextMeshProUGUI>()[0].text = line;
    }
    public void FinishedDisplayingText(){
        // time out panel after a beat once the line is there.
        // if the node being run has the sub tag, start the counter
        string tagsLine = string.Join(" ", DR.GetTagsForNode(DR.CurrentNodeName));
        if(tagsLine.Contains("sub")){
            Debug.Log("starting speaker panel timeout");
            StartCoroutine(TimeOutSpeakerPanel());
        }
    }
    public IEnumerator TimeOutSpeakerPanel(){
        yield return new WaitForSeconds(2);
        ContinueDialogue();
    }

    public void LineEnd(){
       // Debug.Log("Line ended");
    }

    public void scrapOnComplete(){
        
    }

    public void ContinueDialogue(){
        ui.MarkLineComplete();
    }
    public void RunOverwolrdDialogue(){

        //Debug.Log("Running overworld dialogue");
       
    }

    public void RunNode(string nodeToRun){

        // tage a look at those tags
        var tags = string.Join(" ", DR.GetTagsForNode(nodeToRun));
        if(DR.CurrentNodeName != null){
            var tagsCurrent = string.Join(" ", DR.GetTagsForNode(DR.CurrentNodeName));
            if(DR.IsDialogueRunning && tagsCurrent.Contains("main")){ // if we're already talking about a main plot point, don't inturrupt
                return;
            }
        }
        else if(DR.IsDialogueRunning && tags.Contains("sub")){ // if we're already talking, and this isn't that important, don't inturrupt
            return;
        }
        else if(!DR.IsDialogueRunning && tags.Contains("sub")){ // if we aren't talking, and it's a bark, roll to see if we play the bark
            int randomRoll = Random.Range(0, 2);
            Debug.Log("Rolled:" + randomRoll);
            if(randomRoll == 0){
                DR.StartDialogue(nodeToRun);
            }
        }
        else{
            DR.StartDialogue(nodeToRun);
        }
    }


    public void InturruptCheck(){
        Debug.Log("Checking inturrupt");
        if(DR.IsDialogueRunning){
            lineQueue.Add(ui.lastLine); // check to see if this get's inturrupted line b4 trying to fix it
            lineQueueIDs.Add(ui.lastLine.ID);
            Debug.Log("Inturrupted and grabed line: " + ui.lastLine.ID);
        }
    }

    private void ConversationEnded(){
        DR.Stop();
        Debug.Log("conversation ended");
        if(activeSpeakerPanel.tag != "merchant"){
            Director.Dir.StartFadeCanvasGroup(activeSpeakerPanel, "out", 0.25f);
            //activeSpeakerPanel.SetActive(false);
        }
        speakerNameLast = null;
        if(lineQueue.Count > 0){
            Debug.Log("starting transition");
            DR.StartDialogue("transition");
        }
    }

// Commands
    // when we need yarn to tell unity to do something
    [YarnCommand("setPanelVisibility")]
    public void setPanelVisibility(string characterName, string isPanelVisible){
        Debug.Log("setting " + characterName + "'s " + "panel visibility to " + isPanelVisible);
        if(isPanelVisible == "false"){
            characters.Find(x => x.characterName == ui.speakerName).characterPanel.SetActive(false);
        }
        if(isPanelVisible == "true"){
            characters.Find(x => x.characterName == ui.speakerName).characterPanel.SetActive(true);
        }
    }
    [YarnCommand("resume")]
    public void resume(){
        Debug.Log("Resume called");
        DR.Stop();
        if(lineQueue != null){
            // start form the line in queue
            Debug.Log("Resuming from " + lineQueue[0].ID);
            ui.RunLine(lineQueue[0], DR, testAction);
            lineQueue.Clear();
            lineQueueIDs.Clear();
        }
    }
    [YarnCommand("fillFuel")]
    public void fillFuel(){
        Debug.Log("FilledFuel");
        PlayerManager.PM.fuelManager.currentFuelUnits = PlayerManager.PM.fuelManager.maxFuel;
        PlayerManager.PM.fuelManager.UpdateFuelPercent();
        MerchantManager.UpdateFuelPrice();
    }
    [YarnCommand("setObjActive")] // this needs work to get the inactive objs
    public void disableObj(string objName, bool isObjActive){
        if(isObjActive == true){
            GameObject.Find(objName).SetActive(true);
        }
        if(isObjActive == false){
            GameObject.Find(objName).SetActive(false);
        }
        Debug.Log("Disabled: " + objName);
    }
    [YarnCommand("setFuelButtActive")]
    public void setFuelButtActive(string isObjActive){
        if(isObjActive == "true"){
            UIManager.UIM.fillFuelButt.interactable = true;
        }
        if(isObjActive == "false"){
            UIManager.UIM.fillFuelButt.interactable = false;
        }
    }
    
    /* 
    Sorry to inturrupt...

    x    at dialogue started
    x        if dialogue is already running
    x            grab the line that's being inturrupted
    x            add it to dialogue queue
    -            wait until line end
    x            run the new dialogue
                    if it has nothing to say
                        run resume from yarn
                            picks up from next line with no transition
                            clears the dialogue queue
                    if it does have dialogue to run
    x                    it'll run the dialogue
    x                    when we get to dialogue end
    x                    dialogue manager looks at queue
    x                        if something is there
    x                            play transition node
    x                                transition node runs function to pick a random transition
    x                                then runs command to tell dialogue manager to resume
                            if not, dialogue is over



                                    start dialogue in runner 1 at node
                                    on first line complete only
                                        start runner 2 at the same node
                                    on inturrupt
                                        stop runner 2
                                        runner 1 plays inturrupting dialogue
                                    on inturrupt end
                                        on dialogue complete -> runner 1 is done
                                        if runner 2 is paused
                                            resume runner 2
                                            send lines to UI
                                    on resume end
                                        runner 2 dialogue complete
                                        make sure runner 1 becomes primary runner
    */
}
