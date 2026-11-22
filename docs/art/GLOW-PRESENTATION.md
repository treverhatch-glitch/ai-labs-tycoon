# Room glow (presentation)

`GlowSpreadMap` (rules-driven seeds) feeds visual emission on room blocks. Art constants:

| Token | Value | Source |
| --- | --- | --- |
| Max strip glow | align from released/training models | rules |
| Decay | 650 permille per hop | `GlowSpreadMap` |
| Mascot glitch pulse | tied to `ModelMood.Glitching` | `MASCOT-PRESENTATION.md` |

Unity thread: multiply `HqVisualPalette` purpose colour by glow strength; no rule changes in graphics batches.