## Overview

Diorama-style playing field viewed from orbital camera positions where the Watcher deploys real-time hazards against the Runner.

---

## Board Specifications

**Physical Dimensions:**

- Size: 3×3 world units (X/Z axes)
- Height: 0.2 units (raised platform)
- Position: Centered at (0, 0.64, 0)

**Grid System:**

- 12×12 grid cells
- 0.25×0.25 unit cell size
- 144 total placement positions
- Automatic grid-snap alignment

---

## Camera System

5 fixed orbital positions with smooth transitions, continuous diorama focus, and keyboard/gamepad support.

**Reference:** Watcher Controls.md

---

## Interaction & Placement

Mouse raycast targeting with automatic grid-snapping, visual preview ghost, and click-to-confirm placement. Hazards spawn 2 units above grid position, fall with physics, and auto-cleanup after 10 seconds with 1-second cooldown.

**Reference:** Phase 3 Plan.md · Watcher Controls.md

---

## Technical Implementation

Grid-based placement system using `WatcherPlacementGrid.cs` and `WatcherAbilityBase.cs` with raycast targeting, trigger collision detection, and tag-based hazard interaction.

**Reference:** Phase 3 Plan.md

---

## Design Intent

God-like oversight perspective with strategic grid-based planning and real-time reactive gameplay. Creates asymmetric information tension between Watcher (full board visibility) and Runner (ground-level first-person view).

---

**Reference:** GDD.md · Phase 3 Plan.md
