/*
 Hook this script to a ground vehicle to make it walk-up driveable:
   - A Trigger collider is used to detect player walk-ups.

 WS are the throttle
 AD turn the steering wheel back and forth
 Space jumps out of the vehicle.

 
 Orion Lawlor, lawlor@alaska.edu, 2020-07 (Public Domain)

*/
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Controls;
using UnityEngine;

public class GroundVehicle : MonoBehaviour, IVehicleMotionScheme
{
    public float driveSpeed=30.0f; // m/s normal driving speed
    public float sprintSpeed=100.0f; // m/s "spring" driving speed
    public Vector3 localPosition=new Vector3(0,0,0.4f); // driver sits on right side
    public Vector3 localRotation=new Vector3(0,0,0);
    
    // Start is called before the first frame update
    void Start()
    {
        rb=gameObject.GetComponent<Rigidbody>();
    }
    
    private int _restartCooldown=0; // suppress vehicle entry if we just exited it
    
    // OnTriggerEnter is called when our "walk up to drive" trigger gets hit
    void OnTriggerEnter(Collider other) 
    {
        if (_restartCooldown<=0 && other.gameObject.tag=="Mobile")
        { // Player has entered our "begin driving" trigger area
            Debug.Log("Start driving ground vehicle!");
            StartDriving(other.gameObject);
        }
    }
    
    // This object would like to drive us around
    void StartDriving(GameObject player) {
        if (_isDriving) return; // we're already driving
        
        _player=player;
        
        // Turn off player physics while they're in our driver's seat
        playerRB=player.GetComponent<Rigidbody>(); //<- I SO WANT a .enabled on this (doesn't exist)
        playerRB.useGravity=false;
        
        // Don't do player collisions with anything while inside us
        CapsuleCollider cap=_player.GetComponent<CapsuleCollider>();
        cap.enabled=true; 
        // cap.height=cap.radius=0.05f; // shrink player to a tiny dot
        
        _justEntered=true;

        // Register our motion with the player's vehicle manager
        _mgr = player.GetComponent<IVehicleManager>();
        if (_mgr!=null) {
            _mgr.PushVehicle(this);
            Debug.Log("Found vehicle manager");
        } else {
            Debug.Log("WARNING: NO vehicle manager found");
        }
        
        _isDriving=true;
    }
    
    // State variables used while driving (only in Start/Stop driving)
    private bool _isDriving=false;
    private GameObject _player;
    private Rigidbody playerRB;
    private IVehicleManager _mgr;
    
    // Stop driving this vehicle (jump out)
    void StopDriving() {
        if (!_isDriving) return; // we've already stopped
        
        _restartCooldown=10;
        
        // Player is a separate object again
        playerRB.useGravity=true;
        CapsuleCollider cap=_player.GetComponent<CapsuleCollider>();
        cap.enabled=true; 
        
        // Conserve velocities
        playerRB.velocity = rb.velocity;
        playerRB.angularVelocity = rb.angularVelocity;
        
        // Teleport player out our driver's side
        //  FIXME: this seems error-prone, resulting in telefrag
        //_player.transform.position=gameObject.transform.position + 0.7f*_player.transform.forward + 0.5f*_player.transform.up;
        
        // Unregister from the player's controls
        _mgr.PopVehicle(); 
        
        _isDriving=false;
    }
    
    void FixedUpdate() {
        if (_restartCooldown>0) _restartCooldown--;
    }

    // IVehicleMotionScheme interface
    private Rigidbody rb;
    private bool _justEntered=false;
    public float _vehicleHeading; // degrees yaw that vehicle is driving towards
    public float _steerHeading; // degrees of front wheel turn
    public float _steeringWheelbase=3.8f; // front-back wheelbase, meters
    public bool ballistic=true; // are we flying through the air?
        
    // Physics forces
    public void VehicleFixedUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform)
    {
        if (ui.jump) {
            StopDriving();
            return;
        }
        
        Transform tf=gameObject.transform;
        if (tf.up.y<0.1f) { // rollover, eject! (and cover up our hacky physics)
            StopDriving();
            return;
        }
        
        playerRB.velocity=new Vector3(0,0,0); // we move the player--the RB will cause horrible judder
        
        // Check if we're flying through the air
        ballistic=true;
        float groundCheckDistance=0.1f;
        // Look under each axle
        for (float axle=+1.4f;axle>-4.0f;axle-=3.88f)
        {
            Vector3 spot = tf.right*axle-(1.5f*groundCheckDistance)*tf.up;
            if (Physics.CheckSphere(tf.position+spot,groundCheckDistance))
                ballistic=false; 
        }
        if (ballistic) return; // can't apply traction forces unless we're on the ground
        
        
        // Apply traction forces to maintain forward velocity 
        //   FIXME: this should be per-wheel, to enable skids
        float targetSpeed=driveSpeed;
        if (ui.sprint) targetSpeed=sprintSpeed;
        Vector3 targetVelocity=new Vector3(ui.move.x,0.0f,0.0f)*targetSpeed;
        // Rotate the (player local) target velocity out to world space
        var targetWorldVelocity = tf.localRotation*targetVelocity;
        
        // Figure out our current velocity and apply velocity tracking force
        var spring=20000.0f; // Newtons per m/s target speed difference
        var F=spring*(targetWorldVelocity-rb.velocity);
        float maxForce=3*10000.0f; // Newtons, FIXME: depends on downforce per tire
        if (F.magnitude>maxForce) {
            F=F.normalized*maxForce;
        }
        F.y=0; // can't apply vertical force (acts like drag)
        
        rb.AddForce(F); 
        
    }

    // Graphics, like mouse move
    public void VehicleUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform)
    {
        if (_justEntered) {
            ui.pitch=ui.yaw=0.0f; // look straight ahead after entering vehicle
            _justEntered=false;
        }
        ui.pitch=Mathf.Clamp(ui.pitch,-100,+30);
        ui.yaw=Mathf.Clamp(ui.yaw,-130,+130);
        
        // The camera is the only thing that moves for mouse look
        playerTransform.localRotation = Quaternion.Euler(0,0,0);
        cameraTransform.localRotation = Quaternion.Euler(ui.pitch, 90f + _vehicleHeading + ui.yaw, 0f);
        
        // Steering is via A and D keys (when walking this is strafe)
        float steerRange=45.0f; // degrees of front wheel steering angle
        float steerTarget=-steerRange*ui.move.z; // un-smooothed target angle
        float steerTime=0.7f; // seconds to reach steering target
        float steerRate=Time.deltaTime/steerTime; // smoothing rate constant
        if (!ballistic)
            _steerHeading=_steerHeading*(1.0f-steerRate)+steerTarget*steerRate;
        
        Transform tf=gameObject.transform;
        
        float bodyPerSecond=Vector3.Dot(rb.velocity,tf.right)/_steeringWheelbase; // speed in body lengths/second
        _vehicleHeading+=_steerHeading*bodyPerSecond*Time.deltaTime; // change in heading
        
        // Rotate the vehicle according to steering 
        // FIXME: this causes spinning fail if vehicle tips on its side, needs quaternion magic
        if (tf.up.y>0.1f) {
            Vector3 rot=tf.rotation.eulerAngles;
            rot.y=_vehicleHeading;
            tf.rotation = Quaternion.Euler(rot);
        }
        
        // Lock the position of the player to our seat 
        //   (avoid graphics lag/judder by doing this here, not in Fixed)
        _player.transform.position=tf.position + localPosition.z*tf.forward;
    }
    
}
