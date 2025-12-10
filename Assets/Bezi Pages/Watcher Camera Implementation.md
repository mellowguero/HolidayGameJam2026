## Watcher Camera System Implementation

This document explains how to implement both camera movement options for the Watcher role.

---

## Prerequisites

### Input System Setup

1. Create Input Actions asset: `/Assets/Game_Inputs/Watcher.inputactions`
2. Add action map: `WatcherControls`
3. Add 5 actions:
  - `CameraFront` - Binding: W key, RT + Left Stick Up
  - `CameraLeft` - Binding: A key, RT + Left Stick Left
  - `CameraBack` - Binding: S key, RT + Left Stick Down
  - `CameraRight` - Binding: D key, RT + Left Stick Right
  - `CameraTop` - Binding: Space key
4. Set all actions to Button type
5. Save the asset

### Scene Setup - Common Elements

1. Create parent GameObject: `Camera Targets`
2. Add 4 child empty GameObjects as position markers:
  - `Target_Camera_Top` - Position above diorama
  - `Target_Camera_Left` - Position to left side
  - `Target_Camera_Back` - Position behind
  - `Target_Camera_Right` - Position to right side
3. Position each target where you want the camera view
4. Rotate each target to face the diorama center

---

## Option A: Instant Switch Implementation

**Script:** `WatcherInputManager.cs`

### Scene Setup

1. Create 4 Camera GameObjects under `Camera Positions`:
  - `Front Camera/Camera`
  - `Left Camera/Camera`
  - `Back Camera/Camera`
  - `Right Camera/Camera`
  - `Top Camera/Camera`
2. Position and rotate each camera manually
3. Add `AudioListener` to one camera only
4. Disable all cameras except the starting one

### Script Setup

1. Create empty GameObject: `WatcherInputManager`
2. Add `WatcherInputManager` component
3. Assign references in Inspector:
  - Front Camera → `/Camera Positions/Front Camera/Camera`
  - Left Camera → `/Camera Positions/Left Camera/Camera`
  - Back Camera → `/Camera Positions/Back Camera/Camera`
  - Right Camera → `/Camera Positions/Right Camera/Camera`
  - Top Camera → `/Camera Positions/Top Camera/Camera`
  - Input Actions → `Watcher.inputactions` asset

### How It Works

**Script Logic:**

1. `Awake()`: Finds `WatcherControls` action map and all 5 camera actions
2. `OnEnable()`: Enables actions and subscribes to input events
3. Input callback: Calls `SwitchToCamera(Camera targetCamera)`
4. `SwitchToCamera()`: Disables all cameras, enables target camera

**Key Code:**

```csharp
private void SwitchToCamera(Camera targetCamera)
{
    frontCamera.gameObject.SetActive(false);
    leftCamera.gameObject.SetActive(false);
    backCamera.gameObject.SetActive(false);
    rightCamera.gameObject.SetActive(false);
    topCamera.gameObject.SetActive(false);
    
    targetCamera.gameObject.SetActive(true);
}
```

---

## Option B: Smooth Navigation Implementation

**Script:** `WatcherCameraNavigator.cs`

### Scene Setup

1. Create parent GameObject: `Watcher Camera Rig`
2. Add child GameObject: `Watcher Camera` with Camera component
3. Add `AudioListener` to the camera
4. Create focus point inside diorama parent:
  - Navigate to your diorama GameObject (e.g., `/Diorma Placeholder`)
  - Create child empty GameObject: `Diorama Focus Point`
  - Position at (0,0,0) local - this is the center the camera will look at

### Script Setup

1. Add `WatcherCameraNavigator` component to `Watcher Camera Rig`
2. Assign references in Inspector:
  - Watcher Camera → `/Watcher Camera Rig/Watcher Camera`
  - Board Center → `/Diorma Placeholder/Diorama Focus Point`
  - Front Position → `/Camera Targets/Target_Camera_Top`
  - Left Position → `/Camera Targets/Target_Camera_Left`
  - Back Position → `/Camera Targets/Target_Camera_Back`
  - Right Position → `/Camera Targets/Target_Camera_Right`
  - Top Position → `/Camera Targets/Target_Camera_Top`
  - Movement Speed → `4`
  - Rotation Speed → `8`
  - Input Actions → `Watcher.inputactions` asset

