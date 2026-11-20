# unity/

Unity **6000.3** presentation for AI Labs Tycoon. Game rules live in [`Rules/`](../Rules) as a local package; this
folder only draws state and sends commands.

## Layout

| Path | Purpose |
| --- | --- |
| `Packages/manifest.json` | `com.hatchtag.ailabs.rules` → `file:../../Rules` |
| `Assets/Scripts/Bootstrap/GameBootstrap.cs` | Builds the first HQ view in code on Play |
| `Assets/Scripts/Hq/` | Garage grid, room blocks, HUD, era/glow palette |
| `Assets/Scripts/Sim/` | `SimulationHost`, config loader |
| `Assets/Editor/Tests/` | Edit Mode smoke tests |

## HQ view (eras + glow)

- **Era skins** (`HqVisualPalette.EraSkinFor`) tint the floor and accent column when `Simulation.Era` changes.
- **Rooms** use purpose colours from config; `RoomBlockView.SetGlowPermille` is reserved for model glow spreading
  through the building in a later slice.

Open and verify on the owner's PC: [docs/SETUP-PC.md](../docs/SETUP-PC.md) §2.