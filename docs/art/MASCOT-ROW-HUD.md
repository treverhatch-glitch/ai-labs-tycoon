# Model mascot row HUD (presentation)

Per `MASCOT-PRESENTATION.md` and `GLOW-PRESENTATION.md`:

| Mood | Scale | Glow |
| --- | --- | --- |
| Healthy | 1.0 | base spread |
| Overworked | 0.95 | +10% warm |
| Glitching | 1.05 jitter | danger tint edge |

Tokens: `CampusMascotRowHudPresentationSpec` (48px row, max 4 visible, strip fraction from `CampusHudLayoutSpec`).

HQ band: [HQ-MASCOT-STRIP-PRESENTATION.md](HQ-MASCOT-STRIP-PRESENTATION.md) (batch 41).

Rules: `LabQueries.Mood` + `GLOW-PRESENTATION.md`. Cross-ref [MASCOT-PRESENTATION.md](MASCOT-PRESENTATION.md).