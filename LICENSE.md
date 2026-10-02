# License

[Back to the landing page](README.md)

## Project license

This repository carries the **GNU General Public License, version 3**, for the project's original work. The complete, authoritative license text is in [LICENSE](LICENSE); this page explains its scope and the bundled third-party material. The existing license text has been preserved.

GPL-covered code may be used, studied, modified, and redistributed under that license. Distributing covered modifications or binaries also brings notice, licensing, and corresponding-source obligations; follow the full license rather than treating a repository link as sufficient in every case. The software is supplied without warranty.

## Third-party material

The project license does **not** relicense someone else's code, fonts, icons, libraries, or tools. Their original terms and copyright notices remain applicable. [Credits](docs/CREDITS.md) identifies the known sources and local notices, including MIT-licensed tools, OFL fonts, Flaticon icons, and commercial editor tooling.

A credit or source link is not permission to redistribute an asset's raw files. In particular, the [Odin EULA](https://odininspector.com/eula) and [Unity Asset Store EULA](https://unity.com/legal/as-terms) distinguish use in a finished product from sharing the underlying vendor software/assets. Buying a tool does not automatically license its files to everyone who clones this repository.

## Current licensing gaps

The following items were found in the checkout during the **2026-10-02** documentation review:

| Item | What remains to resolve |
| --- | --- |
| `Assets/Plugins/Sirenix` | Odin assemblies are tracked in Git. Confirm permission to distribute these files publicly, or supply a project that requires each developer to install their own licensed copy. The Bootstrap icon notice inside this folder covers those icons, not Odin itself. |
| `Assets/PxlDev/Sprite Blending Layers` | The included documentation explains usage but contains no license grant. Confirm the original package/source terms before redistributing its scripts, shader, and sample textures. |
| `Assets/PluginMaster/B83` | The bundled helper scripts have no license notice in this checkout. Identify their source and applicable terms. |
| Flaticon icons | Four sources are credited on itch.io. Preserve the applicable attribution and the download license for each; confirm whether raw-source redistribution is allowed. |
| Other game art, music, and sound effects | The published page does not identify separate sources. Confirm authorship or record source/license information for any externally sourced files; filenames and local source projects alone do not establish ownership. |
| GPL and proprietary runtime dependencies | No project-specific linking exception was found. Resolve permission for combining/distributing GPL-covered code with Unity and any proprietary runtime libraries before describing a finished build as cleared for redistribution. |

The [GNU licensing FAQ](https://www.gnu.org/licenses/gpl-faq.en.html#GPLIncompatibleLibs) explains additional permissions for GPL-incompatible libraries. Such permission must come from the relevant copyright holders; an exclusion list or credits page does not supply it. This documentation does not add a linking exception or change the existing license.

The package manifest also restores **Hot Reload**, which has its own EULA. Package-manager installation is not a grant to redistribute that tool under GPL. Unity packages retain their package licenses and third-party notices.

Until these provenance and permission questions are resolved, the repository should not be represented as a fully cleared, entirely GPL-licensed asset collection. The documentation records the known terms without claiming permissions that the available notices do not establish.

## Maintaining notices

Keep [LICENSE](LICENSE), this overview, [Credits](docs/CREDITS.md), and the relevant third-party license/copyright notices with redistributed material. Include required notices in release packaging for the dependencies/assets actually shipped, and provide corresponding source where the GPL requires it. Update the inventory when assets or dependencies change.
