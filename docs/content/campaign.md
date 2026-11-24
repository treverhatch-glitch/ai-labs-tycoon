<!-- Generated from Rules/Data/default-config.json (campaign). The data file is the source of truth: edit it, then update this page. -->
# Era goals and the run's score

The bible's chapter 13 says "a sandbox mode has no deadline. A campaign mode sets goals for each era and scores the run at the end." The rules now keep the score either way: when an era ends, the player's lab is marked against that era's goals and the mark is saved. A sandbox player can ignore it; nothing in the sim reads it, so it changes no one's play (see QUESTIONS Q264 to Q266).

## The goals

Each era has three goals in `campaign.goals`, each worth **100** points (`points`), plus **50** (`allMetBonus`) for meeting all three. A goal is a metric and a target, read off the player's lab at the moment the era ends:

| Metric | Reads |
|---|---|
| `ModelsReleased` | Models that have been on sale (on sale now or retired) |
| `SectorsServed` | Sectors with a model on sale |
| `SectorsLed` | Sectors whose crown the lab holds |
| `Reputation` | Reputation, 0 to 1000 |
| `Staff` | People on the payroll |
| `Techs` | Techs researched |
| `LifetimeRevenue` | Everything ever earned from customers |
| `BestAlignment` | Best true alignment of a model on sale, 0 to 1000 |

Every era asks for lifetime revenue (it rises from $400K in Dial-up Days to $6B in Upward) and two others that change by era: sectors, releases, techs, hiring, reputation, crowns, a careful model.

## Marking

- When the next era begins, `Campaign.OnEraBegan` marks the era that ended for the **player only** and saves an `EraScore` (`eraId`, goals met, goals there were, points) on the lab, once per era, with an `EraScored` event.
- A game that starts in a later era has no marks for the eras before it. A bankrupt lab is not marked.
- `Campaign.Progress` lists each goal of an era with the lab's value and whether it is met; `Campaign.Mark` is what an era would score right now; `Campaign.RunScore` is every saved mark plus the current era as it stands.

## Notes

- Saved per lab: `eraScores`, schema 39.
- The golden moved because every run that crosses an era now carries a mark and an event; with the sync line, the hook, the schema number and the data removed, the previous golden returned exactly.
- Open: the scoreboard and the final screen (Unity); a campaign flag that ends the run at a deadline; score bonuses for the ending reached; difficulty scaling of the targets.
