using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasketballHoop : MonoBehaviour
{

    public static int score;

    private Collider2D hoopBlock;
    private Collider2D hoop;

    
    // Start is called before the first frame update
    void Start()
    {
        hoopBlock = transform.GetChild(1).GetComponent<Collider2D>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Debug.Log("slam dunk");
            hoopBlock.enabled = false;
            score++;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            hoopBlock.enabled = true;
        }
    }
}
