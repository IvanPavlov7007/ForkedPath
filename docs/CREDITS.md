[Back to the landing page](../README.md)

# Credits

## Game

**Swine & Dine: The Forked Path** by [IvanPavlov](https://ivanpavlov.itch.io/), built with Unity. The project is inspired by **Pocky & Rocky** and **Elfazar's Hat (UFO 50)**.

The [published itch.io page](https://ivanpavlov.itch.io/swine-dine-the-forked-path) provides the game credit and icon sources below. It identifies the prototype as AI-assisted for code. It does not list separate artwork, music, or sound-effect creators; those credits should be extended if externally sourced material is present.

## Icons

| Asset | Creator/source |
| --- | --- |
| [Salad](https://www.flaticon.com/free-icon/salad_135715) | **Smashicons**, via Flaticon; credited on the game page. |
| [Meat](https://www.flaticon.com/free-icon/meat_2224259) | **Freepik**, via Flaticon; credited on the game page. |
| [Keyboard](https://www.flaticon.com/free-icon/keyboard_2905126) | Flaticon source credited on the game page; creator could not be verified from the source page during this review. |
| [Touch](https://www.flaticon.com/free-icon/touch_4121823) | Flaticon source credited on the game page; creator could not be verified from the source page during this review. |

The salad/meat pages state that attribution is required for their free license. Retain the original download licenses and attribution for the versions actually used. These icons are not automatically relicensed under the project's GPL license. The keyboard/touch textures are under [Sprites/UI/Mobile](../Assets/ForkedPath/Resources/Sprites/UI/Mobile/).

## Fonts

| Font | Credit and license |
| --- | --- |
| [Caveat Brush](../Assets/ForkedPath/Resources/Fonts/CaveatBrush-Regular.ttf) | Font metadata credits **Google Inc. (2015)**, **Creative Lab NY**, and designer **Pablo Impallari**. Licensed under **SIL OFL 1.1**. The [local notice](licenses/CaveatBrush-OFL.txt) was retrieved from [Google Fonts](https://github.com/google/fonts/blob/main/ofl/caveatbrush/OFL.txt); its copyright matches the bundled font metadata. |
| Liberation Sans, bundled with TextMesh Pro | Copyright **Google Corporation (2010)** and **Red Hat, Inc. (2012)**; **SIL OFL 1.1**. Preserve the [bundled license](../Assets/TextMesh%20Pro/Fonts/LiberationSans%20-%20OFL.txt). |

## Tools and libraries

| Component | Credit / terms | Notice or source |
| --- | --- | --- |
| Pixelplacement **Surge** | **Bob Berkebile**, copyright 2017; **MIT**. | [Bundled license](../Assets/Pixelplacement/Surge/License.txt). |
| **UniRx** | **Yoshifumi Kawai / neuecc**; **MIT**, according to the bundled 6.2.2 readme. | [Bundled readme](../Assets/Plugins/UniRx/ReadMe.txt); [local MIT notice](licenses/UniRx-MIT.txt) copied from the upstream license linked by that readme. Preserve any additional notices in borrowed code. |
| **SoftMask for uGUI** | **mob-sakai**, copyright 2018–2024 in the resolved package; **MIT**. | [Local notice](licenses/SoftMask-MIT.txt), copied from the currently resolved package; [upstream](https://github.com/mob-sakai/SoftMaskForUGUI). |
| **Odin Inspector / Validator** | **Sirenix**; commercial vendor terms. | [Bundled readme](../Assets/Plugins/Sirenix/Readme.txt) and [Odin EULA](https://odininspector.com/eula). The notice below does not license Odin itself. |
| **Bootstrap icons inside Odin** | **Twitter, Inc. and The Bootstrap Authors**, copyright 2011–2018; **MIT**. | [Bundled notice](../Assets/Plugins/Sirenix/Odin%20Inspector/Assets/Editor/Bootstrap%20License.txt). |
| **Hot Reload for Unity** | **The Naughty Cult Ltd.**; EULA in the resolved 1.13.11 package. | Restored by [manifest.json](../Packages/manifest.json). The current package notice limits reproduction/distribution; consult the installed package's `LICENSE.md` before sharing it. |
| **Unity**, URP, Input System, Cinemachine, TextMesh Pro/uGUI, and other Unity packages | **Unity Technologies** and each package's credited contributors. Terms vary by package. | [Manifest](../Packages/manifest.json), [lockfile](../Packages/packages-lock.json), and each resolved package's `LICENSE`/`Third Party Notices` files. |
| **EmojiOne sample sprites** | **EmojiOne**, included with TextMesh Pro. | [Bundled attribution](../Assets/TextMesh%20Pro/Sprites/EmojiOne%20Attribution.txt) points to vendor terms but does not include a full grant; verify the applicable terms if retaining or shipping the sample sprites. |
| **Sprite Blending Layers** | Folder identifies **PxlDev**; license not recorded locally. | [Bundled documentation](../Assets/PxlDev/Sprite%20Blending%20Layers/documentation.txt). Original source/redistribution terms need confirmation. |
| **B83 helpers** | Bundled under `PluginMaster/B83`; authorship/license not recorded locally. | [Source folder](../Assets/PluginMaster/B83/). Confirm provenance before treating this code as GPL-covered project code. |

This is an inventory of known bundled/restored dependencies, not a statement that every listed component ships in the browser game. Editor tools, sample assets, and unused packages may be excluded from a build. Keep the notices for the exact versions actually redistributed.

## Documentation media

Documentation and promotional media use existing project material:

- `media/gameplay.jpg` is copied unchanged from `Recordings/Image Sequence_005_0000.jpg`. Recordings are ignored by Git; the documentation copy is included separately so the README image works after cloning.
- `media/broccoli.png` is Unity's preview of the existing [Brocolli Variant prefab](../Assets/ForkedPath/Resources/Prefabs/Characters/Brocolli%20Variant.prefab), preserving the game's character design.
- The logo comes from [Promo/LOGO_380.png](../Promo/LOGO_380.png).
- The README's [banner](../media/banner.png) lays out the existing Porky sprite with the project's **Caveat Brush** font; it introduces no replacement character art.
- The [diet guide](../media/diet-guide.png) uses the game's existing meat, salad, and heart sprites. The meat and salad icon credits above apply to this graphic too.
- [Kitchen combat](../media/kitchen-combat.gif) comes from `Recordings/Movie_002.mp4`, 7–13 seconds; [eating](../media/eat-and-reload.gif) comes from `Recordings/Movie_003.mp4`, 0–4.5 seconds. Both are actual gameplay, cropped to remove side margins and the HUD, exported at 480px wide and 9 fps, and kept below 3 MB each. These copies live in the tracked `media/` folder so the README works after cloning.

For project licensing and the outstanding provenance/redistribution questions, see [License](../LICENSE.md).
