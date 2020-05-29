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

            var controls = new PcControlScheme(0.25f, 100f);
            controls.SetMove(KeyCode.W, new Vector3(0, 0, 1));
            controls.SetMove(KeyCode.A, new Vector3(-1, 0, 0));
            controls.SetMove(KeyCode.D, new Vector3(1, 0, 0));
            controls.SetMove(KeyCode.S, new Vector3(0, 0, -1));

            return controls;
        }
    }
}
