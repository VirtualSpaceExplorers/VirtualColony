/**
 Drive a Cybertruck using Unity WheelColliders

 Written 2020-08-03 by @TeigRolle for Nexus Aurora Virtual Space Explorers.

*/
using System;
using System.Collections;
using System.Collections.Generic;
using Assets.Source.Controls;
using UnityEngine;

public class Cybertruck : MonoBehaviour, IVehicleMotionScheme
{
    [SerializeField] private SteeringWheel _steeringWheel = default;
    [SerializeField] private Vector3 _centerOfMassOffset = default;    
    [SerializeField] private  float _torque = default;
    [SerializeField] private float _steeringAngle = default;
    [SerializeField] private float _rollingBreakTorque = default;
    
    public Transform PlayerSeatTransform = default;
    
    [SerializeField] private AxleInfo[] _axleInfos = default;
    
    public bool IsDriving { get; private set; }

    private GameObject _currentDriver;
    
    private void Start()
    {
        GetComponent<Rigidbody>().centerOfMass +=_centerOfMassOffset;
    }

    void OnTriggerEnter(Collider other)
    {
        if (_currentDriver == null && other.gameObject.tag == "Mobile") {
            // Player has entered our "begin driving" trigger area
            Debug.Log("Start driving ground vehicle!");
            StartDriving(other.gameObject);
        }
    }

    private void StartDriving(GameObject driverObject)
    {
        StopBreaking();
        _currentDriver = driverObject;
        _currentDriver.transform.SetParent(PlayerSeatTransform);
        _currentDriver.transform.localPosition = Vector3.zero;
        _currentDriver.GetComponent<PcControlScheme>().PushVehicle(this);

        if (_steeringWheel != null) {
            _steeringWheel.enabled = true;
        }
    }

    private void StopDriving()
    {
        StartBreaking(_rollingBreakTorque);
        _currentDriver.transform.SetParent(null);
        // Stops the player from shooting into the sky
        _currentDriver.GetComponent<Rigidbody>().velocity = Vector3.zero;
        _currentDriver.GetComponent<PcControlScheme>().PopVehicle();
        _currentDriver = null;

        if (_steeringWheel != null) {
            _steeringWheel.enabled = false;
        }
    }

    private void StartBreaking(float breakingTorque)
    {
        foreach (AxleInfo axleInfo in _axleInfos) {
            axleInfo.leftWheel.brakeTorque = breakingTorque;
            axleInfo.rightWheel.brakeTorque = breakingTorque;
        }
    }

    private void StopBreaking()
    {
        foreach (AxleInfo axleInfo in _axleInfos) {
            axleInfo.leftWheel.brakeTorque = 0;
            axleInfo.rightWheel.brakeTorque = 0;
        }
    }

    public void VehicleUpdate(ref UserInput ui, Transform playerTransform, Transform cameraTransform)
    {
        if (ui.jump) {
            StopDriving();
        }

        /*
        
         */
        
        playerTransform.position = PlayerSeatTransform.position;
        playerTransform.forward = -transform.right;
        // Pitch applies only to the camera
        cameraTransform.localRotation = Quaternion.Euler(ui.pitch, 90f, 0f);
        // We yaw the entire player
        playerTransform.localRotation = Quaternion.Euler(0.0f, ui.yaw, 0f);
    }

    // The following Code was stolen from https://docs.unity3d.com/Manual/class-WheelCollider.html
    [System.Serializable]
    public class AxleInfo
    {
        public WheelCollider leftWheel;
        public WheelCollider rightWheel;
        public bool motor;
        public bool steering;
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;

        // Need to do this otherwise the wheels will have the wrong rotation.
        visualWheel.Rotate(-90, 0, -90);
    }

    public void VehicleFixedUpdate(ref UserInput ui, Transform playerTransform, Transform cameraTransform)
    {
        float motor = _torque * Input.GetAxis("Vertical");
        float steering = _steeringAngle * Input.GetAxis("Horizontal");

        foreach (AxleInfo axleInfo in _axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }
}