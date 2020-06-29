using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIScore : MonoBehaviour
{
    public TextMeshProUGUI text;

    void Start()
    {
            
    }

    // Update is called once per frame
    void Update()
    {
        text.text = "Slam Dunk " + BasketballHoop.score;
    }
}
