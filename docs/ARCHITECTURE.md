%%
Using as inpiration - Issue promt:
Describe responsibilities and connections, linking to the relevant source files instead of repeating their contents:

- **Startup and scene flow:** `GameBootstrapper`, `GameConfig`, `SingletonScriptableObject<T>`, `G`, `InputSelectScene`, `GameManager`, and `Stage1`. Explain Resources-based config loading and scene-assigned manager references.
- **Player and input:** `Player`, `PlayerController`, `PlayerInputController`, `InputSchemeManager`, the mobile UI scripts, and `Resources/Input/ForkedPath_Actions.inputactions`. Cover `Old8Directional`, `New8Directional`, and `Continuous` behavior as implemented.
- **Entities and events:** `Entity`, `EntityConfig`, `Health`, `EntityState`, and `GameEvents`. Trace one example through damage, death, corpse availability, eating, and progression; mention falling and invincibility where they affect this flow.
- **Food and weapons:** `ProgressionManager`, `FoodComboTracker`, `FoodRulesConfig`, `ProgressionConfig`, `AutomaticShooter`, `ProjectilesPattern`, and `ProjectileConfig`. Explain meat/vegetable upgrades, ammo, and downgrade/reset behavior.
- **Level content:** spawn scripts, `HamburgerFightController` and boss phase configs, camera managers/track movement, and the UI/audio/VFX systems that consume game events.
- **Where to edit:** point to `Assets/ForkedPath/Resources/Scriptable Objects`, `Resources/Prefabs`, and `Scenes`. Use concrete examples such as `Game/Game Config.asset`, `Progression/PlayerFoodRulesConfig.asset`, and `Progression/PlayerProgression.asset`. Give a brief recipe for changing food thresholds or a projectile pattern and tracing affected references.
%%

[back to the landing page](README.md)

## Scenes

Currently used scnenes flow is
`Input Select` -> `Stage 1 Refine`

### Input Select
*Input Select Scene* object with `InputSelectScene.cs` component controls the transition to the stated next scene.

A global static variable of the `MobileUIManager.cs` is set depending on which button was pressed

### Stage 1 Refine

Most recent scene with the refined graphics and gameplay data.

## Game Flow

The game is controlled manly by three mechanisms:
1. **Stage progression**
2. **Events bus**
3. **Game Systems**
### Stage progression

`Stage1.cs` controls the levels opening and closing graphics. Closing graphics show the button that allows to reset the level

### Events bus

Exits in the scene as a singleton.

`GameEvents.cs` defines events that happen in the game.

There are two groups of events:
1. **world events**, that maintain the communication between various systems, i a VFX effects
2. **game events**, that 

### Game Systems

#### Player
Player basically consists of
`Player.cs` + current `PlayerController.cs`

`Player.cs` controls the current player's state including how many lives there are left and what's going to happen on the player's character's death

`PlayerController.cs` is a variation of an Entity that can be controlled with inputs. It connects to the `GameEvents.cs`

#### Entities
Enties are built by specific config - recepie for an entity creation, that specify prefab, ids, dictionaries for fx. The project employs configs inheritance.

Entity Spawn, that spawns entites by a specific config
#### Input

There is a `GameObject` holding Unity's PlayerInput as well as a custom `PlayerInputController.cs`( and a `DebugInputController.cs`), which listens to inputs and serves as a source of truth for player components.



There are currently 3 Input schemes:
1) `Old8Directional`
2) `New8Directional`
3) `Continuous`

#### Game Data
In the scene: `G.cs`

Defined Content: GameConfig - %%hint which folders?%%
#### UI
`MobileUI`

`ChangeInputUI`, `EndingUI`, `TransitionUI`

## Level content

Enemy spawns,

Grids

Canvas UI system

## Assets Structure

%%Maybe it should go on top or to the main README%%