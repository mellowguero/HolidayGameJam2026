# Bezi Game Jam Pitch — **WATCHER vs RUNNER**

## High Concept

A 2-player asymmetrical competitive Unity game where players **take turns** being:

- **The Runner** — trapped inside a dangerous world, racing to the goal
- **The Watcher** — an overhead god-like manipulator trying to stop them

**Match Format:** Best of 3 rounds. Players swap roles each round.

---

## Core Fantasy

- The Runner is a subject inside a hostile simulation.
- The Watcher observes from above like a **diorama**, a puppet master controlling hazards.
- The twist: **the Runner collects the cards that power the Watcher next round.**

You unknowingly build the weapons used against you.

---

## Core Game Loop

### Runner

- Spawn → Move → Jump/Dodge → Survive hazards → Reach the goal → Win
- Die → Lose

### Watcher

- Observe from top-down diorama
- Deploy powers using cards
- React in real time to the Runner
- Kill or permanently block progress

---

## Win Conditions (Flexible)

**Primary (MVP):**

- Runner reaches goal = Runner wins
- Runner dies = Watcher wins

**Optional Variants:**

- Fastest total Runner time across rounds
- Hybrid points system (speed + kills)

---

## Watcher Power System (Card-Based)

- Watcher uses **randomly drawn ability cards**
- Cards are **collected by the Runner during their run**
- Collected cards become **Watcher powers next round**

### Card Categories (Examples)

- Environment: walls, falling blocks, rotating floors
- Traps: spikes, lasers, crushers
- Enemies: chasers, turrets
- Modifiers: gravity shift, slow-mo, blackout

**Jam Scope:** 6–8 total cards max

---

## Runner Abilities (MVP)

- Move
- Jump
- Dash / Dodge
- No combat
- No inventory beyond automatic card pickups

---

## Tone & Visual Style (Open)

- Diorama / toy-box
- Dark experiment
- Brutalist simulation
- Abstract digital terrarium

Tone can be finalized during production.

---

## Technical Scope (Jam-Safe)

- **Local 2-player only (no online)**
- Two cameras:
  - Runner = third-person
  - Watcher = orthographic top-down diorama
- Input split: keyboard + mouse/controller

---

## Why This Game Works

- Strong asymmetrical tension
- High spectator readability
- Psychological meta-strategy
- Unique diorama framing
- Extremely clear win/loss condition
- Perfect for short, repeatable matches

---

## MVP Feature Lock (Must Ship)

- 1 playable level
- Runner controller
- Watcher diorama camera
- 6–8 watcher cards
- Goal object
- Death state
- Role swap per round
- Best-of-3 match logic
- Restart match button

---

## Stretch Goals (If Time Exists)

- Multiple levels
- Runner counter-tools
- Card drafting instead of RNG
- More expressive VFX
- Replay or spectator mode

---

## One-Sentence Pitch

*A competitive diorama game where you survive one round, then use the tools your opponent collected to hunt them in the next.*
