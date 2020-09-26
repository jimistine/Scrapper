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
    public DialogueRunner DialogueRunner;
    public SceneController SceneController;
    
    //public string characterName = "Hasron:";
    GameObject activeSpeakerPanel;
    string speakerNameLast;
    
    [System.Serializable]
    public struct Character{
        public string characterName;
        public GameObject characterPanel;
    }
    public List<Character> characters;



    void Awake(){
        DM = this;
    }
    
    void Start(){
        DialogueRunner = GameObject.Find("Yarn Manager").GetComponent<DialogueRunner>();
        ui = GameObject.Find("Yarn Manager").GetComponent<ScrapDialogueUI>();
        SceneController = SceneController.SC;

        ui.onLineStart.AddListener(LineStarted);
        ui.onLineUpdate.AddListener(LineUpdate);
        DialogueRunner.onDialogueComplete.AddListener(ConversationEnded);
        //SceneController.overworldLoaded.AddListener(RunOverwolrdDialogue);
        RunOverwolrdDialogue();
        
    }
    
    

    public void LineStarted(){   // if the speaker of this line is different from the last line, swap active panels
        Debug.Log("Line started");
        if(speakerNameLast != ui.speakerName || speakerNameLast == null){
            if(activeSpeakerPanel != null){
                activeSpeakerPanel.SetActive(false);
            } 
            activeSpeakerPanel = characters.Find(x => x.characterName == ui.speakerName).characterPanel;
            activeSpeakerPanel.SetActive(true);
            Debug.Log("Swapped to " + ui.speakerName);
        }

        speakerNameLast = ui.speakerName;
        
    }

    private void LineUpdate(string line)
    {
        activeSpeakerPanel.GetComponentInChildren<TextMeshProUGUI>().text = line;
        //Debug.Log(line);
    }


    private void ConversationEnded(){
        activeSpeakerPanel.SetActive(false);
        // return control etc
    }


    public void ContinueDialogue(){
        ui.MarkLineComplete();
    }
    public void RunOverwolrdDialogue(){
       DialogueRunner.StartDialogue("intro");
    }
}
