using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class PlayerManager : MonoBehaviour
{

    public fuel Fuel;
    public clickToMove ClickToMove;
    public UIManager UIManager;

    public float currentSpeed;
    public float fuelLevel;

    // Invetory
    public float currentHaul;
    public float maxHaul;
    public float playerCredits;
    public List<GameObject> playerScrap = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        // Keeping an eye on fuel and speed
        fuelLevel = Fuel.currentFuelLevel;
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
                    Debug.Log("Player clicked: " + hit.collider.gameObject.name);
                    UIManager.ShowReadout(newScrap);
                }
            }    
        }
    }

    // 1. Pop goes the scrap!
    void OnTriggerEnter2D(Collider2D other){
        if(other.tag == "Scrap"){
            ScrapObject newScrap = other.GetComponent<ScrapObject>();
            
            UIManager.ShowScrap(newScrap);
        }
    }
    // 3. If they clicked Take, take it
    public ScrapObject TakeScrap(ScrapObject takenScrap){
        playerScrap.Add(takenScrap.gameObject);
        takenScrap.gameObject.SetActive(false);
        currentHaul += takenScrap.size;
        
        return null;
    }
}
