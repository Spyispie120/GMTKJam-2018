using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;

public class GhostTrail : MonoBehaviour
{

    public GameObject ghostTrailPrefab;

    private Player player;
    private SpriteRenderer parentSR;


    // Start is called before the first frame update
    void Start()
    {
        player = GetComponent<Player>();
        parentSR = GetComponent<SpriteRenderer>();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CreateGhostTrail(int n)
    {
        StartCoroutine(IGhostTrail(n));
    }

    IEnumerator IGhostTrail(int num)
    {
        for(int i = 0; i < num; i++)
        {
            GameObject trail = Instantiate(ghostTrailPrefab, transform.position, Quaternion.identity);
            trail.GetComponent<SpriteRenderer>().sprite = parentSR.sprite;
            Destroy(trail, 0.5f);
            yield return new WaitForSeconds(0.1f);
        }
        

    }
}
