using UnityEngine;
using UnityEngine.InputSystem;

public class WatcherInputManager : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera frontCamera;
    [SerializeField] private Camera leftCamera;
    [SerializeField] private Camera backCamera;
    [SerializeField] private Camera rightCamera;
    [SerializeField] private Camera topCamera;

    [Header("Input Action")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction cameraFrontAction;
    private InputAction cameraLeftAction;
    private InputAction cameraBackAction;
    private InputAction cameraRightAction;
    private InputAction cameraTopAction;

    private Camera currentCamera;
    private Camera[] allCameras;

    private void Awake()
    {
        allCameras = new Camera[] { frontCamera, leftCamera, backCamera, rightCamera, topCamera };

        if (inputActions == null)
        {
            Debug.LogError("No input actions assigned to WatcherInputManager!");
            return;
        }

        var watcherActionMap = inputActions.FindActionMap("WatcherControls");
        if (watcherActionMap != null)
        {
            Debug.Log("WatcherControls action map found!");
            cameraFrontAction = watcherActionMap.FindAction("CameraFront");
            cameraLeftAction = watcherActionMap.FindAction("CameraLeft");
            cameraBackAction = watcherActionMap.FindAction("CameraBack");
            cameraRightAction = watcherActionMap.FindAction("CameraRight");
            cameraTopAction = watcherActionMap.FindAction("CameraTop");
            
            Debug.Log($"Actions found - Front: {cameraFrontAction != null}, Left: {cameraLeftAction != null}, Back: {cameraBackAction != null}, Right: {cameraRightAction != null}, Top: {cameraTopAction != null}");
        }
        else
        {
            Debug.LogError("WatcherControls action map not found in input actions!");
        }
    }

    private void OnEnable()
    {
        if (cameraFrontAction != null)
        {
            cameraFrontAction.Enable();
            cameraFrontAction.performed += _ => { Debug.Log("Front Camera Input!"); SwitchToCamera(frontCamera); };
        }

        if (cameraLeftAction != null)
        {
            cameraLeftAction.Enable();
            cameraLeftAction.performed += _ => { Debug.Log("Left Camera Input!"); SwitchToCamera(leftCamera); };
        }

        if (cameraBackAction != null)
        {
            cameraBackAction.Enable();
            cameraBackAction.performed += _ => { Debug.Log("Back Camera Input!"); SwitchToCamera(backCamera); };
        }

        if (cameraRightAction != null)
        {
            cameraRightAction.Enable();
            cameraRightAction.performed += _ => { Debug.Log("Right Camera Input!"); SwitchToCamera(rightCamera); };
        }

        if (cameraTopAction != null)
        {
            cameraTopAction.Enable();
            cameraTopAction.performed += _ => { Debug.Log("Top Camera Input!"); SwitchToCamera(topCamera); };
        }

        SwitchToCamera(frontCamera);
    }

    private void OnDisable()
    {
        if (cameraFrontAction != null)
        {
            cameraFrontAction.performed -= _ => SwitchToCamera(frontCamera);
            cameraFrontAction.Disable();
        }

        if (cameraLeftAction != null)
        {
            cameraLeftAction.performed -= _ => SwitchToCamera(leftCamera);
            cameraLeftAction.Disable();
        }

        if (cameraBackAction != null)
        {
            cameraBackAction.performed -= _ => SwitchToCamera(backCamera);
            cameraBackAction.Disable();
        }

        if (cameraRightAction != null)
        {
            cameraRightAction.performed -= _ => SwitchToCamera(rightCamera);
            cameraRightAction.Disable();
        }

        if (cameraTopAction != null)
        {
            cameraTopAction.performed -= _ => SwitchToCamera(topCamera);
            cameraTopAction.Disable();
        }
    }

    private void SwitchToCamera(Camera targetCamera)
    {
        if (targetCamera == null || targetCamera == currentCamera) return;

        foreach (Camera cam in allCameras)
        {
            if (cam != null)
            {
                cam.gameObject.SetActive(cam == targetCamera);
            }
        }

        currentCamera = targetCamera;
        Debug.Log($"Switched to camera: {currentCamera.name}");
    }
}
