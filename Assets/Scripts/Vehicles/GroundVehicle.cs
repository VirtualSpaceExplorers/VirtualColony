/*
 Hook this script to a ground vehicle to make it walk-up driveable:
   - A Trigger collider is used to detect player walk-ups.

*/
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Controls;
using UnityEngine;

public class GroundVehicle : MonoBehaviour, IVehicleMotionScheme
{
    public float driveSpeed=30.0f; // m/s normal driving speed
    public float sprintSpeed=100.0f; // m/s "spring" driving speed
    public Vector3 localPosition=new Vector3(0,0,-0.4f); // driver sits on right side
    public Vector3 localRotation=new Vector3(0,0,0);
    
    // Start is called before the first frame update
    void Start()
    {
        
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
        _vehicleSlot=player.transform.Find("Vehicle");
        if (_vehicleSlot!=null) {
            Transform ourTransform=gameObject.transform;
            Vector3 rotation=ourTransform.rotation.eulerAngles;
            // Move the player to our position
            player.transform.position=ourTransform.position;
            player.transform.rotation=ourTransform.rotation;
            
            // Re-parent ourselves under this GameObject:
            // vehicleSlot.DetachChildren(); // remove old vehicle, or can it be carried with us?
            _savedParent = ourTransform.parent;
            ourTransform.parent = _vehicleSlot;
            
            ourTransform.localPosition=localPosition;
            ourTransform.localRotation=Quaternion.Euler(localRotation);
            _justEntered=true;

            // Register our motion with the player's vehicle manager
            _mgr = player.GetComponent<IVehicleManager>();
            if (_mgr!=null) {
                _mgr.PushVehicle(this);
                Debug.Log("Found vehicle manager");
            } else {
                Debug.Log("WARNING: NO vehicle manager found");
            }
            
            // Remove our own rigidBody (let player's rigidbody do our physics)
            //  FIXME: this is really pretty bogus, it'd be better to let the player bounce inside us.
            rb=_player.GetComponent<Rigidbody>();
            Rigidbody ourRB=GetComponent<Rigidbody>();
            _savedMass=ourRB.mass; 
            rb.mass += ourRB.mass;
            // _savedRigidbody=JsonUtility.ToJson(ourRB); // <- not on WebGL
            Object.Destroy(ourRB);
            
            _isDriving=true;
        }
        else {
            Debug.Log("WARNING: GroundVehicle can't find player's vehicle slot to drive!");
        }
    }
    
    // State variables used while driving (only in Start/Stop driving)
    private bool _isDriving=false;
    private GameObject _player;
    private Transform _vehicleSlot; // player's subobject for storing vehicles
    private IVehicleManager _mgr;
    private Transform _savedParent; // our old transform slot
    private float _savedMass; // our old Rigidbody mass 
    // Unity: it's STUPID you can't move a component between objects, or even JSON serialize them on WebGL!
    
    // Stop driving this vehicle (jump out)
    void StopDriving() {
        if (!_isDriving) return; // we've already stopped
        
        _restartCooldown=10;
        
        // Restore this vehicle to a freestanding object again
        gameObject.transform.parent = _savedParent;
        Rigidbody ourRB=gameObject.AddComponent<Rigidbody>();
        //JsonUtility.FromJsonOverwrite(_savedRigidbody,ourRB);
        ourRB.mass=_savedMass;
        rb.mass -= ourRB.mass; 
        
        // Conserve velocities
        ourRB.velocity = rb.velocity;
        ourRB.angularVelocity = rb.angularVelocity;
        
        // Teleport player out our driver's side
        //  FIXME: this seems error-prone
        _player.transform.position=gameObject.transform.position + 0.7f*_player.transform.forward + 0.5f*_player.transform.up;
        
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

    // Physics forces
    public void VehicleFixedUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform)
    {
        if (ui.jump) {
            StopDriving();
            return;
        }
        
        // Apply traction forces to maintain forward velocity 
        //   FIXME: this should be per-wheel, to enable skids
        float targetSpeed=driveSpeed;
        if (ui.sprint) targetSpeed=sprintSpeed;
        Vector3 targetVelocity=new Vector3(ui.move.x,0.0f,0.0f)*targetSpeed;
        // Rotate the (player local) target velocity out to world space
        var targetWorldVelocity = playerTransform.localRotation*targetVelocity;
        
        // Figure out our current velocity and apply velocity tracking force
        var spring=10000.0f; // Newtons per m/s target speed difference
        var F=spring*(targetWorldVelocity-rb.velocity);
        float maxForce=3*5000.0f; // Newtons, FIXME: depends on downforce per tire
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
        ui.pitch=Mathf.Clamp(ui.pitch,-20,+90);
        ui.yaw=Mathf.Clamp(ui.yaw,-130,+130);
        
        // The camera is the only thing that moves for mouse look
        cameraTransform.localRotation = Quaternion.Euler(ui.pitch, 90f + ui.yaw, 0f);
        
        
        // Steering is via A and D keys (when walking this is strafe)
        float steerRange=45.0f; // degrees of front wheel steering angle
        float steerTarget=-steerRange*ui.move.z; // un-smooothed target angle
        float steerTime=0.7f; // seconds to reach steering target
        float steerRate=Time.deltaTime/steerTime; // smoothing rate constant
        _steerHeading=_steerHeading*(1.0f-steerRate)+steerTarget*steerRate;
        
        float bodyPerSecond=Vector3.Dot(rb.velocity,playerTransform.right)/_steeringWheelbase; // speed in body lengths/second
        _vehicleHeading+=_steerHeading*bodyPerSecond*Time.deltaTime; // change in heading
        
        // Rotate the vehicle and player 
        Vector3 rot=playerTransform.rotation.eulerAngles;
        rot.y=_vehicleHeading;
        playerTransform.rotation = Quaternion.Euler(rot);
    }
    
}
