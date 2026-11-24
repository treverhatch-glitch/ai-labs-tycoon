<!-- Generated from Rules/Data/default-config.json (chat, glow). The data file is the source of truth: edit it, then update this page. -->
# The company chat and the glow

Two read-only views of the lab. Neither is saved and neither changes any rule.

## The company chat

The bible's chapter 14 asks for "a company chat feed where staff react to your decisions in character". `CompanyChat.Feed(config, state, lab, max)` reads the event log and returns the last lines the lab's people would say, oldest first (12 unless the screen asks for another number).

- A line reacts to one kind of event, and for yes-or-no events to one pick (a counter that worked or one that did not).
- A line can be limited to some roles, and to people with a trait. The first line in the data that fits the event and has someone to say it wins, so the lines for traits come before the plain ones.
- A line can also name `fromYear` and `toYear` (0 for no limit): it is only said for events from those calendar years, judged by the event's own tick. Seven lines use it (Q294): hires in the dial-up days, the chat boom and the upward era, releases early on and in the agent era, and finished models early on and in the harness era. They sit after the trait lines and before the plain line of their kind.
- Ten lines (Q296) cover the chip business and the bank: a tapeout and a respin (a chip architect or engineer first, anyone else the plain line), a yield crash, a delivery and a foundry snub (these three say how many chips or weeks), and a loan taken, missed or repaid.
- Fourteen lines (Q305) cover the player's care tools: crunch ending (a tired team has its researchers and engineers speak, a stopped crunch has one line, the longest crunch or an empty lab get the plain one), an insurance claim and a dropped policy, an offsite, a red team (red-teamers and safety people name the model), a finding of the interpretability team, and a pay raise or cut (a researcher has a line for a cut). `CrunchEnded` and `PayChanged` events carry a pick so a line can tell apart what happened (see the event notes).
- Four lines (Q310) cover the compliance office, papers and the sublet: a paper (a researcher has their own line; the event carries pick 1), a lapsed compliance office, and a change of the compute sublet.
- The speaker is picked from the people who fit by a hash of the event: the same event always gets the same speaker while that person is on the payroll.
- An event kind with no line is not in the feed, and a lab with nobody on the payroll is silent.
- A poach is about the lab it was done to, not the one that did it.
- `{model}`, `{amount}`, `{ref}`, `{sector}`, `{role}` and `{lab}` are filled in from the event.

| Line | Event | Said by | Trait |
| --- | --- | --- | --- |
| `hired-sleeper` | Hired | anyone | sleeps-under-desk |
| `hired` | Hired | anyone |  |
| `fired-doomer` | Fired | anyone | doomer |
| `fired-safety` | Fired | safety, red-teamer, alignment-fellow |  |
| `fired` | Fired | anyone |  |
| `room-built` | RoomBuilt | anyone |  |
| `tech-researched` | TechResearched | researcher |  |
| `training-started` | TrainingStarted | researcher, engineer |  |
| `trained-tenx` | TrainingFinished | anyone | tenx |
| `trained-doomer` | TrainingFinished | anyone | doomer |
| `trained` | TrainingFinished | researcher, engineer |  |
| `released-accel` | ModelReleased | anyone | accelerationist |
| `released` | ModelReleased | sales, product-lead |  |
| `incident-logs` | Incident | anyone | reads-the-logs |
| `incident-doomer` | Incident | anyone | doomer |
| `incident-accel` | Incident | anyone | accelerationist |
| `incident-safety` | Incident | safety, red-teamer, alignment-fellow |  |
| `incident` | Incident | anyone |  |
| `customer-complaint` | CustomerComplaint | sales, product-lead |  |
| `complaint-answered-good` | ComplaintAnswered | sales, product-lead |  |
| `complaint-answered-bad` | ComplaintAnswered | sales, product-lead |  |
| `trust-lost` | TrustLost | sales, product-lead |  |
| `press-good` | PressAnswered | anyone |  |
| `press-bad` | PressAnswered | anyone |  |
| `crown` | GauntletCrown | anyone |  |
| `demo-good` | GauntletDemo | anyone |  |
| `demo-bad` | GauntletDemo | anyone |  |
| `exposed` | GauntletExposed | anyone |  |
| `funding` | FundingClosed | researcher, engineer |  |
| `in-debt` | InDebt | engineer, infra |  |
| `heatwave` | Heatwave | engineer, infra |  |
| `poached` | StaffPoached | anyone |  |
| `poach-offered` | PoachOffered | anyone |  |
| `poach-kept` | PoachCountered | anyone |  |
| `poach-left` | PoachCountered | anyone |  |

## The glow

The bible's chapter 10: the models' presence shows as a light running through the building, and "when a sandbagging model corrupts its dashboard readings, the building still shows the flicker. A careful player learns to trust the room over the chart."

`Glow.Of(config, state, lab)` looks at every released or trained model and gives the worst one's stage. A stage applies when the model's weekly incident chance reaches its chance, or the rung of the incident it would have reaches its rung; the highest that applies wins.

| Stage | Name | From weekly chance | From rung |
| --- | --- | --- | --- |
| `calm` | Calm | 0 permille | - |
| `hum` | Humming | 10 permille | - |
| `flicker` | Flickering | - | 2 |
| `empty-corridor` | Empty corridor | - | 3 |
| `doors-locked` | Doors locked | - | 4 |
| `power-dip` | Power dips | - | 5 |
| `quiet` | Quiet | - | 6 |

The report also carries a second reading, what the dashboard's own numbers would suggest: the same calculation on the displayed alignment with no corruption. When the room is higher than the chart, `ChartIsMisleading` is true. Brightness (0 to 1000) is the combined capability of the latest model of each line, reaching 1000 at 10000.
