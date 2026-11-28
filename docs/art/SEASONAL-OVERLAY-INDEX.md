# Seasonal overlay index (presentation)

HUD cutscenes and seasonal layers on HQ — sim outcomes stay in `Rules/` ([DESIGN.md](../DESIGN.md) yearly Gauntlet).

| Overlay | Doc | Batch | Spec |
| --- | --- | --- | --- |
| Gauntlet stage | [GAUNTLET-STAGE-HUD.md](GAUNTLET-STAGE-HUD.md) | 24 | `CampusGauntletStagePresentationSpec` |
| World map freeze | [WORLD-MAP-FREEZE-PRESENTATION.md](WORLD-MAP-FREEZE-PRESENTATION.md) | 22 | `CampusWorldMapFreezePresentationSpec` |

Stage brief: [GAUNTLET-STAGE.md](GAUNTLET-STAGE.md). Global fade constant: `CampusHudLayoutSpec.GauntletOverlayFadeSeconds`. Freeze chrome: rules-driven bloc state only ([DESIGN.md](../DESIGN.md) world map).

Tokens: `CampusSeasonalOverlayIndexPresentationSpec` (documented overlay count + gauntlet fade alias). Batches 53–54.