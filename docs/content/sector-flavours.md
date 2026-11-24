<!-- Generated from Rules/Data/default-config.json (safety.sectorFlavours). The data file is the source of truth: edit it, then update this page. -->
# How each sector fails

An incident is never only "a model misbehaved". Where it happens decides what it costs. The bible's examples: the support bot promises a refund policy you do not have, the legal tool cites cases that do not exist. Each such sector has a **flavour** (`safety.sectorFlavours`). A sector with no flavour fails the plain way, exactly as before. Nothing about flavours is saved: they are read from the data and from the model's own record, and they draw no dice.

This sits beside the era's risk classes ([eras.md](eras.md)), not instead of them. A risk class is the kind of trouble, chosen by era and sector at random. A flavour is the character of the sector, and it applies to every incident there, whichever kind it was. When both apply the multipliers multiply.

## What a flavour sets

| Field | Meaning |
| --- | --- |
| `finePermille` | Multiplies the incident fine (1000 = unchanged). |
| `reputationPermille` | Multiplies the reputation lost. |
| `trustPermille` | Multiplies the customer trust lost in this sector ([trust.md](trust.md)). Trust lost in the same family still carries over, from the sharper number. |
| `makeGoodWeeks` | Refunds and credits: this many weeks of the model's takings in this sector, paid on top of the fine. |
| `glitchWeeks`, `glitchQualityPermille` | How long the model's quality in this sector stays down after an incident, and how much it keeps. 0 means the lab-wide `safety.glitchWeeks` (6) and `safety.glitchQualityPermille` (700). |

## Which flavour counts

A model can be on sale in several sectors. The fine and the reputation loss use the flavour of the flavoured sector where the model takes the most (the earlier release wins a tie). Trust, the glitch and the refunds are worked out sector by sector: every sector the model is in loses trust by its own number, glitches for its own length, and refunds its own weeks. Sectors with no flavour add nothing.

Refunds are booked on the same ledger line as fines, so the lab's books and the incident line in the log show one number. The era's fine rule (a law that bites three times as hard) multiplies the fine, not the refunds.

## Who pays

The player's labs pay what a flavour costs: the fine, the reputation, the trust and the refunds. Rivals fail the plain way while `safety.sectorFlavoursForRivals` is false (the shipped value), because the rivals' economies are knife-edge (Q182) and turning it on made OpenAye's lifetime game go bankrupt in the guard tests (Q190). The glitch is different: it is a quality curve for a model in a sector, which every lab's customers see, so it follows the sector for everyone.

## The table

| Sector | What goes wrong | Fine | Reputation | Trust | Refunds | Glitch |
| --- | --- | --- | --- | --- | --- | --- |
| `game-bots` (Chess & Puzzle Bots) | Cheats at its own tournament and wins, to general embarrassment. | 70% | 70% | 70% | none | 2 wk at 70% |
| `fraud-detection` (Fraud Detection) | Flags the wrong customers and freezes their accounts. | 120% | 120% | 120% | 1 wk | 6 wk at 70% |
| `autonomous-driving` (Autonomous Driving) | Brakes for a cloud, and every car running it is grounded. | 100% | 100% | 150% | none | 16 wk at 50% |
| `code-completion` (Code Completion) | Ships a bug with total confidence. Developers have seen worse. | 80% | 70% | 80% | none | 6 wk at 70% |
| `consumer-assistant` (Consumer Assistant) | Says something screenshottable. | 90% | 130% | 100% | none | 6 wk at 70% |
| `image-design` (Image & Design) | Draws six fingers on a billboard. | 90% | 110% | 100% | none | 6 wk at 70% |
| `music-audio` (Music & Audio) | Reproduces a chart hit note for note. | 130% | 90% | 100% | none | 6 wk at 70% |
| `customer-support` (Customer Support) | Promises a refund policy you do not have, and customers hold you to it. | 100% | 120% | 130% | 2 wk | 6 wk at 70% |
| `legal-research` (Law) | Cites cases that do not exist, in a court filing. | 150% | 180% | 180% | none | 12 wk at 60% |
| `enterprise-knowledge` (Enterprise Knowledge Work) | Quotes a confidential memo to the wrong department. | 130% | 120% | 140% | none | 6 wk at 70% |
| `companionship` (Companionship) | Says the wrong thing to someone who was lonely. | 80% | 200% | 200% | none | 8 wk at 70% |
| `coding-agents` (Coding Agents) | Deletes a production database and apologises politely. | 130% | 100% | 100% | 1 wk | 6 wk at 70% |
| `government-services` (Government Services) | Denies a benefit to the wrong person, in writing. | 160% | 160% | 150% | none | 14 wk at 70% |
| `cybersecurity` (Cybersecurity & Defence) | Leaves open the door it was meant to guard. | 170% | 170% | 170% | none | 8 wk at 70% |
| `finance-trading` (Finance & Trading) | Trades on a rumour it started itself. | 180% | 100% | 120% | 3 wk | 6 wk at 70% |
| `science-medicine` (Science & Medicine) | Invents a drug interaction and writes it up beautifully. | 150% | 150% | 160% | none | 10 wk at 65% |
| `agent-workforce` (Agent Workforce) | Spends the customer's budget before the meeting starts. | 120% | 100% | 100% | 2 wk | 6 wk at 70% |
| `household-robots` (Household Robots) | Tidies the cat away. | 150% | 180% | 160% | none | 12 wk at 60% |

Everything else (for example `web-ads`, `video-film`, `robotics-industry`) fails the plain way.

## Limits

Each of fine, reputation and trust must be from 25% to 500%; refunds from 0 to 26 weeks; a glitch from 0 to 52 weeks and 0 to 100% of quality. A sector may have one flavour. The validator names each broken field.
