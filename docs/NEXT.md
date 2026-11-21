# What's next

The roadmap after the foundation slice. Design is in [DESIGN.md](DESIGN.md); PC-only steps are in
[SETUP-PC.md](SETUP-PC.md). Items inside a group are in rough order. Sizes are in terms of what has to change, not
time.

## Where things stand

The rules core plays a whole game headless from 1995 or 2035: rooms, staff, GPUs, research, training,
releases into 34 sectors, five parody rivals, the yearly Gauntlet, incidents, bankruptcy, save, load, replay, nine
eras as data with their techs, rooms, sectors, headlines and dated world events, seven Envydia GPU generations, and a
world map of 9 blocs and 11 sites (land, halls, colo, power, heatwaves, export rules, government attention), a chip design screen (own chips, a shared foundry, listings, Envydia's answer) and rivals
that design and buy chips, plus endless play (endings that continue, frontier research, model museum) and the shady path (Heat, stash, investigations, corruption), government attention through Freeze, a Bubble permille meter (schema 12), safety incident rungs 1–6, rival public floats and regulator scrutiny (schema 13), shady depth and China deals (schema 14–18 decline cooldown), containment and superintelligence endings (schema 15), funding follow-ons (schema 16), and forward queue Q97–Q101 (schema 17). 1112 tests pass (schemas 19–27: bubble crash, acquisitions, model sales, risk classes, founding choices, poach offers, press questions, Gauntlet tuning and awards, sector trust; the workplace needs no schema), including an 8-year balance soak (24 games × 3 presets by default), 40- and 60-year headless runs and saves through schema 27. There is no Unity project yet.

Known limits of the slice, so nobody is surprised:

- **Eras 2 to 8 are first-pass content.** Every era has techs, sectors, rooms, headlines and events, and the
  autopilot reaches the end of the tree by 2034. Names, blurbs and the numbers are tuned to keep a whole life
  solvent, not yet to be fun at each step; the eight-year balance soak is the first check. The content tables are in
  [content/](content/eras.md); [QUESTIONS.md](QUESTIONS.md) lists the choices taken.
- **Training cost per era is retuned for GPU generations.** Hardware generations landed, so the era multipliers were
  scaled down by the compute-per-dollar they add (Q35). Late-game cash is still higher than at launch; Q76 retuned
  dial-up costs and wealth growth for the eight-year soak.
- **Role perks are in (Q128 to Q133):** recruiter, red-teamer, alignment fellow, product lead and the four late specialists do more than fill a desk (`rolePerks`, [content/role-perks.md](content/role-perks.md)). The data roles have none yet (they wait for data sources); the later rooms do something too (Q134 to Q136, [content/room-perks.md](content/room-perks.md)): research, training speed, incident catch and robot appeal, per tile under a cap.
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

12. **The wider cast.** ~~Macrosoft, Metta and Palantear plans and join years are in (2005, 2012, 2017).~~ Start-era
    selection is in (rules half, Q118 to Q122): every era is startable with an era-appropriate cast, cash, lot and techs, and
    `StartEras.Offers` feeds a picker. Still open: the picker screen itself.
13. ~~**Make each era feel different.**~~ Done (Q104 to Q109 and Q127): `EraRules`, five era rules beside incident load (talent-war
    hiring fees, chip prices, customer appetite, fine size, and a release law on displayed alignment), the briefing
    and shifts queries, a rules line on the era banner, no save change. Risk classes per era are in (Q110 superseded by
    Q123 to Q126, save schema 21). Still open: per-era mechanics that need new state (they ride on items 7 and 8).

14. ~~**Founding choices (bible ch. 5).**~~ Done (Q137 to Q140, save schema **22**): four founders (Professor, Dropout, Defector, Heir), three
    strategy styles (Sniper, Shotgun, Wildcard) and a home-base option, chosen in `NewGameOptions` (`simcli --founder --style --home`);
    founders change the start and carry a lasting edge and cost, styles lift appeal in their sectors and lower it elsewhere
    ([content/founding.md](content/founding.md)). Still open: the picker screen (Unity), and the Hardware Baron, Open-Weights Rebel
    and Garage Hacker presets (they wait for chips, open weights and shady offers).
15. ~~**Talent market (bible ch. 7, "poaching is constant").**~~ Done (Q141 to Q144, save schema **23**): a rival's poach is an offer that waits
    4 weeks (`poachOffers` on the lab); the player counters with `money`, `compute`, a `title` or the `mission` (each a cost and a chance, taken
    apart in `ChanceParts`), or lets the person go; stars cost twice the package; `PoachStaff` raids a rival's people for a fee
    ([content/talent-market.md](content/talent-market.md)). Planners and the autopilot never answer, so autopilot labs lose people late.
    Still open: the offers and counters screen (Unity) and a suggestion that prompts the player.
16. ~~**The press and reputation (bible ch. 14, the press room).**~~ Done (Q145 to Q149, save schema **25**): a `press-room` (The Punchline) and
    an `auditorium` (The Chat Boom) dampen scandal and sharpen launches; three journalists ask about four beats (launch, incident, hearing,
    crown); the player answers with `honest`, `spin` or `exclusive` or lets the question lapse; `LeakStory` turns the press on a rival
    ([content/press.md](content/press.md)). Planners, autopilot and rivals do not use it. Still open: the press desk screen (Unity),
    rivals that leak at the player.

17. ~~**The Gauntlet, deeper (bible ch. 8, "teach to the test").**~~ Done (Q160 to Q164, save schema **26**): the player sets a lab-wide tuning
    that lifts every score for a prep bill, a tuned win can be exposed (more likely with feuding rivals, and the win can then go to a rival),
    each crown entry takes a live demo decided by its reliability, two joke awards ("Most Confident While Wrong", "Best Newcomer") carry real
    effects, and `GauntletSystem.SeasonTable` ranks labs across the kept years ([content/gauntlet.md](content/gauntlet.md)). Rivals never tune.
    Still open: a playable demo scene and tuning screen (Unity), rivals that tune and get exposed.
18. ~~**Trust and the customer desk (bible ch. 8, "trust is slow to win and fast to lose").**~~ Done (Q165 to Q169, save schema **27**): a trust
    meter per lab per sector (+2 a week while selling, +40 for a bracket win, -60 for an incident, -100 for an exposed tuning) that carries
    30% to the other sectors of the same family and moves appeal by up to 15% either way; an incident writes a complaint to the player's desk,
    answered with a refund, a patch or an apology ([content/trust.md](content/trust.md)). Rivals have trust but no desk. Still open: the
    customer desk screen and a trust display (Unity), rivals that answer complaints.
19. ~~**The workplace (bible ch. 14 room list and ch. 7, "morale, retention").**~~ Done (Q180 to Q183, no schema change): three Comfort rooms
    (`coffee-corner`, `kitchen`, `nap-pods`) lift the level morale settles at (up to +150) and new hires start there; the kitchen and the
    recruiting office take a share off a rival's weekly offer chance (up to 60%); researchers above the baseline do up to 20% more
    ([content/workplace.md](content/workplace.md)). Low morale does not slow research in the shipped data (Q182: a loss slope exists, set to 0).
    Still open: hiring speed from the recruiting office (Q183), a workplace screen (Unity), and a rival that is harder to push into
    bankruptcy so the loss slope can be switched on.

20. ~~**Staff traits (bible ch. 7, "trait ideas").**~~ Done (Q184 to Q187, no schema change): the player's hires can arrive as a 10X, a Doomer, an
    Accelerationist, a Mentor, someone who actually reads the logs, someone secretly interviewing, or someone who sleeps under the desk; each
    does one thing for the lab, and a lab with both a Doomer and an Accelerationist starts arguing ([content/staff-traits.md](content/staff-traits.md)).
    Rivals keep `steady` and `star`. Still open: a staff screen that shows traits (Unity), rivals with traits, "posts through it" and
    "will quit by blog post" (they need a press hook on poaching).
21. **Sector incident flavours (bible ch. 8, "what goes wrong").** Each sector fails in its own way (the support bot promises a refund
    policy you do not have, the legal tool cites cases that do not exist); a flavour sets how hard an incident hits there and what it
    costs beyond reputation. ~~Done~~: `safety.sectorFlavours` (18 sectors), `SectorFlavours`, hooks in `SafetySystem`, `TrustSystem` and the quality
    glitch in `DisplayQueries`; refunds on the fines line; player-only costs behind `safety.sectorFlavoursForRivals` (Q188-Q190); no schema change;
    `SectorFlavourTests`, `docs/content/sector-flavours.md`.
22. **Safety culture and release gates (bible ch. 10, "release gates", "culture").** A culture meter that rises when the lab listens and
    falls when it rushes, and a gate the player sets for themselves and then has to obey. ~~Done~~: `culture` in the data, `SafetyCulture`,
    `SetReleaseGate`, culture adds to the incident catch, save schema 29 (`cultureShift`, `releaseGate`, `gateSetTick`; Q191-Q194),
    `SafetyCultureTests`, `docs/content/safety-culture.md`. Still open: a screen for both (Unity), rivals with a culture, a culture effect on staff morale.

23. **Version naming rot (bible ch. 6, "joke that is also a system").** Model names grow more confusing with every version (suffixes stack
    up: Pro, Max, Turbo, Ultra); customers are measurably confused if the lab lets it rot; clean naming costs a little marketing effort.
    ~~Done~~: `naming` in the data, `VersionNaming`, `CleanUpNaming`, a confusion part on appeal, save schema 30 (`namingMess`; Q195-Q197),
    `VersionNamingTests`, `docs/content/version-naming.md`. Player labs only. Still open: a naming screen (Unity), rivals with confusing names, renaming old models.
24. **The company chat and the glow (bible ch. 14 "company chat feed", ch. 10 "the glow", "truth through the walls").** Staff react to the
    lab's decisions in character, and the building shows the worst model's risk by rung, including when a sandbagging model corrupts the
    dashboard. ~~Done~~: `chat` and `glow` in the data, `CompanyChat.Feed`, `Glow.Of`, no schema change and no rule effect (Q198-Q199),
    `CompanyChatTests`, `GlowTests`, `docs/content/company-chat-and-glow.md`. Still open: the news ticker headlines, the chat as a Unity panel,
    the mascot face on screens (art), chat lines for more event kinds.
25. **Rooms that age (bible ch. 14, "old rooms go out of date").** A room built in an earlier era goes beige and dusty; staff in it complain
    (morale) until the lab refurbishes it for a fraction of the build cost. ~~Done~~: `ageing` in the data, `RoomAge`, `RefurbishRoom`,
    extra upkeep and a morale penalty from the dated stage, save schema 31 (`eraAge` per room; Q230-Q232), `RoomAgeTests`,
    `docs/content/room-ageing.md`. Player labs only. Still open: a refurbish button and a room tint (Unity), rivals with old rooms, room redesigns on refurbish.
26. **Control goes stale (bible ch. 10, "what erodes it").** Each new era, the control techniques of older eras count for less until the
    lab does new safety research; ignoring or firing the safety team shows up in the lab's culture. ~~Done~~: `control` in the data,
    `ControlStale`, alignment work uses the stale bonus, `ControlStale`/`ControlRefreshed` events, a culture cost for firing the listeners,
    no schema change (Q233-Q235), `ControlStaleTests`, `docs/content/control-stale.md`. Player labs only. Still open: a screen for it (Unity),
    rivals going stale, the stale readout on finished models.

27. **Research leads from lucky runs (bible ch. 6, "that mystery becomes a research lead").** A run that comes out far better than aimed
    for leaves a lead the lab can follow for research, a lasting training speed-up, or a joke. ~~Done~~: `leads` in the data, `ResearchLeads`,
    `FollowLead`, `LeadFound`/`LeadResolved`/`LeadWentCold`, save schema 32 (`researchLeads`, `leadSpeedPermille`; Q236-Q238),
    `ResearchLeadTests`, `docs/content/research-leads.md`. Player labs only. Still open: a leads screen (Unity), rivals with leads, leads that
    name a tech to unlock, the autopilot following leads.

28. **Fine-tune and rollback for glitching models (bible ch. 6 state table: "fix with fine-tune or rollback").** ~~Done~~: `repair` in the
    data, `ModelRepair`, `FineTuneModel` and `RollBackModel`, `ModelFineTuned`/`ModelRolledBack` events, no schema change (Q239-Q241),
    `ModelRepairTests`, `docs/content/model-repair.md`. Still open: buttons on the model screen (Unity), fixing a corrupted or sandbagging model
    (these need the shady and sandbag rules), the autopilot mending its models.

29. **Office events with a choice (bible ch. 14 "Event ideas").** ~~Done~~: `office` in the data (eleven events, three answers each), `OfficeEvents`,
    `AnswerOfficeEvent`, `OfficeEventPosed`/`OfficeEventAnswered`/`OfficeEventLapsed`, save schema 33 (`officeEventId`, `officeEventExpires`,
    `officeSeen`; Q242-Q245), `OfficeEventTests`, `docs/content/office-events.md`. Player labs only. Still open: an office-desk screen (Unity),
    more events (needs for a data centre, a chip, a merger), events with effects that last, rivals with events.

30. **Training objective (bible ch. 6 recipe: correct / engaging / obedient / careful).** ~~Done~~: `objectives` in the data,
    `TrainingObjectives`, `SetTrainingObjective`, `ObjectiveSet` event, appeal breakdown line, save schema 34 (`objective` per model; Q246-Q248),
    `TrainingObjectiveTests`, `docs/content/training-objectives.md`. Still open: a recipe-screen row (Unity), a disposition meter, objectives for rivals.

31. **A model helps design its successor (bible ch. 10 "what erodes it").** ~~Done~~: `successor` in the data, `SuccessorDesign`,
    `DesignWithModel`, `SuccessorDesigned` event, save schema 35 (`designerId` per model; Q249-Q251), `SuccessorDesignTests`,
    `docs/content/successor-design.md`. Still open: a button on the recipe screen (Unity), a takeoff incident from long chains, rivals doing it.

32. **Release testing and the rushed launch (bible ch. 6, ch. 10).** ~~Done~~: `rush` in the data, `RushedLaunch`, `RushRelease`,
    `ModelRushed` event, appeal `LaunchEffect` and incident `FromRush` lines, save schema 36 (`rushWeeks` per model; Q252-Q254),
    `RushedLaunchTests`, `docs/content/rushed-launch.md`. Still open: a rush slider on the release screen (Unity), rivals rushing to beat the player to a sector.

33. **News ticker headlines (bible ch. 14, ch. 17).** ~~Done~~: `news` in the data, `NewsTicker.Headlines` / `About` / `Money`, 14 headline lines and 8 filler lines,
    read-only, no save change (Q255-Q257), `NewsTickerTests`, `docs/content/news-ticker.md`. Still open: the ticker strip itself (Unity), headlines for the player's
    own big moments in the bible's voice, era-specific filler.

34. **Lobbying (bible ch. 12, ch. 14 rooms).** ~~Done~~: `lobbying` in the data, `Command.Lobby` (a quiet word or a subsidy push, per government), `Lobbying` system,
    `EventKind.Lobbied`, save schema 37 (`lobbyUntilTick` per standing; Q258-Q259), `LobbyingTests`, `docs/content/lobbying.md`. Still open: choosing which law a
    government writes next, the lobby screen (Unity).

35. **Speciality pivot (bible ch. 3, ch. 5).** ~~Done~~: `pivot` in the data, `Command.PivotStyle` (once per era; staff leave, money, a year of confused customers), `Pivot`
    system, `PivotConfusion` line in the appeal breakdown, `EventKind.StylePivoted`, save schema 38 (Q260-Q261), `PivotTests`, `docs/content/pivot.md`. Still open: the pivot
    screen (Unity), what a pivot does to the founder's edge.

36. **More office events (bible ch. 14 event ideas, "more events").** ~~Done~~: eight more events in `office` (nineteen in all), `OfficeEffectKind.Attention`,
    no save change (Q262-Q263), tests added to `OfficeEventTests`, `docs/content/office-events.md`. Still open: events that need a rival, a chip deal or a data centre
    to exist, and answers that change a rival's feelings.

37. **Campaign era goals and scoring (bible ch. 13).** ~~Done~~: `campaign` in the data (27 goals, three per era), `Campaign` system (`Progress`, `Mark`, `RunScore`, `OnEraBegan`),
    `EraScore` saved on the player's lab, `EventKind.EraScored`, save schema 39 (Q264-Q266), `CampaignTests`, `docs/content/campaign.md`. Still open: the scoreboard (Unity),
    a campaign deadline, an ending bonus, difficulty scaling.

38. **Experimental runs and checkpoints (bible ch. 6 recipe table, "running it").** ~~Done~~: `runs` in the data, `Command.SetRunApproach` / `SetCheckpoints`,
    `TrainingRuns` system (failure hash, loss, weekly cost, capability bonus), `EventKind.TrainingFailed` / `RunSetUp`, save schema 40 (Q267-Q269), `TrainingRunTests`,
    `docs/content/training-runs.md`. Still open: the recipe-screen rows (Unity), other failure causes, rivals trying variants.
39. **Engineers cut failed runs (bible ch. 7 roles).** ~~Done~~: `runs.engineerSavingPermillePerStaff` / `engineerSavingCapPermille`, `TrainingRuns.EngineerSavingPermille`, failure chance now takes
    the lab (Q270-Q271), four tests added to `TrainingRunTests`; no schema, golden unchanged.
40. **Hearing questions (bible ch. 12 hearings).** ~~Done~~: `OfficeTrigger` on office events, `OfficeEvents.OnHearing`, four hearing questions with three answers each in the data
    (Q272-Q274), `HearingQuestionTests`, `docs/content/hearing-questions.md`; no schema, golden unchanged. Still open: a share-price effect, hearings abroad, questions that depend on the lab's history.
41. **Staff traits round 2 (bible ch. 7 trait ideas, ch. 14 "leave in protest").** ~~Done~~: `posts-through-it`, `quits-by-blog-post`, `only-interesting` in the data, `Traits.RunQuits`,
    `EventKind.StaffQuit`, buzz and scandal sums in `TraitReport`, `interestingPermille` in the research sum (Q275-Q277), `MoreTraitsTests`, `docs/content/staff-traits.md`; no schema; golden moved with a verified cause.
42. **Chip-backed loans (bible ch. 9 funding ladder).** ~~Done~~: `loans` in the data, `Command.TakeLoan` / `RepayLoan`, `Loans` system (room, weekly payments, misses, default and repossession),
    `Loan` on the lab, `EventKind.LoanTaken` / `LoanRepaid` / `LoanMissed` / `LoanDefaulted`, save schema 41 (Q278-Q280), `LoanTests`, `docs/content/loans.md`. Still open: loans against own chips, a margin call, exit prices that count debt, a screen (Unity).
43. **The escape-attempt crisis (bible ch. 10 incident ladder, rung 5).** ~~Done~~: `OfficeTrigger.Escape`, `OfficeEventConfig.lapseChoice`, `OfficeEffectKind.Corruption`, `OfficeEvents.OnEscape`,
    the `escape-attempt` event with three answers (Q281-Q283), `EscapeCrisisTests`, `docs/content/escape-crisis.md`; no schema, golden unchanged. Still open: a real gamble, a rung-6 crisis, one-site shutdowns.
44. **Scenarios (bible ch. 15 "start as a named archetype in a specific year").** ~~Done~~: `scenarios` in the data (seven named starts), `Scenarios.List` / `Options` / `Start`, the validator (Q284-Q285),
    `ScenarioTests`, `docs/content/scenarios.md`; no schema, golden unchanged. Still open: starting situations, per-scenario goals, the picker (Unity), the 1956 prologue.
45. **Rivals pay sector flavours (Q190 switch).** ~~Done~~: `safety.sectorFlavoursForRivals` ships true after a forty-year soak on a throwaway merge with the rival-robustness work (Q286); the sector-flavours page says every lab pays; tests set the switch explicitly where they want the plain way. No schema, golden unchanged.
46. **More office events, round three (bible ch. 14 "event ideas", v0.2 headlines).** ~~Done~~: six events (a goodbye, a rival's Friday, what "off" means, the reply, the first chip benchmark, the landlord's note), `OfficeRequires.OwnChip` and `RentedCompute` (Q287), `MoreOfficeEventsTests`, `docs/content/office-events.md`. No schema.
47. **Foundry snub (bible ch. 7 chip risk events).** ~~Done~~: `chips.snubStandingBelow`/`snubChancePermille`/`snubWeeks`, `ChipSystem.SnubWeeks`, `EventKind.ChipSlotSnubbed`, `FoundrySnubTests`, `docs/content/chips.md` (Q288). No schema.
48. **Overload costs reliability (bible ch. 6 model states, "overworked: serving load too high; reliability drops").** ~~Done~~: `safety.overloadIncidentPermille` / `overloadForRivals`, `SafetySystem.OverloadPermille` and `WeeklyChancePermille`, `IncidentBreakdown.FromOverload`, `OverloadTests`, `docs/content/overload.md` (Q289). No schema.
49. **Release notes (bible ch. 14 "its release notes").** ~~Done~~: `OfficeTrigger.Release`, `OfficeEvents.OnRelease`, three events, `ReleaseNotesTests`, `docs/content/office-events.md` (Q290). No schema.
50. **Research experiments, the third share of compute (bible ch. 3 "split compute three ways").** ~~Done~~: `SetResearchShare`, `lab.researchSharePermille` (schema 42, `MigrateTo42`), `research.maxComputeSharePermille` / `computeShareBonusPermille`, `ResearchShareTests`, `docs/content/research-share.md` (Q291).
51. **Era-flavoured office events (bible "events by era").** ~~Done~~: `fromYear` / `toYear` on `OfficeEventConfig`, `OfficeEvents.InYears`, six events, `EraOfficeEventsTests`, `docs/content/office-events.md` (Q292).
52. **Era-flavoured filler headlines (bible ch. 12 "the ticker").** ~~Done~~: `toYear` on `FillerConfig`, `NewsTicker.PickFiller`, 18 new lines, `EraFillerTests`, `docs/content/news-ticker.md` (Q293).
53. **Era-flavoured company chat (bible ch. 10).** ~~Done~~: `fromYear` / `toYear` on `ChatLineConfig`, `CompanyChat.Fits`, seven lines, `EraChatTests`, `docs/content/company-chat-and-glow.md` (Q294).
54. **Headlines for chip and loan events.** ~~Done~~: eight lines in `news.headlines`, `ChipLoanNewsTests`, `docs/content/news-ticker.md` (Q295).
55. **Chat lines for chip and loan events.** ~~Done~~: ten lines in `chat.lines`, `ChipLoanChatTests`, `docs/content/company-chat-and-glow.md` (Q296).
56. **Headlines for press, poaching and Gauntlet events.** ~~Done~~: five lines in `news.headlines`, `PressNewsTests`, `docs/content/news-ticker.md` (Q297).
57. **Crunch time (staff).** ~~Done~~: `SetCrunch`, `lab.crunchWeeks` (schema 43, `MigrateTo43`), `Crunch`, `staffPeople.crunch*`, `CrunchEnded`, `CrunchTests`, `docs/content/crunch-time.md` (Q298).
58. **Incident insurance (finance).** ~~Done~~: `SetInsurance`, lab insurance fields and a ledger line (schema 44, `MigrateTo44`), `Insurance`, `finance.insurance*`, `InsuranceTests`, `docs/content/insurance.md` (Q299).
59. **The offsite (staff).** ~~Done~~: `HoldOffsite`, `Offsite`, `staffPeople.offsite*`, `OffsiteHeld`, `OffsiteTests`, `docs/content/offsite.md` (Q300).
60. **Red team a model (safety).** ~~Done~~: `RedTeamModel`, `RedTeam`, `redTeam` data, model `redTeamUntil` (schema 45, `MigrateTo45`), `FromRedTeam` in the incident breakdown, `RedTeamTests`, `docs/content/red-team.md` (Q301).
61. **Interpretability programme (safety).** ~~Done~~: `SetInterpretability`, `Interpretability`, `interp` data, lab `interpLevel` (schema 46, `MigrateTo46`), a stale-tech relief in `ControlStale`, `InterpretabilityTests`, `docs/content/interpretability.md` (Q302).
62. **Pay policy (staff).** ~~Done~~: `SetPayLevel`, `Pay`, `staffPeople.pay*` data, lab `payLevel`/`payLockedUntil` (schema 47, `MigrateTo47`), `PayPolicyTests`, `docs/content/pay-policy.md` (Q303).
63. **Headlines for the care tools (news).** ~~Done~~: seven lines for crunch, insurance, the offsite, red teams, interpretability and pay; data and `CareToolsNewsTests` only (Q304).
64. **Chat lines for the care tools (chat).** ~~Done~~: 14 lines for crunch, insurance, the offsite, red teams, findings and pay; picks on `CrunchEnded` and `PayChanged`; `CareToolsChatTests` (Q305).
65. **The care tools together (tests).** ~~Done~~: `CareToolsTogetherTests` over 260 weeks and three seeds (Q306).
66. **Compliance office (government).** ~~Done~~: `SetCompliance`, `Compliance`, `compliance` data, lab `complianceLevel` (schema 48, `MigrateTo48`), `ComplianceTests`, `docs/content/compliance.md` (Q307).
67. **Published papers and academic prestige (talent).** ~~Done~~: `PublishPaper`, `Prestige`, `prestige` data, lab `prestige` (schema 49, `MigrateTo49`), `PrestigeTests`, `docs/content/papers.md` (Q308).
68. **Compute sublet (compute).** ~~Done~~: `SetSublet`, `Sublet`, `sublet` data, lab `subletPermille`/`subletLockedUntil`, ledger `sublet` (schema 50, `MigrateTo50`), `SubletTests`, `docs/content/compute-sublet.md` (Q309).
69. **Headlines and chat for the office, papers and the sublet (news, chat).** ~~Done~~: three headlines and four chat lines, `OfficePapersSubletNewsChatTests`, `news-ticker.md`, `company-chat-and-glow.md` (Q310).
70. **The office, papers and the sublet together (tests).** ~~Done~~: `OfficePapersSubletTogetherTests` (Q311).
71. **External safety audit (safety, reputation).** ~~Done~~: `CommissionAudit`, `SafetyAudit`, `audit` data, model `auditedUntil` (schema 51, `MigrateTo51`), `SafetyAuditTests`, `docs/content/safety-audit.md` (Q312).
72. **Research focus (research).** ~~Done~~: `SetResearchFocus`, `ResearchFocus`, `focus` data, lab `researchFocus` and `focusLockedUntil` (schema 52, `MigrateTo52`), `ResearchFocusTests`, `docs/content/research-focus.md` (Q313).
73. **Hack week (research, staff).** ~~Done~~: `HoldHackWeek`, `HackWeek`, `hack` data, lab `hackWeeks` and `hackLockedUntil` (schema 52), `HackWeekTests`, `docs/content/hack-week.md` (Q314).
74. **Staff equity (pay, funding).** ~~Done~~: `GrantStaffEquity`, `StaffEquity`, `equity` data, lab `equityLevel` and `equityPermille` (schema 53, `MigrateTo53`), `StaffEquityTests`, `docs/content/staff-equity.md` (Q315).
75. **Retrain a person (staff).** ~~Done~~: `RetrainStaff`, `Retraining`, `retrain` data, no new state, `RetrainTests`, `docs/content/retraining.md` (Q316).
76. **Headlines and chat for audits, focus, hack week, equity and retraining.** ~~Done~~: `ResearchAndStaffToolsNewsChatTests`, news and chat data (Q317).
77. **Audits, focus, hack week, equity and retraining together (tests).** ~~Done~~: `ResearchStaffToolsTogetherTests` (Q318).
78. **Chief officers (staff, research, safety).** ~~Done~~: `AppointChief`, `Chiefs`, `chiefs` data, lab `chiefScientistId` and `chiefSafetyId` (schema 54, `MigrateTo54`), `FromChief` in the incident breakdown, `ChiefsTests`, `docs/content/chiefs.md` (Q319).
79. **Ethics board (safety, government).** ~~Done~~: `SetEthicsBoard`, `EthicsBoard`, `ethicsBoard` data, lab `ethicsLevel` and `ethicsLockedUntil` (schema 54), `BoardVeto` and `EthicsLocked`, `EthicsBoardTests`, `docs/content/ethics-board.md` (Q320).
80. **Inference optimisation (engineering, compute).** ~~Done~~: `SetOptimisation`, `Optimisation`, `optimise` data, lab `optimiseLevel` (schema 55, `MigrateTo55`), `FromOptimisation` in the incident breakdown, `OptimisationTests`, `docs/content/inference-optimisation.md` (Q321).
81. **Marketing campaign (sales, reputation).** ~~Done~~: `RunCampaign`, `MarketingCampaign`, `marketing` data, lab campaign fields (schema 55, `MigrateTo55`), `CampaignEffect` in the appeal breakdown, `MarketingCampaignTests`, `docs/content/marketing-campaign.md` (Q322).
82. **Incident response / on-call (safety, staff).** ~~Done~~: `SetOnCall`, `OnCall`, `onCall` data, lab `onCallLevel` and `onCallLockedUntil` (schema 56, `MigrateTo56`), `OnCallTests`, `docs/content/incident-response.md` (Q323).
83. **Headlines and chat for chiefs, the board, campaigns, optimisation and the rota.** ~~Done~~: ten headlines and eleven chat lines, data only, `ChiefsBoardCampaignNewsChatTests` (Q324).
84. **Chiefs, the board, optimisation, campaigns and the rota together (tests).** ~~Done~~: `ChiefsBoardOptimiseCampaignOnCallTogetherTests` (Q325).
86. **Incident floors (safety).** ~~Done~~: `incidentFloor` data, `IncidentFloor`, an insurer payout cap, `IncidentFloorTests`, `docs/content/incident-floors.md` (Q327). No state.
85. **Policy consultants (government, staff).** ~~Done~~: `HireConsultants`, `PolicyConsultants`, `consultants` data, lab `consultantLevel`, `consultantWeeks`, `consultantLockedUntil` (schema 57, `MigrateTo57`), `WorldQueries.PolicyStaffWithConsultants`, `ConsultantsTests`, `docs/content/policy-consultants.md` (Q326). Early audit settlement was considered and left out (Q326).
87. **Public post-mortems (safety).** ~~Done~~: `PublishPostMortem`, `PostMortem`, `postMortem` data, model `incidentRepLoss` and `postMortemDone` (schema 58, `MigrateTo58`), `PostMortemTests`, `docs/content/post-mortems.md` (Q328).
88. **Service-level pledge (economy, safety).** ~~Done~~: `SetSla`, `Sla`, `sla` data, lab `slaLevel`, `slaLockedUntil`, `slaStrikes` (schema 59, `MigrateTo59`), `SlaTests`, `docs/content/service-level-pledge.md` (Q329).
89. **Research grants (research, funding).** ~~Done~~: `ApplyForGrant`, `ResearchGrants`, `researchGrants` data, five lab fields (schema 60, `MigrateTo60`), `ResearchGrantsTests`, `docs/content/research-grants.md` (Q333). The recruiting drive was rejected as a priced timed modifier (Q330).
90. **Headlines and chat for consultants, post-mortems, the pledge and grants.** ~~Done~~: twelve headlines and twelve chat lines, data only, `ConsultantsPostMortemSlaNewsChatTests` (Q331).
91. **Consultants, post-mortems, the pledge and grants together (tests).** ~~Done~~: `ConsultantsPostMortemSlaTogetherTests` (Q332).
92. **Scrap a training run (research, compute).** ~~Done~~: `ScrapRun`, `Abandon`, `abandon` data, no new state, `AbandonTests`, `docs/content/scrapping-a-run.md` (Q334).
93. **Layoff rounds (staff, cash).** ~~Done~~: `Layoff`, `Layoffs`, `layoffs` data, no new state, `LayoffsTests`, `docs/content/layoff-rounds.md` (Q335).
94. **Technical debt (safety, cash, engineers).** ~~Done~~: `techDebt` data, `PayDownDebt`, `TechDebt`, schema 61, `TechDebtTests`, `docs/content/technical-debt.md` (Q336).
95. **Promotions (staff, cash, seats).** ~~Done~~: `seniority` data, `PromoteStaff`, `Seniority`, schema 62, `PromotionsTests`, `docs/content/promotions.md` (Q337).
96. **Board targets (finance, reputation).** ~~Done~~: `revenueTargets` data, `SetRevenueTarget`, `RevenueTargets`, schema 63, `RevenueTargetsTests`, `docs/content/board-targets.md` (Q338).
97. **News and chat for scrapped runs, layoffs, debt, promotions and targets.** ~~Done~~: ten headlines and ten chat lines (79 and 132 in all), `ScrapLayoffDebtPromotionTargetNewsChatTests` (Q339).
98. **Scrapping, layoffs, debt, promotions and targets together (tests).** ~~Done~~: `ScrapLayoffDebtPromotionTargetTogetherTests` (Q340).
99. **Endowed chairs (research, prestige, cash).** ~~Done~~: `chairs` data, `EndowChair`, `Chairs`, schema 64, `ChairsTests`, `docs/content/endowed-chairs.md` (Q341).

## Rules: later

- ~~**Freeze permits and audit investigations (Q45 tail, Q89).**~~ Done: home Freeze blocks `QueueGpuPurchase`, `RaiseFunding`, and the rest of the site commands; `OnAuditOpen` respects `auditInvestigationPermille`; `FreezeAuditLaterTests`.
- ~~**Bubble meter (schema 12, Q90).**~~ Done: `BubbleConfig`, `BubbleSystem`, hooks on funding, chip listings, month roll and world events; `BubbleMeterTests`.
- ~~**Incident rungs 4–6 (Q91).**~~ Done: corruption thresholds, six rungs in data, relaxed validator; `IncidentRungsLaterTests`.
- ~~**Circular financing and shares; monopoly vs regulators (Q92).**~~ Done: rival `publicFloatPpm`, monthly IPO loop feeding the bubble, regulator scrutiny when the meter is hot and extra antitrust on monopoly; `SharesRegulatorsLaterTests`.
- ~~**Shady path depth and China deals (Q93).**~~ Done: player-only shady jobs, model corruption carry/mood/side jobs, China state requests and export vs smuggle; `ShadyChinaLaterTests`.
- ~~**Containment rooms and superintelligence ending gates (Q94).**~~ Done: containment-wing alignment bonus, year-end `TrySuperintelligence` for paperclipped / long-summer / loose; schema 15; `ContainmentSuperintelLaterTests`.
- **Specialist** model lines, data sources, **open weights**, sector pricing.
- ~~**Funding ladder follow-ons (Q95).**~~ Done: down-round terms when the bubble is hot, player board lock and investor board visits; schema 16; `FundingFollowOnLaterTests`.
- Real **Bodies** and **Upward** expansions (era data is in, flagged `expansion`; mechanics and art are not).

## Rules: forward queue (from bible / systems-design)

- ~~**The Pop** ending when the bubble crashes on the player (Q97).~~ Done: `BubbleConfig.thePopEndingForPlayer`; `ThePopEndingLaterTests`.
- ~~**Second Place** ending when the player is crown runner-up N years (Q98).~~ Done: `crownRunnerUpStreak`; schema 17.
- ~~**Exit sell-price breakdown** (Q99).~~ Done: `DisplayQueries.ExitPriceParts`.
- ~~**Era incident load** shifts risk by era (Q100).~~ Done: `incidentLoadPermille` on eras.
- ~~**60-year headless soak** pins long-horizon stability (Q101).~~ Done: `LongHorizonSoakLaterTests` sixty-year step.
- ~~**Runaway economy report**~~ Done (Q111 to Q115): not a runaway (a `simcli` column overflow), but rivals could bankrupt after losing their market; plans now retrench before debt (`rivalry.retrenchRunwayWeeks`, `retrenchCostPermille`), and `EconomyBoundsTests` pins solvency and cash bounds over 40 years.

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
