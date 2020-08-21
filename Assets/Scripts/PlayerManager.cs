using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerManager : MonoBehaviour
{
    [Header("Scripts")]
    [Space(5)]
    public fuel fuelManager;
    public clickToMove ClickToMove;
    public UIManager UIManager;

    [Header("Rig Stats")]
    [Space(5)]
    public float currentSpeed;
    public float fuelLevel;
    public Collider2D searchRadius;

    [Header("Player Inventory")]
    [Space(5)]
    public float currentHaul;
    public float maxHaul;
    public float playerCredits;
    public List<ScrapObject> playerScrap = new List<ScrapObject>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Keeping an eye on fuel and speed
        fuelLevel = fuelManager.currentFuelUnits;
        currentSpeed = ClickToMove.currentSpeed;

        // INTERACTIONS
        if (Input.GetMouseButtonDown(0)) {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            // 2. If player clicked something, and that something was scrap, show the readout panel
            if(hit.collider != null){
                if (hit.collider.tag == "Scrap") {
                    ScrapObject newScrap = hit.collider.GetComponent<ScrapObject>();
                    UIManager.ShowReadout(newScrap);
                }
            }    
        }
    }

    
    void OnTriggerEnter2D(Collider2D other){
        // 1. Pop goes the scrap!
        if(other.gameObject.tag == "Scrap"){
            ScrapObject newScrap = other.gameObject.GetComponent<ScrapObject>();
            UIManager.ShowScrap(newScrap);
        }
        if(other.gameObject.name == "Town"){
            UIManager.OfferTown();
        }
    }
    // void OnCollisionEnter2D(Collision2D other){
    //     if(other.gameObject.tag == "Scrap"){
    //         ScrapObject newScrap = other.gameObject.GetComponent<ScrapObject>();
    //         UIManager.ShowScrap(newScrap);
    //     }
    
    // 3. If they clicked Take, take it
    public ScrapObject TakeScrap(ScrapObject takenScrap){
        playerScrap.Add(takenScrap);
        foreach(ScrapObject scrap in playerScrap){
            Debug.Log("Player has: " + scrap.GetComponent<ScrapObject>().scrapName);
        }
        
        takenScrap.gameObject.SetActive(false);
        currentHaul += takenScrap.size;
        
        return null;
    }
}
