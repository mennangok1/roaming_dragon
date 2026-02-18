# Roaming Dragon

A 2D action-platformer built in Unity (URP).  
This repo includes a playable main menu + one level, with room transitions, combat, traps, checkpoints, health UI, audio controls, and player-movement feel systems (coyote time, jump buffering, wall movement, and corner correction).

## Table of Contents
- Overview
- Tech Stack
- Project Structure
- Requirements
- Quick Start
- Controls
- Gameplay Systems
- Tunable Gameplay Parameters
- Build a Playable Game
- Troubleshooting
- Roadmap Ideas

## Overview
You control a side-scrolling character through rooms filled with enemies and hazards.

Current implemented highlights:
- Smooth horizontal movement with separate ground/air accel-decel.
- Ground jump + wall jump + wall slide.
- Jump forgiveness systems:
  - Coyote time
  - Jump input buffer
  - Corner correction (head-bonk forgiveness)
- Ranged attack (fireball pool) with recoil.
- Enemy set: Knight, Arrow Trap, Fire Trap, Spike Head, Ground Spikes.
- Checkpoints + respawn flow.
- Room activation/deactivation and camera room transitions.
- UI for pause, game over, and menu selection.
- Sound and music volume cycling persisted via `PlayerPrefs`.

## Tech Stack
- Engine: Unity 6.2
- Language: C#
- Input: Unity Input + direct keyboard polling in scripts

## Project Structure
```text
Assets/
  Levels/                 # Scenes (_MainMenu, Level1)
  Scripts/
    Player/               # Movement, attack, projectile, respawn
    Enemies/              # Traps + enemy behaviors
    Enemies/Knight/       # Knight AI/state/attack/collision/health
    Health/               # Health, healthbar, heart pickup
    Rooms/                # Door transitions + room reset/activation
    Core/                 # Camera + sound manager
    UI/                   # Pause/game over/menu selection
  Prefabs/                # Player, enemies, projectiles, room elements
  Animations/             # Controllers + clips
  Audio/                  # SFX + music
ProjectSettings/
Packages/
```

## Requirements
- Unity Hub
- Unity Editor `6000.2.12f1`
- macOS/Windows with standard Unity 2D support

## Quick Start
1. Clone the repo:
   ```bash
   git clone roaming_dragon
   cd roaming_dragon
   ```
2. Open Unity Hub -> `Open` -> select this folder.
3. Install/open Unity Editor version `6000.2.12f1` when prompted.
4. Open scene:
   - `Assets/Levels/_MainMenu.unity` (recommended entry point)
5. Press Play in the Unity Editor.

Build scenes are already configured in `ProjectSettings/EditorBuildSettings.asset`:
- `Assets/Levels/_MainMenu.unity`
- `Assets/Levels/Level1.unity`

## Controls
### In-Game
- Move: `Left/Right Arrow`
- Jump: `Space`
- Attack (fireball): `Q`
- Pause toggle: `Esc`

### Menu / UI
- Navigate options: `Up/Down Arrow`
- Confirm option: `Enter` or `E`

### Dev/Test Hotkey
- `E` triggers `TakeDamage(1)` in `Health` for quick testing.

## Gameplay Systems
### Player Movement (`Assets/Scripts/Player/Player.cs`)
- Ground/air acceleration and deceleration.
- Wall jump with lockout timer and gravity multiplier.
- Wall slide with tunable gravity scale.
- Coyote time and jump buffering.
- Corner correction system to reduce frustrating near-corner bonks.

### Combat
- Player attacks via pooled fireballs:
  - `Assets/Scripts/Player/PlayerAttack.cs`
  - `Assets/Scripts/Player/Projectile.cs`
- Knight enemy state-driven behavior:
  - `Assets/Scripts/Enemies/Knight/KnightEnemy.cs`
  - `Assets/Scripts/Enemies/Knight/KnightAttack.cs`
  - `Assets/Scripts/Enemies/Knight/EnemyHealth.cs`

