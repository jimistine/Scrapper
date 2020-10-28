using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadoutManager : MonoBehaviour
{

    public UIManager UIM;
    public MerchantManager MerchantManager;
    public Object[] scrapImages;

    // Start is called before the first frame update
    void Start()
    {
        UIM = this.GetComponent<UIManager>();
        MerchantManager = UIM.MerchantManager;
        scrapImages = Resources.LoadAll("Sprites/ScrapPortraits", typeof(Sprite));
    }

    
    public ScrapObject UpdateReadout(ScrapObject newScrap){
        UIM.scrapToTake = newScrap;
        UIM.readoutName.text = newScrap.scrapName;
        UIM.readoutMat.text = newScrap.material;
        UIM.readoutDesc.text = newScrap.description;
        UIM.readoutSize.text = newScrap.size.ToString("Size: " + "#,#" + " m<sup>3</sup>");
        // check the sold scrap list for the incoming piece
        if(MerchantManager.SoldScrap.Exists(x => x.scrapName == newScrap.scrapName)){
            //Debug.Log("Already sold a " + newScrap);
            UIM.readoutValue.text = newScrap.value.ToString("Value: " + "#,#" + " cr.");
        }
        else{
            //Debug.Log("Never sold a " + newScrap);
            UIM.readoutValue.text = "Price: Unknown";
        }
//        Debug.Log("scprap image value: " + newScrap.image);
        UIM.readoutImage.sprite = (Sprite)scrapImages[newScrap.image];
        return null;
    }
    public ScrapObject ShowTickReadout(ScrapObject tickScrap){
        UIM.tickReadout.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.scrapName;
        UIM.tickReadout.transform.Find("Material").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.material;
        UIM.tickReadout.transform.Find("Image").gameObject.GetComponent<Image>().sprite = (Sprite)scrapImages[tickScrap.image];
        UIM.tickReadout.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.description;
        UIM.tickReadout.transform.Find("Size").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.size.ToString("Size: "+ "#,#" + " m<sup>3</sup>");
        if(MerchantManager.SoldScrap.Exists(x => x.scrapName == tickScrap.scrapName)){
            UIM.tickReadout.transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.value.ToString("Value: " + "#,#" + " cr.");
        }
        else{
            UIM.tickReadout.transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>().text = "Price: Unknown";
        }
        UIM.tickReadout.SetActive(true);
        return null;
    }
    public void FillScrapLogItem(GameObject scrapLogReadout, ScrapObject newScrap){

        scrapLogReadout.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = newScrap.scrapName;
        scrapLogReadout.transform.Find("Material").gameObject.GetComponent<TextMeshProUGUI>().text = newScrap.material;
        scrapLogReadout.transform.Find("Image").gameObject.GetComponent<Image>().sprite = (Sprite)scrapImages[newScrap.image];
        scrapLogReadout.transform.Find("Size").gameObject.GetComponent<TextMeshProUGUI>().text = newScrap.size.ToString("Size: "+ "#,#" + " m<sup>3</sup>");
        scrapLogReadout.transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>().text = newScrap.value.ToString("Value: " + "#,#" + " cr.");
        scrapLogReadout.transform.Find("Description").gameObject.GetComponentInChildren<TextMeshProUGUI>().text = newScrap.description;
    }
}
