/*
 Implement a smooth human walking motion.
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Source.Controls;

public class WalkingMotion : MonoBehaviour, IVehicleMotionScheme {
    public float _moveSpeed=2.0f; // m/s walking speed (per unit player height)
    public float _sprintSpeed=12.0f; // sprint speed (Usain Bolt peak) 
    
    public float _playerCameraOffset=1.8f; // meters between the player frame and camera height
    private float _playerHeight=1.8f; // meters above terrain to lock camera
    
    public float maxWalkForce=1000.0f; // Newtons maximum horizontal force you can apply (friction, muscles, etc)
    public float maxJumpForce=2000.0f; // jump force, in Newtons
    
    
    // Physics forces
    public void VehicleFixedUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform) 
    {
        // Pressing Q or E scales the camera down or up, limited to human height (1.8 meters)
        if (ui.move.y!=0.0f) 
        { 
            float heightScale = 1.0f+1.0f*Time.deltaTime*ui.move.y;
            _playerHeight = Mathf.Max(1.8f,_playerHeight*heightScale);
            ui.move.y=0.0f;
        }
        
        // Hold shift to sprint
        float moveScale = _moveSpeed*_playerHeight;//<- move faster as we grow higher
        if (ui.sprint)  // sprint mode with shift key
        {
            moveScale = _sprintSpeed*_playerHeight/1.8f;
        }
        Vector3 targetVelocity = ui.move*moveScale;
        
        
        /* This is a goofy way to accomplish walking:
           Basically a spring simulator to lock in the player's velocity,
           while respecting the other world physics like colliders.
        */
        var rb=playerTransform.gameObject.GetComponent<Rigidbody>();
        bool onGround=true; // <- FIXME: figure out when we're ballistic
        if (onGround) { // we're standing, so we can apply walking force
            var spring=300.0f; // Newtons per m/s target difference
            if (ui.move.magnitude<0.01) spring*=3.0f; // stop faster
            
            var currentVelocity = rb.velocity;
            
            // Rotate the (player local) target velocity out to world space
            var targetWorldVelocity = playerTransform.localRotation*targetVelocity;
            
            var F=spring*(targetWorldVelocity-currentVelocity);
            if (F.magnitude>maxWalkForce) {
                F=F.normalized*maxWalkForce;
            }
            
            F.y=0; // can't apply vertical force (acts like drag)
            if (ui.jump) 
                F.y+=maxJumpForce;
            
            rb.AddForce(F); 
        }
        
        // Shift the collider to push us up off the ground with Q and E
        //   FIXME: stop bothering with physics & walking once you get too big.
        var capsule=playerTransform.gameObject.GetComponent<CapsuleCollider>();
        capsule.center=new Vector3(0.0f,_playerHeight/2,0.0f);
        capsule.height=_playerHeight;
        float extraHeight=_playerHeight-_playerCameraOffset;
        capsule.radius=0.2f+0.2f*extraHeight;
        
        // Push camera to match player size
        var camPos=cameraTransform.localPosition;
        camPos.y=_playerHeight-0.1f;
        cameraTransform.localPosition=camPos;
        
        
        /* // Old pre-rigidbody movement (reference for teleport)
        playerTransform.Translate(targetVelocity * Time.deltaTime);
        */
        
        /* // Terrain-relative position setting
        var pos=playerTransform.position;
        var ht=Terrain.activeTerrain.SampleHeight(pos);
        pos.y = ht + playerHeight;
        tf.position=pos;
        */
    }
    
    // Graphics, like mouse move
    //  (Doing rotation here prevents juddering)
    public void VehicleUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform) 
    {
        // While walking, your head needs to stay level (ish)
        if (Mathf.Abs(ui.pitch)>90.0f) ui.pitch*=1.0f-0.5f*Time.deltaTime; // <- gently return head to neutral
        
        // Pitch applies only to the camera
        cameraTransform.localRotation = Quaternion.Euler(ui.pitch, 90f, 0f);
        
        // We yaw the entire player
        playerTransform.localRotation = Quaternion.Euler(0.0f,ui.yaw,0f);
    }
}
