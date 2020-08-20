using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
[System.Serializable]
public class UIManager : MonoBehaviour
{
    [Header("Statbar")]
    [Space(5)]
    public TextMeshProUGUI haulText;
    public PlayerManager PlayerManager;

    [Header("Readout")]
    [Space(5)]
    public GameObject readoutPanel;
    public TextMeshProUGUI readoutName;
    public TextMeshProUGUI readoutDesc;
    public TextMeshProUGUI readoutSize;
    public TextMeshProUGUI readoutValue;
    public Button readoutTakeButt;
    public Button readoutLeaveButt;
    
    ScrapObject scrapToTake;
    

    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    // 1. Player finds scrap
    public ScrapObject ShowScrap(ScrapObject newScrap){
        Debug.Log("Showing: " + newScrap.scrapName);
        newScrap.GetComponent<SpriteRenderer>().enabled = true;
        scrapToTake = newScrap;
        return newScrap;
    }

    // 2. Player has clicked on scrap
    public ScrapObject ShowReadout(ScrapObject newScrap){
        readoutName.text = newScrap.scrapName;
        readoutDesc.text = newScrap.description;
        //readoutSize.text = newScrap.size.ToString();
        readoutSize.text = string.Format("Size: {0:#,#}", newScrap.size);
        readoutValue.text = newScrap.value.ToString("Value: " + "#,#");
        readoutPanel.SetActive(true);
        return newScrap;
    }

    // 3. Player clicks "take" or "leave" and we do what they tell us
    public void TakeScrap(){
        PlayerManager.TakeScrap(scrapToTake);
        haulText.text = "Current Haul: " + PlayerManager.currentHaul.ToString() + " m<sup>3</sup>";
        readoutPanel.SetActive(false);
        Destroy(scrapToTake.gameObject);
    }
    public void LeaveScrap(){
        readoutPanel.SetActive(false);
    }
    
}
