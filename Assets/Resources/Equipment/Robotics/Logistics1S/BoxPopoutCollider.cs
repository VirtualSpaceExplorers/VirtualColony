/**
 Used on sheet metal container walls:
    When a robot tries to scoop up a container,
      the container's top edge collider pops out and enlarges.
    This hack reduces problems with roundoff and thin colliders,
      letting us use physics to pick up boxes by their thin walls.

 Add this script to a wall object, with at least 4 box colliders:
  [0] is the trigger
  [1] is the "pop in" collider location, enabled normally when robots are not nearby
  [2] is the "popped out" collider location, enabled only when a robot is picking us up.
  futher colliders like the main wall are not modified

 Orion Lawlor, lawlor@alaska.edu, 2020-07-30 (Public Domain)
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxPopoutCollider : MonoBehaviour
{
    public int count=0; // count of robot parts inside our collider
    private BoxCollider[] boxes;

    // Start is called before the first frame update
    void Start()
    {
        boxes=gameObject.GetComponents<BoxCollider>();
        if (count==0) Popin();
    }
    
    // Retract the collider
    void Popin() 
    {
        boxes[1].enabled=true;
        boxes[2].enabled=false;
    }
    // Extend the collider
    void Popout()
    {
        boxes[1].enabled=false;
        boxes[2].enabled=true;
    }
    
    // Extend when a robot is trying to pick us up
    public void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag=="RobotPickup")
        {
            count++;
            Popout();
        }
    }
    
    // Retract otherwise
    public void OnTriggerExit(Collider other) {
        if (other.gameObject.tag=="RobotPickup")
        {
            count--;
            if (count<=0) { // We're now empty
                count=0;  // Don't go negative (e.g., spawned inside)
                
                Popin();
            }
        }
    }
}
