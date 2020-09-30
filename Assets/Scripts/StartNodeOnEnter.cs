﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yarn.Unity;

public class StartNodeOnEnter : MonoBehaviour
{

    public string nodeToTrigger;
    public DialogueRunner DialogueRunner; 
    // Start is called before the first frame update
    void Start()
    {
        DialogueRunner = GameObject.Find("YarnManager").GetComponent<DialogueRunner>();
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.gameObject.tag == "Player" && other.GetType() == typeof(EdgeCollider2D)){
            DialogueRunner.StartDialogue(nodeToTrigger);
        }
    }
}
