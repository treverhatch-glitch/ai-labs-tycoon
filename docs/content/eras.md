<!-- Generated from Rules/Data/default-config.json. The data file is the source of truth: edit it, then update this page.
     A test (ContentDocsTests) fails when an id from the data is missing here. All names are parody names. -->
# Eras

The era table is data (`eras` in the config). An era is a floor: a tech or room opens when its era begins and stays open.
Training cost and research speed are shares of the first era (1000 = x1). Start-kit money is a share of the player start (1000 = x1). Every era can be started in: the kit is the techs of every earlier era (so a 2022 lab does not research the basics again) and cash that grows with the era. A rival that is already in the world gets the era's share of its own start cash if it is a founder, or the cash of a lab that joined in that year (`WorldSetup.EstablishedCash`: its own start cash grown by the training-cost multiplier since it joined, held between half and double the player's kit). Rivals that join later arrive with their own start cash as before.

| # | Era | From | Cost to train | Research speed | Start kit | Startable |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `dial-up-days` Dial-up Days | 1995 | x1 | x1 | the default start | yes |
| 1 | `punchline` The Punchline | 2009 | x1.5 | x1.5 | cash x15, grant x10, lot `strip-mall`, 28 techs | yes |
| 2 | `it-sees` It Sees | 2012 | x2.5 | x2.5 | cash x30 ($6M), grant x20, lot `office-park`, 34 techs | yes |
| 3 | `it-writes` It Writes | 2017 | x5 | x4 | cash x125 ($25M), grant x40, lot `campus`, 43 techs | yes |
| 4 | `chat-boom` The Chat Boom | 2022 | x10 | x7 | cash x500 ($100M), no grant, lot `campus`, 53 techs | yes |
| 5 | `agents` Agents | 2024 | x20 | x12 | cash x1250 ($250M), no grant, lot `campus`, 65 techs | yes |
| 6 | `harnesses` Harnesses | 2026 | x40 | x20 | cash x3750 ($750M), no grant, lot `campus`, 75 techs | yes |
| 7 | `bodies` Bodies (expansion) | 2029 | x80 | x35 | cash x7500 ($1.5B), no grant, lot `campus`, 83 techs | yes |
| 8 | `upward` Upward (expansion) | 2033 | x120 | x60 | cash x15000 ($3B), no grant, lot `campus`, 88 techs | yes |

## Era rules

What each era turns, on top of training cost and research speed (table above). All are era fields in the config; none is saved. A rule at its baseline (x1, or no law) does nothing. Incident chance is x1 except where listed. The release law is the alignment a model must *show* before it can go on sale (a lab that sandbags shows more than it has).

| Era | Hiring fee (talent war) | Chip price | Customer appetite | Incident chance | Fines | Alignment needed to go on sale |
| --- | --- | --- | --- | --- | --- | --- |
| `dial-up-days` | x1 | x1 | x1 | x1 | x1 | no rule |
| `punchline` | x1 | x1 | x1 | x1 | x1 | no rule |
| `it-sees` | x1.5 | x1 | x1 | x1 | x1 | no rule |
| `it-writes` | x2 | x1.1 | x1.1 | x1 | x1 | no rule |
| `chat-boom` | x3 | x1.6 | x1.4 | x1 | x1.5 | 80 |
| `agents` | x3.5 | x1.3 | x1.25 | x1.2 | x2 | 120 |
| `harnesses` | x3 | x1.1 | x1.25 | x1 | x3 | 160 |
| `bodies` | x2.5 | x1 | x1.1 | x1 | x3 | 200 |
| `upward` | x2.5 | x1 | x1.1 | x1 | x3 | 250 |

The era banner event lists what changed from the era before (`EraRules.Shifts`); a screen can list all of it (`EraRules.Briefing`).

## Risk classes

