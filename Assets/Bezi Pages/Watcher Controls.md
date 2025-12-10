## Control Scheme Overview

The Watcher observes the Runner from a diorama view and deploys ability cards to stop them. Controls support both **Keyboard** and **Gamepad**.

---

## Camera Movement (Prototyped Options)

### Controls (Both Options)

**Keyboard:** W (Top) | A (Left) | S (Back) | D (Right)  
**Gamepad:** RT + Left Stick (directional)

---

### Option A: Instant Switch

**Script:** `/Assets/Scripts/WatcherInputManager.cs`

**Behavior:**

- Instantly teleports between 4 fixed camera GameObjects
- Each position has pre-set rotation
- Immediate response, no transition

**Pros:** Fast, simple  
**Cons:** Jarring, disorienting

---

### Option B: Smooth Navigation ✅ Current Prototype

**Script:** `/Assets/Scripts/WatcherCameraNavigator.cs`

**Behavior:**

- Camera orbits smoothly around diorama to positions
- Always focuses on diorama center (even when stopped)
- Travels shortest circular path
- Speed: 4 movement | 8 rotation

**Scene Setup:**

- `Watcher Camera Rig` + Navigator component
- `Diorama Focus Point` (center target)
- `Camera Targets` (4 position transforms)

**Pros:** Cinematic, maintains spatial awareness, strategic positioning  
**Cons:** Slight travel delay, more complex

---

## Ability Deployment Controls (To Be Implemented)

- Card selection
- Targeting & placement in diorama
- Confirm/cancel actions

---

## Input Action Map

**Action Map:** `WatcherControls` in `Watcher.inputactions`

**Camera Actions (Implemented):**

- `CameraFront`, `CameraLeft`, `CameraBack`, `CameraRight`, `CameraTop`

**Future Actions:**

- Card selection (`SelectCard1-4`, `CycleCards`)
- Cursor movement (`MoveCursor`)
- Placement (`Confirm`, `Cancel`)

---

**Status:** Camera ✅ Complete | Abilities ⚠️ Pending

**Implementation Guide:** Watcher Camera Implementation.md

**Reference:** Phase 2 Plan - Watcher Camera & Input.md | GDD.md
