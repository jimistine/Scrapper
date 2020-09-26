using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using TMPro;
using UnityEngine.Events;

public class DialogueManager : MonoBehaviour
{
    
    public static DialogueManager DM;

    public ScrapDialogueUI ui;
    public DialogueRunner DR;
    public SceneController SceneController;
    
    GameObject activeSpeakerPanel;
    string speakerNameLast;
    List<string> lineQueue;
    
    [System.Serializable]
    public struct Character{
        public string characterName;
        public GameObject characterPanel;
    }
    public List<Character> characters = new List<Character>();





    void Awake(){
        DM = this;
        SceneController.initCharacters.AddListener(InitCharacters);
        SceneController.overworldLoaded.AddListener(RunOverwolrdDialogue);
    }
    
    void Start(){
        

        ui.onDialogueStart.AddListener(InturruptCheck);
        ui.onLineStart.AddListener(LineStarted);
        ui.onLineUpdate.AddListener(LineUpdate);
        DR.onDialogueComplete.AddListener(ConversationEnded);
        
        DR = GameObject.Find("YarnManager").GetComponent<DialogueRunner>();
        ui = GameObject.Find("YarnManager").GetComponent<ScrapDialogueUI>();
    }
    
    public void InitCharacters(){
        Debug.Log("Initializing characters");
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
        if(speakerNameLast != ui.speakerName || speakerNameLast == null){
            if(activeSpeakerPanel != null){
                activeSpeakerPanel.SetActive(false);
            } 
            activeSpeakerPanel = characters.Find(x => x.characterName == ui.speakerName).characterPanel;
            activeSpeakerPanel.SetActive(true);
        }

        speakerNameLast = ui.speakerName;
        
    }

    private void LineUpdate(string line){
        activeSpeakerPanel.GetComponentInChildren<TextMeshProUGUI>().text = line;
    }

    private void ConversationEnded(){
        activeSpeakerPanel.SetActive(false);
        // return control etc
    }

    public void ContinueDialogue(){
        ui.MarkLineComplete();
    }
    public void RunOverwolrdDialogue(){

        

        Debug.Log("Running overworld dialogue");
        DR.StartDialogue("intro");
    }

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

    public void InturruptCheck(){
        if(DR.IsDialogueRunning){
            lineQueue.Add(ui.currentLineID); // check to see if this get's inturrupted line b4 trying to fix it
        }
    }
    /* 
    Sorry to inturrupt...

    at dialogue started
        if dialogue is already running
            grab the line
            add it to dialogue queue
            wait until line end
            run the new dialogue
                if it has nothing to say
                    run resume from yarn
                        picks up from next line with no transition
                        clears the dialogue queue
                if it does have dialogue to run
                    it'll run the dialogue
                    when we get to dialogue end
                    dialogue manager looks at queue
                        if something is there
                            play transition node
                                transition node picks a random transition
                                then runs command to tell dialogue runner to resume
                        if not, dialogue is over
    */
}
