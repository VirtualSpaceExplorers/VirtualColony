using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirlockBehavior : MonoBehaviour
{
    
    public GameObject controlledSpace;
    public int mobilesInside=0;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Called when anything enters the collider
    void OnTriggerEnter(Collider c) 
    {
        if (c.gameObject.tag=="Mobile") {
            mobilesInside++;
            Debug.Log("Entering airlock...");
            
            // HACK: take down the whole force field when anything enters.
            //   This means a random bot entering a random airlock drops 
            //   the shields for an entire building.
            controlledSpace.GetComponent<Collider>().enabled=false;
        }
    }
    
    // Called when anything leaves the collider
    void OnTriggerExit(Collider c) 
    {
        if (c.gameObject.tag=="Mobile") {
            mobilesInside--;
            Debug.Log("Leaving airlock...");
            if (mobilesInside==0) {
                // This seems to boot you back out of the building again...
                //controlledSpace.GetComponent<Collider>().enabled=true;
            }
        }
    }
}
