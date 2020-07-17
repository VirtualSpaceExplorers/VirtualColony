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
        private float _xRotation = 0f;
        private float _playerCameraOffset=1.8f; // meters between the player frame and camera height
        private float _terrainHeight=1.8f; // meters above terrain to lock camera

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

        public void MovePlayer(GameObject playerObject)
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
                _terrainHeight = Mathf.Max(1.8f,_terrainHeight*heightScale);
                tickVector.y=0.0f;
            }
            
            // Hold shift to sprint
            var moveScale = _moveSpeed*_terrainHeight;//<- get faster as we go higher
            if (Input.GetKey(KeyCode.LeftShift))  // sprint mode with shift key
            {
                moveScale = moveScale * 5.0f;
            }
            
        // SNIP HERE to separate UI from application to model
            
            var targetVelocity = tickVector*moveScale;
            float playerHeight = _terrainHeight-_playerCameraOffset;
            
             // Horizontal move: set the velocity to the target
            var tf=playerObject.transform;
            var pos=tf.position;
            var terrainHt=Terrain.activeTerrain.SampleHeight(pos);
            var headHt = pos.y - terrainHt;
            bool onGround = headHt < playerHeight; // HACK!  What about in tunnel, on hab, etc?
            
            /* // Old pre-rigidbody movement (still useful for teleport)
            tf.Translate(targetVelocity * Time.deltaTime);
            */
            
            var rb=playerObject.GetComponent<Rigidbody>();
            
            /* This is a goofy way to accomplish walking:
               Basically a spring simulator to lock in the player's velocity.
            */
            if (onGround) { // we're standing, so we can apply walking force
                var walkSpring=200.0f; // Newtons per m/s target difference
                var currentVelocity = rb.velocity;
                
                // Rotate the (player local) target velocity out to world space
                var targetWorldVelocity = tf.localRotation*targetVelocity;
                
                var walkForce=walkSpring*(targetWorldVelocity-currentVelocity);
                
                walkForce.y=0; // can't apply vertical force (acts like drag)
                if (Input.GetKey(KeyCode.Space)) 
                    walkForce.y+=2000.0f; // jump force, in Newtons
                
                rb.AddForce(walkForce); 
                // Debug.Log("Added walking force "+walkForce);
            }
            
            // Shift the collider to push us up off the ground with Q and E
            //   FIXME: stop bothering with physics & walking once you get too big.
            var capsule=playerObject.GetComponent<CapsuleCollider>();
            capsule.center=new Vector3(0.0f,1.0f-0.8f*playerHeight,0.0f);
            capsule.radius=0.2f+0.1f*playerHeight;
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

        private Vector2 lastMouseInput; // for smoothing
        public void RotatePlayer(GameObject gameObject, Transform cameraTransform)
        {
            var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime, 
                                        Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime);
            float smooth=0.5f; // amount of mouse smoothing to apply
            lastMouseInput = (1.0f-smooth)*mouseInput + smooth*lastMouseInput;

            _xRotation -= lastMouseInput.y;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            var cameraRotation = Quaternion.Euler(_xRotation, 0f, 0f);
            
         // SNIP HERE to separate targeted rotation 

            cameraTransform.localRotation = cameraRotation;
            gameObject.transform.Rotate(Vector3.up, mouseInput.x);
        }

        // Called by ControlSchemeBuilder
        public void SetMove(KeyCode keyCode, Vector3 direction)
        {
            _movementControls.Add(keyCode, direction);
        }

    }
}
