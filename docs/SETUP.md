[Back to the landing page](../README.md)

# Setup

Use **Unity 6.3, editor version `6000.3.25f1`**, as recorded in [ProjectVersion.txt](../ProjectSettings/ProjectVersion.txt). The connected Editor was also running this version when the documentation was checked.

## Opening the project

1. Install Unity Hub and editor **6000.3.25f1**. Include **Web Build Support** if you plan to build the browser version.
2. Install Git, then clone the repository:

   ```sh
   git clone https://github.com/IvanPavlov7007/forked-path.git
   ```

3. In Unity Hub, add the cloned project directory containing `Assets`, `Packages`, and `ProjectSettings`, and open it with that editor version.
4. Let Unity import assets and restore packages. The project uses Git-based packages, so Git must be available to Unity and the package hosts must be reachable.
5. Check the Console for import or compilation errors before entering Play mode.

### Dependencies

[manifest.json](../Packages/manifest.json) declares the packages; [packages-lock.json](../Packages/packages-lock.json) records their resolved versions and Git revisions. Keep both files in version control.

| Dependency | Role and location |
| --- | --- |
| URP **17.3.0** | Rendering; restored by Package Manager. |
| Input System **1.20.0** | Player actions and UI input; restored by Package Manager. |
| Cinemachine | Managed cameras; restored through the cinematic feature's dependencies. |
| SoftMask for uGUI | UI masking; restored from the Git URL in the manifest. |
| Hot Reload **1.13.11** | Editor iteration tool; restored from Git and governed by its own EULA. |
| Pixelplacement Surge | Singletons and tweening; bundled in `Assets/Pixelplacement/Surge`. |
| UniRx **6.2.2**, according to its bundled readme | Reactive utilities; bundled in `Assets/Plugins/UniRx`. |
| Odin Inspector / Validator | Inspector tooling and serialization-related assemblies; bundled in `Assets/Plugins/Sirenix`. Requires the appropriate vendor license for use. |

The root GPL license does not grant access to commercial tools. See [Credits](CREDITS.md) and [License](../LICENSE.md) before redistributing the source or bundled assets.

## Building

The project currently targets **WebGL** in the connected Editor. The [published game](https://ivanpavlov.itch.io/swine-dine-the-forked-path) is listed as HTML5; that release does not establish that the current checkout has passed a fresh build.

### Browser build

1. Open **File → Build Profiles** and select the **Web / WebGL** platform. Install its build-support module if Unity asks for it, then switch to that platform.
2. Check that the enabled scene list is in this order:

   | Build index | Scene |
   | --- | --- |
   | 0 | [Input Select](../Assets/ForkedPath/Scenes/Input%20Select.unity) |
   | 1 | [Stage 1 Refine](../Assets/ForkedPath/Scenes/Stage%201%20Refine.unity) |

   This is the order in [EditorBuildSettings.asset](../ProjectSettings/EditorBuildSettings.asset). Keep draft/test scenes out of the release scene list.

3. Choose **Build And Run** and use an output folder such as `Builds/WebGL`. Build outputs are ignored by Git.
4. Use Unity's local server to open the result in a browser; opening the generated `index.html` directly as a file is not the normal test workflow.
5. Run the checklist under [Launching](#launching). Publishing or updating the itch.io release is tracked separately in [issue #3](https://github.com/IvanPavlov7007/forked-path/issues/3).

Unity's [Web development and publishing guide](https://docs.unity.com/en-us/engine/6000.3/manual/platform-specific/webgl/intro/gettingstarted) explains the local-server workflow and deployment requirements.

### Build incrementor

[BuildIncrementor.cs](../Assets/CommonScripts/Editor/BuildIncrementor.cs) runs **before** a build. It creates or updates [Build.asset](../Assets/Resources/Build.asset), increments `BuildNumber`, and sets `PlayerSettings.bundleVersion` to `Version.BuildNumber`.

Because this happens before compilation/build completion, a failed build can still consume a number. Check changes to the build asset and project settings after building.

[BuildDisplayer.cs](../Assets/CommonScripts/Build/BuildDisplayer.cs), used in `Input Select`, loads the same asset from Resources and displays the version. The incrementor is an Editor build hook; the displayer is the scene component.

## Launching

Open [Input Select](../Assets/ForkedPath/Scenes/Input%20Select.unity), enter Play mode, and choose keyboard or mobile input. The selection screen preloads `Stage 1 Refine` and activates it after the selection.

You can open `Stage 1 Refine` directly while working on gameplay, but this skips the input-selection screen. The other scenes are draft/test content; see [Architecture → Scenes](ARCHITECTURE.md#scenes).

### Mobile UI in the Editor

The current `Stage 1 Refine` scene overrides `MobileUIManager` with **Debug Force Mobile UI Active enabled** and **Start With Mobile UI Active disabled**. In the Editor, this forces the virtual controls off even after choosing mobile input.

To check the mobile selection flow, disable the debug override on the scene's `MobileUIManager` before playing. To work directly in the gameplay scene with touch controls visible, keep the override enabled and enable its start-state checkbox. Review/revert those scene changes when finished. The override is inside `UNITY_EDITOR`; it does not run in the browser build.

### Smoke checklist

Run this in both Editor Play mode and a fresh browser build. For each run, record the commit, Unity version, browser/device, input scheme, and result.

- [ ] **Input selection:** keyboard opens gameplay without virtual sticks; mobile shows the appropriate virtual controls, with the Editor override accounted for.
- [ ] **Movement and firing:** WASD/arrows move; X/left mouse fires; Mono uses movement to aim; Bi uses separate aim. Cycle all three schemes and check the eight-directional lock toggle.
- [ ] **Eating:** defeat an edible enemy, stop moving and firing within reach, and wait about half a second. Confirm it is consumed and the food display changes.
- [ ] **Progression:** collect one food type for an upgrade, try changing diet, and check healing/reset and ammo-based downgrade behavior.
- [ ] **Death and restart:** take damage, die, check respawn/lives, exhaust the remaining lives, and use the game-over restart button.
- [ ] **Boss and ending:** reach the boss trigger, check phase transitions, defeat it, and confirm the ending UI appears.

### Verification record

Checked on **2026-10-02**, against documentation-draft commit `9aa1f3c`, using the existing Windows checkout and the connected Unity Editor:

| Check | Result |
| --- | --- |
| Editor version and active platform | `6000.3.25f1`, WebGL. |
| Enabled build scenes | `Input Select` → `Stage 1 Refine`. |
| Gameplay-scene input asset and notification mode | `ForkedPath_Actions`, `Player` map, `SendMessages`. |
| Scheme display data and mobile override | Read from the loaded scene; override described above. |
| Food rules and progression references | Both assigned to the expected assets under `Resources/Scriptable Objects/Progression`. |
| Current Console error/exception query | No entries returned at the time of inspection. |
| Clean-checkout import, full gameplay checklist, and fresh WebGL build | **Not run during this documentation pass.** Current descriptions were checked against source and scene configuration; these runtime checks remain to be performed. |

### Keeping the checkout clean

Commit source, asset `.meta` files, `Packages`, and `ProjectSettings`. Keep `Library`, `Temp`, `Logs`, generated solution/project files, and build outputs out of version control. Use Unity to move or rename assets so their references stay intact.