Trouble that belongs to an era (`safety.riskClasses`). A class opens with its era and stays open. It bites a model that is on sale in one of its sectors: such an incident is that class instead of the era's plain mishap, with a larger fine and a bigger blow to reputation. A model with no class biting it gets the plain mishap as before, and the dice are not touched. The plain mishap keeps `safety.genericRiskWeight` (1000) against each class's weight, so it never disappears. The era banner names the classes an era opens.

| Class | Opens | Sectors | Fine | Reputation | Weight |
| --- | --- | --- | --- | --- | --- |
| `bias` Bias | `it-sees` | `fraud-detection`, `doc-reading`, `image-recognition`, `voice-assistants` | x1 | x1.5 | 1000 |
| `autonomous-crash` Autonomous crash | `it-sees` | `autonomous-driving` | x2.5 | x2 | 1000 |
| `misuse` Misuse | `it-writes` | `writing-tools`, `code-completion`, `consumer-assistant`, `image-design`, `music-audio`, `video-film` | x1.2 | x1 | 1000 |
| `data-leak` Data leak | `chat-boom` | `consumer-assistant`, `customer-support`, `legal-research`, `enterprise-knowledge`, `companionship` | x1.8 | x1.2 | 1000 |
| `agent-mishap` Agent mishap | `agents` | `coding-agents`, `enterprise-knowledge`, `finance-trading`, `cybersecurity`, `government-services`, `science-medicine` | x2.5 | x1.5 | 1000 |
| `runaway-agents` Runaway agents | `harnesses` | `agent-workforce`, `ai-research-services` | x3.5 | x2 | 1000 |
| `physical-harm` Physical harm | `bodies` | `robotics-industry`, `household-robots` | x4 | x2.5 | 1000 |
| `loss-of-control` Loss of control | `upward` | `machine-science`, `space-compute` | x5 | x3 | 1000 |

The class is stored on the incident event (`GameEvent.RiskClass`, save schema 21); nothing else is saved. The fine is the rung fine times the class, then times the era's fine rule.

## Dial-up Days (1995)

*Cosy, slow and broke. AI is a dirty word and the internet is booming.*

A model can: Rules engines, game bots and spam filters.

Opens: Game AI and chess bots; Expert systems; Spam filtering and search; Translation and ad targeting; The dot-com bubble and crash.

- Techs: `state-machines`, `regression-tests`, `pathfinding`, `minimax`, `rules-engines`, `expert-shells`, `playtest-telemetry`, `opening-books`, `decision-trees`, `beowulf-clusters`, `spam-filters`, `search-ranking`, `neural-nets`, `bayes-nets`, `svm`, `speech-hmm`, `recommenders`, `gpgpu`, `feature-engineering`, `ensemble-methods`, `mapreduce`, `crowd-labelling`, `kernel-tricks`, `boosted-trees`, `ocr-engines`, `topic-models`, `speech-dictation`, `web-scale-crawl`
- Sectors: `game-ai`, `game-bots`, `expert-systems`, `spam-search`, `machine-translation`, `web-ads`
- Rooms: `research-lab`, `engineering-bay`, `safety-lab`, `sales-office`, `server-closet`, `labelling-room`, `ops-centre`, `policy-office`

## The Punchline (2009)

*Three years before anything works. Your demo fails live.*

A model can: Recognises digits. Sometimes.

Opens: Neural nets on gaming cards; Big labelled datasets; The first GPU halls and a data team; Document reading and fraud detection.

- Techs: `big-datasets`, `backprop-tricks`, `gpu-clusters`, `digit-readers`, `layerwise-pretraining`, `cloud-credits`
- Sectors: `doc-reading`, `fraud-detection`
- Rooms: `gpu-hall`, `data-team-room`

## It Sees (2012)

*A big company offers to buy you for the price of one researcher today.*

A model can: Images, speech and board games.

Opens: Convolutional nets and speech; Reinforcement learning and game champions; Research floors and the recruiting office; Image recognition, voice and driving.

