using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClickDrag : MonoBehaviour
{
    Rigidbody2D PlayerRB;
    public float speed;

    // Start is called before the first frame update
    void Start()
    {
        PlayerRB = gameObject.GetComponent<Rigidbody2D>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0)){           
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);
            Vector2 direction = gameObject.transform.position - mousePos;

            PlayerRB.AddForce((direction.normalized * -1) * speed);
        }

    }
}
