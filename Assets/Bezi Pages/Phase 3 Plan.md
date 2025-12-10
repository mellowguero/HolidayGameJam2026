**Status:** ✅ Complete

**Goal:** First Watcher ability with grid-based placement and Runner interaction

---

## Implementation Summary

**Core Systems:**

- Ability base class with cooldown lifecycle
- Grid-based targeting (12x12 grid on 3x3 diorama)
- Real-time placement with visual preview
- Hazard collision detection and Runner death

**First Ability: Falling Block**

- Click to place at grid-snapped position
- Spawns 2 units above target
- Falls with gravity, kills Runner on contact
- Auto-destroys after 10 seconds
- 1-second cooldown between uses

---

## Scripts Created

`WatcherAbilityBase.cs`

- Abstract base for all Watcher abilities
- Cooldown system and activation lifecycle
- Template for future abilities

`FallingBlockAbility.cs`

- Spawns falling hazard block prefab
- Configurable spawn height, lifetime, cooldown

`WatcherPlacementGrid.cs`

- Raycasts from camera to ground plane
- Snaps to 0.25 unit grid cells (12x12 grid)
- Shows placement preview at cursor
- Uses Input System (MoveCursor, Confirm actions)

`RunnerHealth.cs` (Updated)

- Added hazard collision detection via "Hazard" tag
- Triggers death on contact with hazards

---

## Assets & Setup

**GameObjects:**

- `FallingBlock` - Red cube (0.25×0.5×0.25), Rigidbody, trigger collider, "Hazard" tag
- `PlacementGhost` - Green cube (0.25×0.5×0.25), placement preview
- `TestRunner` - Blue capsule with RunnerHealth for testing

**Materials:**

- `FallingBlockMaterial.mat` (red)
- `PlacementGhostMaterial.mat` (green, transparent)
- `RunnerMaterial.mat` (blue)

**Configuration:**

- Grid: 12×12 cells (0.25 unit spacing)
- Spawn height: 2 units
- Block lifetime: 10 seconds
- Cooldown: 1 second

---

## Gameplay Flow

1. Move mouse over diorama → green preview appears at grid-snapped position
2. Click to place → red falling block spawns 2 units above
3. Block falls with gravity
4. Runner dies on contact with block
5. Block auto-destroys after 10 seconds

---

## Next Steps

**Manual Setup Required:**

1. Create prefabs: Drag `FallingBlock` and `PlacementGhost` to `/Assets/Prefabs/`
2. Delete scene instances
3. Re-assign prefab references in `WatcherInputManager` component

**Testing:**

- Use keys 1-5 to switch camera views
- Move mouse to see placement preview
- Click to spawn blocks
- Move TestRunner under blocks to verify death

---

**Reference:** GDD.md
