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

    // Update is called once per frame
    void Update()
    {
        var movementCommand = _currentlyActiveControlScheme.MovePlayer(gameObject);
        var rotateCommand = _currentlyActiveControlScheme.RotatePlayer(gameObject, Camera.main.transform);

        movementCommand.ExecuteMovement();
        rotateCommand.ExecuteRotate();
    }
}

