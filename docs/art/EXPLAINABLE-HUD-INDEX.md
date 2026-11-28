# Explainable HUD index (presentation)

`DisplayQueries` in rules; Unity draws parts that sum to totals ([DESIGN.md](../DESIGN.md)).

| Drawer | Doc | Batch | Spec |
| --- | --- | --- | --- |
| HQ quality / revenue / incidents | [DISPLAY-BREAKDOWN-PRESENTATION.md](DISPLAY-BREAKDOWN-PRESENTATION.md) | 20 | `CampusBreakdownPresentationSpec` |
| Research screen detail | [RESEARCH-PANEL-BREAKDOWN.md](RESEARCH-PANEL-BREAKDOWN.md) | 25 | `CampusResearchBreakdownPresentationSpec` |

Shared row height: 22px (`PartRowHeightPixels`). Research screen reuses drawer width fraction per batch 47.

Tokens: `CampusExplainableHudIndexPresentationSpec` (2 drawers + shared row height). Batch 56.