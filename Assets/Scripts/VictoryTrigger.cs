using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VictoryTrigger : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collider)
    {
        if(collider.tag == "Player")
        {
            
            this.GetComponent<VictorySequenceController>().enabled = true;
        }
        else
        {
            Debug.Log("ERROR: non player collision in the earth's core");
        }
    }
}
