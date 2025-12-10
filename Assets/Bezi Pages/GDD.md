## Game Overview

**Title:** WATCHER vs RUNNER

**Genre:** 2-player asymmetrical competitive action game

**Platform:** PC (local multiplayer only)

**Core Pitch:** A competitive diorama game where you survive one round, then use the tools your opponent collected to hunt them in the next.

---

## Core Concept

Players take turns in two distinct roles across a best-of-3 match format:

**The Runner** — A third-person character trapped in a dangerous simulation, racing to reach the goal while avoiding hazards and collecting power cards.

**The Watcher** — An overhead god-like observer controlling the environment from a top-down diorama view, using ability cards to stop the Runner.

**The Twist:** The Runner collects cards during their run that become the Watcher's powers in the next round. You unknowingly build the weapons used against you.

---

## Game Loop

### Runner Phase

- Spawn in the level
- Navigate obstacles using movement, jumping, and dashing
- Collect ability cards (automatic pickups)
- Survive hazards deployed by the Watcher
- Reach the goal to win OR die and lose

### Watcher Phase

- Observe from top-down diorama camera
- Deploy powers using collected cards
- React in real-time to Runner movement
- Kill the Runner or block their progress to win

---

## Win Conditions

**Primary (MVP):**

- Runner reaches goal = Runner wins the round
- Runner dies = Watcher wins the round
- Best of 3 rounds determines match winner

**Optional Variants:**

- Fastest cumulative time
- Hybrid points system

---

## Power System

### Card Mechanics

- Watcher uses randomly drawn ability cards
- Cards are collected by the Runner during their run
- Collected cards become the Watcher's arsenal in the next round

### Card Categories (6-8 total for jam scope)

- **Environment:** walls, falling blocks, rotating floors
- **Traps:** spikes, lasers, crushers
- **Enemies:** chasers, turrets
- **Modifiers:** gravity shift, slow-mo, blackout

---

## Runner Abilities (MVP)

- Move
- Jump
- Dash/Dodge
- No combat mechanics
- Automatic card pickup

**Reference:** Runner Controls.md

---

## Watcher Abilities (MVP)

- Navigate between 4 camera views with smooth orbital movement
- Deploy ability cards
- Target placement in diorama
- Real-time reaction to Runner movement

**Reference:** Watcher Controls.md

---

## Watcher's Game Board

**Diorama Specifications:** ✅

- Size: 3×3 world units playing field
- Grid: 12×12 placement cells (0.25 unit spacing)
- 144 total placement positions
- Elevated platform design with clear boundaries
- Position: (0, 0.64, 0) with 0.2 unit height
- Visual boundary markers (corner posts and edge lines)

**Camera Views:**

- 5 fixed orbital positions (Front, Left, Back, Right, Top)
- Smooth transitions between views
- Consistent focus on diorama center
- Switch via number keys 1-5 or gamepad

**Placement System:**

- Mouse raycast to ground plane
- Automatic grid-snapping
- Visual preview ghost (green cube)
- Left-click to confirm placement
- Works from all camera angles

**Hazard Behavior:**

- Spawn 2 units above grid position
- Physics-based falling
- Trigger collision detection
- Auto-cleanup after 10 seconds
- 1-second cooldown between uses

**Reference:** Watchers Game Board.md

---

## Technical Implementation

### Camera System

- **Runner:** Third-person perspective
- **Watcher:** Smooth orbital navigation around diorama with 4 viewing positions
  - Continuous focus on diorama center
  - WASD directional control (Top, Left, Back, Right)
  - Gamepad support (RT + Left Stick)

### Input

- Local 2-player split input (keyboard + mouse/controller)
- No online multiplayer

### Visual Style (To Be Finalized)

- Diorama/toy-box aesthetic
- Options: dark experiment, brutalist simulation, abstract digital terrarium

---

## MVP Feature Lock (Must Ship)

- ✅ 1 playable level
- ✅ Runner controller (move, jump, dash)
- ✅ Watcher diorama camera
- ✅ 6-8 Watcher ability cards
- ✅ Goal object
- ✅ Death state
- ✅ Role swap per round
- ✅ Best-of-3 match logic
- ✅ Restart match button

---

## Stretch Goals

- Multiple levels
- Runner counter-tools
- Card drafting system
- Enhanced VFX
- Replay/spectator mode

---

## Why This Game Works

- Strong asymmetrical tension between roles
- High spectator readability
- Psychological meta-strategy (collecting cards that hurt you later)
- Unique diorama framing
- Clear win/loss conditions
- Perfect for short, repeatable matches

---

**Reference:** Game Pitch Working File.md

# Plans

First Master Prototype Plan.md

Phase 2 Plan - Watcher Camera & Input.md - ✅

Phase 3 Plan.md - ✅
