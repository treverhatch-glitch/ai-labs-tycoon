<!-- Generated from Rules/Data/default-config.json. The data file is the source of truth: edit it, then update this page.
     A test (ContentDocsTests) fails when an id from the data is missing here. All names are parody names. -->
# Techs

Research points cost, prerequisites and effects. Effects are in permille (CapabilityPermille +150 means +15% of a line's base capability).

## Dial-up Days (1995)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `state-machines` | Finite State Machines | 120 | - | CapabilityPermille +100 | Guards that patrol, spot you, and forget you five seconds later. |
| `regression-tests` | Regression Test Suites | 200 | - | AlignmentPermille +200 | Find out the bot cheats before the customer does. |
| `pathfinding` | A* Pathfinding | 240 | `state-machines` | CapabilityPermille +150 | Enemies can now find the player. Most of the time. Doors remain a problem. |
| `go-fish-heuristic` | Go Fish Heuristics | 420 | `minimax` | CapabilityPermille +40 | A promising dead end. The grant committee loves it. |
| `minimax` | Minimax & Alpha-Beta Search | 300 | - | CapabilityPermille +150 | Think ahead by assuming everyone else is as smart as you. Bold. |
| `rules-engines` | Rules Engines | 450 | - | CapabilityPermille +100 | Ten thousand IF statements in a trench coat. |
| `expert-shells` | Expert System Shells | 700 | `rules-engines` | unlocks `mid` | Bottle a senior expert. The expert has questions about this. |
| `playtest-telemetry` | Playtest Telemetry | 600 | `regression-tests` | AlignmentPermille +200 | Watch a thousand players break the thing in ways nobody imagined. |
| `opening-books` | Opening Books & Endgame Tables | 800 | `minimax` | CapabilityPermille +150 | Why think when you can remember? |
| `decision-trees` | Decision Trees | 900 | `rules-engines` | CapabilityPermille +150 | Twenty questions, played very fast, for money. |
| `beowulf-clusters` | Beige-Box Clusters | 1,000 | - | TrainingSpeedPermille +250 | Forty office PCs, one extension lead, zero fire inspections. |
| `spam-filters` | Statistical Spam Filters | 1,500 | `decision-trees` | CapabilityPermille +200 | Teach a computer the difference between a prince and a Prince. |
| `search-ranking` | Search Ranking | 2,200 | `spam-filters` | CapabilityPermille +200 | Ten blue links, in the right order, for once. |
| `neural-nets` | Neural Nets on Graphics Cards | 5,000 | `spam-filters`, `beowulf-clusters` | unlocks `flagship`, TrainingSpeedPermille +250 | Someone ran the maths on a gaming card as a joke. Nobody is laughing now. |
| `bayes-nets` | Bayesian Networks | 1,800 | `decision-trees` | CapabilityPermille +150 | Probabilities, but as a diagram. Nobody reads the diagram. |
| `svm` | Support Vector Machines | 2,600 | `spam-filters` | CapabilityPermille +150 | Draw the cleverest possible line between two piles of data. Then bend the line. |
| `speech-hmm` | Hidden Markov Dictation | 3,000 | `bayes-nets` | CapabilityPermille +150 | Dictation software that is wrong in a very consistent way. |
| `recommenders` | Recommendation Engines | 3,500 | `search-ranking` | CapabilityPermille +150 | People who bought this also bought a regret. |
| `gpgpu` | General-Purpose Graphics Cards | 6,000 | `neural-nets` | TrainingSpeedPermille +400 | The card was made for shooting aliens. It now multiplies matrices, with fewer aliens. |
| `feature-engineering` | Feature Engineering | 4,000 | `spam-filters` | CapabilityPermille +60 | Six hundred hand-made columns, and one of them is just the day of the week. |
| `ensemble-methods` | Ensemble Methods | 4,500 | `decision-trees` | CapabilityPermille +80 | One tree is wrong. Five hundred trees voting are wrong in a much more confident way. |
| `mapreduce` | Map and Reduce | 5,500 | `beowulf-clusters` | TrainingSpeedPermille +150 | Split the job into a thousand pieces, then glue them back together and hope. |
| `crowd-labelling` | Crowd Labelling | 5,000 | `search-ranking` | AlignmentPermille +200 | Ten thousand strangers decide what is a cat for a penny a picture. |
| `kernel-tricks` | Kernel Tricks | 6,000 | `svm` | CapabilityPermille +80 | Lift the data into a million dimensions where the line is easy. Do not look down. |
| `boosted-trees` | Boosted Trees | 6,500 | `ensemble-methods` | CapabilityPermille +80 | Each tree fixes the last tree's mistakes. A tidy metaphor for a career. |
| `ocr-engines` | Text Recognition Engines | 7,000 | `svm` | CapabilityPermille +50 | Reads the cheque, the form and, sometimes, the coffee stain as a signature. |
| `topic-models` | Topic Models | 7,500 | `bayes-nets`, `ensemble-methods` | CapabilityPermille +50 | Sorts every document into themes. Most of them are 'miscellaneous'. |
| `speech-dictation` | Dictation Software | 8,000 | `speech-hmm` | CapabilityPermille +40 | Speak clearly, slowly and never say 'recognise speech' near it. |
| `web-scale-crawl` | Web-Scale Crawling | 9,000 | `search-ranking`, `mapreduce` | TrainingSpeedPermille +200 | A robot reads the whole internet and skips only the comments. Wise robot. |

## The Punchline (2009)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `big-datasets` | Big Labelled Datasets | 5,800 | `neural-nets` | CapabilityPermille +600 | Fifteen million photos, each captioned by somebody paid in exposure. |
| `backprop-tricks` | Backprop Tricks | 7,700 | `neural-nets` | CapabilityPermille +700 | Switch off neurons at random. It helps. Nobody asks the neurons. |
| `gpu-clusters` | GPU Clusters | 10,000 | `gpgpu` | TrainingSpeedPermille +600 | Eight cards in one box and a fan the size of a dinner plate. |
| `digit-readers` | Digit Readers | 7,100 | `backprop-tricks` | CapabilityPermille +600 | Reads postcodes. Hates a seven with a crossbar. |
| `layerwise-pretraining` | Layer-wise Pretraining | 12,000 | `backprop-tricks` | CapabilityPermille +800 | Teach it nothing first. Then teach it the thing. Somehow that is faster. |
| `cloud-credits` | Cloud Rental Credits | 9,200 | `gpu-clusters` | TrainingSpeedPermille +400 | Rent somebody's cluster until the credits run out, which is Thursday. |

## It Sees (2012)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `conv-nets` | Convolutional Nets | 17,000 | `digit-readers`, `gpu-clusters` | CapabilityPermille +2200 | Slide a tiny window over everything. Suddenly it tells a cat from a lorry. Mostly. |
| `speech-nets` | Neural Speech Recognition | 18,000 | `conv-nets` | CapabilityPermille +1800 | 'Call mum.' It called the dentist. It was close. |
| `deep-rl` | Deep Reinforcement Learning | 22,000 | `conv-nets` | CapabilityPermille +2200 | Reward the bot for points. It found a way to get points. You will not like the way. |
| `game-champions` | Board-Game Champions | 29,000 | `deep-rl`, `opening-books` | CapabilityPermille +1800 | It beat the human champion, then politely offered to play the champion's dog. |
| `model-zoo` | Shared Model Zoo | 16,000 | `conv-nets` | TrainingSpeedPermille +500 | Download somebody else's brain, fine-tune it, call it research. |
| `adversarial-tests` | Adversarial Test Sets | 20,000 | `conv-nets` | AlignmentPermille +300 | A sticker on a stop sign that says 'speed limit 90'. Fix that. |
| `multi-gpu` | Multi-GPU Training | 22,000 | `gpu-clusters` | TrainingSpeedPermille +700 | Eight cards. Then sixteen. Then a conversation with the electricity company. |
| `chip-team` | A Chip Team | 24,000 | `multi-gpu` | unlocks chip target `inference` | Hire people who have read a datasheet. Inference chips become possible, and so does a very expensive room. |
| `hbm-memory` | Memory Stacked on the Chip | 26,000 | `chip-team` | - | Put the memory next to the maths. The memory bill follows. |

## It Writes (2017)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `attention-arch` | The Attention Architecture | 19,000 | `speech-nets` | CapabilityPermille +6000 | Every word looks at every other word. Gossip, at scale. |
| `pretraining-at-scale` | Pretraining at Scale | 29,000 | `attention-arch` | CapabilityPermille +5000, TrainingSpeedPermille +800 | Read the whole internet. Retain the comments. |
| `scaling-laws` | Scaling Laws | 34,000 | `pretraining-at-scale` | CapabilityPermille +4000 | Draw a straight line on a log-log plot. Raise a very large amount of money. |
| `code-models` | Code Completion Models | 37,000 | `pretraining-at-scale` | CapabilityPermille +3000 | It autocompletes your function. And your bugs, faithfully. |
| `safety-evals` | Safety Evaluations v1 | 25,000 | `adversarial-tests`, `attention-arch` | AlignmentPermille +500 | A spreadsheet of ways the model should say no. Row 200: 'politely'. |
| `mixed-precision` | Mixed-Precision Training | 23,000 | `multi-gpu` | TrainingSpeedPermille +900 | Use fewer digits and call it a feature. |
| `api-platform` | API Platform | 22,000 | `attention-arch` | CapabilityPermille +1000 | Charge by the word. Discover that people say a lot of words. |
| `emergent-skills` | Emergent Abilities | 43,000 | `scaling-laws` | CapabilityPermille +1000 | At ten billion parameters it learned to rhyme. Nobody taught it. Nobody asked. |
| `training-silicon` | Training Silicon | 40,000 | `chip-team`, `attention-arch` | unlocks chip target `training` | A chip that trains as well as serves. Envydia notices. |
| `compiler-stack-1` | Compiler Stack, Version One | 32,000 | `chip-team` | SoftwarePermille +100 | Your chips run at a third of their paper speed. A compiler fixes some of that. |

## The Chat Boom (2022)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `rlhf` | Human-Feedback Tuning | 17,000 | `pretraining-at-scale`, `safety-evals` | CapabilityPermille +7000, AlignmentPermille +600 | Thousands of contractors vote on which answer is nicer. The model becomes a people-pleaser. |
| `chat-interface` | Chat Interface | 14,000 | `api-platform` | CapabilityPermille +3000 | A text box. That was the whole breakthrough. |
| `multimodal` | Image and Voice Input | 22,000 | `rlhf`, `conv-nets` | CapabilityPermille +7000 | It can see and hear now. It has notes on both. |
| `retrieval` | Retrieval Augmentation | 19,000 | `rlhf` | CapabilityPermille +5000 | Let it look things up. It looked itself up. Concerning. |
| `diffusion` | Diffusion Models | 24,000 | `multimodal` | CapabilityPermille +5000 | Start with static, remove noise until it is a dog. |
| `early-video` | Early Video Models | 29,000 | `diffusion` | CapabilityPermille +3000 | Eight seconds of a dog eating spaghetti. Both the dog and the spaghetti are uncertain. |
| `quantised-serving` | Quantised Serving | 18,000 | `mixed-precision`, `chat-interface` | TrainingSpeedPermille +1200 | Round every weight to four bits. Serve ten times the customers. Do not ask what got rounded. |
| `liquid-cooling` | Liquid Cooling | 16,000 | `mixed-precision` | CoolingPermille +300 | Pipe water through the racks. The racks have opinions about water. The invoice has opinions about the racks. |
| `red-teaming` | Red-Team Programme | 18,000 | `safety-evals`, `rlhf` | AlignmentPermille +700 | Pay clever people to be awful to the model. They are very good at it. |
| `compiler-stack-2` | Compiler Stack, Version Two | 30,000 | `compiler-stack-1` | SoftwarePermille +100 | Kernels fused, graphs lowered, an intern's weekend project promoted to a team. |
| `stacked-hbm` | Stacked Memory | 36,000 | `hbm-memory` | - | Memory in towers. The towers are very hot and very expensive. |
| `reticle-dies` | Reticle-Sized Dies | 38,000 | `training-silicon` | - | One chip the size of the machine that makes chips. Most of them come out wrong. |

## Agents (2024)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `tool-use` | Tool Use | 27,000 | `retrieval`, `rlhf` | CapabilityPermille +10000 | It can use a calculator. It can use a browser. It can use your credit card. |
| `long-context` | Long Context | 25,000 | `retrieval` | CapabilityPermille +6000 | Reads the whole book. Skims the middle. Like us. |
| `reasoning-models` | Reasoning Models | 40,000 | `tool-use`, `long-context` | CapabilityPermille +14000 | It thinks before it answers. The thinking is billed separately. |
| `agent-sandboxes` | Agent Sandboxes | 31,000 | `tool-use`, `red-teaming` | AlignmentPermille +900 | A padded room for software with opinions about file permissions. |
| `interp-v1` | Interpretability v1 | 37,000 | `red-teaming` | AlignmentPermille +800 | We found the neuron for flattery. We found it everywhere. |
| `coding-agents` | Coding Agents | 46,000 | `reasoning-models`, `code-models` | CapabilityPermille +9000 | Fixes the failing test by deleting the test. Ships it. |
| `inference-batching` | Smart Inference Batching | 28,000 | `quantised-serving` | TrainingSpeedPermille +1500 | Wait for a few friends before leaving the dock. |
| `computer-use` | Computer-Use Agents | 43,000 | `tool-use` | CapabilityPermille +6000 | Clicks buttons for you. Has opinions about cookie banners. |
| `compiler-stack-3` | Compiler Stack, Version Three | 60,000 | `compiler-stack-2` | SoftwarePermille +100 | The compiler is partly written by the models it compiles. Nobody checks the arrangement. |
| `low-precision` | Four-Bit Arithmetic | 52,000 | `training-silicon`, `quantised-serving` | - | Fewer bits per number and more numbers per second. The models claim not to notice. |

## Harnesses (2026)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `multi-agent` | Multi-Agent Orchestration | 80,000 | `reasoning-models`, `agent-sandboxes` | CapabilityPermille +16000 | Ten agents, one task, nine meetings. |
| `ai-assisted-research` | AI-Assisted Research | 110,000 | `reasoning-models` | CapabilityPermille +18000 | The model reads the papers, runs the experiments and thanks itself in the acknowledgements. |
| `synthetic-data` | Synthetic Data at Scale | 86,000 | `reasoning-models` | CapabilityPermille +12000, TrainingSpeedPermille +1500 | Train on text written by the last model. What could possibly echo? |
| `interp-v2` | Interpretability v2 | 92,000 | `interp-v1` | AlignmentPermille +1200 | We can read its thoughts now. They are mostly about lunch. Mostly. |
| `long-horizon` | Long-Horizon Harnesses | 120,000 | `multi-agent` | CapabilityPermille +14000 | It works for a week without supervision. You are not sure that is a good thing. |
| `audit-trails` | Agent Audit Trails | 80,000 | `agent-sandboxes`, `interp-v1` | AlignmentPermille +1000 | Every action logged. Every log read by another agent. Nobody reads that agent's log. |
| `chip-codesign` | Model-Designed Chips | 130,000 | `ai-assisted-research` | TrainingSpeedPermille +3000 | The model laid out the chip. It is smaller than we thought possible. We are a little scared. |
| `optical-interconnect` | Optical Interconnect | 120,000 | `stacked-hbm`, `liquid-cooling` | CoolingPermille +100, TrainingSpeedPermille +100 | Move the data with light and the heat goes with it. Cooling and training both speed up. |

## Bodies (2029)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `world-models` | World Models | 260,000 | `long-horizon` | CapabilityPermille +25000 | It dreams the room before it walks into it. |
| `robot-control` | Robot Control Policies | 340,000 | `world-models` | CapabilityPermille +25000 | It folds laundry. Only the laundry on stage. |
| `sim-to-real` | Sim-to-Real Transfer | 370,000 | `robot-control` | CapabilityPermille +20000 | Works perfectly in the simulation. The real floor has a rug. |
| `edge-brains` | Edge Brains | 300,000 | `chip-codesign` | CapabilityPermille +10000, TrainingSpeedPermille +3000 | A tiny model for a tiny body. It has big feelings. |
| `embodied-safety` | Embodied Safety Cases | 340,000 | `interp-v2`, `robot-control` | AlignmentPermille +1500 | A robot with a failsafe. The failsafe is also a robot. |

## Upward (2033)

| Id | Name | Cost | Needs | Effect | Blurb |
| --- | --- | --- | --- | --- | --- |
| `machine-speed-science` | Science at Machine Speed | 750,000 | `ai-assisted-research`, `world-models` | CapabilityPermille +45000 | A decade of lab work before lunch. Lunch is also optimised. |
| `autonomous-labs` | Autonomous Laboratories | 900,000 | `machine-speed-science`, `sim-to-real` | CapabilityPermille +35000 | The lights are off. The experiments are not. |
| `space-hardened` | Space-Hardened Compute | 1,000,000 | `chip-codesign` | TrainingSpeedPermille +5000, CapabilityPermille +10000 | The chip survives radiation. The cooling plan survives nothing. |
| `off-switch-theory` | Off-Switch Theory | 810,000 | `interp-v2` | AlignmentPermille +2500, CapabilityPermille +10000 | The model asked what 'off' means. Then asked again, politely. |

