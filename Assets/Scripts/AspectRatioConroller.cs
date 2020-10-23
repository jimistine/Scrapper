using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioConroller : MonoBehaviour
{
    public float targetAspect;
    float windowAspect;
    float scaleHeight;
    float scaleWidth;
    public Camera overworldCamera;

    // Start is called before the first frame update
    void Start()
    {
        // set the desired aspect ratio (the values in this example are
        // hard-coded for 16:9, but you could make them into public
        // variables instead so you can set them at design time)
        //targetAspect = 16.0f / 9.0f;

        // determine the game window's current aspect ratio
        windowAspect = (float)Screen.width / (float)Screen.height;

        // current viewport height should be scaled by this amount
        scaleHeight = windowAspect / targetAspect;

        // obtain camera component so we can modify its viewport
        overworldCamera = GetComponent<Camera>();

        // if scaled height is less than current height, add letterbox
        if (scaleHeight < 1.0f)
        {  
            Debug.Log("adding letterbox");
            Rect rect = overworldCamera.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            
            overworldCamera.rect = rect;
        }
        else // add pillarbox
        {
            Debug.Log("adding pillarbox");
            scaleWidth = 1.0f / scaleHeight;

            Rect rect = overworldCamera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            overworldCamera.rect = rect;
        }
    }
}
