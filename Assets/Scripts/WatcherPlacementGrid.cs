using UnityEngine;
using UnityEngine.InputSystem;

public class WatcherPlacementGrid : MonoBehaviour
{
    [Header("Grid Settings")]
    [SerializeField] private float gridCellSize = 1f;
    [SerializeField] private LayerMask groundLayerMask;

    [Header("Placement Preview")]
    [SerializeField] private GameObject placementPreviewPrefab;

    [Header("Ability")]
    [SerializeField] private WatcherAbilityBase currentAbility;

    private GameObject placementPreviewInstance;
    private bool hasValidPlacement = false;
    private Vector3 currentGridPosition;
    private Camera activeCamera;

    private InputAction mousePositionAction;
    private InputAction placementAction;

    private void Awake()
    {
        var playerInput = FindAnyObjectByType<PlayerInput>();
        if (playerInput != null)
        {
            var watcherActionMap = playerInput.actions?.FindActionMap("WatcherControls");
            if (watcherActionMap != null)
            {
                mousePositionAction = watcherActionMap.FindAction("MoveCursor");
                placementAction = watcherActionMap.FindAction("Confirm");
                
                if (mousePositionAction == null)
                {
                    Debug.LogError("MoveCursor action not found in WatcherControls!");
                }
                
                if (placementAction == null)
                {
                    Debug.LogError("Confirm action not found in WatcherControls!");
                }
            }
            else
            {
                Debug.LogError("WatcherControls action map not found!");
            }
        }
        else
        {
            Debug.LogError("PlayerInput component not found in scene!");
        }
    }

    private void OnEnable()
    {
        if (placementAction != null)
        {
            placementAction.Enable();
            placementAction.performed += OnPlacementPerformed;
        }

        if (mousePositionAction != null)
        {
            mousePositionAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (placementAction != null)
        {
            placementAction.performed -= OnPlacementPerformed;
            placementAction.Disable();
        }

        if (mousePositionAction != null)
        {
            mousePositionAction.Disable();
        }
    }

    private void Start()
    {
        if (placementPreviewPrefab != null)
        {
            placementPreviewInstance = Instantiate(placementPreviewPrefab);
            placementPreviewInstance.SetActive(false);
        }
    }

    private void Update()
    {
        UpdateActiveCamera();
        UpdatePlacementPreview();
    }

    private void UpdateActiveCamera()
    {
        Camera[] cameras = FindObjectsByType<Camera>(FindObjectsSortMode.None);
        foreach (Camera cam in cameras)
        {
            if (cam.isActiveAndEnabled && cam.gameObject.activeInHierarchy)
            {
                activeCamera = cam;
                break;
            }
        }
    }

    private void UpdatePlacementPreview()
    {
        if (activeCamera == null || mousePositionAction == null)
        {
            if (placementPreviewInstance != null)
            {
                placementPreviewInstance.SetActive(false);
            }
            hasValidPlacement = false;
            return;
        }

        Vector2 mousePosition = mousePositionAction.ReadValue<Vector2>();
        Ray ray = activeCamera.ScreenPointToRay(mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayerMask))
        {
            currentGridPosition = SnapToGrid(hit.point);
            hasValidPlacement = true;

            if (placementPreviewInstance != null)
            {
                placementPreviewInstance.SetActive(true);
                placementPreviewInstance.transform.position = currentGridPosition;
            }
        }
        else
        {
            hasValidPlacement = false;
            if (placementPreviewInstance != null)
            {
                placementPreviewInstance.SetActive(false);
            }
        }
    }

    private Vector3 SnapToGrid(Vector3 worldPosition)
    {
        float snappedX = Mathf.Round(worldPosition.x / gridCellSize) * gridCellSize;
        float snappedZ = Mathf.Round(worldPosition.z / gridCellSize) * gridCellSize;
        return new Vector3(snappedX, worldPosition.y, snappedZ);
    }

    private void OnPlacementPerformed(InputAction.CallbackContext context)
    {
        if (hasValidPlacement && currentAbility != null && currentAbility.IsReady)
        {
            currentAbility.TryActivate(currentGridPosition);
        }
    }

    private void OnDestroy()
    {
        if (placementPreviewInstance != null)
        {
            Destroy(placementPreviewInstance);
        }
    }
}