### Hazards
- Arrow trap projectile launcher (`ArrowTrap.cs`, `EnemyProjectile.cs`)
- Fire trap with warning flashes (`FireTrap.cs`)
- Spike head directional lunge (`SpikeHead.cs`)

### Health / Respawn / Checkpoints
- Player health + invincibility flashes (`Health.cs`)
- Checkpoint activation and room-aware respawn (`PlayerRespawn.cs`)
- Health UI fill bars (`Healthbar.cs`)
- Heart pickup (`Assets/Scripts/Health/HeartCollectible.cs`)

### Room and Camera Flow
- Door transitions and room activation toggling:
  - `Assets/Scripts/Rooms/Door.cs`
  - `Assets/Scripts/Rooms/Room.cs`
- Camera follows room anchor x-position:
  - `Assets/Scripts/Core/CameraController.cs`

### Audio
- Global `SoundManager` singleton:
  - SFX one-shots
  - music/sfx volume cycling
  - volume persistence in `PlayerPrefs`

## Tunable Gameplay Parameters
Most balancing values are `[SerializeField]` fields in scripts and can be changed in the Unity Inspector.

### Player feel (`Player.cs`)
- Movement:
  - `xSpeed`, `groundAcceleration`, `airAcceleration`
  - `groundDeceleration`, `airDeceleration`
- Jump:
  - `jumpForce`
  - `coyoteTime`
  - `jumpBufferDuration`
  - `jumpBufferCheckInterval` (code field)
- Wall systems:
  - `wallJumpForceX`, `wallJumpForceY`
  - `wallJumpLockTime`
  - `wallJumpGravityMultiplier`
  - `wallSlideGravityScale`
- Gravity shaping:
  - `defaultGravity`
  - `upwardGravityMultiplier`
  - `downwardGravityMultiplier`
- Corner correction:
  - `enableCornerCorrection`
  - `cornerCorrectionDistance`
  - `cornerCeilingCheckDistance`
  - `cornerProbeInset`
  - `cornerPostCorrectionClearance`
  - `cornerCorrectionCooldown`

### Combat and enemies
- Player attack cooldown/recoil: `PlayerAttack.cs`
- Projectile speed/lifetime/damage: `Projectile.cs`
- Knight patrol, attack, dizzy, recoil params: `KnightEnemy.cs`, `KnightAttack.cs`
- Trap timings/damage: `FireTrap.cs`, `ArrowTrap.cs`, `SpikeHead.cs`

### Room pacing
- Enemy reset delay per room: `Room.cs -> resetCooldown`

## Build a Playable Game
1. Open Unity -> `File` -> `Build Profiles` (or `Build Settings` depending on editor UI).
2. Ensure platform is selected (e.g., PC, Mac & Linux Standalone).
3. Confirm scenes are in build list:
   - `_MainMenu`
   - `Level1`
4. Click `Build` (or `Build and Run`).

## Troubleshooting
### Project opens with many compile errors
- Confirm Unity version is exactly `6000.2.12f1`.
- Let Unity finish package import and script compilation.

### Inputs do not work
- Check the Game view has focus.
- Verify keyboard controls above.
- Confirm scene is `_MainMenu` or `Level1`.

### No audio
- Ensure an active `SoundManager` exists in the loaded scene hierarchy.
- Check AudioListener (usually on Camera).
- Use menu volume options to cycle SFX/music levels.

### Enemies or room transitions behave unexpectedly
- Verify layer/tag setup on player, walls, ground, enemy hitboxes, and checkpoint colliders.
- Confirm `Door` references (`previousRoom`, `nextRoom`, `cam`, `player`) are assigned in Inspector.

## Roadmap Ideas
- Add more levels and biomes.
- Add save/load for checkpoint progress between sessions.
- Improve enemy variety and boss encounters.
- Add gamepad input mappings.
- Add automated playmode/unit tests.