### How It Works

**Initialization:**

1. `Awake()`: Finds action map and all camera actions
2. `Start()`: Sets initial camera position to front target
3. `OnEnable()`: Subscribes input actions to `RequestPosition()`

**Movement Logic:**

1. Input triggers `RequestPosition(Transform target, CameraPosition position)`
2. Sets `desiredTarget` and marks `isMoving = true`
3. `Update()` runs movement if `isMoving`:
  - Calculates distance to target
  - If close (<0.1), snaps to position and stops
  - Otherwise, calculates orbital path via `CalculatePathPosition()`
  - Moves camera along path using `Vector3.MoveTowards()`
4. `Update()` always runs rotation logic:
  - Calculates direction to `boardCenter`
  - Smoothly rotates to look at center using `Quaternion.Slerp()`

**Orbital Path Calculation:**

```csharp
private Vector3 CalculatePathPosition()
{
    // For top camera, move directly (no orbit)
    if (currentPosition == CameraPosition.Top || desiredTarget == topPosition)
        return desiredTarget.position;
    
    // Calculate current and target angles around center
    float currentAngle = Mathf.Atan2(currentPos.z - boardCenter.position.z, 
                                      currentPos.x - boardCenter.position.x);
    float targetAngle = Mathf.Atan2(targetPos.z - boardCenter.position.z, 
                                     targetPos.x - boardCenter.position.x);
    
    // Calculate angle difference and next angle step
    float angleDiff = Mathf.DeltaAngle(currentAngle * Mathf.Rad2Deg, targetAngle * Mathf.Rad2Deg);
    float nextAngle = currentAngle + Mathf.Sign(angleDiff) * movementSpeed * Time.deltaTime;
    
    // Interpolate radius (distance from center)
    float currentRadius = Vector3.Distance(...);
    float targetRadius = Vector3.Distance(...);
    float radius = Mathf.Lerp(currentRadius, targetRadius, Time.deltaTime * movementSpeed);
    
    // Calculate next position on orbital path
    Vector3 nextPosition = new Vector3(
        boardCenter.position.x + Mathf.Cos(nextAngle) * radius,
        Mathf.Lerp(currentPos.y, targetPos.y, Time.deltaTime * movementSpeed),
        boardCenter.position.z + Mathf.Sin(nextAngle) * radius
    );
    
    return nextPosition;
}
```

**Key Features:**

- Camera orbits around `boardCenter` in a circular arc
- Takes shortest path (doesn't go full circle)
- Smoothly interpolates height (Y-axis)
- Always looks at focus point via continuous rotation update
- Top camera bypasses orbital logic (direct movement)

---

## Switching Between Systems

### Enable Option A (Instant Switch)

1. Enable `WatcherInputManager` GameObject
2. Disable `Watcher Camera Rig` GameObject
3. Enable one of the cameras in `Camera Positions`

### Enable Option B (Smooth Navigation)

1. Disable `WatcherInputManager` GameObject
2. Enable `Watcher Camera Rig` GameObject
3. Disable all cameras in `Camera Positions` (only used as reference transforms now)

---

## Tuning Parameters

### Option A

- No tunable parameters - instant response

### Option B

**Movement Speed** (default: 4)

- Lower (1-2): Slow, cinematic camera movement
- Higher (5-8): Fast, responsive camera movement

**Rotation Speed** (default: 8)

- Lower (3-5): Gradual turn toward center
- Higher (10-15): Snappy, immediate focus on center

**Testing Tip:** Adjust speeds in Play Mode to feel the difference, then apply values to the prefab/scene.

---

## Common Issues

### Option A

- **Multiple AudioListeners warning**: Ensure only one camera has AudioListener component
- **Camera not switching**: Check action names match exactly (no spaces)
- **Wrong camera active**: Set starting camera active in scene

### Option B

- **Camera doesn't look at center when stopped**: Ensure rotation logic runs every frame (outside `isMoving` check)
- **Camera takes weird path**: Check `boardCenter` is positioned correctly at diorama center
- **Camera doesn't move**: Verify `Camera Targets` positions are different from starting position
- **Jittery movement**: Lower movement/rotation speeds or use `SmoothDamp` instead of `Lerp`

---

**Reference:** Watcher Controls.md
