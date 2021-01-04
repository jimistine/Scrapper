using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameOverManager : MonoBehaviour
{
    public float transitionTime;
    public TextMeshProUGUI endText;
    public TextMeshProUGUI restartButt;
    public TextMeshProUGUI creditsButt;
    public Color restartButtEndColor;
    public Color creditsButtEndColor;
    public List<string> endTexts;


    void Start(){
        StartCoroutine(GameOverRoutine());
        endTexts[0] = "And the Maelstrom Embraced them\nWith Arms\nas Wide\nas Yun";
        endTexts[1] = "And the Maelstrom Embraced them\nWith Arms\nas Wide\nas Yun";
        endTexts[2] = "And the Galaxy Embraced them\nWith Arms\nas Wide\nas Yun";
        if(Director.Dir.ticketsPurchased == 0){
            endText.text = endTexts[0];
        }
        if(Director.Dir.ticketsPurchased == 1){
            endText.text = endTexts[1];
        }
        if(Director.Dir.ticketsPurchased == 2){
            endText.text = endTexts[2];
        }
    }
    IEnumerator GameOverRoutine(){
        yield return new WaitForSeconds(1);

        float elapsedTime = 0;
        Color textStart = endText.color;
        endText.gameObject.SetActive(true);
        while(elapsedTime < transitionTime){
            endText.color = Color.Lerp(textStart, Color.white, elapsedTime/transitionTime); 
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        yield return new WaitForSeconds(1);

        elapsedTime = 0;
        Color restartButtStartColor = restartButt.color;
        Color creditsButtStartColor = creditsButt.color;
        restartButt.gameObject.SetActive(true);
        creditsButt.gameObject.SetActive(true);
        while(elapsedTime < transitionTime){
            restartButt.color = Color.Lerp(restartButtStartColor,restartButtEndColor, elapsedTime/transitionTime);
            creditsButt.color = Color.Lerp(creditsButtStartColor,creditsButtEndColor, elapsedTime/transitionTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

    }

    public void BeginAgain(){
        SceneManager.LoadScene("TitleScreen");
        SceneManager.UnloadSceneAsync("GameOver");
        //SceneController.SC.RestartGame();
    }
}
