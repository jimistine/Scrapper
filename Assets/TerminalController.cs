using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class TerminalController : MonoBehaviour
{
    public TextMeshProUGUI ticketNumber;
    public TextMeshProUGUI ticketPrice;
    public GameObject window_1;
    public GameObject window_2;
    public GameObject window_thankyou;

    public void ChangeNumTickets(){
        if(ticketNumber.text == "1"){
            ticketNumber.text = "2";
            ticketPrice.text = "TOTAL:  99,998 cr.";
        }
        else{
            ticketNumber.text = "1";
            ticketPrice.text = "TOTAL:  49,999 cr.";
        }
    }

    public void PurchaseTickets(){
        if(ticketNumber.text == "1"){
            if(PlayerManager.PM.playerCredits < 49999){
                DialogueManager.DM.RunNode("cant-afford-single-ticket");
            }
            else{
                PlayerManager.PM.playerCredits -= 49999;
                if(Director.Dir.ticketsPurchased == 0){
                    DialogueManager.DM.RunNode("bought-first-ticket");
                }
                else if(Director.Dir.ticketsPurchased == 1){
                    DialogueManager.DM.RunNode("bought-second-ticket");
                }
                window_2.SetActive(false);
                window_thankyou.SetActive(true);
            }
        }
        else{
            if(PlayerManager.PM.playerCredits < 99998){
                DialogueManager.DM.RunNode("cant-afford-both-tickets");
            }
            else{
                PlayerManager.PM.playerCredits -= 99998;
                DialogueManager.DM.RunNode("bought-both-tickets");
                window_2.SetActive(false);
                window_thankyou.SetActive(true);
            }
        }
    }

    public void DontBuyThose(){
        DialogueManager.DM.RunNode("wrong-ticket");
    }
}
