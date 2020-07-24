using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Controls.Command
{
    public interface IMovementCommand
    {
        void ExecuteMovement();
    }

    public interface IPlayerRotationCommand
    {
        void ExecuteRotate();
    }

    public class KeyboardMoveCommand : IMovementCommand
    {
        /* Motion rate, in meters per second */
        public Vector3 Velocity { get; set; }
        
        /* Height above terrain to maintain (player / vehicle height) */
        public float HeightOverTerrain;

        public GameObject AffectedGameObject { get; set; }

        public void ExecuteMovement()
        {
            // Horizontal move
            var tf=AffectedGameObject.transform;
            tf.Translate(Velocity * Time.deltaTime);
            
            // Vertical move
            var pos=tf.position;
            var ht=Terrain.activeTerrain.SampleHeight(pos);
            pos.y = ht + HeightOverTerrain;
            tf.position=pos;
        }
    }

    public class MouseRotateCommand : IPlayerRotationCommand
    {
        public Vector2 CurrentMouseLook { get; set; }

        public Quaternion CameraRotation { get; set; }

        public GameObject AffectedGameObject { get; set; }

        public Transform AffectedCamera { get; set; }

        public void ExecuteRotate()
        {
            AffectedCamera.localRotation = CameraRotation;
            AffectedGameObject.transform.Rotate(Vector3.up, CurrentMouseLook.x);
        }
    }

    public class VRMoveCommand : IMovementCommand
    {
        public void ExecuteMovement()
        {
            //define teleport movement
        }
    }


}
