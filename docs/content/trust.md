# Sector trust and the customer desk

Customers of a sector trust a lab the way they trust a brand: slowly, and it can go in one bad week. Trust is kept per lab per sector, from 0 to 1000, and it moves **appeal** (see [sectors.md](sectors.md)). It is saved on the lab (`sectorTrust`, and `complaints`, save schema 27). A sector with no record sits at the baseline of 500, where trust changes nothing.

## What trust is worth

Appeal is multiplied by 1 + (trust - 500) x 0.3: full trust is +15%, none is -15%. It appears as its own part of the appeal (`TrustBoost`), after the role and founding perks and before the Gauntlet champion bonus.

## Winning it slowly

- **+2 a week** in every sector where the lab has a model on sale, up to 1000.
- **+40** for taking a sector's Gauntlet bracket.
- A sector the lab no longer sells in drifts back to 500 by 1 a week, and its record goes when it gets there.

A bankrupt lab neither gains nor loses trust.

## Losing it fast

- **-60** in every sector an incident's model is on sale in. One incident undoes 30 weeks of gains.
- **-100** in every sector where a tuned Gauntlet win was exposed (see [gauntlet.md](gauntlet.md)).
- Trust never goes below 0, and only what was actually lost counts as lost.

## Carrying between sectors

Sectors belong to a **family**, and the customers of a family talk to each other. **30%** of any trust move (a win or a loss) also lands in each other sector of the same family where the lab has a model on sale. A sector the lab is not in is not touched.

| Family | Sectors |
| --- | --- |
| `games` | `game-ai`, `game-bots` |
| `finance` | `expert-systems`, `fraud-detection`, `doc-reading`, `finance-trading` |
| `web` | `spam-search`, `web-ads`, `voice-assistants`, `consumer-assistant`, `companionship` |
| `language` | `machine-translation`, `writing-tools`, `customer-support`, `legal-research`, `enterprise-knowledge` |
| `creative` | `image-design`, `music-audio`, `video-film` |
| `code` | `code-completion`, `coding-agents`, `agent-workforce` |
| `state` | `government-services`, `cybersecurity` |
| `physical` | `image-recognition`, `autonomous-driving`, `agri-logistics`, `devices`, `robotics-industry`, `household-robots`, `space-compute` |
| `science` | `science-medicine`, `machine-science`, `ai-research-services` |

## The customer desk

When an incident costs the **player's** lab trust, one customer writes in, from the sector where the model earns most (the first one listed on a tie). The complaint's **stake** is the trust the incident cost there. A desk holds 3 complaints; a complaint waits 4 weeks and then lapses with no further cost, so a lab on autopilot plays on as before. Rival labs lose trust the same way but have no desk.

The player answers with `AnswerComplaint`. Each answer costs hiring fees of a sales rep of the era, paid whether or not it works. It works with a chance (the base, plus the lab's reputation above 500 times the slope, never under 5%). A working answer wins back a share of the stake in that sector (not carried to the others). One that fails loses another share of it. Money is booked under the Hiring ledger line.

| Answer | Cost | Chance | Wins back | Loses if it fails |
| --- | --- | --- | --- | --- |
| `refund` | 3 fees and a week of the model's takings in that sector | 100% | 70% | nothing |
| `patch` | 4 fees | 60% + 0.3 per reputation point above 500 | 60% | 30% |
| `apology` | free | 100% | 15% | nothing |

## What rivals do

Rivals gain and lose trust like the player, and it moves their appeal, but they have no desk and no planner answers complaints.