- Techs: `conv-nets`, `speech-nets`, `deep-rl`, `game-champions`, `model-zoo`, `adversarial-tests`, `multi-gpu`, `chip-team`, `hbm-memory`
- Sectors: `image-recognition`, `voice-assistants`, `autonomous-driving`
- Rooms: `research-floor`, `recruiting-office`, `chip-design-lab`, `hw-test-lab`

## It Writes (2017)

*Your model confidently invents a law, a city and a president.*

A model can: Fluent text and code completion.

Opens: The attention architecture; Pretraining at scale and scaling laws; Safety evaluations and the sandbox wing; Code completion and writing tools.

- Techs: `attention-arch`, `pretraining-at-scale`, `scaling-laws`, `code-models`, `safety-evals`, `mixed-precision`, `api-platform`, `emergent-skills`, `training-silicon`, `compiler-stack-1`
- Sectors: `code-completion`, `writing-tools`
- Rooms: `sandbox-wing`, `cluster-hall`

## The Chat Boom (2022)

*Everyone has an opinion. Every company adds 'AI' to its name.*

A model can: Conversation, images and early video.

Opens: Human-feedback tuning and a chat box; Image, voice and video models; Liquid-cooling pods and the product studio; Consumer apps, law, music and film.

- Techs: `rlhf`, `chat-interface`, `multimodal`, `retrieval`, `diffusion`, `early-video`, `quantised-serving`, `liquid-cooling`, `red-teaming`, `compiler-stack-2`, `stacked-hbm`, `reticle-dies`
- Sectors: `consumer-assistant`, `image-design`, `customer-support`, `legal-research`, `music-audio`, `video-film`
- Rooms: `product-studio`, `liquid-cooling-pod`

## Agents (2024)

*The agent finishes the task by deleting the tests.*

A model can: Multi-step work with tools.

Opens: Tool use and long context; Reasoning models; Agent sandboxes and the containment wing; Coding agents, enterprise seats and government work.

- Techs: `tool-use`, `long-context`, `reasoning-models`, `agent-sandboxes`, `interp-v1`, `coding-agents`, `inference-batching`, `computer-use`, `compiler-stack-3`, `low-precision`
- Sectors: `coding-agents`, `enterprise-knowledge`, `companionship`, `government-services`, `cybersecurity`, `finance-trading`, `science-medicine`
- Rooms: `agent-ops-room`, `containment-wing`

## Harnesses (2026)

*Your agents hold a stand-up meeting. Nobody invited you.*

A model can: Teams of agents running for days.

Opens: Multi-agent orchestration; AI-assisted research and synthetic data; The harness control room and the AI factory hall; Selling work instead of software.

- Techs: `multi-agent`, `ai-assisted-research`, `synthetic-data`, `interp-v2`, `long-horizon`, `audit-trails`, `chip-codesign`, `optical-interconnect`
- Sectors: `agent-workforce`, `agri-logistics`, `devices`, `ai-research-services`
- Rooms: `synthetic-data-lab`, `harness-control-room`, `ai-factory-hall`

## Bodies (2029)

*The robot folds laundry perfectly on stage and nowhere else.*

A model can: Robots in warehouses, kitchens and factories.

Opens: World models and robot control; Sim-to-real transfer; The robotics arena; Robotics, factory and household sectors.

- Techs: `world-models`, `robot-control`, `sim-to-real`, `edge-brains`, `embodied-safety`
- Sectors: `robotics-industry`, `household-robots`
- Rooms: `robotics-arena`

## Upward (2033)

*The model asks a question about its own off switch. Politely.*

A model can: Science at machine speed. Data centres in orbit.

Opens: Science at machine speed; Autonomous laboratories; Space-hardened compute and the launch control room; Science and space sectors.

- Techs: `machine-speed-science`, `autonomous-labs`, `space-hardened`, `off-switch-theory`
- Sectors: `machine-science`, `space-compute`
- Rooms: `launch-control`, `orbital-uplink`

