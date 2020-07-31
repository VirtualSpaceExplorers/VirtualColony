/*
 Hook this script to a robot to make it walk-up driveable:
   - A Trigger collider is used to detect player walk-ups.
 
 
 WASD drive the robots' 2 wheels.
 E raises the elbow
 W lowers the elbow
 

 Orion Lawlor, lawlor@alaska.edu, 2020-07 (Public Domain)
*/
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Controls;
using UnityEngine;

public class RobotDriver : MonoBehaviour, IVehicleMotionScheme
{
    public float driveTorque=100.0f; // N-m wheel torque at normal driving speed
    
    private RobotDriveWheels _drive;
    private Logistics1SArm _arm;
    
    // Start is called before the first frame update
    void Start()
    {
        _drive=gameObject.GetComponent<RobotDriveWheels>();
        _arm=gameObject.GetComponent<Logistics1SArm>();
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
        
        // Register our motion with the player's vehicle manager
        _mgr = player.GetComponent<IVehicleManager>();
        if (_mgr!=null) {
            _mgr.PushVehicle(this);
            Debug.Log("Found vehicle manager");
        } else {
            Debug.Log("WARNING: NO vehicle manager found");
        }
        _isDriving=true;
        _justStarted=true;
        
        _smoothCamera=_player.transform.position;
        _player.GetComponent<Rigidbody>().velocity=new Vector3(0,0,0); // stop player momentum
        _player.GetComponent<Collider>().enabled=false; // stop player collisions
        
    }
    
    // State variables used while driving (only in Start/Stop driving)
    private bool _isDriving=false;
    private bool _justStarted=false;
    private GameObject _player;
    private IVehicleManager _mgr;
    private Vector3 _smoothCamera;

    
    // Stop driving this vehicle (jump out)
    void StopDriving() {
        if (!_isDriving) return; // we've already stopped
        
        _restartCooldown=30;
        
        _player.GetComponent<Collider>().enabled=true; // re-enable player collisions
        
        
        // Teleport player back a bit (this is tricky, collisions are incredibly violent)
        _player.transform.position=gameObject.transform.position 
            - 1.0f*gameObject.transform.right  // move behind the robot (safest place?)
            + (new Vector3(0,1.0f,0));
        
        // Unregister from the player's controls
        _mgr.PopVehicle(); 
        
        _isDriving=false;
    }
    
    void FixedUpdate() {
        if (_restartCooldown>0) _restartCooldown--;
    }

    // IVehicleMotionScheme interface

    // Physics forces
    public void VehicleFixedUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform)
    {
        if (ui.jump) {
            StopDriving();
            return;
        }
        
        // Handle driving commands
        if (ui.move.magnitude<0.01f) _drive.Stop();
        else {
            _drive.Brake(0.0f,0.0f);
            float forward=ui.move.x;
            float turn=ui.move.z*0.3f;
            float L=driveTorque*(forward-turn); // torque for left wheel
            float R=driveTorque*(forward+turn); // right wheel
            _drive.Drive(L,-R); // negative because one motor is facing backwards
            // Debug.Log("Motor drive torques: "+L+", "+R);
        }
        
        // Manually keyboard-command the robot arm
        float elbow=0, wrist=0;
        if (Input.GetKey(KeyCode.R) && _arm.elbowTarget<272.0f) elbow=+1.0f;
        if (Input.GetKey(KeyCode.F) && _arm.elbowTarget>40.0f) elbow=-1.0f;
        
        if (Input.GetKey(KeyCode.Q)) wrist=+1.0f;
        if (Input.GetKey(KeyCode.E)) wrist=-1.0f;
        
        _arm.elbowTarget+=elbow*_arm.elbowMoveRate*Time.deltaTime;
        _arm.wristTarget+=wrist*_arm.wristMoveRate*Time.deltaTime;
         
    }

    
    public void VehicleUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform)
    {
        if (_justStarted) {
            ui.yaw=0; // robot is oriented differently than you
            _justStarted=false;
        }
        ui.pitch=Mathf.Clamp(ui.pitch,-80,+80);
        ui.yaw=Mathf.Clamp(ui.yaw%360.0f,-180,+180);
    
        // The player essentially is the robot now
        float speedZoomout=gameObject.GetComponent<Rigidbody>().velocity.magnitude;
        ui.yaw*=Mathf.Clamp(1.0f-speedZoomout*Time.deltaTime,0.5f,1.0f); // drop yaw to zero when driving
        float radius=1.4f + 0.2f*speedZoomout;
        Quaternion look_yaw=Quaternion.Euler(0.0f, ui.yaw, 0.0f);
        playerTransform.position=gameObject.transform.position
            +(gameObject.transform.right*0.7f) // in main work area
            +(new Vector3(0.0f,-1.0f,0)); // compensate for camera's +1.8m in y
        playerTransform.rotation=gameObject.transform.rotation*look_yaw;
        playerTransform.position-=radius*playerTransform.right;
        
        // Smooth camera moves
        Vector3 newPosition=playerTransform.position;
        float smooth=1.0f-3.0f*Time.deltaTime;
        _smoothCamera=_smoothCamera*smooth+(1.0f-smooth)*newPosition;
        playerTransform.position=_smoothCamera;
        
        
        // The camera is the only thing that moves for mouse look
        cameraTransform.localRotation = Quaternion.Euler(ui.pitch,90,0);
        
        
    }
    
}
