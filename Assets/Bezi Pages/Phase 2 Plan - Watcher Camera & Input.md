**Status:** 🔄 In Progress

**Goal:** Create Watcher's control room scene with diorama view of Runner's world

---

## Current Implementation

### WatcherScene Hierarchy

```
WatcherCamera
Directional Light
Watcher (character model)
Plane (ground visualization)
Plane (1) (ground visualization)
WatcherInputManager
Diorama Placeholder
Camera Positions
  ├── Front Camera
  ├── Left Camera
  ├── Back Camera
  ├── Right Camera
  └── Top Camera
```

### Key Components

**Diorama Placeholder**

- Represents the Runner's world
- Will contain a masked view of RunnerScene focused on the Runner
- Uses custom shader (by Macy) that masks content based on parent volume

**Camera Positions (5 fixed angles)**

- Front, Left, Back, Right, Top
- Watcher can live-switch between them at will
- Only one camera active at a time
- Camera settings TBD
- **Switching implemented via WatcherInputManager script**
  - Uses Previous/Next input actions from InputSystem_Actions
  - Cycles through cameras (wraps around at ends)
  - Only active camera is enabled at any time

**Ground Planes**

- Visualization placeholder for the Watcher's room environment

---

## Core Concept

The Watcher observes the Runner's world through a diorama display in the center of their scene. They can switch between 5 fixed camera angles to track the Runner from different perspectives.

---

## Implementation Status

✅ WatcherScene created  
✅ Diorama Placeholder added  
✅ 5 Camera Positions created (Front, Left, Back, Right, Top)  
✅ Ground planes added for visualization  
✅ WatcherInputManager in scene  
✅ Camera switching logic - complete (Previous/Next inputs)  
⚠️ Diorama masking shader integration - pending (Macy)  
⚠️ RunnerScene instance in diorama - pending  
⚠️ Camera settings finalization - pending  

---

## Next Steps

- Test camera switching in play mode (use Previous/Next keys - check Input Actions asset for bindings)
- Integrate Macy's masking shader with Diorama Placeholder
- Create/instantiate RunnerScene content within diorama
- Configure camera perspectives and settings
- Fine-tune camera transitions (add smoothing if needed)

---

## Control Scheme

Full Watcher control layout documented in Watcher Controls.md

**Current Phase Focus:**

- Camera switching (Q/E for keyboard, LB/RB for gamepad)

**Next Phase:**

- Card selection inputs
- Cursor targeting system
- Confirm/cancel actions

---

**Reference:** GDD.md
