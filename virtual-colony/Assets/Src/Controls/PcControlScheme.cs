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
        private float _terrainHeight=2.0f; // meters above terrain to lock camera

        private Dictionary<KeyCode, Vector3> _movementControls = new Dictionary<KeyCode, Vector3>();

        public PcControlScheme(float moveSpeed, float mouseSensitivity)
        {
            _moveSpeed = moveSpeed;
            _sensitivity = mouseSensitivity;
        }

        public Action StartupActions()
        {
            return () => { Cursor.lockState = CursorLockMode.Locked; Debug.Log("Started Pc Controls"); };
        }

        public IMovementCommand MovePlayer(GameObject playerObject)
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
            if (Input.GetKeyDown(KeyCode.LeftShift)) 
            {
                Debug.Log("SPRINT MODE");
                moveScale = moveScale * 3.0f;
            }

            return new KeyboardMoveCommand
            {
                Velocity = tickVector*moveScale, 
                HeightOverTerrain=_terrainHeight,
                AffectedGameObject = playerObject
            };
        }

        private Vector2 lastMouseInput; // for smoothing
        public IPlayerRotationCommand RotatePlayer(GameObject gameObject, Transform cameraTransform)
        {
            var mouseInput = new Vector2(Input.GetAxisRaw("Mouse X") * _sensitivity * Time.deltaTime, 
                                        Input.GetAxisRaw("Mouse Y") * _sensitivity * Time.deltaTime);
            float smooth=0.5f; // amount of mouse smoothing to apply
            lastMouseInput = (1.0f-smooth)*mouseInput + smooth*lastMouseInput;

            _xRotation -= lastMouseInput.y;
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            var cameraRotation = Quaternion.Euler(_xRotation, 0f, 0f);

            return new MouseRotateCommand
            {
                AffectedCamera = cameraTransform,
                AffectedGameObject = gameObject,
                CurrentMouseLook = mouseInput,
                CameraRotation = cameraRotation
            };
        }

        public void SetMove(KeyCode keyCode, Vector3 direction)
        {
            _movementControls.Add(keyCode, direction);
        }

    }
}
