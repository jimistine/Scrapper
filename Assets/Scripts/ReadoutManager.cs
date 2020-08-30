using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ReadoutManager : MonoBehaviour
{

    public UIManager UIM;
    public Object[] scrapImages;

    // Start is called before the first frame update
    void Start()
    {
        UIM = this.GetComponent<UIManager>();
        scrapImages = Resources.LoadAll("Sprites/ScrapPortraits", typeof(Sprite));
    }

    
    public ScrapObject UpdateReadout(ScrapObject newScrap){
        UIM.scrapToTake = newScrap;
        UIM.readoutName.text = newScrap.scrapName;
        UIM.readoutMat.text = newScrap.material;
        UIM.readoutDesc.text = newScrap.description;
        UIM.readoutSize.text = string.Format("Size: {0:#,#}", newScrap.size + " m<sup>3</sup>");
        UIM.readoutValue.text = newScrap.value.ToString("Value: " + "#,#" + " cr.");
        Debug.Log("scprap image value: " + newScrap.image);
        UIM.readoutImage.sprite = (Sprite)scrapImages[newScrap.image];
        return null;
    }
    public ScrapObject ShowTickReadout(ScrapObject tickScrap){
        UIM.tickReadout.transform.Find("Name").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.scrapName;
        UIM.tickReadout.transform.Find("Material").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.material;
        UIM.tickReadout.transform.Find("Image").gameObject.GetComponent<Image>().sprite = (Sprite)scrapImages[tickScrap.image];
        UIM.tickReadout.transform.Find("Description").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.description;
        UIM.tickReadout.transform.Find("Size").gameObject.GetComponent<TextMeshProUGUI>().text = string.Format("Size: {0:#,#}", tickScrap.size + " m<sup>3</sup>");
        UIM.tickReadout.transform.Find("Value").gameObject.GetComponent<TextMeshProUGUI>().text = tickScrap.value.ToString("Value: " + "#,#" + " cr.");
        UIM.tickReadout.SetActive(true);
        return null;
    }
}
