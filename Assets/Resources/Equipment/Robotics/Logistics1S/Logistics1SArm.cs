/**
 Drives the joint angles of a 1S Logistics robot arm.
 
 It might be worthwhile to look at replacing the static transforms here 
 with Unity's Rigidbody "Joints".  This would let us model deflections and forces
 within the robot, at the expense of more weird behavior when they all interact.
 
 Orion Lawlor, lawlor@alaska.edu, 2020-07 (Public Domain)
*/
﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Logistics1SArm : MonoBehaviour
{
    public GameObject elbow; // game object for elbow + forearm
    public float elbowTarget=269; // degree angle intended for elbow joint
    public float elbowAngle=180; // degree angle of current elbow joint
    public float elbowMoveRate=30; // degrees/second maximum motion rate
    private Quaternion elbowStart; // initial localRotation for elbow (==180)
    public float elbowStartAngle=180; // degrees for initial elbow rotation
    public Vector3 elbowAxis=new Vector3(0.0f,0.430f,0.405f);

    public float forearmLength=0.430f; // meters between elbow and wrist joints
    
    public GameObject wrist; // game object for wrist and wristblade pickup
    public float wristTarget=0; // degree angle intended for wrist joint
    public float wristAngle=0; // degree angle of current wrist joint
    public float wristMoveRate=30; // degrees/second maximum motion rate
    private Quaternion wristStart; // initial localRotation for wrist (==0)
    public float wristStartAngle=0; // degrees for initial wrist rotation

    // Start is called before the first frame update
    void Start()
    {
        elbowStart=elbow.transform.localRotation;
        wristStart=wrist.transform.localRotation;
    }
    
    private float elbowSmoothDel=0.0f; // elbow angular rate
    private float wristSmoothDel=0.0f; // wrist angular rate
    
    // FixedUpdate is called once per physics frame
    void FixedUpdate() 
    {
    // Rotate elbow towards elbowTarget;
        // Figure out this step's angular change
        float del=elbowTarget - elbowAngle;
        float limit=elbowMoveRate*Time.deltaTime;
        del=Mathf.Clamp(del,-limit,+limit);  // limit to maximum allowable motion rate  
        
        // Filter to avoid jerking motions (these throw off the physics)   
        //  (This also introduces a bit of overshoot, which actually looks pretty realistic) 
        float smooth=Mathf.Clamp(1.0f-5.0f*Time.deltaTime,0.01f,1.0f);
        elbowSmoothDel=smooth*elbowSmoothDel+(1.0f-smooth)*del; 

        // Apply angle to elbow transform (for geometry & colliders)
        elbowAngle+=elbowSmoothDel; 
        elbow.transform.localRotation=Quaternion.Euler(elbowStartAngle-elbowAngle,0,0)*elbowStart;
        
    // Translate and rotate wrist
        // Angle change is exactly like elbow (should probably be a "joint" class)
        del=wristTarget-wristAngle;
        limit=wristMoveRate*Time.deltaTime;
        del=Mathf.Clamp(del,-limit,+limit);  // limit to maximum allowable motion rate  
        wristSmoothDel=smooth*wristSmoothDel+(1.0f-smooth)*del; 
        wristAngle+=wristSmoothDel;
        
        // Apply position and angle to wrist transform
        float elbowRads=(elbowAngle-90.0f)*Mathf.PI/180.0f;
        Vector3 forearmDir=new Vector3(0.0f,Mathf.Sin(elbowRads),Mathf.Cos(elbowRads));
        wrist.transform.localPosition=elbowAxis+forearmLength*forearmDir;
        wrist.transform.localRotation=Quaternion.Euler(wristAngle-wristStartAngle,0,0)*wristStart;
    }

    // Update is called once per graphics frame
    void Update()
    {
        
    }
}
