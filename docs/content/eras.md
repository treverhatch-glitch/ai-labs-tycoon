<!-- Generated from Rules/Data/default-config.json. The data file is the source of truth: edit it, then update this page.
     A test (ContentDocsTests) fails when an id from the data is missing here. All names are parody names. -->
# Eras

The era table is data (`eras` in the config). An era is a floor: a tech or room opens when its era begins and stays open.
Training cost and research speed are shares of the first era (1000 = x1). Start-kit money is a share of the player start (1000 = x1).

| # | Era | From | Cost to train | Research speed | Start kit | Startable |
| --- | --- | --- | --- | --- | --- | --- |
| 0 | `dial-up-days` Dial-up Days | 1995 | x1 | x1 | the default start | yes |
| 1 | `punchline` The Punchline | 2009 | x1.5 | x1.5 | cash x15, grant x10, lot `strip-mall`, 28 techs | yes |
| 2 | `it-sees` It Sees | 2012 | x2.5 | x2.5 | - | no |
| 3 | `it-writes` It Writes | 2017 | x5 | x4 | - | no |
| 4 | `chat-boom` The Chat Boom | 2022 | x10 | x7 | - | no |
| 5 | `agents` Agents | 2024 | x20 | x12 | - | no |
| 6 | `harnesses` Harnesses | 2026 | x40 | x20 | - | no |
| 7 | `bodies` Bodies (expansion) | 2029 | x80 | x35 | - | no |
| 8 | `upward` Upward (expansion) | 2033 | x120 | x60 | - | no |

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

