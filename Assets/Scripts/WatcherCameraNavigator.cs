using UnityEngine;
using UnityEngine.InputSystem;

public class WatcherCameraNavigator : MonoBehaviour
{
    [Header("Camera References")]
    [SerializeField] private Camera watcherCamera;
    [SerializeField] private Transform boardCenter;

    [Header("Target Positions")]
    [SerializeField] private Transform frontPosition;
    [SerializeField] private Transform leftPosition;
    [SerializeField] private Transform backPosition;
    [SerializeField] private Transform rightPosition;
    [SerializeField] private Transform topPosition;

    [Header("Movement Settings")]
    [SerializeField] private float movementSpeed = 2f;
    [SerializeField] private float rotationSpeed = 5f;

    [Header("Input")]
    [SerializeField] private InputActionAsset inputActions;

    private InputAction cameraFrontAction;
    private InputAction cameraLeftAction;
    private InputAction cameraBackAction;
    private InputAction cameraRightAction;
    private InputAction cameraTopAction;

    private Transform currentTarget;
    private Transform desiredTarget;
    private bool isMoving;

    private enum CameraPosition { Front, Left, Back, Right, Top }
    private CameraPosition currentPosition = CameraPosition.Front;

    private void Awake()
    {
        if (inputActions == null)
        {
            Debug.LogError("No input actions assigned to WatcherCameraNavigator!");
            return;
        }

        var watcherActionMap = inputActions.FindActionMap("WatcherControls");
        if (watcherActionMap != null)
        {
            cameraFrontAction = watcherActionMap.FindAction("CameraFront");
            cameraLeftAction = watcherActionMap.FindAction("CameraLeft");
            cameraBackAction = watcherActionMap.FindAction("CameraBack");
            cameraRightAction = watcherActionMap.FindAction("CameraRight");
            cameraTopAction = watcherActionMap.FindAction("CameraTop");
        }
        else
        {
            Debug.LogError("WatcherControls action map not found!");
        }
    }

    private void Start()
    {
        if (watcherCamera == null)
        {
            watcherCamera = Camera.main;
        }

        if (frontPosition != null)
        {
            currentTarget = frontPosition;
            desiredTarget = frontPosition;
            watcherCamera.transform.position = frontPosition.position;
            watcherCamera.transform.rotation = frontPosition.rotation;
        }

        if (boardCenter == null)
        {
            boardCenter = transform;
            Debug.LogWarning("Board center not assigned, using this GameObject as center");
        }
    }

    private void OnEnable()
    {
        if (cameraFrontAction != null)
        {
            cameraFrontAction.Enable();
            cameraFrontAction.performed += _ => RequestPosition(frontPosition, CameraPosition.Front);
        }

        if (cameraLeftAction != null)
        {
            cameraLeftAction.Enable();
            cameraLeftAction.performed += _ => RequestPosition(leftPosition, CameraPosition.Left);
        }

        if (cameraBackAction != null)
        {
            cameraBackAction.Enable();
            cameraBackAction.performed += _ => RequestPosition(backPosition, CameraPosition.Back);
        }

        if (cameraRightAction != null)
        {
            cameraRightAction.Enable();
            cameraRightAction.performed += _ => RequestPosition(rightPosition, CameraPosition.Right);
        }

        if (cameraTopAction != null)
        {
            cameraTopAction.Enable();
            cameraTopAction.performed += _ => RequestPosition(topPosition, CameraPosition.Top);
        }
    }

    private void OnDisable()
    {
        cameraFrontAction?.Disable();
        cameraLeftAction?.Disable();
        cameraBackAction?.Disable();
        cameraRightAction?.Disable();
        cameraTopAction?.Disable();
    }

    private void RequestPosition(Transform target, CameraPosition position)
    {
        if (target == null) return;

        desiredTarget = target;
        
        if (currentTarget != desiredTarget)
        {
            isMoving = true;
            currentPosition = position;
        }
    }

    private void Update()
    {
        if (isMoving && desiredTarget != null)
        {
            Vector3 targetPos = desiredTarget.position;

            float distance = Vector3.Distance(watcherCamera.transform.position, targetPos);

            if (distance < 0.1f)
            {
                watcherCamera.transform.position = targetPos;
                currentTarget = desiredTarget;
                isMoving = false;
            }
            else
            {
                Vector3 pathPosition = CalculatePathPosition();
                
                watcherCamera.transform.position = Vector3.MoveTowards(
                    watcherCamera.transform.position,
                    pathPosition,
                    movementSpeed * Time.deltaTime
                );
            }
        }

        if (boardCenter != null)
        {
            Vector3 directionToCenter = boardCenter.position - watcherCamera.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(directionToCenter);
            watcherCamera.transform.rotation = Quaternion.Slerp(
                watcherCamera.transform.rotation,
                lookRotation,
                rotationSpeed * Time.deltaTime
            );
        }
    }

    private Vector3 CalculatePathPosition()
    {
        if (currentPosition == CameraPosition.Top || desiredTarget == topPosition)
        {
            return desiredTarget.position;
        }

        Vector3 currentPos = watcherCamera.transform.position;
        Vector3 targetPos = desiredTarget.position;

        float currentAngle = Mathf.Atan2(currentPos.z - boardCenter.position.z, currentPos.x - boardCenter.position.x);
        float targetAngle = Mathf.Atan2(targetPos.z - boardCenter.position.z, targetPos.x - boardCenter.position.x);

        float angleDiff = Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg);
        
        float nextAngle = currentAngle + Mathf.Sign(angleDiff) * movementSpeed * Time.deltaTime;

        float currentRadius = Vector3.Distance(new Vector3(currentPos.x, 0, currentPos.z), new Vector3(boardCenter.position.x, 0, boardCenter.position.z));
        float targetRadius = Vector3.Distance(new Vector3(targetPos.x, 0, targetPos.z), new Vector3(boardCenter.position.x, 0, boardCenter.position.z));
        float radius = Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * movementSpeed);

        Vector3 nextPosition = new Vector3(
            boardCenter.position.x + Mathf.Cos(nextAngle) * radius,
            Mathf.Lerp(currentPos.y, targetPos.y, Time.deltaTime * movementSpeed),
            boardCenter.position.z + Mathf.Sin(nextAngle) * radius
        );

        return nextPosition;
    }
}
