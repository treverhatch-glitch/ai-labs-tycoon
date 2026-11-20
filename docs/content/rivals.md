# Rivals and Envydia

The cast of plan-run labs and the GPU vendor. Numbers are read from `Rules/Data/default-config.json` (`rivals`, `rivalry`,
`vendor`); the test `The_rivals_page_lists_every_rival_and_the_vendor_supply_years` keeps this page honest about the
ids and the years. All names are parody near-misses; no real names or logos (the real-name test enforces it).

## The labs

| Id | Lab | Boss | Model family | Joins | Home | Start cash |
| --- | --- | --- | --- | --- | --- | --- |
| `openaye` | OpenAye | Jam Saltmen | ChatGBT | start | us | $250,000 |
| `anthoptics` | AntHopticsAI | Durian Amodayo | Clause | start | us | $300,000 |
| `xay` | xAy | Elton Husk | Grokk | start | us | $600,000 |
| `goggle` | Goggle DeepMined | Sunder Pitchai | Gemeny | 2010 | us | $5,000,000 |
| `deepsneak` | DeepSneak | The Quiet Committee | Sneak | 2023 | china | $60,000,000 |
| `successor` | Second Wind Labs | A Very Patient Investor | Second | never (9999) | us | $1,000,000 |

A late lab joins at the first month start of its year with its full start cash, the techs of the eras before the current
one, and a standing of 500 with Envydia. `successor` is the plan for a lab the player sold (a later phase); it never spawns.

## Signature behaviours (plan data)

| Lab | Release gate (alignment) | Publishes safety research | Board drama a year | Hall appetite | Blocs | Chip programme | Buys listed chips | Lists chips at |
| --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `openaye` | none | no | 8% | 400 | us | 2024 | yes | n/a |
| `anthoptics` | 550 | yes | 0 | 300 | us, eu | 2025 | yes | n/a |
| `xay` | none | no | 4% | 1000 | us | 2020 | no | n/a |
| `goggle` | 300 | no | 0 | 800 | us, eu, nordics | 2014 | no | +25% |
| `deepsneak` | none | no | 0 | 700 | china | 2023 | yes | n/a |

- **Release gate.** A model below the gate is held back and the next run raises its alignment share by 100 until one passes.
- **Published safety research.** Every other lab pays 30% less for a safety tech a publishing lab already has.
- **Board drama.** A monthly roll; the plan does nothing for 4 weeks.
- **xAy builds fast.** Its six infra engineers join in 2008 (Q51); a hall that opens within 8 weeks posts an event.
- **Decision logs.** 60 lines per lab, oldest dropped. Skipped runs and held-back models are written at most once a quarter per line.

## Feelings toward the player

`FeelingToPlayer` runs from -1000 to 1000 and drifts to 0 by 5 a month. Crown win: -30 for every rival and -60 for the
runner-up. A bracket the player wins: -10 for each other entrant. Below -500 (Feud) a rival adds 15 attention a month in the
player's home bloc and, once chips exist, will not buy the player's chips. Above 300 (Friendly) it pays up to 95% of
Envydia's price per compute for listed chips instead of 90%.

## Envydia, the vendor

Not a lab: no cash, no models. It decides who gets GPUs.

- **Supply** per month, in current-generation units, by year: 1995 400, 2003 1,500, 2009 4,000, 2012 10,000, 2017 40,000, 2020 30,000 (the shortage), 2022 60,000, 2026 150,000.
- **Allocation:** each active lab may buy supply x (200 + standing) / the sum of (200 + standing) each month. Beyond it, `BuyGpus` fails with `AllocationExhausted`. Renting is not rationed.
- **Standing** 0 to 1000, starts at 500: +1 per 2 GPUs bought, drifts to 500 by 5 a month.
- **Price:** list price, +10% below standing 300, -5% above 800.
- **Keynote:** each March in a year a generation opens, that month's supply is halved and an event is posted.
- **Log:** a line whenever a lab's share moves by 10% or more month to month, with the standing, the share before and after, and what moved the standing.
