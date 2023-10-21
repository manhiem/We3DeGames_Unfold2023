using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    Vector2 _viewInput;
    float _camRotationX;
    float _camRotationY;

    public Camera _localCamera { get; private set; }

    PlayerMovementController _movementController;
    private void Awake()
    {
        _localCamera = GetComponent<Camera>();
        _movementController = GetComponentInParent<PlayerMovementController>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (_localCamera.enabled)
        {
            _localCamera.transform.parent = null;
        }

        //_camRotationX = GameManager.instance._cameraViewRotation.x;
        //_camRotationY = GameManager.instance._cameraViewRotation.y;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraAnchorPoint == null)
            return;

        if (!_localCamera.enabled)
            return;

        _localCamera.transform.position = cameraAnchorPoint.position;

        // Get the rotation of the camera anchor point
        Quaternion targetRotation = cameraAnchorPoint.rotation;

        // Set the X rotation of the camera to match the anchor point's rotation
        _localCamera.transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, _camRotationY, 0);

        // Update the Y rotation based on input
        _camRotationY += _viewInput.x * Time.deltaTime * _movementController.rotationSpeed;

        // Apply the updated Y rotation
        _localCamera.transform.rotation = Quaternion.Euler(targetRotation.eulerAngles.x, _camRotationY, 0);
    }


    public void SetViewInputVector(Vector2 vI)
    {
        _viewInput = vI;
    }

    //private void OnDestroy()
    //{
    //    if (_camRotationX != 0 && _camRotationY != 0)
    //    {
    //        GameManager.instance._cameraViewRotation.x = _camRotationX;
    //        GameManager.instance._cameraViewRotation.y = _camRotationY;
    //    }
    //}
}
