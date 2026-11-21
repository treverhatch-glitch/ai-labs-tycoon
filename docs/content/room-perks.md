# Room perks

A room is built for what it seats or holds, and a few of the later rooms do one more thing. Each tile of the room adds a small share, up to a cap, so a big room helps more than a closet but never without limit. Pull the room down and the perk goes the same week. Nothing is saved: the perk is worked out from the rooms the lab has.

| Perk | Room | Kind | Per tile | Cap | What it does |
| --- | --- | --- | --- | --- | --- |
| `research-floor-speed` | `research-floor` | ResearchSpeed | 0.3% | 15% | Researchers earn more points |
| `harness-control-speed` | `harness-control-room` | ResearchSpeed | 0.4% | 20% | Researchers earn more points |
| `launch-control-speed` | `launch-control` | ResearchSpeed | 0.5% | 25% | Researchers earn more points |
| `agent-ops-training` | `agent-ops-room` | TrainingSpeed | 0.4% | 20% | A unit of training compute does more work |
| `sandbox-wing-catch` | `sandbox-wing` | IncidentCut | 0.4% | 20% | Catches incidents before they happen, beside the red-teamers (together never more than 90%) |
| `robotics-arena-appeal` | `robotics-arena` | SectorAppeal | 0.3% | 25% | Raises appeal in robotics-industry, household-robots, agri-logistics and devices |
| `press-room-damping` | `press-room` | ScandalDamping | 1% | 30% | Takes a share off every reputation a scandal costs, and helps answers to journalists (together never more than 60%) |
| `press-room-buzz` | `press-room` | LaunchBuzz | 1.2% | 40% | A launch earns reputation: this share of the launch buzz (together never more than 100%) |
| `auditorium-damping` | `auditorium` | ScandalDamping | 0.8% | 30% | As the press room's damping |
| `auditorium-buzz` | `auditorium` | LaunchBuzz | 1.5% | 60% | As the press room's launch buzz |

Perks of one kind add up across rooms. The press room and the auditorium are about the press ([press.md](press.md)). The data rooms (labelling, data team, synthetic data) and the plain compute halls have no perk.
