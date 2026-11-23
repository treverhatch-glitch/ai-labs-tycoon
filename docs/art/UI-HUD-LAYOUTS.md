# UI / HUD layout specs

Reference resolution: **1920×1080** design, **Scale With Screen Size** (see `HqHudView`). Safe area: 24px inset on all edges.

## Global chrome (all screens)

```
┌────────────────────────────────────────────────────────────┐
│ TOP BAR (72px, ui-slate)                                    │
│  Cash + breakdown link (L)     Era name (R)                 │
│  Date / quarter (L, 2nd line)                               │
├────────────────────────────────────────────────────────────┤
│                                                             │
│                    MAIN STAGE (screen-specific)             │
│                                                             │
│                                    ┌─────────────────┐    │
│                                    │ Advance week    │    │
│                                    │ 200×48 BR       │    │
│                                    └─────────────────┘    │
└────────────────────────────────────────────────────────────┘
```

| Element | Anchor | Size | Data binding |
| --- | --- | --- | --- |
| Top bar | top stretch | h=72 | static chrome |
| Cash label | top-left | 420×28 | `Simulation.Player.Cash` |
| Date label | top-left, y−28 | 420×28 | `Simulation.DateLabel` |
| Era label | top-right | 420×28 | `Simulation.Era.Name` |
| Advance week | bottom-right | 200×48 | `SimulationHost.StepWeek()`; see [HUD-DATE-WEEK-PRESENTATION.md](HUD-DATE-WEEK-PRESENTATION.md) |

**Future:** tab rail left (64px) for HQ / Research / Training / Rivals / World — v1 uses separate panels opened from HQ (`RoomBuildPanel`, `TrainingRunPanel`, etc.).

## HQ office (primary stage)

**Camera:** `CampusPresentationSpec` defaults; lot centred; garage 14×10 tiles visible without scroll.

| Zone | Rect (of main stage) | Content |
| --- | --- | --- |
| Lot viewport | 70% width, full height | `HqGridView` + props |
| Mascot strip | bottom 18%, full width | `ModelMascotRow` |
| Build panel | right 28%, top 40% | `RoomBuildPanel` — [ROOM-BUILD-PRESENTATION.md](ROOM-BUILD-PRESENTATION.md) (batch 38) |
| Staff/GPU | right 28%, mid 30% | `StaffGpuPanel` — [STAFF-GPU-PANEL-PRESENTATION.md](STAFF-GPU-PANEL-PRESENTATION.md) (batch 39) |
| Training peek | right 28%, bottom 22% | collapsed `TrainingRunPanel` summary |

**Interaction:** click tile → `RoomBuildController` ghost; right-click cancel.

**Visual layers (back → front):** sky colour → lot plate → room blocks → props → glow overlay → mascot strip → UI.

## Research screen

Full-screen modal or dedicated scene (Unity thread chooses; layout spec is identical).

```
┌──────── TOP BAR (global) ────────┐
│ ┌─────────────┬────────────────┐ │
│ │ Tech tree   │ Detail drawer  │ │
│ │ 62%         │ 38%            │ │
│ │ eras as     │ selected tech  │ │
│ │ swimlanes   │ cost, deps,    │ │
│ │             │ "Research" CTA │ │
│ └─────────────┴────────────────┘ │
│ Active leads (h=120): chips     │
└──────────────────────────────────┘
```

| Control | Placement | Rules touchpoint |
| --- | --- | --- |
| Tech card grid | left scroll | `GameConfig.Techs`, era gates |
| Detail drawer | right fixed | `ResearchTech` command |
| Lead chips | bottom | research lead systems (era content) |
| CTA | drawer bottom | disabled when `CommandError.EraLocked` |

## Training screen

```
┌──────── TOP BAR ────────┐
│ Run queue (60% W) │ Split │
│ ordered runs      │ compute│
│ status bars       │ train  │
│                   │ serve  │
│                   │ R&D %  │
├───────────────────┴───────┤
│ Model mascots (full width) │
│ recipe + data source row   │
└───────────────────────────┘
```

| Control | Placement | Rules touchpoint |
| --- | --- | --- |
| Run queue | left | `TrainingRunPanel`, active runs |
| Compute split sliders | right | `ComputeSplit` command |
| Recipe picker | bottom-left | `StartTrainingRecipe` |
| Data source icons | bottom-right | license / scrape flags |
| Mood row | bottom | `ModelMood` per line |

## Rivals screen

```
┌──────── TOP BAR ────────┐
│ Lab picker tabs (top)    │
│ ┌──────┬───────────────┐│
│ │ List │ Decision log  ││
│ │ 35%  │ 65% monospace ││
│ │ labs │ scrollback    ││
│ └──────┴───────────────┘│
│ Feeling meter + chips    │
└──────────────────────────┘
```

| Control | Placement | Rules touchpoint |
| --- | --- | --- |
| Lab list | left | rival labs, bankruptcy greyed |
| Badge | list item leading | `ui-rival-badge` + palette |
| Feeling bar | under selection | `LabState.FeelingToPlayer` |
| Log | right | `DecisionText` templates |

## World map screen

```
┌──────── TOP BAR ────────┐
│ ┌─────────────────────┐│
│ │ Map (70%)           ││
│ │ blocs + sites       ││
│ │ pins on sites       ││
│ └─────────────────────┘│
│ ┌────────── Site card ─┐│
│ │ 30%: power, freeze,  ││
│ │ holdings, build CTA   ││
│ └──────────────────────┘│
└──────────────────────────┘
```

| Control | Placement | Rules touchpoint |
| --- | --- | --- |
| Map plane | left | `world.sites`, bloc colours |
| Pin | site lat/long norm | `ui-world-map-pin` |
| Site card | right | `BuyLand`, `BuildHall`, freeze badge |
| Power breakdown | card body | `DisplayQueries` power parts |

## Accessibility (v1 targets)

- All CTAs ≥ 44×44 px hit target at 1080p.
- Colour never sole indicator: room purpose shows **text label** on hover.
- Cash and freeze states include **icon + colour**.