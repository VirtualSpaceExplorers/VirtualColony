/**
 Manage both the inner and outer airlock doors.

 Original by Orion Lawlor, 2020-07, for Nexus Aurora (Public Domain)
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modular_6m_Airlock : MonoBehaviour
{
    public GameObject doorPrefab; /// The airlock door graphical prefab
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
                    last=0; // Give up priority
                }
            }
        }
    }
    
    
    private PlayerWatcher innerWatcher, outerWatcher;
    
    
    public GameObject[] doors=new GameObject[2]; // 0 is inner; 1 is outer (instantiated prefabs)
    public Modular_6m_Airlock_Door[] scripts=new Modular_6m_Airlock_Door[2];
    
    public Vector3[] startPositions={new Vector3(1.0f,0.0f,0.0f), new Vector3(3.0f,0.0f,0.0f)};
    
    
    // Start is called before the first frame update
    //   This makes our prefabs and hooks up our companion script for each door.
    void Start()
    {
        // Make watchers for the triggers
        innerWatcher=innerTrigger.AddComponent<PlayerWatcher>();
        outerWatcher=outerTrigger.AddComponent<PlayerWatcher>();
        
        
        // Attach prefab doors to the inside and outside positions
        for (int outer=0;outer<=1;outer++) {
            GameObject door=Instantiate(doorPrefab,gameObject.transform,false);
            door.SetActive(true);
            Vector3 position = startPositions[outer];
            Modular_6m_Airlock_Door doorScript=door.AddComponent<Modular_6m_Airlock_Door>();
            doorScript.openPos += position;
            doorScript.closePos += position;
            
            scripts[outer]=doorScript;
            doors[outer]=door;
        }
    }

    // Check to see if somebody wants to use the airlock,
    //   by watching the triggers.
    //   FIXME: Totally nonphysical, will happily open both doors at the same time.
    void Update()
    {
        int inWait=innerWatcher.count;
        int outWait=outerWatcher.count;
        
        if (inWait>0 && outWait>0) 
        { // Need to resolve priority:
        //   FIXME: REPLACE THIS HACK WITH SOMETHING REAL
            if (innerWatcher.last>outerWatcher.last) outWait=0; else inWait=0;
        }
        
        if (inWait>0) scripts[0].Open(); else scripts[0].Close();
        if (outWait>0) scripts[1].Open(); else scripts[1].Close();
        
    }
}
