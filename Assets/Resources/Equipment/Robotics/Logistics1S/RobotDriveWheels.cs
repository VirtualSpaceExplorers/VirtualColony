/*
Applies torques to the WheelCollider objects on a 2-wheel (roomba style) robot.

  Robot +Z direction is forward drive.
  Robot +Y direction is up.
  Robot +X direction is to the right.

*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RobotDriveWheels : MonoBehaviour
{
    public GameObject LWheelObject;
    public GameObject RWheelObject;
    public WheelCollider[] wheels;

    // Start is called before the first frame update
    void Start()
    {
        wheels=new WheelCollider[2] {
            LWheelObject.GetComponent<WheelCollider>(),
            RWheelObject.GetComponent<WheelCollider>()
        };
        
        Stop();
    }
    
    // Apply these drive torques (N-m) to our wheels
    public void Drive(float Ltorque,float Rtorque) {
        wheels[0].motorTorque=Ltorque;
        wheels[1].motorTorque=Rtorque;
    }
    // Apply these braking torques (N-m) to our wheels
    public void Brake(float Ltorque,float Rtorque) {
        wheels[0].brakeTorque=Ltorque;
        wheels[1].brakeTorque=Rtorque;
    }
    public void Coast() {
        Drive(0.0f,0.0f); 
        Brake(0.0f,0.0f); 
    }
    public void Stop() { 
        Drive(0.0f,0.0f); 
        Brake(100.0f,100.0f); 
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}


