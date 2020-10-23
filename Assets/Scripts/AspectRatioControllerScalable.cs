using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AspectRatioControllerScalable : MonoBehaviour
{
    int lastWidth = 0;
    int lastHeight = 0;
     

void Update ()
{
    int width = Screen.width;
    int height = Screen.height;

    if(lastWidth != width) // if the user is changing the width
    {
        // update the height
        int heightAccordingToWidth = width / 4 * 3;
        Screen.SetResolution(width, (int)Mathf.Round(heightAccordingToWidth), false, 0);
    }
    else if(lastHeight != height) // if the user is changing the height
    {
        // update the width
        int widthAccordingToHeight = height / 3 * 4;
        Screen.SetResolution((int)Mathf.Round(widthAccordingToHeight), height, false, 0);
    }

    lastWidth = width;
    lastHeight = height;
}
}
