/*
  Keyboard and mouse based control scheme.

  Good key mapping discussion here:
     https://forum.unity.com/threads/most-common-keyboard-mouse-inputs-for-pc-games.380594/

*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine; 
using Assets.Source.Controls;

public class PcControlScheme : MonoBehaviour, IVehicleManager
{
    public float _sensitivity=2.0f; // mouse look speed (degrees rotation per pixel of mouse movement?)
    
    // User Input data
    public UserInput ui; 
    
    public Vector3 playerPosition; // <- in global coordinates
    public Vector3 playerVelocity; // <- for keeping an eye on speed
    
    private GameObject playerObject;
    private Transform playerTransform;
    private Transform cameraTransform;
    
    private IVehicleMotionScheme currentVehicle;
    private Stack<IVehicleMotionScheme> vehicleStack;
    public void PushVehicle(IVehicleMotionScheme newVehicle) 
    {
        vehicleStack.Push(currentVehicle);
        currentVehicle=newVehicle;
    }
    public void PopVehicle() 
    {
        if (vehicleStack.Count>0) {
            currentVehicle=vehicleStack.Pop();
        }
    }

    public void Start()
    {
        playerObject = gameObject; // This script is loaded as a component of the Player
        playerTransform = playerObject.transform;
        cameraTransform = Camera.main.transform; // The main camera, inside the Player
        ui.yaw=playerTransform.eulerAngles.y;
        
        Cursor.lockState = CursorLockMode.Locked;  // grab mouse
        Debug.Log("Started Pc Controls"); 
        
        // WASD move the player around
        SetMove(KeyCode.W, new Vector3(1, 0, 0));
        SetMove(KeyCode.S, new Vector3(-1, 0, 0));
        SetMove(KeyCode.A, new Vector3(0, 0, 1));
        SetMove(KeyCode.D, new Vector3(0, 0, -1));
        
        // + and - scale the player up and down
        SetMove(KeyCode.Plus, new Vector3(0, +1, 0));
        SetMove(KeyCode.Equals, new Vector3(0, +1, 0));
        SetMove(KeyCode.Minus, new Vector3(0, -1, 0));
        SetMove(KeyCode.Underscore, new Vector3(0, -1, 0));
        
        currentVehicle = gameObject.AddComponent<WalkingMotion>(); // new WalkingMotion(); 
        vehicleStack=new Stack<IVehicleMotionScheme>();
    }
    
    // This maps a keyboard character to a player motion vector.
    //  The up direction is overloaded to mean "scale the player".
    private Dictionary<KeyCode, Vector3> _movementControls = new Dictionary<KeyCode, Vector3>();

    public void SetMove(KeyCode keyCode, Vector3 direction)
    {
        _movementControls.Add(keyCode, direction);
    }

    // Apply physics forces
    public void FixedUpdate()
    {
        var moveVector = new Vector3();
        foreach(var key in _movementControls.Keys)
        {
            if (Input.GetKey(key))
            {
                moveVector += _movementControls[key];
            }
        }
        
        ui.move=moveVector;
        ui.action=Input.GetKey(KeyCode.F) || Input.GetKey(KeyCode.Mouse0) || Input.GetKey(KeyCode.Return);
        ui.jump=Input.GetKey(KeyCode.Space);
        ui.sprint=Input.GetKey(KeyCode.LeftShift);
        ui.menu=Input.GetKey(KeyCode.Tab);
        
        currentVehicle.VehicleFixedUpdate(ref ui,playerTransform,cameraTransform);
        
    }

    private Vector2 smoothedMouse; // for smoothing between frames
    private float lastFpsTime=0;
    private int lastFpsCount=0;
    
    // Update graphics motions
    public void Update()
    {
        // var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        var mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseInput *= _sensitivity; // no Time.deltaTime, because "you do not need to be concerned about varying frame-rates when using this value."
        
        float smooth=Mathf.Clamp(1.0f-60.0f*Time.deltaTime,0.01f,1.0f); // amount of mouse smoothing to apply
        smoothedMouse = (1.0f-smooth)*mouseInput + smooth*smoothedMouse;
        
        float Y_look_direction=+1.0f; // normal Y mouse
        // Y_look_direction=-1.0f; // inverted Y look
        
        // Apply incremental mouse movement to the look directions
        ui.pitch += -Y_look_direction*smoothedMouse.y; // degrees rotation about X axis (pitch)
        ui.yaw += smoothedMouse.x; // degrees rotation about Y axis (yaw)
        
        currentVehicle.VehicleUpdate(ref ui,playerTransform,cameraTransform);
        
        lastFpsTime+=Time.deltaTime;
        lastFpsCount++;
        if (lastFpsTime>10.0f) {
            float fps=(int)(lastFpsCount/lastFpsTime);
            Debug.Log("Framerate: "+fps+" frames per second");
            lastFpsCount=0;
            lastFpsTime=0;
        }
    }


}
