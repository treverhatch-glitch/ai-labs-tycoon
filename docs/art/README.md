# Art and UI (presentation)

Game rules stay in `Rules/`; everything here is **presentation-only** for Unity (`unity/`).

| Doc | Purpose |
| --- | --- |
| [ART-PIPELINE.md](ART-PIPELINE.md) | How assets are authored, generated, reviewed, and land in Unity |
| [STYLE-GUIDE.md](STYLE-GUIDE.md) | Cozy isometric late-campus office look |
| [ASSET-REGISTER.md](ASSET-REGISTER.md) | Placeholder → final list for main screens |
| [UI-HUD-LAYOUTS.md](UI-HUD-LAYOUTS.md) | Screen layout specs (HQ, research, training, rivals, world map) |
| [UNITY-INTEGRATION.md](UNITY-INTEGRATION.md) | Handoff for the Unity thread (`bc-93b5443a`) |
| [SCREEN-KITS.md](SCREEN-KITS.md) | Per-screen prop and icon kits (batch 2) |
| [MATERIAL-TOKENS.json](MATERIAL-TOKENS.json) | URP colour/smoothness tokens |
| [WORLD-MAP-PANEL.md](WORLD-MAP-PANEL.md) | World map overlay mock (batch 4) |
| [RESEARCH-PANEL.md](RESEARCH-PANEL.md) / [TRAINING-PANEL.md](TRAINING-PANEL.md) | Screen panel specs (batch 5) |
| [RIVALS-PANEL.md](RIVALS-PANEL.md) / [GAUNTLET-STAGE.md](GAUNTLET-STAGE.md) | Rivals + gauntlet overlay (batch 6 draft) |
| [PROP-PLACEMENT-HINTS.md](PROP-PLACEMENT-HINTS.md) | Default props per room purpose (Unity wire-up) |
| [BATCH-6-PLAN.md](BATCH-6-PLAN.md) | Batch 6 checklist (GO `a4a7d54`) |
| [TAB-RAIL-INTEGRATION.md](TAB-RAIL-INTEGRATION.md) / [BATCH-7-PLAN.md](BATCH-7-PLAN.md) | Tab rail + world map (batch 7) |
| [MASCOT-PRESENTATION.md](MASCOT-PRESENTATION.md) / [GLOW-PRESENTATION.md](GLOW-PRESENTATION.md) | Mascot + glow (batch 8) |
| [ERA-SKIN-TOKENS.md](ERA-SKIN-TOKENS.md) / [era-skin-tokens.json](era-skin-tokens.json) | Era floor/accent (batch 9) |
| [MATERIAL-ERA-SKINS.md](MATERIAL-ERA-SKINS.md) / [BATCH-10-PLAN.md](BATCH-10-PLAN.md) | P3 materials (batch 10) |

**Generated meshes:** `unity/Assets/Art/Generated/` (see `manifest.json`). Regenerate with:

```bash
python3 tools/art/generate_campus_kit.py
```

**Code hooks today:** `HqVisualPalette`, `HqGridLayout`, `CampusPresentationSpec` under `unity/Assets/Scripts/Hq/`.