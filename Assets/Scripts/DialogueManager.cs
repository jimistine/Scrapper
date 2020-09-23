using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Yarn.Unity;

public class DialogueManager : MonoBehaviour
{
    // public float speed;
    // public Text text;
    // public string fullText;
    // private string currentText = "";
    public static DialogueManager DM;

    public DialogueRunner dialogueRunner;
    public DialogueUI DialogueUI;

    void Awake(){
        DM = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //ShowText();
    }

    // IEnumerator ShowText(){
    //     for(int i = 0; i < fullText.Length; i++){
    //         currentText = fullText.Substring(0, 1);
    //         this.GetComponent<Text>().text = currentText;
    //         yield return new WaitForSeconds(speed);
    //     }
    // }

    public void ContinueDialogue(){
        DialogueUI.MarkLineComplete();
    }
}
