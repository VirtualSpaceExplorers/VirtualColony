/*
Applies torques to the WheelCollider objects on a 2-wheel (roomba style) robot.



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
        
        // Unity "forward" is Z.  Vehicle drive direction is X.  So rotate wheels to match.
        wheels[0].steerAngle=+80.0f;
        wheels[1].steerAngle=-80.0f;
        
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


