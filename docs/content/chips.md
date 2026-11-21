# Your own chips

How a lab designs its own accelerators, books them at the foundry and what that does to training, serving, the vendor and the
other labs. Numbers are read from `Rules/Data/default-config.json` (`chips`, `vendor`); the test
`The_chips_page_lists_every_target_process_and_option` keeps this page honest about the ids. All names are parody near-misses.

## The chain

1. Research `chip-team` (it unlocks the `chip-design-lab`, the `hw-test-lab`, the role `chip-architect` and inference chips).
2. Hire chip architects. Each one adds 10 design points a week.
3. Start a design: a target, a process, a die, a number format, a memory and a cooling. One design at a time.
4. When the points are done, tape out. The tape-out takes 8 weeks and costs the process's tape-out price. Each tape-out rolls
   for a respin (a design bug): the process's own chance, less 1% for each engineer up to 10. A respin costs 40% of the
   tape-out again and takes 12 more weeks.
5. A design that works starts at the process's yield and climbs every week (10 permille plus one per engineer up to 10,
   doubled by a hardware test lab) up to its maximum.
6. Order chips for a data centre. The cost is the wafers divided by the yield, the foundry has limited room each month, and the
   chips arrive that month's first week plus the process's lead time. The hall's kilowatts are held for them meanwhile.
7. Deliveries have a 5% chance of a yield crash: only 30% of the chips arrive and the money is gone.

## Targets

| Id | Name | Design points | Compute | Trains |
| --- | --- | --- | --- | --- |
| `inference` | Inference chip | 2,000 | 130% | no: it only serves customers |
| `training` | Training chip | 5,000 | 100% | yes |

## Processes (the foundry is TSMSea)

| Id | Name | Opens | Tier | Compute/chip/week | kW | Wafer | Tape-out | Respin | Yield start / max | Lead | Room/month |
| --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- | --- |
| `n28` | TSMSea 28 | 2012 | 3 | 160 | 1 | $9k | $1.5M | 30% | 40% / 90% | 12 weeks | 2,500 |
| `n16` | TSMSea 16 | 2016 | 4 | 450 | 2 | $14k | $4M | 28% | 35% / 90% | 14 weeks | 5,000 |
| `n7` | TSMSea 7 | 2019 | 4 | 1,200 | 2 | $25k | $12M | 26% | 35% / 92% | 14 weeks | 10,000 |
| `n5` | TSMSea 5 | 2022 | 5 | 1,800 | 3 | $45k | $30M | 25% | 30% / 92% | 16 weeks | 15,000 |
| `n3` | TSMSea 3 | 2025 | 6 | 5,000 | 4 | $70k | $80M | 25% | 30% / 93% | 18 weeks | 20,000 |

The 3 nm process also needs the `chip-codesign` tech before a lab may start a design on it (calendar year alone is not enough).

Half the foundry's capacity goes to the vendor's own customers; the other half is the "room" shared by every lab. The tier is the
export tier: the export rules of [world.md](world.md) apply to a chip as to a GPU of that tier, to the buyer's home and the
designer's home both.

## Options

| Group | Id | Needs | Compute | Cost | Power | Points | Yield |
| --- | --- | --- | --- | --- | --- | --- | --- |
| Die | `small` | - | 60% | 50% | 60% | 70% | +10 pts |
| Die | `medium` | - | 100% | 100% | 100% | 100% | 0 |
| Die | `large` | - | 170% | 200% | 160% | 150% | -15 pts |
| Die | `reticle` | `reticle-dies` | 250% | 350% | 230% | 220% | -25 pts |
| Die | `chiplet-mcm` | `chiplet-packaging` | 220% | 280% | 200% | 210% | -20 pts |
| Format | `fp16` | `chip-team` | 100% | 100% | 100% | 100% | 0 |
| Format | `fp8` | `training-silicon` | 160% | 100% | 100% | 130% | 0 |
| Format | `fp4` | `low-precision` | 240% | 100% | 100% | 160% | 0 |
| Memory | `ddr` | - | 80% | 80% | 100% | 90% | 0 |
| Memory | `hbm` | `hbm-memory` | 100% | 100% | 100% | 100% | 0 |
| Memory | `hbm-stacked` | `stacked-hbm` | 130% | 130% | 110% | 120% | -5 pts |
| Cooling | `air` | - | 100% | 100% | 100% | 100% | 0 |
| Cooling | `liquid` | `liquid-cooling` | 110% | 110% | 80% | 110% | 0 |
| Cooling | `optical` | `optical-interconnect` | 118% | 115% | 75% | 115% | -5 pts |

Worked examples (pinned by tests): an inference chip on `n28` with a small die, 16-bit numbers, plain memory and air has compute 99,
1 kW, 1,260 design points and wafers of $3,600. A training chip on `n5` with a medium die, 8-bit numbers, towers of memory and
liquid cooling has compute 4,118, 3 kW, 15,444 design points and wafers of $64,350.

## Software

A lab's chips run at its software maturity: 30% of paper speed when the first design starts, growing with the architects (2
permille per architect per week, on average) to 70%, with the compiler techs (`compiler-stack-1`, `compiler-stack-2`,
`compiler-stack-3`: +10% each) on top, to 100%. Buyers run the seller's software.

## Training and serving

- Training takes the training share of *general* compute: rented and owned GPUs plus training chips. Inference chips never train.
- The part from a lab's *own* training chips counts 15% more (the models are tuned to them). Bought chips do not.
- Customers are served from everything that is not training, inference chips included, each part losing its site's latency.

## Envydia's reaction

| What | Standing |
| --- | --- |
| First training chip taped out | -150 |
| A delivery of your own chips | -50 |
| First chips listed for sale | -100 |
| A lab with 30% of its general compute on its own chips | every other lab +20 a month |

While a lab runs its own training chips Envydia charges it 10% more. A lab whose chip came out in the 8 weeks before the keynote is
upstaged for 12 weeks: it will not buy lists meanwhile.

## Selling chips

A lab can list a working design at a price per chip and a monthly limit. A buyer pays the price; the seller pays the foundry. A plan-run
lab buys a listing when the price per unit of compute is at most 90% of the vendor's GPU (95% for a friendly rival; never from a rival
the player is feuding with, never in an ambush), up to its GPU gap. Goggle lists its chips at the cost of a good chip plus 25%. Every
trade with the player adds 20 to that rival's feeling for the player.
