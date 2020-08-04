using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SteeringWheel : MonoBehaviour
{
    [SerializeField] private float _rotationMultiplier = default;
    [SerializeField] private WheelCollider _wheelToObserve = default;
    [SerializeField] private float _smoothingMultiplier = default;

    private Vector3 _defaultLocalRotation;
    private float _rotationTarget;
    private float _currentRotation;

    private void Start()
    {
        _defaultLocalRotation = transform.localEulerAngles;
        _currentRotation = 0.0f;
    }

    private void Update()
    {
        if (_wheelToObserve == null)
            return;
        
        _rotationTarget = _wheelToObserve.steerAngle * _rotationMultiplier;
        _currentRotation = Mathf.Lerp(_currentRotation, _rotationTarget , Time.deltaTime * _smoothingMultiplier);
        
        transform.localEulerAngles = _defaultLocalRotation + new Vector3(0, 0, _currentRotation);
    }
}
