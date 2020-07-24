using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Assets.Src.Controls.Command;
using UnityEngine; 

namespace Assets.Src.Controls
{
    public class PcControlScheme : IControlScheme
    {
        private float _moveSpeed;
        private float _sensitivity;
        
        public float _playerCameraOffset=1.8f; // meters between the player frame and camera height
        private float _playerHeight=1.8f; // meters above terrain to lock camera
        
        public float maxWalkForce=1000.0f; // Newtons maximum horizontal force you can apply (friction, muscles, etc)
        public float maxJumpForce=2000.0f; // jump force, in Newtons
        
        public Vector3 playerPosition; // <- in global coordinates
        public Vector3 playerVelocity; // <- for keeping an eye on speed
        
        // This maps a keyboard character to a player motion vector.
        //  The up direction is overloaded to mean "scale the player".
        private Dictionary<KeyCode, Vector3> _movementControls = new Dictionary<KeyCode, Vector3>();

        public PcControlScheme(float moveSpeed, float mouseSensitivity)
        {
            _moveSpeed = moveSpeed;
            _sensitivity = mouseSensitivity;
        }

        public Action StartupActions()
        {
            return () => { 
                Cursor.lockState = CursorLockMode.Locked; 
                Physics.gravity = new Vector3(0.0f,-3.69f,0.0f); // Mars gravity, along -Y axis
                Debug.Log("Started Pc Controls"); 
            };
        }

        public void MovePlayer(GameObject playerObject, Transform cameraTransform)
        {
            var tickVector = new Vector3();
            foreach(var key in _movementControls.Keys)
            {
                if (Input.GetKey(key))
                {
                    tickVector += _movementControls[key];
                }
            }
            
            // Pressing Q or E scales the camera down or up, limited to human height (1.8 meters)
            if (tickVector.y!=0.0f) 
            { 
                float heightScale = 1.0f+1.0f*Time.deltaTime*tickVector.y;
                _playerHeight = Mathf.Max(1.8f,_playerHeight*heightScale);
                tickVector.y=0.0f;
            }
            
            // Hold shift to sprint
            var moveScale = _moveSpeed*_playerHeight;//<- get faster as we go higher
            if (Input.GetKey(KeyCode.LeftShift))  // sprint mode with shift key
            {
                moveScale = moveScale * 5.0f;
            }
            
        // SNIP HERE to separate UI from application to model
            
            var targetVelocity = tickVector*moveScale;
            
             // Horizontal move: set the velocity to the target
            var tf=playerObject.transform;
            var pos=tf.position;
            var terrainHt=Terrain.activeTerrain.SampleHeight(pos);
            var headHt = pos.y - terrainHt;
            bool onGround = true; // headHt < playerHeight; // HACK!  What about in tunnel, on hab, etc?
            
            /* // Old pre-rigidbody movement (still useful for teleport)
            tf.Translate(targetVelocity * Time.deltaTime);
            */
            
            var rb=playerObject.GetComponent<Rigidbody>();
            playerVelocity=rb.velocity;
            playerPosition=pos;
            
            /* This is a goofy way to accomplish walking:
               Basically a spring simulator to lock in the player's velocity.
            */
            if (onGround) { // we're standing, so we can apply walking force
                var walkSpring=200.0f; // Newtons per m/s target difference
                var currentVelocity = rb.velocity;
                
                // Rotate the (player local) target velocity out to world space
                var targetWorldVelocity = tf.localRotation*targetVelocity;
                
                var walkForce=walkSpring*(targetWorldVelocity-currentVelocity);
                if (walkForce.magnitude>maxWalkForce) {
                    walkForce=walkForce.normalized*maxWalkForce;
                }
                
                walkForce.y=0; // can't apply vertical force (acts like drag)
                if (Input.GetKey(KeyCode.Space)) 
                    walkForce.y+=maxJumpForce;
                
                
                rb.AddForce(walkForce); 
                // Debug.Log("Added walking force "+walkForce);
            }
            
            // Shift the collider to push us up off the ground with Q and E
            //   FIXME: stop bothering with physics & walking once you get too big.
            var capsule=playerObject.GetComponent<CapsuleCollider>();
            capsule.center=new Vector3(0.0f,_playerHeight/2,0.0f);
            capsule.height=_playerHeight;
            float extraHeight=_playerHeight-_playerCameraOffset;
            capsule.radius=0.2f+0.2f*extraHeight;
            
            // Push camera to match player size
            var camPos=cameraTransform.localPosition;
            camPos.y=_playerHeight-0.1f;
            cameraTransform.localPosition=camPos;
            
            /*
            if (headHt<playerHeight/2) { // We ended up below the terrain, fix it.
                var pos=tf.position;
                var ht=Terrain.activeTerrain.SampleHeight(pos);
                pos.y = terrainHt + playerHeight;
                tf.position=pos;
            }*/
            /*
            // Vertical move: limit position
            var pos=tf.position;
            var ht=Terrain.activeTerrain.SampleHeight(pos);
            pos.y = ht + playerHeight;
            tf.position=pos;
            */
        }

        private Vector2 smoothedMouse; // for smoothing
        public void RotatePlayer(GameObject gameObject, Transform cameraTransform)
        {
            // var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
            var mouseInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            mouseInput *= _sensitivity; // no Time.deltaTime, because "you do not need to be concerned about varying frame-rates when using this value."
            
            float smooth=0.5f; // amount of mouse smoothing to apply
            smoothedMouse = (1.0f-smooth)*mouseInput + smooth*smoothedMouse;
            
            float Y_look_direction=+1.0f; // normal Y mouse
            // Y_look_direction=-1.0f; // inverted Y look
            
            float xRotation = -Y_look_direction*smoothedMouse.y; // degrees rotation about X axis (pitch)
            float yRotation = smoothedMouse.x; // degrees rotation about Y axis (yaw)
            
            // Apply incremental rotations to the game objects
            cameraTransform.localRotation *= Quaternion.Euler(xRotation, 0f, 0f);
            gameObject.transform.localRotation *= Quaternion.Euler(0.0f,yRotation,0f);
        }

        // Called by ControlSchemeBuilder
        public void SetMove(KeyCode keyCode, Vector3 direction)
        {
            _movementControls.Add(keyCode, direction);
        }

    }
}
