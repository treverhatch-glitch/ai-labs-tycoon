# Tab rail active state (presentation)

Index: [TAB-RAIL-INTEGRATION-INDEX.md](TAB-RAIL-INTEGRATION-INDEX.md) (batch 67). Per `TAB-RAIL-INTEGRATION.md`:

| State | Treatment |
| --- | --- |
| Active tab | `ui-tab-rail-plaque` at 100% + icon full opacity |
| Inactive | plaque 85% scale, icon 70% opacity |

Tokens: `CampusTabRailActivePresentationSpec` (`ui-tab-rail-plaque`, inactive 0.85 scale / 0.70 icon opacity).

Unity thread drives selection; graphics supplies constants only (batch 28 draft).