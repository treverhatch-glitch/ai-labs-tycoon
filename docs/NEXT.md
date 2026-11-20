# What's next

The roadmap after the foundation slice. Design is in [DESIGN.md](DESIGN.md); PC-only steps are in
[SETUP-PC.md](SETUP-PC.md). Items inside a group are in rough order. Sizes are in terms of what has to change, not
time.

## Where things stand

The rules core plays a whole game headless from 1995 or 2009 to 2035: rooms, staff, GPUs, research, training,
releases into 34 sectors, three parody rivals, the yearly Gauntlet, incidents, bankruptcy, save, load, replay, nine
eras as data with their techs, rooms, sectors, headlines and dated world events, seven Envydia GPU generations, and a
world map of 9 blocs and 11 sites (land, halls, colo, power, heatwaves, export rules, government attention). 233 tests
pass, including 40-year headless runs and a schema 1 and a schema 2 save that migrate. There is no Unity project yet.

Known limits of the slice, so nobody is surprised:

- **Eras 2 to 8 are first-pass content.** Every era has techs, sectors, rooms, headlines and events, and the
  autopilot reaches the end of the tree by 2034. Names, blurbs and the numbers are tuned to keep a whole life
  solvent, not yet to be fun at each step: the bot-balance item below is how that gets checked. The content tables are in
  [content/](content/eras.md); [QUESTIONS.md](QUESTIONS.md) lists the choices taken.
- **Training cost per era is retuned for GPU generations.** Hardware generations landed, so the era multipliers were
  scaled down by the compute-per-dollar they add (Q35). Late-game cash is much higher than it was; the bot-balance
  item below is how that gets checked.
- **Some rooms and every role beyond the first four have no special rule yet.** New roles fill one of the four
  functions; a room is built for what it seats or holds.
- **Rivals are still the three parody labs** across all nine eras; the wider cast (item 12 below) is not in.
- **Retired models stay in the lab's model list** for the whole game. Fine for decades of play (a 40-year test keeps
  the save in bounds), and worth an archive once the model museum is designed.
- **The world map is in, the plans barely use it.** Only xAy builds data centres (OpenAye and AntHopticsAI rent), and no
  plan replaces obsolete hardware, so a rival's fleet stops growing once its halls fill with old generations (Q44).
  The player can use every command; the host UI is the missing half.
- **Staff are head counts**, not people.
- **One incident kind.** The six-rung ladder, the alignment gap and endings are not in yet.
- **Money is cash and a grant.** No investors, shares, debt or bubble.
- **Rivals follow plans, they do not explain themselves.** The plan runs through the same commands as the player, so
  a decision log is an addition to `LabPlanner`, not a rewrite.

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
3. **Explain every number.** A breakdown for each displayed value (appeal, quality, revenue, incident odds) whose
   parts sum to the value, with a test per breakdown. New read-only query classes; no state change.
4. **Rival decision logs.** `LabPlanner` records why it did each thing; the log is bounded and saved. Planned as
   phase C of the systems design, with Envydia's standing and the feelings rivals have toward the player.
5. **Reserved compute and the Envydia queue.** Reduced to reserved contracts and allocation by standing with Envydia:
   the hardware generations it needed (item 6) are in, so what remains is the contract and the queue. Touches
   `ComputeConfig`, lab state, `Finance`.
6. ~~**Hardware generations.**~~ Done (systems design phases 0 and A): `HardwareBatch` rows by generation and place,
   seven Envydia generations as data, resale that falls a generation at a time, `BuyGpus`/`SellGpus`/`MoveHardware`
   to a holding, the fleet migration (schema 2).
7. **The funding ladder.** Angels and two venture rounds with dilution and a board; the Ousted ending. New system
   and state.
8. **The alignment gap and incident rungs 1 to 3**, per model, replacing the single incident kind. Sandbagging
   corrupts displayed readings, so it lands after item 3.
9. **Staff as people.** Traits, morale, poaching by rivals. Changes how `StaffPool` is stored, so it needs a save
   migration (the `SaveGame.Migrate` hook is there for it).
10. **Bot balance at scale.** `simcli soak` over 200 games and 3 player presets as a test: no preset wins more than
    60%, a rival survives in 95% of games, money still matters in year 8.
11. **Endings that continue.** Bankrupt, Ousted and Exit as recorded moments after which play goes on. Planned as
    phase F of the systems design (an Exit keeps the lab as a plan-run rival, Q24).

12. **The wider cast.** The other labs and companies named in the bible (Goggle DeepMined, Macrosoft, Metta,
    DeepSneak, Palantear) as rival plans that enter in later eras, plus a start-era picker screen that shows each
    era's tagline and kit. Data and plans; no new systems.
13. **Make each era feel different.** Today an era changes what is available and what things cost. The bible's
    "the rules shift" (new laws, new risk classes, per-era mechanics) comes with items 5, 7 and 8.

## Rules: later

- Custom chips: the chip design screen's rules (generations 1 to 4, a foundry deal, selling chips to others).
- Chips, the wider cast, endings and the shady path from the systems design (phases C to G). Its first PR landed the
  world map: ~~data-centre sites with power, cooling, export rules and government attention~~ (schema 3). What is left of
  it: Audit's investigation roll and Freeze consequences beyond blocking purchases come with the shady path (Q45).
- The Bubble meter and circular financing; shares in rivals; monopoly and regulators.
- The shady path with per-model corruption; government and China deals.
- Incident rungs 4 to 6, containment rooms, the superintelligence endings.
- Specialist model lines, data sources, open weights, pricing.
- Real Bodies and Upward expansions (the data for both eras is already in, flagged `expansion`; what is missing is their own mechanics and art).

## Unity

Needs the owner's PC ([SETUP-PC.md](SETUP-PC.md)).

1. Create the Unity 6 project in `unity/` with `Rules/` as a local package. No rules in Unity scripts: Unity sends
   `Command`s and draws `GameState`.
2. A first scene built in code: the garage lot as a grid, rooms as coloured blocks, a week button, cash and date.
3. Room building with the mouse; the hire and GPU panels; the training-run screen; the Gauntlet show.
4. The model mascots and their three states; the glow.
5. The land script and its entry in `studio/game.json`, so the harness can land Unity branches.

## Studio and CI

- Create the studio on the PC and copy `studio/game.json` into it (SETUP-PC.md).
- Beta mode is the owner's switch in the studio; it is off until the owner turns it on (SETUP-PC.md).
- The `unity` workflow is manual and skips until `unity/` holds a project.
