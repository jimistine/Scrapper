using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGenerator : MonoBehaviour
{

/*
Notes on Randomness
-------------------
From: https://www.codeproject.com/Articles/420046/Loot-Tables-Random-Maps-and-Monsters-Part-I
Item 1 - Probability 10
Item 2 - Probability 5
Item 3 - Probability 1.5
The sum of all is 16.5 - If you calculate 16 drops from this table, you will likely have 10 times Item 1, 
  5 times Item 2 and maybe the 16th will be one single Item 3.
The result will just take a random value and loop through the contents of a table until it hits the first 
  value that is bigger than the random value. This is the item hit.

If weighting values are used, the Random.Range function would be from 1 to the sum of all the
  weighted values.(?)
*/
    // Start is called before the first frame update

    public int[] items = {0,1,5,10,20};
    public int total;
    public int max;
    public int randomPick;
    public int picked;

    void Start()
    {
        int seed = (int)System.DateTime.Now.Ticks;
        Random.InitState(seed);
        //Debug.Log("Seed is: " + seed);

        // run the pick x times to test probability
        for(int i = 0; i < 50; i++){
            PickItem(); 
            Debug.Log("We picked: " + picked);       
        }
    }

    public void PickItem(){
        // add up the total of all weights
        foreach(int itemWeight in items){
            total =+ itemWeight;
        }

        // pick a random number between 0 and the highest weight
        randomPick = Random.Range(0, Mathf.Max(items));

        // iterate through the array and pick the first item that is bigger than the randomPick and gtfo
        foreach(int itemWeight in items){
            if(itemWeight > randomPick){
                picked = itemWeight;
                break;
            }
        }

        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
