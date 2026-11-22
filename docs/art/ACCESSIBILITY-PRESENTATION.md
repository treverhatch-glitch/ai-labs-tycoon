# Accessibility (presentation v1)

From `UI-HUD-LAYOUTS.md` — enforced in tokens for Unity UI rebuild.

| Rule | Constant |
| --- | --- |
| Primary CTA hit target | ≥ 44×44 px @ 1080p |
| Tab rail cell | `CampusHudLayoutSpec.TabRailWidth` ≥ 44 |
| Freeze / danger | colour + `icon-danger` sprite |
| Room purpose hover | text label required |

**CI:** `HqViewCaptureTests` artifact remains non-empty regression guard.