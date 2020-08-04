/**
 Create and manage both the inner and outer airlock doors.

 Original by Orion Lawlor, 2020-07, for Nexus Aurora (Public Domain)
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirlockLogic : MonoBehaviour
{
    public GameObject innerDoor; /// The inner airlock door (with AirlockDoor script)
    public GameObject outerDoor; 
    
    public GameObject innerTrigger; /// Raw hitboxes for walking in and out
    public GameObject outerTrigger;
    
    // Watches a collision trigger for start / exit events
    private class PlayerWatcher : MonoBehaviour
    {
        public int count=0; // count of people waiting
        public float last=0; // time we last saw someone arrive
        public void OnTriggerEnter(Collider other) {
            if (other.gameObject.tag=="Mobile")
            {
                count++;
                last=Time.timeSinceLevelLoad;
            }
        }
        public void OnTriggerExit(Collider other) {
            if (other.gameObject.tag=="Mobile")
            {
                count--;
                if (count<=0) { // We're now empty
                    count=0;  // Don't go negative (e.g., spawned inside)
                    last=0; // Give up access time priority
                }
            }
        }
    }
    
    
    private PlayerWatcher innerWatcher, outerWatcher;
    private AirlockDoor innerScript, outerScript;
    
    
    // Start is called before the first frame update
    //   This makes our prefabs and hooks up our companion script for each door.
    void Start()
    {
        // Make watchers for the triggers
        innerWatcher=innerTrigger.AddComponent<PlayerWatcher>();
        outerWatcher=outerTrigger.AddComponent<PlayerWatcher>();
        
        innerScript=innerDoor.GetComponent<AirlockDoor>();
        outerScript=outerDoor.GetComponent<AirlockDoor>();
    }

    // Check to see if somebody wants to use the airlock,
    //   by watching the triggers.
    // FIXME: could save some CPU by making this trigger-driven above.
    void Update()
    {
        int inWait=innerWatcher.count;
        int outWait=outerWatcher.count;
        
        if (inWait>0 && outWait>0) 
        { // Need to resolve priority:
        //   FIXME: REPLACE THIS HACK WITH SOMETHING REAL
            if (innerWatcher.last>outerWatcher.last) outWait=0; else inWait=0;
        }
        
        if (inWait>0) innerScript.Open(); else innerScript.Close();
        if (outWait>0) outerScript.Open(); else outerScript.Close();
    }
}
