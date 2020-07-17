using Assets.Src.Controls;
using UnityEngine;

public class UniversalController : MonoBehaviour
{
    private IControlScheme _currentlyActiveControlScheme;


    void Start()
    {
        _currentlyActiveControlScheme = ControlSchemeBuilder.BuildDefaultControlSheme();

        _currentlyActiveControlScheme.StartupActions()();
    }

    // FixedUpdate is called once per physics frame
    void FixedUpdate()
    {
        _currentlyActiveControlScheme.MovePlayer(gameObject);
        _currentlyActiveControlScheme.RotatePlayer(gameObject, Camera.main.transform);
    }
}

