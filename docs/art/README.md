# Art and UI (presentation)

Game rules stay in `Rules/`; everything here is **presentation-only** for Unity (`unity/`).

| Doc | Purpose |
| --- | --- |
| [ART-PIPELINE.md](ART-PIPELINE.md) | How assets are authored, generated, reviewed, and land in Unity |
| [STYLE-GUIDE.md](STYLE-GUIDE.md) | Cozy isometric late-campus office look |
| [ASSET-REGISTER.md](ASSET-REGISTER.md) | Placeholder → final list for main screens |
| [UI-HUD-LAYOUTS.md](UI-HUD-LAYOUTS.md) | Screen layout specs (HQ, research, training, rivals, world map) |
| [UNITY-INTEGRATION.md](UNITY-INTEGRATION.md) | Handoff for the Unity thread (`bc-93b5443a`) |

**Generated meshes:** `unity/Assets/Art/Generated/` (see `manifest.json`). Regenerate with:

```bash
python3 tools/art/generate_campus_kit.py
```

**Code hooks today:** `HqVisualPalette`, `HqGridLayout`, `CampusPresentationSpec` under `unity/Assets/Scripts/Hq/`.