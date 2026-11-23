# Rival feeling meter (presentation)

Rules: `LabState.FeelingToPlayer` in **-1000…1000** (`docs/DESIGN.md`).

| UI | Spec |
| --- | --- |
| Normalized fill | `(value + 1000) / 2000` |
| Hostile | &lt; 0.35 → accent desaturate |
| Neutral band | 0.35–0.65 |
| Friendly | &gt; 0.65 → rival accent full |

Constants: `CampusRivalFeelingPresentationSpec` (`NormalizedFill`, band thresholds, 6px meter height).

Index: [RIVAL-FEELING-INDEX.md](RIVAL-FEELING-INDEX.md) (batch 58). Place under rival list row in `RIVALS-PANEL.md`; no `LabState` reads in graphics batches.