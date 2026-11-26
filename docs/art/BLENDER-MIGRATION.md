# Blender migration inventory

Audit of migratable art across `main`, `cursor/early-game-art-bootstrap`, and `cursor/graphics-pipeline-5032`. Inventory only — no rebuilds in this commit.

**Audited:** 2026-11-26 14:21 ICT (composer-2.5 inventory worker)

## Lane split

| Lane | Branch / agent | Scope |
| --- | --- | --- |
| **early** | `cursor/early-game-art-bootstrap` (`bc-d48d70eb`, [#86](https://github.com/treverhatch-glitch/ai-labs-tycoon/pull/86)) | `unity/Assets/Art/EarlyGame/**`, first-scene URP lighting + post for `GameBootstrap`, early HUD/wisp Blender renders |
| **graphics** | `cursor/graphics-pipeline-5032` (`bc-6502c7bf`) | All other `unity/Assets/Art/**`, rival badges, campus kit, UI icons |

## Summary

| Metric | Count |
| --- | ---: |
| **Total migratable assets** | **92** |
| Early lane | 48 |
| Graphics lane | 44 |

Counts are the **union** of paths on the three refs (deduplicated by repo path). JSON manifests under `Art/` are excluded (not migratable pixels).

**Early lane follow-ups (not in table):** URP volume / post profile for first `GameBootstrap` scene — not present under `unity/Assets/Art/` on any audited ref; track when Unity thread adds assets.

## Assets

| Path | Lane | Type | Refs | Status | Notes |
| --- | --- | --- | --- | --- | --- |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-cable-spool.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-cable-spool.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-coffee-drip.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-coffee-drip.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-crt-tower.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-crt-tower.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-desk-crt.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-desk-crt.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-exterior-wall.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-exterior-wall.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-floor-tile.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-floor-tile.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-folding-chair.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-folding-chair.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-land-for-sale-sign.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-land-for-sale-sign.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-lot-curb.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-lot-curb.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-roll-door.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-roll-door.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-server-closet.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-server-closet.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-voodude-rack.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-voodude-rack.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-wall-panel.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-wall-panel.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-whiteboard-1995.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-whiteboard-1995.py` |
| `unity/Assets/Art/EarlyGame/garage-kit/garage-window-frame.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_garage_kit.py`); packet-1 placeholder fidelity; target `tools/art/blender/garage-window-frame.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/hud-topbar-beige.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/hud-topbar-beige.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-build-panel.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-build-panel.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-cash-era0.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-cash-era0.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-date-era0.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-date-era0.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-era-bar.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-era-bar.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-gpu-hire.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-gpu-hire.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-hire-panel.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-hire-panel.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-train-panel.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-train-panel.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/icon-week-advance.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/icon-week-advance.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/panel-build-bg.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/panel-build-bg.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/panel-hire-bg.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/panel-hire-bg.py` |
| `unity/Assets/Art/EarlyGame/hud-icons/panel-train-bg.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_early_hud_icons.py`); target `tools/art/blender/panel-train-bg.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-engineer-work.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-engineer-work.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-founder-idle.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-founder-idle.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-gpu-tech-idle.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-gpu-tech-idle.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-pose-typing.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-pose-typing.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-recruiter-idle.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-recruiter-idle.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-researcher-idle.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-researcher-idle.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-safety-idle.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-safety-idle.py` |
| `unity/Assets/Art/EarlyGame/staff-kit/staff-sales-idle.obj` | early | mesh | early | todo | Procedural OBJ (`tools/art/generate_early_staff.py`); silhouettes need Blender rebuild; target `tools/art/blender/staff-sales-idle.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-crt-phosphor.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-crt-phosphor.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-garage-carpet.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-garage-carpet.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-garage-concrete.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-garage-concrete.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-garage-door-metal.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-garage-door-metal.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-garage-server-paint.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-garage-server-paint.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-garage-trim-orange.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-garage-trim-orange.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-garage-wall-beige.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-garage-wall-beige.py` |
| `unity/Assets/Art/EarlyGame/textures/tex-hud-paper-warm.png` | early | texture | early | todo | Procedural PNG (`tools/art/generate_early_textures.py`); rebake 1024 PBR in Blender; target `tools/art/blender/tex-hud-paper-warm.py` |
| `unity/Assets/Art/EarlyGame/wisp-moods/wisp-angry.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_wisp_mascot.py`); animated glow target; target `tools/art/blender/wisp-angry.py` |
| `unity/Assets/Art/EarlyGame/wisp-moods/wisp-glowing.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_wisp_mascot.py`); animated glow target; target `tools/art/blender/wisp-glowing.py` |
| `unity/Assets/Art/EarlyGame/wisp-moods/wisp-happy.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_wisp_mascot.py`); animated glow target; target `tools/art/blender/wisp-happy.py` |
| `unity/Assets/Art/EarlyGame/wisp-moods/wisp-neutral.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_wisp_mascot.py`); animated glow target; target `tools/art/blender/wisp-neutral.py` |
| `unity/Assets/Art/EarlyGame/wisp-moods/wisp-worried.svg` | early | icon | early | todo | Procedural SVG (`tools/art/generate_wisp_mascot.py`); animated glow target; target `tools/art/blender/wisp-worried.py` |
| `unity/Assets/Art/Generated/campus-kit/building-elevator-core.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/building-elevator-core.py` |
| `unity/Assets/Art/Generated/campus-kit/building-module-1x1.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/building-module-1x1.py` |
| `unity/Assets/Art/Generated/campus-kit/exterior-bike-rack.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/exterior-bike-rack.py` |
| `unity/Assets/Art/Generated/campus-kit/exterior-fence-segment.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/exterior-fence-segment.py` |
| `unity/Assets/Art/Generated/campus-kit/exterior-hallway-bench.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/exterior-hallway-bench.py` |
| `unity/Assets/Art/Generated/campus-kit/exterior-quad-tree.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/exterior-quad-tree.py` |
| `unity/Assets/Art/Generated/campus-kit/exterior-sign-pole.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/exterior-sign-pole.py` |
| `unity/Assets/Art/Generated/campus-kit/exterior-solar-shade.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/exterior-solar-shade.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-cafeteria-table.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-cafeteria-table.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-chair.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-chair.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-coffee-cart.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-coffee-cart.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-data-stack.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-data-stack.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-desk.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-desk.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-gauntlet-lectern.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-gauntlet-lectern.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-gpu-blade.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-gpu-blade.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-lounge-sofa.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-lounge-sofa.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-monitor-wall.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-monitor-wall.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-potted-plant.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-potted-plant.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-research-bench.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-research-bench.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-safety-cone.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-safety-cone.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-scoreboard-slab.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-scoreboard-slab.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-server-rack.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-server-rack.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-trophy-pedestal.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-trophy-pedestal.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-vending-machine.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-vending-machine.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-water-cooler.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-water-cooler.py` |
| `unity/Assets/Art/Generated/campus-kit/prop-whiteboard.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/prop-whiteboard.py` |
| `unity/Assets/Art/Generated/campus-kit/ui-bloc-tile.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/ui-bloc-tile.py` |
| `unity/Assets/Art/Generated/campus-kit/ui-rival-badge.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/ui-rival-badge.py` |
| `unity/Assets/Art/Generated/campus-kit/ui-tab-rail-plaque.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/ui-tab-rail-plaque.py` |
| `unity/Assets/Art/Generated/campus-kit/ui-world-map-pin.obj` | graphics | mesh | main, early, graphics | todo | Procedural OBJ (`tools/art/generate_campus_kit.py`); target `tools/art/blender/ui-world-map-pin.py` |
| `unity/Assets/Art/Generated/rival-silhouettes/silhouette-anthoptics.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_rival_silhouette_svgs.py`); target `tools/art/blender/silhouette-anthoptics.py` |
| `unity/Assets/Art/Generated/rival-silhouettes/silhouette-envydia.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_rival_silhouette_svgs.py`); target `tools/art/blender/silhouette-envydia.py` |
| `unity/Assets/Art/Generated/rival-silhouettes/silhouette-openaye.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_rival_silhouette_svgs.py`); target `tools/art/blender/silhouette-openaye.py` |
| `unity/Assets/Art/Generated/rival-silhouettes/silhouette-xay.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_rival_silhouette_svgs.py`); target `tools/art/blender/silhouette-xay.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-calendar.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-calendar.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-cash.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-cash.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-compute.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-compute.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-danger.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-danger.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-hq.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-hq.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-research.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-research.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-rivals.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-rivals.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-staff.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-staff.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-training.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-training.py` |
| `unity/Assets/Art/Generated/ui-icons/icon-world.svg` | graphics | icon | main, early, graphics | todo | Procedural SVG (`tools/art/generate_ui_icons.py`); target `tools/art/blender/icon-world.py` |
