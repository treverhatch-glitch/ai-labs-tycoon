# Global top bar token index (presentation)

72px chrome shared by every screen ([UI-HUD-LAYOUTS.md](UI-HUD-LAYOUTS.md)). Rules: cash, grant, date, era from sim queries only — no new bindings here.

| Slot | Doc | Batch | Spec |
| --- | --- | --- | --- |
| Cash + grant | [HQ-CASH-DISPLAY-PRESENTATION.md](HQ-CASH-DISPLAY-PRESENTATION.md) | 27 | `CampusHqCashDisplayPresentationSpec` |
| Date / week row | [HUD-DATE-WEEK-PRESENTATION.md](HUD-DATE-WEEK-PRESENTATION.md) | 31 | `CampusHudDateWeekPresentationSpec` |
| Era name | [HUD-ERA-LABEL-PRESENTATION.md](HUD-ERA-LABEL-PRESENTATION.md) | 52 | `CampusHudEraLabelPresentationSpec` |
| Advance week | [ADVANCE-WEEK-PRESENTATION.md](ADVANCE-WEEK-PRESENTATION.md) | 46 | `CampusAdvanceWeekPresentationSpec` |

Capture regions: [CAPTURE-HUD-SNAPSHOT.md](CAPTURE-HUD-SNAPSHOT.md) (batch 35).

Tokens: `CampusTopBarTokenIndexPresentationSpec` (bar height + 4 chrome slots). Batch 51–52.