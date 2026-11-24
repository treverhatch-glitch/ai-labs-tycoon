# What's next

The roadmap after the foundation slice. Design is in [DESIGN.md](DESIGN.md); PC-only steps are in
[SETUP-PC.md](SETUP-PC.md). Items inside a group are in rough order. Sizes are in terms of what has to change, not
time.

## Where things stand

The rules core plays a whole game headless from 1995 or 2035: rooms, staff, GPUs, research, training,
releases into 34 sectors, five parody rivals, the yearly Gauntlet, incidents, bankruptcy, save, load, replay, nine
eras as data with their techs, rooms, sectors, headlines and dated world events, seven Envydia GPU generations, and a
world map of 9 blocs and 11 sites (land, halls, colo, power, heatwaves, export rules, government attention), a chip design screen (own chips, a shared foundry, listings, Envydia's answer) and rivals
that design and buy chips, plus endless play (endings that continue, frontier research, model museum) and the shady path (Heat, stash, investigations, corruption), government attention through Freeze, a Bubble permille meter (schema 12), safety incident rungs 1–6, rival public floats and regulator scrutiny (schema 13), shady depth and China deals (schema 14–18 decline cooldown), containment and superintelligence endings (schema 15), funding follow-ons (schema 16), and forward queue Q97–Q101 (schema 17). **683** tests pass locally on stack2 (pushed tip **658** at Opus bar; unpushed adds Treaty/Nationalised schema **24**, Q154–Q159 hatch follow-ups), including an 8-year balance soak (24 games × 3 presets by default), 40- and 60-year headless runs and saves through schema 20. There is no Unity project yet.

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
  use every command; the host UI is the missing half.
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

## Rules: Hatch (high priority)

1. ~~**AI bubble meter and crash.**~~ Done: schema **19**; `BubbleMeterCrashHatchTests` + `BubbleMeterHatchBarTests` (Hatch bar).
2. ~~**Acquire other labs.**~~ Done: schema **20**; `AcquireLab` / `AcquisitionSystem`; `AcquireLabLaterTests`.
3. ~~**Sell your own models.**~~ Done: `SellModel` / `ModelSalesSystem`; `SellModelLaterTests`.

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
11. ~~**Endings that continue (Q96).**~~ Done: continue modes for monopoly and superintelligence endings;
    `EndingsContinuationTailTests`; exits/ousted/bankrupt unchanged from phase E.

12. **The wider cast.** ~~Macrosoft, Metta and Palantear plans and join years are in (2005, 2012, 2017).~~ Still open:
    a start-era picker screen that shows each era's tagline and kit. Data and plans; no new systems.
13. **Make each era feel different.** Today an era changes what is available and what things cost. The bible's
    "the rules shift" (new laws, new risk classes, per-era mechanics) comes with items 5, 7 and 8.

## Rules: later

- ~~**Freeze permits and audit investigations (Q45 tail, Q89).**~~ Done: home Freeze blocks `QueueGpuPurchase`, `RaiseFunding`, and the rest of the site commands; `OnAuditOpen` respects `auditInvestigationPermille`; `FreezeAuditLaterTests`.
- ~~**Bubble meter (schema 12, Q90).**~~ Done: `BubbleConfig`, `BubbleSystem`, hooks on funding, chip listings, month roll and world events; `BubbleMeterTests`.
- ~~**Incident rungs 4–6 (Q91).**~~ Done: corruption thresholds, six rungs in data, relaxed validator; `IncidentRungsLaterTests`.
- ~~**Circular financing and shares; monopoly vs regulators (Q92).**~~ Done: rival `publicFloatPpm`, monthly IPO loop feeding the bubble, regulator scrutiny when the meter is hot and extra antitrust on monopoly; `SharesRegulatorsLaterTests`.
- ~~**Shady path depth and China deals (Q93).**~~ Done: player-only shady jobs, model corruption carry/mood/side jobs, China state requests and export vs smuggle; `ShadyChinaLaterTests`.
- ~~**Containment rooms and superintelligence ending gates (Q94).**~~ Done: containment-wing alignment bonus, year-end `TrySuperintelligence` for paperclipped / long-summer / loose; schema 15; `ContainmentSuperintelLaterTests`.
- ~~**Specialist** model lines, data sources, **open weights**, sector pricing.~~ Done on `cursor/rules-side-items` (PR #2); rules-forward does not reimplement.
- ~~**Funding ladder follow-ons (Q95).**~~ Done: down-round terms when the bubble is hot, player board lock and investor board visits; schema 16; `FundingFollowOnLaterTests`.
- Real **Bodies** and **Upward** expansions (mechanics): **owned by Unity thread** on `cursor/rules-side-items`; era **data** only on stack2.
- ~~**Treaty & Nationalised** ending triggers (systems-design §5.1)~~ **done (local)** — Q150–Q153; schema **24** `bubbleHadCrash`; `TreatyNationalisedLaterTests` (15); **683** tests at local tip.
- **Hatch follow-ups** (Opus review): **done (local)** — Q154–Q159; `HatchFollowUpLaterTests` (**41**).
- ~~**Hatch ledger lines** (Opus review)~~ **done (local)** — Q170; schema **28**; `HatchLedgerLaterTests` (8).
- ~~**Bubble capital reset after crash**~~ **done (local)** — Q171; `BubbleCapitalResetLaterTests` (7).
- ~~**Crash distress and M&amp;A gameplay**~~ **done (local)** — Q172–Q175; `HatchAcquisitionGameplayLaterTests` (**25**); golden unchanged (**758** tests).
- ~~**Rival distress in crash**~~ **done (local)** — Q176–Q179; `LabPlanner.Distress`, `HatchRivalDistressLaterTests` (**28**).
- ~~**Rival retrenchment soak**~~ **done (pushed)** — Q200; `LabPlanner.Retrenchment`, `RivalEconomyRobustnessLaterTests`; golden `15fc3234…`.
- ~~**Rival economy robustness (stack2)**~~ **done (pushed @ `477c3d2`)** — Q201 full bar (`RivalEconomyRobustnessLaterTests`); era `sectorFlavoursForRivals` forked to PRs #4–#10 (see Q201). **795** tests at PR #3 tip.
- ~~**Hatch economy bar batch 3**~~ **done (pushed @ `308463e`)** — Q202–Q214 + Q216/Q218 rival soak; schema **29** only (era owns **21–27**; **30** reserved for parked SolvencyHold). Golden `55e673a0…`.
- ~~**Hatch rivals/chips batch 7**~~ **pushed @ `28de997`** — Q233–Q262 consolidation cross-bars (**41** tests in `HatchRivalsChipsConsolidationLaterTests`).
- ~~**Hatch rivals/chips batch 8**~~ **pushed @ `3c4c8b7`** — Q263–Q292 (`HatchRivalsChipsDeliveryLaterTests`, **30**); schema **29** only.
- ~~**Hatch rivals/chips batch 9**~~ **pushed @ `6c4d155`** — Q293–Q330 (`HatchRivalsChipsCrossBarLaterTests`, **38**).
- ~~**Hatch rivals/chips batch 10**~~ **pushed @ `c58e395`** — Q331–Q365 (`HatchRivalsChipsIntegrateLaterTests`, **35**); schema **29** only; SolvencyHold **30** parked.
- ~~**Hatch rivals/chips batch 11**~~ **pushed @ `31e6c2a`** — Q366–Q400 (`HatchRivalsChipsTailLaterTests`, **35**).
- ~~**Hatch rivals/chips batch 12**~~ **pushed @ `ee78b4e`** — Q401–Q495 (`HatchRivalsChipsCapstoneLaterTests`, **95** grouped); schema **29** only.
- ~~**Hatch rivals/chips batch 13**~~ **pushed @ `3b3019e`** — Q496–Q565 (`HatchRivalsChipsCodaLaterTests`, **70**); schema **29** only.
- ~~**Hatch rivals/chips batch 14**~~ **pushed @ `3454140` (GO #3, Nov 22 ~15:12 UTC)** — Q566–Q665 (`HatchRivalsChipsEpilogueLaterTests`, **100**); parent `3b3019e`; gate **1269/0** / **1267/0** WT.
- ~~**Hatch rivals/chips batch 15**~~ **pushed @ `6827a12` (Nov 22 ≥15:42 UTC)** — Q666–Q735 (`HatchRivalsChipsFinisherLaterTests`, **70**); gate **1339/0** in-repo; parent `3454140`.
- ~~**Hatch rivals/chips batch 16**~~ **pushed @ `6f60391` (Nov 22 ~16:31 UTC)** — Q736–Q805 (`HatchRivalsChipsDenouementLaterTests`, **70**); parent `6827a12`; gate **1409/0** / **1407/0** WT.
- ~~**Hatch rivals/chips batch 17**~~ **pushed @ `c90e29b` (≥17:01 UTC GO)** — Q806–Q905 (`HatchRivalsChipsSequelLaterTests`, **100**); gate **1509/0** / **1507/0** WT.
- ~~**Hatch rivals/chips batch 18**~~ **pushed @ `4460d60`** — Q906–Q975 (`HatchRivalsChipsCoda2LaterTests`, **70**); parent `c90e29b`; gate **1579/0** / **1577/0** WT.
- ~~**Hatch rivals/chips batch 19**~~ **pushed @ `a61660c`** — Q976–Q1045 (`HatchRivalsChipsFinaleLaterTests`, **70**); parent `4460d60`; gate **1649/0** / **1647/0** WT.
- ~~**Hatch rivals/chips batch 20**~~ **pushed @ `a87d35b`** — Q1046–Q1115 (**70**); gate **1719/0** / **1717/0** WT; parent `a61660c`.
- ~~**Hatch era/cast batch 21**~~ **pushed @ `eaa0701`** — Q1116–Q1185 (**70**); gate **1789/0** / **1787/0** WT; parent `a87d35b`.
- ~~**Hatch era-feel batch 22**~~ **pushed @ `1a3b9db`** — Q1186–Q1255 (**70**); gate **1859/0** / **1857/0** WT; parent `eaa0701`.
- ~~**Hatch rival-arrival batch 23**~~ **pushed @ `b14e047`** — Q1256–Q1325 (**70**); gate **1929/0** / **1927/0** WT; parent `1a3b9db`.
- ~~**Hatch sector-rival batch 24**~~ **pushed @ `c98a033`** — Q1326–Q1395 (**70**); gate **1999/0** / **1997/0** WT; parent `b14e047`.
- ~~**Hatch funding/bubble batch 25**~~ **pushed @ `5da29a0`** — Q1396–Q1465 (**70**); gate **2069/0** / **2067/0** WT; parent `c98a033`.
- ~~**Hatch shady/china batch 26**~~ **pushed @ `f50900e`** — Q1466–Q1535 (**70**); gate **2139/0** / **2137/0** WT; parent `5da29a0`.
- ~~**Hatch freeze/audit batch 27**~~ **pushed @ `d05c9cc`** — Q1536–Q1605 (**70**); gate **2209/0** / **2207/0** WT; parent `f50900e`.
- ~~**Hatch incident/bubble batch 28**~~ **pushed @ `7fae71e`** — Q1606–Q1675 (**70**); gate **2279/0** / **2277/0** WT; parent `d05c9cc`.
- ~~**Hatch containment/ending batch 29**~~ **pushed @ `10f32fb`** — Q1676–Q1745 (**70**); gate **2349/0** / **2347/0** WT; parent `7fae71e`.
- ~~**Hatch treaty/ending batch 30**~~ **pushed @ `dee894f`** — Q1746–Q1815 (`HatchTreatyEndingBarLaterTests`, **70**); gate **2419/0** / **2417/0** WT; parent `10f32fb`.
- ~~**Hatch pop/second-place batch 31**~~ **pushed @ `0e5e13d`** — Q1816–Q1885 (`HatchPopSecondPlaceBarLaterTests`, **70**); gate **2489/0** / **2487/0** WT; parent `dee894f` (+ `c400fb5` test, `0e5e13d` docs).
- ~~**Hatch forward-queue batch 32**~~ **pushed @ `e3a5a6d`** — Q1886–Q1955 (`HatchForwardQueueBarLaterTests`, **70**); gate **2559/0** / **2557/0** WT; parent `0e5e13d` (+ `26258c2` test, `e3a5a6d` docs).
- ~~**Hatch balance/soak batch 33**~~ **pushed @ `2c15504`** — Q1956–Q2025 (`HatchBalanceSoakBarLaterTests`, **70**); gate **2629/0** / **2627/0** WT; parent `e3a5a6d` (+ `29d5c6c` test, `2c15504` docs).
- ~~**Hatch rival/plan batch 34**~~ **pushed @ `fc167aa`** — Q2026–Q2095 (`HatchRivalPlanBarLaterTests`, **70**); gate **2699/0** / **2697/0** WT; parent `2c15504` (+ `bae467b` test, `fc167aa` docs).
- ~~**Hatch acquisition bar batch 35**~~ **pushed @ `ec6f1a9`** — Q2096–Q2165 (`HatchAcquireGameplayBarLaterTests`, **70**); gate **2769/0** / **2767/0** WT; parent `fc167aa` (+ `4d1a26c` test, `ec6f1a9` docs).
- ~~**Hatch distress/M&amp;A batch 36**~~ **pushed @ `ae38c65`** — Q2166–Q2235 (`HatchDistressAcquireBarLaterTests`, **70**); gate **2839/0** / **2837/0** WT; parent `ec6f1a9` (+ `62d23d9` test, `ae38c65` docs).
- ~~**Hatch bubble/distress batch 37**~~ **pushed @ `0a9afc3`** — Q2236–Q2305 (`HatchBubbleDistressBarLaterTests`, **70**); gate **2909/0** / **2907/0** WT; parent `ae38c65` (+ `5e2aee5` test, `0a9afc3` docs).
- ~~**Hatch capital/bubble batch 38**~~ **pushed @ `b268afb`** — Q2306–Q2375 (`HatchCapitalBubbleBarLaterTests`, **70**); gate **2979/0** / **2977/0** WT; parent `0a9afc3` (+ `1f67dc0` test, `b268afb` docs).
- ~~**Hatch funding follow-on batch 39**~~ **pushed @ `f4a7e4f`** — Q2376–Q2445 (`HatchFundingFollowOnBarLaterTests`, **70**); gate **3049/0** / **3047/0** WT; parent `b268afb` (+ `d0e1303` test, `f4a7e4f` docs).
- ~~**Hatch model-sales/chip batch 40**~~ **pushed @ `4fdf674`** — Q2446–Q2515 (`HatchModelSalesChipBarLaterTests`, **70**); gate **3119/0** / **3117/0** WT; parent `f4a7e4f` (+ `95bf36d` test, `4fdf674` docs).
- ~~**Hatch acquisition/listing batch 41**~~ **pushed @ `dcf79d8`** — Q2516–Q2585 (`HatchAcquireListingBarLaterTests`, **70**); gate **3189/0** / **3187/0** WT; parent `4fdf674` (+ `1507d5b` test, `dcf79d8` docs).
- ~~**Hatch rival/economy batch 42**~~ **pushed @ `38f5385`** — Q2586–Q2655 (`HatchRivalEconomyBarLaterTests`, **70**); gate **3259/0** / **3257/0** WT; parent `dcf79d8` (+ `1e58224` test, `38f5385` docs).
- ~~**Hatch world-map economy batch 43**~~ **pushed @ `5937e90`** — Q2656–Q2725 (`HatchWorldMapEconomyBarLaterTests`, **70**); gate **3329/0** / **3327/0** WT; parent `38f5385` (+ `d01f62b` test, `5937e90` docs).
- ~~**Hatch shady/china batch 44**~~ **pushed @ `32fc117`** — Q2726–Q2795 (`HatchShadyChinaBarLaterTests2`, **70**); gate **3399/0** / **3397/0** WT; parent `5937e90` (+ `53261ee` test, `32fc117` docs).
- ~~**Hatch incident/safety batch 45**~~ **pushed @ `6dbc3ac`** — Q2796–Q2865 (`HatchIncidentSafetyBarLaterTests`, **70**); gate **3469/0** / **3467/0** WT; parent `32fc117` (+ `5d681e6` test, `6dbc3ac` docs).
- ~~**Hatch containment/ending batch 46**~~ **pushed @ `77ff014`** — Q2866–Q2935 (`HatchContainmentEndingBarLaterTests2`, **70**); gate **3539/0** / **3537/0** WT; parent `6dbc3ac` (+ `63295a9` test, `77ff014` docs).
- ~~**Hatch regulators/bubble batch 47**~~ **pushed @ `e0b3311`** — Q2936–Q3005 (`HatchRegulatorBubbleBarLaterTests`, **70**); gate **3609/0** / **3607/0** WT; parent `77ff014` (+ `2c53e0d` test, `e0b3311` docs); Q200+golden **13/0** at tip prepush.
- ~~**Hatch era/incident batch 48**~~ **pushed @ `d0aca54`** — Q3006–Q3075 (`HatchEraIncidentBarLaterTests`, **70**); gate **3679/0** / **3677/0** WT.
- ~~**Hatch exit/sell batch 49**~~ **pushed @ `69e7282`** — Q3076–Q3145 (`HatchExitSellBarLaterTests`, **70**); gate **3749/0** in-repo; landed ~19:11 UTC.
- ~~**Hatch bubble/acquire batch 50**~~ **pushed @ `afd2f0a`** — Q3146–Q3215 (`HatchBubbleAcquireBarLaterTests`, **70**); gate **3819/0** in-repo; parent `69e7282` (+ `386df26`, `afd2f0a`).
- ~~**Hatch balance/soak batch 51**~~ **pushed @ `b4186b0`** — Q3216–Q3285 (`HatchBalanceSoakBarLaterTests2`, **70**); gate **3889/0** in-repo; parent `afd2f0a` (+ `66ff0bd`, `b4186b0`).
- ~~**Hatch rival/plan batch 52**~~ **pushed @ `b5d8265`** — Q3286–Q3355 (`HatchRivalPlanBarLaterTests2`, **70**); gate **3959/0** in-repo; parent `b4186b0` (+ `3ca55e4`, `b5d8265`); landed ~21:23 UTC.
- ~~**Hatch funding/follow-on batch 53**~~ **pushed @ `b661d1f`** — Q3356–Q3425 (`HatchFundingFollowOnBarLaterTests2`, **70**); gate **4029/0** in-repo; parent `b5d8265` (+ `ce6ace8`, `b661d1f`); landed ~21:53 UTC.
- ~~**Hatch model-sales/chip batch 54**~~ **pushed @ `a62fc4a`** — Q3426–Q3495 (`HatchModelSalesChipBarLaterTests2`, **70**); gate **4099/0** in-repo; parent `b661d1f` (+ `230bfa8`, `b93c35f`, `a62fc4a`); landed ~22:42 UTC.
- ~~**Hatch world-map/economy batch 55**~~ **pushed @ `14428ec`** — Q3496–Q3565 (`HatchWorldMapEconomyBarLaterTests2`, **70**); gate **4169/0** in-repo; parent `a62fc4a` (+ `8e17df7`, `182ed11`, `14428ec`); landed coordinator GO ~23:27 UTC.
- ~~**Hatch regulator/bubble batch 56**~~ **pushed @ `3f88c5b`** — Q3566–Q3635 (`HatchRegulatorBubbleBarLaterTests2`, **70**); gate **4239/0** in-repo; parent `14428ec` (+ `6159ece`, `3f88c5b`); landed GO ~00:18 UTC Nov 24.
- **Hatch acquisition/listing batch 57 (LOCAL commit-ready — NO PUSH until GO ≥ 00:48 UTC / ~07:48 ICT on parent `3f88c5b`)** — Q3636–Q3705 (`HatchAcquireListingBarLaterTests2`, **70**); (+ `8bc8dd0` test, docs @ tip); filter **70/0**; five-slot bar (distress price, in-house bonus, fleet lock, min offer/architect weeks, runtime distress); expected full gate **4309/0** at push; schema **29** only.
- **Hatch rival/economy batch 58 (LOCAL — hold after batch 57 land)** — Q3706–Q3775 (`HatchRivalEconomyBarLaterTests2`, **70**); parent queued @ `3f88c5b` until **57** pushes; filter **70/0** green; **Q44** hardware neighbour; schema **29** only.
- **Rivals hatch bar forward queue (bible / stack2 forks, test bar per item):**
  - **batch 59+ (TBD)** — shady/china coda, incident/safety, containment coda from backlog; SolvencyHold / schema **30** parked.
- **Refill:** store bible backlog + batch19 refill; batch **57** **D7** listings; batch **58** **Q44** rival hardware refresh + crown rollup neighbours; batch **59+** shady/china Q102 tails; schema **29** only.

## Rules: forward queue (from bible / systems-design)

- ~~**The Pop** ending when the bubble crashes on the player (Q97).~~ Done: `BubbleConfig.thePopEndingForPlayer`; `ThePopEndingLaterTests`.
- ~~**Second Place** ending when the player is crown runner-up N years (Q98).~~ Done: `crownRunnerUpStreak`; schema 17.
- ~~**Exit sell-price breakdown** (Q99).~~ Done: `DisplayQueries.ExitPriceParts`.
- ~~**Era incident load** shifts risk by era (Q100).~~ Done: `incidentLoadPermille` on eras.
- ~~**60-year headless soak** pins long-horizon stability (Q101).~~ Done: `LongHorizonSoakLaterTests` sixty-year step.

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
