# The press

The press room is where launches, crisis comms and keynotes live (bible ch. 14). Three things work together: the **press rooms** you build, the **journalists** who ask about the lab's news, and the **leak**, which turns the press on a rival. Open questions are saved on the lab (`pressInquiries`, save schema 25); everything else is worked out from the config and the rooms each time.

## Press rooms

Two rooms have no seats. They are about the lab's name.

| Room | Opens | Size | Build | Upkeep |
| --- | --- | --- | --- | --- |
| `press-room` | The Punchline (2009) | 3 x 3 or more | $600 a tile | $12 a tile a week |
| `auditorium` | The Chat Boom (2022) | 5 x 4 or more | $1.5k a tile | $30 a tile a week |

Each tile adds a small share (see [room-perks.md](room-perks.md)), up to a cap per room. Together all the press rooms never pass the press caps: scandal damping 60%, launch buzz 100%.

- **Scandal damping** takes a share off every reputation loss from an incident or a hearing. It also raises the chance of your answers to journalists and of your leaks.
- **Launch buzz** is a share of 120 reputation points. A model that goes on sale for the first time earns that many points in reputation. With no press room a launch earns nothing, as before.

## Beats and journalists

A beat in the player's lab brings one journalist to the desk with a question. A beat that moved nothing (a hearing the lab was prepared for) asks nothing, and a desk that already holds 3 questions does not take a fourth. A question waits 3 weeks; unanswered, it lapses and nothing changes, so a lab on autopilot plays as it did before.

| Beat | Kind | Stake |
| --- | --- | --- |
| `launch` | Good | 40 reputation |
| `crown` | Good | What the Gauntlet crown paid |
| `incident` | Bad | The reputation the incident cost (after damping) |
| `hearing` | Bad | The reputation the hearing cost (after damping) |

The journalists who cover a beat take it in turn, by the week.

| Journalist | Outlet | Covers | Chance | A gaffe costs |
| --- | --- | --- | --- | --- |
| `nell-hype` | The Daily Parameter | `incident`, `hearing`, `launch` | -10% | 150% |
| `ravi-benchmark` | Token & Tonic | `launch`, `crown` | +10% | 80% |
| `ada-fine-print` | The Regulator's Ledger | `incident`, `hearing`, `crown` | 0 | 100% |

## Answering

An answer costs some hiring fees of a sales rep of the era, paid whether or not it works. It works with a chance: the base, plus the journalist's chance, plus the lab's scandal damping times the room slope, plus the lab's reputation above 500 times the reputation slope; never under 5% and never over 100%. A working answer wins back a share of the stake; one that fails loses a share of it, scaled by the journalist.

| Answer | Cost | Base | Room slope | Reputation slope | Wins | Loses if it fails |
| --- | --- | --- | --- | --- | --- | --- |
| `honest` | free | 100% | 0 | 0 | 25% | nothing |
| `spin` | 6 fees | 45% | 1.0 | 0.3 | 70% | 60% |
| `exclusive` | 15 fees | 75% | 0.5 | 0.2 | 60% | 15% |

Telling the truth is free and certain but wins back little. Spin pays well and a press room makes it safe. An exclusive costs the most and is the safe bet at a lab with no press room.

## Leaking

A lab can leak a rival's incident to the press. The rival must be a working rival lab that had an incident in the last 26 weeks. The fee is 20 hiring fees of a sales rep of the era, paid win or lose. The chance is 45%, plus the lab's scandal damping times 0.5, plus the lab's reputation above 500 times 0.3.

- **It runs:** the rival loses 70 reputation (less its own press rooms' damping) and 12 points of feeling toward you.
- **It is traced:** you lose 40 reputation (less your own damping) and the rival loses 4 points of feeling.

## What rivals do

Rivals do not leak, answer or build press rooms. The planners and the autopilot never answer a journalist.
