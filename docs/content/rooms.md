<!-- Generated from Rules/Data/default-config.json. The data file is the source of truth: edit it, then update this page.
     A test (ContentDocsTests) fails when an id from the data is missing here. All names are parody names. -->
# Rooms, roles and lots

## Lots

| Id | Name | Size | Price |
| --- | --- | --- | --- |
| `garage` | Mum's Garage | 14 x 10 | $0 |
| `strip-mall` | Strip-Mall Unit | 22 x 14 | $60k |
| `office-park` | Office Park Floor | 32 x 20 | $450k |
| `campus` | Campus | 48 x 32 | $4.0M |
| `megacampus` | Data-Centre Megacampus | 80 x 56 | $80.0M |
| `hyperscale` | Hyperscale Park | 128 x 96 | $1.5B |

## Roles

| Id | Name | Function | Hire | Weekly salary |
| --- | --- | --- | --- | --- |
| `researcher` | Researcher | Research | $2k | $1.1k |
| `engineer` | Engineer | Engineering | $2k | $1k |
| `safety` | Safety Tester | Safety | $1.5k | $900 |
| `sales` | Sales Rep | Sales | $1.5k | $800 |
| `labeller` | Data Labeller | Engineering | $800 | $600 |
| `data-engineer` | Data Engineer | Engineering | $2.5k | $1.5k |
| `recruiter` | Talent Recruiter | Sales | $3k | $1.8k |
| `red-teamer` | Red-Teamer | Safety | $3k | $1.7k |
| `product-lead` | Product Lead | Sales | $3.5k | $2.2k |
| `alignment-fellow` | Alignment Fellow | Safety | $4.5k | $2.5k |
| `agent-wrangler` | Agent Wrangler | Engineering | $4.5k | $2.6k |
| `data-gardener` | Synthetic Data Gardener | Engineering | $5k | $3k |
| `harness-operator` | Harness Operator | Research | $5k | $3k |
| `roboticist` | Roboticist | Engineering | $5.5k | $3.2k |
| `mission-controller` | Mission Controller | Research | $6k | $3.5k |
| `infra` | Infrastructure Engineer | Infrastructure | $2k | $1k |
| `policy` | Policy Lead | Policy | $2.5k | $1.2k |
| `chip-architect` | Chip Architect | ChipDesign | $3k | $1.8k |

New roles do the work of one of the functions (Research, Engineering, Safety, Sales), so the rules stay the same while the cast grows. Chip architects (function ChipDesign) work on the lab's own chips ([chips.md](chips.md)). Two functions belong to the world map ([world.md](world.md)): Infrastructure engineers shorten hall construction, Policy leads soften what governments make of the lab.

## Room types

Comfort rooms seat nobody: they are for the people already there ([workplace.md](workplace.md)).

### Dial-up Days (1995)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `research-lab` | Research Lab | Research | `researcher` | 4 | - | 3 x 3 | $400 | $8 |
| `engineering-bay` | Engineering Bay | Engineering | `engineer` | 4 | - | 3 x 3 | $400 | $8 |
| `safety-lab` | Safety & Evals Lab | Safety | `safety` | 4 | - | 3 x 3 | $450 | $8 |
| `sales-office` | Sales & Partnerships | Sales | `sales` | 4 | - | 3 x 3 | $350 | $8 |
| `server-closet` | Server Closet | Compute | - | - | 2 | 2 x 2 | $900 | $20 |
| `labelling-room` | Labelling Room | Engineering | `labeller` | 3 | - | 3 x 3 | $300 | $7 |
| `ops-centre` | Ops Centre | Infrastructure | `infra` | 4 | - | 3 x 3 | $400 | $8 |
| `policy-office` | Policy Office | Policy | `policy` | 4 | - | 3 x 3 | $450 | $8 |
| `coffee-corner` | Coffee Corner | Comfort | - | - | - | 2 x 2 | $250 | $4 |

### The Punchline (2009)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `gpu-hall` | GPU Hall | Compute | - | - | 4 | 4 x 4 | $1.4k | $36 |
| `data-team-room` | Data Team Room | Engineering | `data-engineer` | 4 | - | 3 x 3 | $700 | $12 |
| `press-room` | Press Room | Press | - | - | - | 3 x 3 | $600 | $12 |

### It Sees (2012)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `research-floor` | Research Floor | Research | `researcher` | 3 | - | 4 x 4 | $700 | $12 |
| `recruiting-office` | Recruiting Office | Sales | `recruiter` | 4 | - | 3 x 3 | $800 | $12 |
| `kitchen` | Kitchen | Comfort | - | - | - | 3 x 3 | $500 | $9 |
| `chip-design-lab` | Chip Design Lab | ChipDesign | `chip-architect` | 4 | - | 3 x 3 | $600 | $12 |
| `hw-test-lab` | Hardware Test Lab | HardwareTest | - | - | - | 3 x 3 | $700 | $14 |

Both chip rooms also need the tech `chip-team`, whatever the year (the room type's `requiresTech`).

### It Writes (2017)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `sandbox-wing` | Sandbox Wing | Safety | `red-teamer` | 4 | - | 3 x 3 | $900 | $14 |
| `cluster-hall` | Cluster Hall | Compute | - | - | 8 | 4 x 4 | $2.4k | $60 |

### The Chat Boom (2022)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `product-studio` | Product Studio | Sales | `product-lead` | 4 | - | 4 x 3 | $1k | $16 |
| `auditorium` | Auditorium | Press | - | - | - | 5 x 4 | $1.5k | $30 |
| `liquid-cooling-pod` | Liquid-Cooling Pod | Compute | - | - | 16 | 4 x 4 | $4.2k | $100 |
| `nap-pods` | Nap Pods | Comfort | - | - | - | 3 x 3 | $700 | $12 |

### Agents (2024)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `agent-ops-room` | Agent Ops Room | Engineering | `agent-wrangler` | 4 | - | 4 x 4 | $1.2k | $18 |
| `containment-wing` | Containment Wing | Safety | `alignment-fellow` | 4 | - | 4 x 4 | $1.5k | $22 |

### Harnesses (2026)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `synthetic-data-lab` | Synthetic Data Lab | Engineering | `data-gardener` | 4 | - | 4 x 4 | $1.4k | $20 |
| `harness-control-room` | Harness Control Room | Research | `harness-operator` | 3 | - | 4 x 4 | $1.6k | $24 |
| `ai-factory-hall` | AI Factory Hall | Compute | - | - | 36 | 6 x 6 | $8k | $180 |

### Bodies (2029)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `robotics-arena` | Robotics Arena | Engineering | `roboticist` | 6 | - | 6 x 6 | $2k | $30 |

### Upward (2033)

| Id | Name | Purpose | Seats | Tiles/seat | GPU slots/tile | Min size | Build/tile | Upkeep/tile/week |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `launch-control` | Launch Control | Research | `mission-controller` | 4 | - | 4 x 4 | $2.5k | $36 |
| `orbital-uplink` | Orbital Uplink Bay | Compute | - | - | 90 | 6 x 6 | $16k | $320 |

