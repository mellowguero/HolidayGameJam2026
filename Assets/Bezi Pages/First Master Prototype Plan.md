## Prototype Milestone 1: Core Foundation

**Goal:** Get a working test loop where both roles feel playable

**Timeline:** Days 1-2

---

### Phase 1: Runner Movement (Day 1 Morning) ✅ COMPLETE

**Deliverables:**

- ✅ Basic third-person character controller (using StarterAssets.ThirdPersonController)
- ✅ Move, jump, sprint mechanics
- ✅ Simple test level (world floor + goal)
- ✅ Cinemachine third-person camera setup

**What We're Using:**

- `StarterAssets.ThirdPersonController` - Handles all runner movement
- `StarterAssetsInputs` - Input handling
- PlayerArmature with animations
- Existing Cinemachine camera setup

---

### Phase 2: Watcher Scene & Camera System (Day 1 Afternoon) ✅ COMPLETE

**Deliverables:**

- ✅ WatcherScene with diorama placeholder
- ✅ 5 camera viewpoints (Front, Left, Back, Right, Top)
- ✅ Camera switching input system (keyboard + gamepad support)
- ✅ Smooth camera navigation with orbit movement

**What We Built:**

- `WatcherInputManager.cs` - Instant camera switching between positions
- `WatcherCameraNavigator.cs` - Smooth orbit camera movement (current implementation)
- `Watcher.inputactions` - WatcherControls action map with camera inputs
- Scene hierarchy with Camera Positions, Diorama Placeholder, focus point

**Note:** Multi-monitor setup deferred; using smooth camera orbit for prototype testing

---

### Phase 3: First Watcher Ability (Day 2 Morning)

**Deliverables:**

- One working Watcher ability (e.g., spawn falling block)
- Click-to-place interaction
- Hazard kills Runner

**Scripts to Create:**

- `WatcherAbility.cs` - Base class for all abilities
- `FallingBlockAbility.cs` - First concrete ability
- `RunnerHealth.cs` - Death detection

**Prefabs to Create:**

- Falling Block prefab with trigger collision

---

### Phase 4: Game State & Round System (Day 2 Afternoon)

**Deliverables:**

- Role swap between rounds
- Win/lose detection
- Basic UI for round state
- Simple AI Runner for solo testing (optional)

**Scripts to Create:**

- `GameManager.cs` - Round tracking, role assignment
- `PlayerRole.cs` - Enum/data for Runner vs Watcher
- `RoundUI.cs` - Simple text display for state
- `AIRunnerController.cs` - NavMesh-based test AI (optional)

**Flow:**

1. Player 1 = Runner, Player 2 = Watcher
2. Runner reaches goal OR dies
3. Roles swap
4. Repeat for 3 rounds

**AI Runner (Optional):**

- Simple NavMeshAgent that walks toward goal
- No dodging or intelligence - just forward movement
- Toggle between player control and AI mode
- Used for testing Watcher placement without second player
- Uses existing `com.unity.ai.navigation` package

---

## Success Criteria for Prototype 1

✅ You can run as the Runner and reach a goal  
✅ WatcherScene created with diorama view  
✅ Watcher can switch between 5 camera viewpoints  
⚠️ Watcher can spawn 1 hazard that kills the Runner  
⚠️ Roles swap after a round ends  
⚠️ Game tracks who won each round  
⚠️ (Optional) AI Runner can navigate to goal for solo testing  

---

## What We're NOT Building Yet

- Card collection system
- Multiple abilities
- Polish/VFX
- Final art/visuals
- Sound
- Best-of-3 full match logic

---

## Technical Dependencies

**Packages Already Installed:**

- ✅ Input System (1.15.0)
- ✅ Cinemachine (2.10.5)
- ✅ ProBuilder (6.0.8)
- ✅ URP (17.3.0)

**Project Structure:**

```
/Assets
  /Scenes
    - RunnerScene.unity (renamed from scene_test.unity)
    - WatcherScene.unity
  /Scripts
    /Runner
      - RunnerHealth.cs
      - AIRunnerController.cs (optional)
    /Watcher
      - MultiMonitorManager.cs
      - WatcherViewCamera.cs
      - WatcherInput.cs
      - WatcherAbility.cs
      - FallingBlockAbility.cs
    /Core
      - GameManager.cs
      - PlayerRole.cs
      - RoundUI.cs
  /Prefabs
    /Watcher
      - FallingBlock.prefab
  /Materials
    - DioramaMaterial.mat
    - GroundMat.mat
    - HazardMat.mat
  /RenderTextures
    - WatcherView_RT.renderTexture
```

---

## Next Steps After Prototype 1

1. Add 2-3 more Watcher abilities
2. Implement card collection system
3. Build ability card UI for Watcher
4. Create proper level geometry
5. Add timer/scoring
6. Enhance AI Runner with dodging and hazard avoidance (if needed)

---

**Reference:** GDD.md
