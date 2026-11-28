# HUD era label (presentation)

Top-right era name in the global chrome ([UI-HUD-LAYOUTS.md](UI-HUD-LAYOUTS.md)). Rules: `Simulation.Era.Name` from era data (`docs/content/eras.md`); no picker logic here.

| Element | Spec |
| --- | --- |
| Anchor | top-right, safe inset |
| Size | 420×28 |
| Text | right-aligned, `CampusUiTokens.UiSlate` |
| Accent | optional floor swatch from [ERA-SKIN-TOKENS.md](ERA-SKIN-TOKENS.md) on era transition |

Index: [TOP-BAR-TOKEN-INDEX.md](TOP-BAR-TOKEN-INDEX.md) (batch 51). Start-era flow: [START-ERA-PICKER-PRESENTATION.md](START-ERA-PICKER-PRESENTATION.md) (batch 37).

Tokens: `CampusHudEraLabelPresentationSpec` (420×28, 14px min font, links era skin transition). Batch 52.