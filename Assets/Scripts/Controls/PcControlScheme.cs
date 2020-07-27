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
    private class WalkingMotionScheme : IVehicleMotionScheme {
        private PcControlScheme _param;
        public WalkingMotionScheme(PcControlScheme param) {
            _param=param;
        }
        
        // Physics forces
        public void VehicleFixedUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform) 
        {
            // Pressing Q or E scales the camera down or up, limited to human height (1.8 meters)
            if (ui.move.y!=0.0f) 
            { 
                float heightScale = 1.0f+1.0f*Time.deltaTime*ui.move.y;
                _param._playerHeight = Mathf.Max(1.8f,_param._playerHeight*heightScale);
                ui.move.y=0.0f;
            }
            
            // Hold shift to sprint
            float moveScale = _param._moveSpeed*_param._playerHeight;//<- move faster as we grow higher
            if (ui.sprint)  // sprint mode with shift key
            {
                moveScale = moveScale * 5.0f;
            }
            Vector3 targetVelocity = ui.move*moveScale;
            
            
            /* This is a goofy way to accomplish walking:
               Basically a spring simulator to lock in the player's velocity,
               while respecting the other world physics like colliders.
            */
            var rb=playerTransform.gameObject.GetComponent<Rigidbody>();
            bool onGround=true; // <- FIXME: figure out when we're ballistic
            if (onGround) { // we're standing, so we can apply walking force
                var spring=200.0f; // Newtons per m/s target difference
                var currentVelocity = rb.velocity;
                
                // Rotate the (player local) target velocity out to world space
                var targetWorldVelocity = playerTransform.localRotation*targetVelocity;
                
                var F=spring*(targetWorldVelocity-currentVelocity);
                if (F.magnitude>_param.maxWalkForce) {
                    F=F.normalized*_param.maxWalkForce;
                }
                
                F.y=0; // can't apply vertical force (acts like drag)
                if (ui.jump) 
                    F.y+=_param.maxJumpForce;
                
                rb.AddForce(F); 
            }
            
            // Shift the collider to push us up off the ground with Q and E
            //   FIXME: stop bothering with physics & walking once you get too big.
            var capsule=playerTransform.gameObject.GetComponent<CapsuleCollider>();
            capsule.center=new Vector3(0.0f,_param._playerHeight/2,0.0f);
            capsule.height=_param._playerHeight;
            float extraHeight=_param._playerHeight-_param._playerCameraOffset;
            capsule.radius=0.2f+0.2f*extraHeight;
            
            // Push camera to match player size
            var camPos=cameraTransform.localPosition;
            camPos.y=_param._playerHeight-0.1f;
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
        public void VehicleUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform) 
        {
            // While walking, your head needs to stay level (ish)
            if (Mathf.Abs(ui.pitch)>90.0f) ui.pitch*=1.0f-0.5f*Time.deltaTime; // <- gently return head to neutral
            
            // Pitch applies only to the camera
            cameraTransform.localRotation = Quaternion.Euler(ui.pitch, 90f, 0f);
            
            // We yaw the entire player
            playerTransform.localRotation = Quaternion.Euler(0.0f,ui.yaw,0f);
        }
    };
    
    public float _moveSpeed=5.0f; // m/s motion speed (per unit player height)
    public float _sensitivity=3.0f; // mouse look speed (degrees rotation per pixel of mouse movement?)
    
    public float _playerCameraOffset=1.8f; // meters between the player frame and camera height
    private float _playerHeight=1.8f; // meters above terrain to lock camera
    
    public float maxWalkForce=1000.0f; // Newtons maximum horizontal force you can apply (friction, muscles, etc)
    public float maxJumpForce=2000.0f; // jump force, in Newtons
    
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
        
        currentVehicle = new WalkingMotionScheme(this);
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
    
    // Update graphics motions
    public void Update()
    {
        // var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
        var mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        mouseInput *= _sensitivity; // no Time.deltaTime, because "you do not need to be concerned about varying frame-rates when using this value."
        
        float smooth=0.5f; // amount of mouse smoothing to apply
        smoothedMouse = (1.0f-smooth)*mouseInput + smooth*smoothedMouse;
        
        float Y_look_direction=+1.0f; // normal Y mouse
        // Y_look_direction=-1.0f; // inverted Y look
        
        // Apply incremental mouse movement to the look directions
        ui.pitch += -Y_look_direction*smoothedMouse.y; // degrees rotation about X axis (pitch)
        ui.yaw += smoothedMouse.x; // degrees rotation about Y axis (yaw)
        
        currentVehicle.VehicleUpdate(ref ui,playerTransform,cameraTransform);
    }


}
