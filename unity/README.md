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
- **Rooms** use purpose colours tinted by era; `GlowSpreadMap` drives `RoomBlockView` emission from compute/research seeds.
- **Mascots** (`ModelMascotRow`) show each model's `ModelMood` via `ModelMascotPresentation`.
- **Starter layout** (`HqStarterLayout`) places the same four starter rooms as SimTests via `Command.BuildRoom`.
- **Build menu** (`RoomBuildPanel` + `RoomBuildController`) sends `Command.BuildRoom` only.
- **Campus phase** (`HqCampusPhase`) labels garage vs campus from era and cash; floor polish follows cash growth.
- **Live glow** (`HqGridView.RefreshGlowFromTraining`) refreshes room emission while models are training.
- **HUD** shows cash (bankrupt / zero-cash styling), calendar, era, phase, save/load, and week / month / year controls.
- **Save/load** (`HqSaveLoad`, `SimulationHost.SaveJson` / `LoadJson`) round-trips rules JSON from the HQ HUD.
- **Lived-in scenario** (`HqLivedInCampus`) seeds staff, a training run, and time steps for screenshots.
- **CI capture** (`HqViewCaptureTests`) writes `unity/artifacts/hq-view.png` after rendering the lived-in grid.

Open and verify on the owner's PC: [docs/SETUP-PC.md](../docs/SETUP-PC.md) §2.