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
    public bool autoAdvanceOverWorldText;

    public System.Action testAction;
    
    [System.Serializable]
    public struct Character{
        public string characterName;
        public GameObject characterPanel;
        public GameObject characterPanelTown;
    }

    public GameObject activeSpeakerPanel;
    public List<Character> characters = new List<Character>();

    public List<Yarn.Line> lineQueue = new List<Yarn.Line>();
    public List<string> lineQueueIDs = new List<string>();
    public Yarn.Line queuedLine;
    
    [Header("Images")]
    [Space(10)]
    public Sprite continueIcon;
    public Sprite finishedIcon;

    string speakerNameLast;
    Dictionary<string, int> randomLinePulls = new Dictionary<string, int>();




    void Awake(){
        DM = this;
        SceneController.initCharacters.AddListener(InitCharacters);
        SceneController.overworldLoaded.AddListener(RunOverwolrdDialogue);
    }
    
    void Start(){
        

        //ui.onDialogueStart.AddListener(InturruptCheck);
        ui.onLineStart.AddListener(LineStarted);
        ui.onLineUpdate.AddListener(LineUpdate);
        ui.onLineEnd.AddListener(LineEnd);
        ui.onLineFinishDisplaying.AddListener(FinishedDisplayingText);
        ui.onDialogueEnd.AddListener(scrapOnDialogueComplete);
        DR.onDialogueComplete.AddListener(ConversationEnded);
        //DR.onNodeComplete.AddListener(NodeEnded);
        
        DR = GameObject.Find("YarnManager").GetComponent<DialogueRunner>();
        ui = GameObject.Find("YarnManager").GetComponent<ScrapDialogueUI>();

// Functions
    // when we need unity to tell yarn something in the middle of a conversation
        // adds the node requesting the random numebr    to a dictionary to make sure
        //   we don't play the same random line for that node twice in a row
        DR.AddFunction("random", 2, delegate(Yarn.Value[] parameters){
            var requestingNode = parameters[0];
            var maxNum = parameters[1];
            int numberToReturn = (int)Random.Range(0, (Mathf.Round(maxNum.AsNumber)+1));
            //Debug.Log("Max number was: " + Mathf.Round(maxNum.AsNumber));

            if(!randomLinePulls.ContainsKey(requestingNode.AsString)){
                randomLinePulls.Add(requestingNode.AsString, numberToReturn);
            }
            else if(randomLinePulls[requestingNode.AsString] == numberToReturn){
                while(randomLinePulls[requestingNode.AsString] == numberToReturn){
                    //Debug.Log("updating node number to speak");
                    numberToReturn = (int)Random.Range(0, Mathf.Round(maxNum.AsNumber));
                }
                randomLinePulls[requestingNode.AsString] = numberToReturn;
            }
            //Debug.Log("Random line number is: " + numberToReturn);
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
        Hasron.characterPanelTown = GameObject.Find("Hasron Callout Town");
        Hasron.characterPanel.SetActive(false);
        Hasron.characterPanelTown.SetActive(false);
        characters.Add(Hasron);

        Character Chip = new Character();
        Chip.characterName = "CH1-P";
        Chip.characterPanel = GameObject.Find("CH1-P Callout");
        Chip.characterPanelTown = GameObject.Find("CH1-P Callout Town");
        Chip.characterPanel.SetActive(false);
        Chip.characterPanelTown.SetActive(false);
        characters.Add(Chip);
    }

    public void LineStarted(){   // if the speaker of this line is different from the last line, swap active panels
        
        if(speakerNameLast != ui.speakerName || speakerNameLast == null){
            if(activeSpeakerPanel != null){
                activeSpeakerPanel.SetActive(false);
            } 
            if(UIManager.UIM.playerLocation != "overworld" && ui.speakerName != "Chundr"){
                activeSpeakerPanel = characters.Find(x => x.characterName == ui.speakerName).characterPanelTown;
                //Debug.Log("Using town panels. Current active panel is " + activeSpeakerPanel.name);
            }
            else{
                activeSpeakerPanel = characters.Find(x => x.characterName == ui.speakerName).characterPanel;
            }
            Director.Dir.StartFadeCanvasGroup(activeSpeakerPanel,"in", 0.1f);
        }
        if(UIManager.UIM.playerLocation == "overworld" && activeSpeakerPanel.tag == "merchant"){
            activeSpeakerPanel = characters.Find(x => x.characterName == ui.speakerName).characterPanel; 
        }

        speakerNameLast = ui.speakerName;
        //prefill the dynamic textbox with invisible text
        //Debug.Log(ui.currentLine.ID);
        //string prefit = "hank";//DR.strings[ui.currentLine.ID];
        string prefit = DR.strings[ui.currentLine.ID];
        prefit = Regex.Replace(prefit, ui.speakerName + ": ", "");
        activeSpeakerPanel.GetComponentsInChildren<TextMeshProUGUI>()[1].text = prefit;
        // set the end icon
        if(activeSpeakerPanel.tag == "merchant"){
            activeSpeakerPanel.GetComponentsInChildren<Image>()[1].sprite = continueIcon;
        }
    }
    private void LineUpdate(string line){
        activeSpeakerPanel.GetComponentsInChildren<TextMeshProUGUI>()[0].text = line;
    }
    public void FinishedDisplayingText(){
        // Show the continue or period icon if there is or isn't more dialogue repectively


        // time out panel after a beat once the line is there.
        // if the node being run has the sub tag, start the counter
        string tagsLine = string.Join(" ", DR.GetTagsForNode(DR.CurrentNodeName));
        if(tagsLine.Contains("sub") || autoAdvanceOverWorldText == true){
            //Debug.Log("starting speaker panel timeout");
            StartCoroutine(TimeOutSpeakerPanel());
        }
        if(Director.Dir.showTip_1){
            Director.Dir.StartFadeCanvasGroup(GameObject.Find("Tip_1"), "in", 0.5f, 1f);
        }
    }
    public void toggleAutoAdvance(bool setting){
        autoAdvanceOverWorldText = setting;
    }
    public IEnumerator TimeOutSpeakerPanel(){
        yield return new WaitForSeconds(2);
        ContinueDialogue();
    }

    public void LineEnd(){
        if(GameObject.Find("Tip_1") != null){
            Director.Dir.showTip_1 = false;
            Director.Dir.StartFadeCanvasGroup(GameObject.Find("Tip_1"), "out",1f);
        }
        // Debug.Log("Line ended");
    }

    public void scrapOnDialogueComplete(){
        if(activeSpeakerPanel.tag == "merchant"){
            activeSpeakerPanel.GetComponentsInChildren<Image>()[1].sprite = finishedIcon;
        }
        //Debug.Log("Dialogue complete");
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
            //Debug.Log("Got tags");
            var tagsCurrent = string.Join(" ", DR.GetTagsForNode(DR.CurrentNodeName));
            if(tags.Contains("main")){ // MAIN is never interrupted and interrupts anything else, including Main
                //Debug.Log("running MAIN dialogue");
                DR.StartDialogue(nodeToRun);
            }
            else if(tags.Contains("world")){ // WORLD is only interrupted by Main and World, only interrupts Sub and World
                if(DR.IsDialogueRunning && tagsCurrent.Contains("main")){
                    return;
                }
                else{
                    //Debug.Log("running WORLD dialogue");
                    DR.StartDialogue(nodeToRun); 
                }
            }
            else if(tags.Contains("sub")){ // SUB is interrupted by anything, and does not interrupt anything
                if(DR.IsDialogueRunning){
                    return;
                }
                else{
                   // Debug.Log("running SUB dialogue");
                    int randomRoll = Random.Range(0, 4);
                    //Debug.Log("Rolled: " + randomRoll);
                    if(randomRoll == 0){
                        DR.StartDialogue(nodeToRun);
                    }
                }
            }
            else{
                if(tagsCurrent.Contains("main")){
                    return;
                }
                else{
                    DR.StartDialogue(nodeToRun); 
                }
            }
        }
        else{
            if(tags.Contains("sub")){
                int randomRoll = Random.Range(0, 4);
                //Debug.Log("Rolled: " + randomRoll);
                if(randomRoll == 0){
                    DR.StartDialogue(nodeToRun);
                }
            }
            else{
                DR.StartDialogue(nodeToRun);
            }
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

    public void ConversationEnded(){
        DR.Stop();
        //Debug.Log("conversation ended");
        if(UIManager.UIM.playerLocation == "town hub"){
            activeSpeakerPanel.SetActive(false);
        }
        else{
            if(UIManager.UIM.playerLocation != "overworld"){
                return;
            }
            else{
                Director.Dir.StartFadeCanvasGroup(activeSpeakerPanel, "out", 0.25f);
            }
        }
        speakerNameLast = null;
        if(lineQueue.Count > 0){
            //Debug.Log("starting transition");
            DR.StartDialogue("transition");
        }
    }
    public void NodeEnded(){

    }

// Commands
    // when we need yarn to tell unity to do something
    [YarnCommand("setPanelVisibility")]
    public void setPanelVisibility(string characterName, string isPanelVisible){
        //Debug.Log("setting " + characterName + "'s " + "panel visibility to " + isPanelVisible);
        if(isPanelVisible == "false"){
            Director.Dir.StartFadeCanvasGroup(characters.Find(x => x.characterName == ui.speakerName).characterPanel, "out", 0.1f);
            //characters.Find(x => x.characterName == ui.speakerName).characterPanel.SetActive(false);
        }
        if(isPanelVisible == "true"){
            Director.Dir.StartFadeCanvasGroup(characters.Find(x => x.characterName == ui.speakerName).characterPanel, "in", 0.1f);
            //characters.Find(x => x.characterName == ui.speakerName).characterPanel.SetActive(true);
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
        AudioManager.AM.FillFuel();
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
    [YarnCommand("giveScanner")]
    public void giveScanner(){
        PlayerManager.PM.scannerActive =  true;
        AudioManager.AM.PlayRandomUpgrade();
    }
    [YarnCommand("introWaitingForAcceleration")]
    public void introWaitingForAcceleration(){
        Director.Dir.waitingForAcceleration = true;
    }
    [YarnCommand("introComplete")]
    public void introComplete(){
        Director.Dir.introCompleted = true;
    }
    [YarnCommand("ogdenVisited")]
    public void ogdenVisited(){
        Director.Dir.ogdenVisited = true;
    }
    [YarnCommand("ogdenVisited")]
    public void chundrVisited(){
        Director.Dir.chundrVisited = true;
    }
    [YarnCommand("outOfFuelTutorialCompleted")]
    public void completedOutOfFuelTut(){
        Director.Dir.outOfFuelCompleted = true;
    }
    [YarnCommand("towReady")]
    public void dialogueTowReady(string isTowReady){
        if(isTowReady == "true"){
            OverworldManager.OM.waitingOnDialogue = false;
        }
        if(isTowReady == "false"){
            OverworldManager.OM.waitingOnDialogue = true;
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
