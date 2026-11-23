# Model mascots (presentation)

Rules: `LabQueries.Mood` → Healthy / Overworked / Glitching. Graphics supplies motion amplitudes only.

| Mood | Tint delta | Bob amplitude | Glow |
| --- | --- | --- | --- |
| Healthy | +5% saturation | 0.04 m | off |
| Overworked | −10% value | 0.08 m | faint warm rim |
| Glitching | hue jitter ±8° | 0.12 m | `GlowSpreadMap` pulse |

**Strip layout:** [MASCOT-ROW-HUD.md](MASCOT-ROW-HUD.md) + `CampusMascotRowHudPresentationSpec`; fraction from `CampusHudLayoutSpec`.