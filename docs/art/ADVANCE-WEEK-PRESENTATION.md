# Advance week control (presentation)

Global chrome per `UI-HUD-LAYOUTS.md` — bottom-right **200×48** button, anchored with 24px safe inset.

| Token | Value |
| --- | --- |
| Size | 200×48px |
| Anchor | `CampusHudLayoutSpec.AdvanceWeekAnchoredPosition` |

Index: [TOP-BAR-TOKEN-INDEX.md](TOP-BAR-TOKEN-INDEX.md) (batch 51). Spec: `CampusAdvanceWeekPresentationSpec` (batch 46 stub). Date/week labels: [HUD-DATE-WEEK-PRESENTATION.md](HUD-DATE-WEEK-PRESENTATION.md). Safe area: [SAFE-AREA-PRESENTATION.md](SAFE-AREA-PRESENTATION.md).

Rules: `SimulationHost.StepWeek()` binding on Unity thread only.