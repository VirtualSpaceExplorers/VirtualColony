/**
 Animate an airlock door moving up and down.

 Original by Orion Lawlor, 2020-07, for Nexus Aurora (Public Domain)
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modular_6m_Airlock_Door : MonoBehaviour
{
    public GameObject door; /// The instantiated door object
    
    public Vector3 closePos=new Vector3(0.0f,0f,0f);
    public Vector3 openPos=new Vector3(0.75f,2.2f,0f);
    private Quaternion closeRot=Quaternion.Euler(0f,0f,0f);
    private Quaternion openRot=Quaternion.Euler(0f,0f,90f);
    
    public float openState=0.0f; // current open state, from 0 to 1
    public float openDir=0.0f; // + for opening, - for closing.
    public float openSpeed=0.8f; // constant open/close speed (in cycles/second)
    public void Open() {
        openDir=+openSpeed;
    }
    public void Close() {
        openDir=-openSpeed;
    }
    
    // Move the graphical door to this location
    private void setOpenClose(float openness)
    {
        door.transform.localPosition=Vector3.Lerp(closePos,openPos,openness);
        door.transform.localRotation=Quaternion.Lerp(closeRot,openRot,openness);
    }
    
    // Start is called before the first frame update
    void Start()
    {
        door=gameObject; // This script is attached to the door itself.
        
        // Our setup is all in the airlock
        setOpenClose(openState);
    }

    // Update is called once per frame
    void Update()
    {
        openState+=openDir*Time.deltaTime;
        if (openState>1.0f) openState=1.0f;
        if (openState<0.0f) openState=0.0f;
        setOpenClose(openState);
    }
}
