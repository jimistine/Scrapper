using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;
using UnityEngine.UI;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    
    public static DialogueManager DM;
    public ScrapDialogueUI ui;

    public string test = "Hasron: Well look at that.";
    public string characterName = "Hasron:";
    GameObject activeSpeakerPanel;
    
    [System.Serializable]
    public struct Characters{
        public string characterName;
        public GameObject characterPanel;
    }
    public List<Characters> characters;

    void Awake(){
        DM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        ui.onLineStart.AddListener(LineStarted);
        ui.onLineUpdate.AddListener(LineUpdate);
    }
    void Update(){
        
    }

    private void LineUpdate(string line){
        Debug.Log(line);
    }

    private void LineStarted(){
        //play a sound
        //...
    }

    private void ConversationEnded(){
        // return control etc
    }

    /*
    To swap panels based on who is talking
    DialogueUI looks at each line
        -> doRunLine
        specifically everything until thre is a : in the line
            we pass 
                line = "Hasron: Well look at that"
            get the name
                Match speakerName = RegEx.Match(line, @"^.*?(?=:)");
            set the current dialogue container to the gameObject associated with the same name in our list of names
                dialogueConainer = characterNames.Find(x => x.characterName == speakerName).characterPanel);
    */

    public void ContinueDialogue(){
        ui.MarkLineComplete();
    }
    public void SwapTest(string speakerName){
        // set proper text elements active
        activeSpeakerPanel = characters.Find(x => x.characterName == speakerName).characterPanel;
        activeSpeakerPanel.SetActive(true);
        activeSpeakerText = activeSpeakerPanel.GetComponentInChildren<TextMeshProUGUI>().text;
        Debug.Log("Swapping to " + characterName);
    }
}
