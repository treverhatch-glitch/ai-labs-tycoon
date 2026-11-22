# Gauntlet stage (presentation overlay)

Seasonal overlay on HQ — does not replace sim rules.

| Element | Asset |
| --- | --- |
| Lectern | `prop-gauntlet-lectern` |
| Trophy / board | `prop-trophy-pedestal`, `prop-scoreboard-slab` |
| Backdrop | `prop-monitor-wall` (leaderboard placeholder texture later) |
| Mascots | existing `ModelMascotRow`, scale 1.2× during overlay |
| Lighting | warm accent +10% on `CampusPresentationSpec` pitch |

**Enter/exit:** fade 0.6s; HUD hides build panel, keeps cash/date.