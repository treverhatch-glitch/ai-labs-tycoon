# unity/

Unity **6000.0.58f1** LTS (GameCI `ubuntu-6000.0.58f1-linux-il2cpp-3`) for AI Labs Tycoon. Game rules live in [`Rules/`](../Rules) as a local package; this
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
- **Rooms** use purpose colours tinted by era; `GlowSpreadMap` drives `RoomBlockView` emission from compute/research seeds.
- **Mascots** (`ModelMascotRow`) show each model's `ModelMood` via `ModelMascotPresentation`.

Open and verify on the owner's PC: [docs/SETUP-PC.md](../docs/SETUP-PC.md) §2.

Art specs and the campus OBJ kit: [docs/art/README.md](../docs/art/README.md).