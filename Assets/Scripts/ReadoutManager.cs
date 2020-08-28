using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
}
