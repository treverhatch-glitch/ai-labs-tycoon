# Style guide — cozy isometric late-campus office

## North star

**Late-campus office:** the player graduates from a garage into glassy quads, coffee carts, and lawn trees—but the game still reads as *one building you cut open*, Mad Games Tycoon 2–style, with warm late-afternoon light and soft edges.

Keywords: **cozy**, **readable**, **slightly goofy**, **honest about danger** (glow and safety yellow peek through cute props).

## Camera and projection

| Property | Value | Notes |
| --- | --- | --- |
| Projection | Perspective | FOV 48–55°; not true isometric |
| Azimuth | 35–42° from north | Match `CampusPresentationSpec.CameraYawDegrees` |
| Elevation | 28–35° | Shallow enough to see room labels |
| Cutaway | Front wall stripped on −Z | Rooms are open boxes on the lot plate |
| Lot plate | Warm concrete / grass border | Era tint from `HqVisualPalette.EraSkinFor` |

**Fork vs bible:** gameplay grid remains tile-based (`HqGridLayout.TileSize = 0.45m`). The camera *feels* isometric; the sim stays top-down logical.

## Colour

### Era ambient (floor / sky wash)

Use `HqVisualPalette` as the source of truth. Late-campus eras push **warmer ambients**:

| Era id | Mood | Accent use |
| --- | --- | --- |
| `dial-up-days` | Beige garage, scuffed carpet | Burnt orange trim |
| `the-punchline` | Cool startup open-plan | Teal accent wall |
| `it-sees` | Glass + mint plants | Mint neon on compute |
| `it-writes` | Purple twilight open office | Lavender UI chrome |
| `the-chat-boom` | Sunset lobby | Peach / coral CTAs |
| `agents` | Cyan atrium | Aqua glow on racks |
| `harnesses` | Violet research campus | Magenta safety pulses |

### Room purpose (base albedo)

From `HqVisualPalette.PurposeColor` — keep saturated enough to read at 960×540 CI capture:

- Research: soft blue `#5A8CF2`
- Engineering: leaf green `#66BF73`
- Safety: mustard `#F2BF5A`
- Sales: coral `#F2808C`
- Compute: violet `#8C66F2` (+ emission via glow map)
- Infrastructure: warm grey `#9999A6`
- Policy: lilac `#BF8CD9`

### UI chrome

| Token | Hex | Use |
| --- | --- | --- |
| `ui-ink` | `#1A1D24` | Body text on light panels |
| `ui-paper` | `#F4F0E8` | Cards, modals |
| `ui-slate` | `#14171D` @ 92% | Top HUD bar (matches `HqHudView`) |
| `ui-accent` | era accent × 1.2 brightness | Primary buttons |
| `ui-danger` | `#E85D4A` | Incidents, frozen blocs |
| `ui-glow` | compute violet @ HDR | Model mood / training pulse |

Typography (v1): Unity built-in `LegacyRuntime.ttf` until a bundled OFL font lands (no Microsoft fonts in repo).

## Shape language

- **Props:** low-poly, 12–120 tris each; slight bevel on desks and racks.
- **Architecture:** 1×1 modules (`building-module-1x1.obj`) stack to room footprints from rules config.
- **Characters:** capsules + hat until rigged mascots; rivals use **badge + palette**, not portraits v1.
- **World map:** abstract heightfield + pin markers (`ui-world-map-pin.obj`), not satellite imagery.

## Animation and life

| Signal | Presentation |
| --- | --- |
| Training active | Pulsing emission on compute tiles, `GlowSpreadMap` |
| Model mood | `ModelMascotPresentation` colour + bounce |
| Era change | 0.8s cross-fade on floor tint |
| Week advance | HUD date tick + subtle camera nudge (future) |

## Don'ts

- No real logos, flag-accurate government seals, or celebrity likenesses.
- No PBR hero assets until performance budget is measured on target hardware.
- No dark-unreadable UI: minimum 4.5:1 contrast on cash/date labels.