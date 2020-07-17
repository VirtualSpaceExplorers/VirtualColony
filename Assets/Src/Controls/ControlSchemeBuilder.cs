using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Src.Controls
{
    public class ControlSchemeBuilder
    {
        public static IControlScheme BuildDefaultControlSheme()
        {
            switch (Application.platform)
            {
                case RuntimePlatform.WindowsEditor:
                    Debug.Log("It's the default editor");
                    return _buildPcControlScheme();
                default:
                    return _buildPcControlScheme();
            }
        }

        private static IControlScheme _buildPcControlScheme()
        {
            //Check for existing saved settings?
            //Implement default button?
            float moveSpeed=5.0f; // m/s motion speed (per unit height)
            float lookSpeed=100.0f; // mouse look speed
            var controls = new PcControlScheme(moveSpeed, lookSpeed);
            controls.SetMove(KeyCode.W, new Vector3(0, 0, 1));
            controls.SetMove(KeyCode.A, new Vector3(-1, 0, 0));
            controls.SetMove(KeyCode.D, new Vector3(1, 0, 0));
            controls.SetMove(KeyCode.S, new Vector3(0, 0, -1));
            controls.SetMove(KeyCode.Q, new Vector3(0, -1, 0));
            controls.SetMove(KeyCode.E, new Vector3(0, +1, 0));

            return controls;
        }
    }
}
