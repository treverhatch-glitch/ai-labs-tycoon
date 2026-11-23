# What's next

The roadmap after the foundation slice. Design is in [DESIGN.md](DESIGN.md); PC-only steps are in
[SETUP-PC.md](SETUP-PC.md). Items inside a group are in rough order. Sizes are in terms of what has to change, not
time.

## Where things stand

The rules core plays a whole game headless from 1995 or 2035: rooms, staff, GPUs, research, training,
releases into 34 sectors, five parody rivals, the yearly Gauntlet, incidents, bankruptcy, save, load, replay, nine
eras as data with their techs, rooms, sectors, headlines and dated world events, seven Envydia GPU generations, and a
world map of 9 blocs and 11 sites (land, halls, colo, power, heatwaves, export rules, government attention), a chip design screen (own chips, a shared foundry, listings, Envydia's answer) and rivals
that design and buy chips, plus endless play (endings that continue, frontier research, model museum) and the shady path (Heat, stash, investigations, corruption). SimTests pass in the hundreds, including balance soaks and saves through schema 11. The Unity project lives under `unity/` (garage grid, HUD, week button in code); GameCI runs on PRs that touch `unity/**`; full editor verification is on the PC.

Known limits of the slice, so nobody is surprised:

- **Eras 2 to 8 are first-pass content.** Every era has techs, sectors, rooms, headlines and events, and the
  autopilot reaches the end of the tree by 2034. Names, blurbs and the numbers are tuned to keep a whole life
  solvent, not yet to be fun at each step; the eight-year balance soak is the first check. The content tables are in
  [content/](content/eras.md); [QUESTIONS.md](QUESTIONS.md) lists the choices taken.
- **Training cost per era is retuned for GPU generations.** Hardware generations landed, so the era multipliers were
  scaled down by the compute-per-dollar they add (Q35). Late-game cash is still higher than at launch; Q76 retuned
  dial-up costs and wealth growth for the eight-year soak.
- **Some rooms and every role beyond the first four have no special rule yet.** New roles fill one of the four
  functions; a room is built for what it seats or holds.
- **The cast is eight labs.** OpenAye, AntHopticsAI and xAy from the start; Metta (2007), Macrosoft (2012), Palantear
  (2017), Goggle DeepMined (2010) and DeepSneak (2023) as arrivals. The start-era picker (item 12) is still UI-only.
- **Retired models are pruned** past six per line (museum); Gauntlet records keep names.
- **The world map is in and the plans use it.** xAy, Goggle DeepMined and DeepSneak build data centres, OpenAye and
  AntHopticsAI rent, and owning plans swap hardware two generations old for the current one (Q52). The player can
  use every command; the host UI is the missing half (graphics batch 36 documents shell chrome only).
- **Staff are people** with traits and morale; feud poaching is in (schema 10).
- **Endings continue the run** (bankrupt bailout, sell the lab, monopoly, handcuffs); alignment gap and rungs 1–3 are in; reserved compute and the Envydia queue are in (schema 11).
- **The shady path is in** for the player (cards, launder, investigations); parody labs never take shady jobs (`shadyAppetite` 0).
- **Money is cash and a grant.** No investors, shares, debt or bubble.
- **Rivals explain themselves, the player does not.** Plan-run labs keep a bounded decision log (Q48); the host can keep
  the player's side.

## Who works where

Agents work in parallel. Stay in your area and the merges stay boring.

| Area | Folder |
| --- | --- |
| Rules, tests, headless sim | `Rules/`, `tools/` |
| Unity project | `unity/` |
| Content tables (eras, techs, sectors, events, cast) | `docs/content/`, then into `Rules/Data/` by a rules task |
| Studio wiring, CI | `studio/`, `.github/` |

A rules change that moves the golden run (`The_golden_run_has_not_moved`) re-records the digest in the same commit
and says why in the commit message.

## Rules: next

1. ~~**Eras as data.**~~ Done: `eras` in the config, `EraSystem`, era-gated techs and rooms (`EraLocked`), the
   start-era picker (`NewGameOptions.StartEraId`, `simcli --era`), validator checks and tests.
2. ~~**Era content for every era.**~~ Done: 81 techs, 34 sectors, 22 room types, 15 roles (82, 24 and 17 with the world map's Liquid Cooling, Ops Centre, Policy Office, Infrastructure and Policy), 6 lots, 50 world events and
   per-era incident headlines, from 1995 game AI to 2033 machine science. Tables in `docs/content/`.
3. ~~**Explain every number.**~~ Done: `DisplayQueries` breakdowns for quality, appeal, weekly revenue and incident
   odds (parts sum to the displayed value); `BreakdownTests` (one fact each); `LabQueries` / `MarketSystem` delegate.
4. ~~**Rival decision logs.**~~ Done (systems design phase C, schema 4): `LabPlanner` logs each order with reason codes,
   bounded at 60 and saved; feelings toward the player; Envydia's allocation, standing, price, keynote and log;
   signature behaviours; Goggle DeepMined and DeepSneak as arrivals; the Refresh step (Q52).
4b. ~~**Own chips.**~~ Done (systems design phase D, schema 5): `ChipSystem`, `ChipQueries`, foundry months, the
    six chip commands, `ComputeSplit`, listings, Envydia's reactions, the planner's chip programme (Q57 to Q67).
5. ~~**Reserved compute and the Envydia queue.**~~ Done: `SignReservedCompute`, `QueueGpuPurchase`,
   `ComputeContractSystem`, discounted rent and deposit, queue fulfilled at month start, schema 11;
   `ComputeContractTests`. Golden digest `61bacf6b…`.
6. ~~**Hardware generations.**~~ Done (systems design phases 0 and A): `HardwareBatch` rows by generation and place,
   seven Envydia generations as data, resale that falls a generation at a time, `BuyGpus`/`SellGpus`/`MoveHardware`
   to a holding, the fleet migration (schema 2).
7. ~~**The funding ladder.**~~ Done: `FundingConfig`, `RaiseFunding`, founder share and investor board seats,
   schema 8 migration, `RecordOusted` when share hits `controlPermille`; `FundingTests`.
8. ~~**The alignment gap and incident rungs 1 to 3.**~~ Done: `displayedAlignment`, rungs 1–3 in `SafetySystem`, sandbag on
   release, `SandbagBoost` in `DisplayQueries`, schema 9; `AlignmentGapTests`.
9. ~~**Staff as people.**~~ Done: `StaffMember` rows (trait, morale), `StaffSystem`, feud poaching, schema 10
   migration from pools; `StaffPeopleTests`.
10. ~~**Bot balance at scale.**~~ Done: `BalanceSoak` in `Rules/Sim`, `simcli soak` with three presets, `BalanceTests`
    (default 24 games per preset; set `BALANCE_SOAK_FULL=1` for 200 each; optional `BALANCE_SOAK_GAMES_PER_PRESET`).
    Dial-up `trainingCostPermille` raised to 1500 and plan `wealthGrowthPermillePerMillion` cut (Q72); Envydia
    courting still waits on cast chip share (Q67).
11. **Endings that continue.** Bankrupt, Ousted and Exit as recorded moments after which play goes on. Planned as
    phase F of the systems design (an Exit keeps the lab as a plan-run rival, Q24).

12. **The wider cast.** ~~Macrosoft, Metta and Palantear plans and join years are in (2005, 2012, 2017).~~ Still open:
    a start-era picker screen that shows each era's tagline and kit. Data and plans; no new systems.
    Graphics batch 29 documents era swatch preview chrome (`ERA-SKIN-PREVIEW.md`) for that screen; batch 37 adds
    `START-ERA-PICKER-PRESENTATION.md` layout tokens; rules/data unchanged.
13. **Make each era feel different.** Today an era changes what is available and what things cost. The bible's
    "the rules shift" (new laws, new risk classes, per-era mechanics) comes with items 5, 7 and 8.

## Rules: later

- Endings and the
  shady path from the systems design (phases E to G). The first PRs landed the world map (schema 3), rival depth (C) and own chips (D). What is left of
  the world map: Audit's investigation roll and Freeze consequences beyond blocking purchases come with the shady path (Q45).
- The Bubble meter and circular financing; shares in rivals; monopoly and regulators.
- The shady path with per-model corruption; government and China deals.
- Incident rungs 4 to 6, containment rooms, the superintelligence endings.
- ~~Specialist model lines, data sources, open weights, pricing.~~ Done on `cursor/rules-side-items`: sector price (schema 12), open weights (13), code-specialist, data sources (14), image-specialist, market/world/training breakdown queries (Q9–Q15 in `docs/questions-sim-hardening.md`).
- ~~Real Bodies and Upward expansions~~ Done on `cursor/rules-side-items`: expansion appeal kits + room gates (`expansions` config); art/mechanics beyond appeal still open for a future pass.
- **Explain every number (tail):** Q12–Q17 breakdowns on `cursor/rules-side-items` (pushed through Q16 @ `a4f16dd`; Q17 frontier cost local); chip derive + shady odds — optional hardening only.

## Unity

Needs the owner's PC ([SETUP-PC.md](SETUP-PC.md)).

1. ~~**Create the Unity 6 project in `unity/`** with `Rules/` as a local package.~~ Scaffolded on branch `cursor/unity-scaffold`; Hatch verifies in the editor (SETUP-PC §2).
2. ~~**First scene in code:** garage lot grid, room blocks, week button, cash and date.~~ In `GameBootstrap`; room building UI is item 3 (graphics batch 38 documents `RoomBuildPanel` chrome only).
3. ~~**Room building, hire/GPU, training, Gauntlet show.**~~ On `cursor/unity-hq-views` (local); editor verification in SETUP-PC §2.
4. ~~**Model mascots (Healthy / Overworked / Glitching) and room glow spread.**~~ Presentation uses `LabQueries.Mood` and `GlowSpreadMap`; graphics tokens in `docs/art/MASCOT-PRESENTATION.md` + `GLOW-PRESENTATION.md` (batch 8); batch 33 adds row HUD spec for mascot strip polish on PC.
5. The land script and its entry in `studio/game.json`, so the harness can land Unity branches. Graphics batch 34 documents PR #41 branch landing notes for studio (`STUDIO-GRAPHICS-HANDOFF.md`); does not change `studio/` on this branch.

**Graphics on PR #41** (`cursor/graphics-pipeline-5032`): campus kit (30 props), HUD icons (10), rival silhouettes
(4), bloc/era JSON, handoff list checks in prepush, and presentation specs (`Campus*PresentationSpec` + docs). Remote tip
`5e6b64c` (batch 58: rival feeling index); batch 59 (a11y chrome index) on push queue.
Unity thread imports meshes and wires bindings per `docs/art/UNITY-INTEGRATION.md`; host UI for the world map and
start-era picker (items 12–13 below) stays rules/Unity presentation, not new rule systems.

## Art / presentation (active)

| Track | Status |
| --- | --- |
| PR #41 graphics batches | Spaced squashes on `cursor/graphics-pipeline-5032`; gate `./tools/art/prepush.sh` |
| Unity import (`bc-93b5443a`) | `UNITY-INTEGRATION.md`, prop placement hints, tab icons |
| Explainable HUD | Index (`EXPLAINABLE-HUD-INDEX.md`, batch 56); `DisplayQueries` in rules; HQ (batch 20), research drawer (batch 25), research screen 62/38 (batch 47) |
| Hire / staff UI | Index (`STAFF-PRESENTATION-INDEX.md`, batch 57); staff rows (batch 26); meshes still PC pass per `STAFF-PRESENTATION.md` |
| HQ chrome | Cash/grant (batch 27), date/week row (batch 31); breakdowns from `DisplayQueries` |
| Seasonal overlays | Index (`SEASONAL-OVERLAY-INDEX.md`, batch 53); Gauntlet chrome (`GAUNTLET-STAGE-HUD.md`, batch 24); sim outcomes stay in rules |
| Screen layouts | Research (47), rivals (48), training (49) — `UI-HUD-LAYOUTS` + `Campus*ScreenPresentationSpec` |
| A11y chrome | Index (`A11Y-CHROME-INDEX.md`, batch 59); focus ring (batch 30); capture HUD (batch 35) + `CAPTURE-REGRESSION.md` |

## Studio and CI

- Create the studio on the PC and copy `studio/game.json` into it (SETUP-PC.md).
- Beta mode is the owner's switch in the studio; it is off until the owner turns it on (SETUP-PC.md).
- The `unity` workflow is manual and skips until `unity/` holds a project.
