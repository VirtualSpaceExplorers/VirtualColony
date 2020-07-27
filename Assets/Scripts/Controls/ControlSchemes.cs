/*
Generic interfaces shared all control input and vehicle motion schemes.

Player / Vehicle coordinate system:
    +Y is up (against gravity)
    +X is forward (direction of natural motion, NASA standard)
    +Z is to the left, because Unity uses a left-handed coordinate system, so help us all.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Source.Controls
{
    /* This data-only class is used to describe player input */
    public struct UserInput {
        public Vector3 move; // XYZ translation (WASD QE)
        
        public bool action; // activate object (mouseclick / F / enter)
        public bool jump; // extra lift (spacebar)
        public bool sprint; // extra speed (shift)
        public bool menu; // bring up menu (tab toggles)
        
        public float roll,yaw,pitch; // camera rotation about XYZ axes (degrees)
    };
    
    /** This interface is used to take player command input, and move a vehicle
        (e.g., walking, driving, flying, etc)    */
    public interface IVehicleMotionScheme
    {
        // Physics forces
        void VehicleFixedUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform);

        // Graphics, like mouse move
        void VehicleUpdate(ref UserInput ui,Transform playerTransform,Transform cameraTransform); 
    }
    
    /** This interface is for swapping nestable vehicles */
    public interface IVehicleManager 
    {
        /// Start driving this new vehicle
        void PushVehicle(IVehicleMotionScheme newVehicle);
        
        /// Back to the last vehicle
        void PopVehicle();
    }
    
}
