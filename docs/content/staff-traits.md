<!-- Generated from Rules/Data/default-config.json (staffPeople). The data file is the source of truth: edit it, then update this page. -->
# Staff traits

Every person on the payroll has one trait. Until now it was `steady` or `star`; the bible's "trait ideas" add seven more. Each does one thing for the lab, worked out from the staff each time. Nothing new is saved: a person's trait id was always in the save.

| Trait | Share of the player's hires | What it does (each person) |
| --- | --- | --- |
| `steady` | the rest | Nothing. |
| `star` | 12%, rolled first | +15% to their own research; a rival's favourite target ([talent-market.md](talent-market.md)). |
| `tenx` (10X, self-reported) | 4% | +10% to their own research. |
| `doomer` | 6% | +4% alignment work, -3% training speed. Camp "doom". |
| `accelerationist` | 6% | +4% training speed, -3% alignment work. Camp "accel". |
| `mentor` | 4% | +2% to every researcher's output. |
| `reads-the-logs` (Actually Reads The Logs) | 4% | +4% to the share of incidents the lab catches (beside the red-teamers). |
| `interviewing` (Secretly Interviewing) | 5% | +10% on the chance a rival's offer arrives, and a target for offers even when not a star. |
| `sleeps-under-desk` | 4% | -10% to their own research; morale settles 60 points higher. |

## Sums and caps

- Training speed and alignment work from traits stay within 30% either way (`maxTraitSwingPermille`).
- Mentors add at most 20% (`maxMentorPermille`); secret interviews add at most 50% to the offer chance (`maxOfferBoostPermille`).
- The incident-catch share from traits adds to the red-teamers' and the rooms' and the whole stays between 0 and 90%.

## Arguing

Put doomers and accelerationists in one lab and they start arguing. Every doomer and accelerationist pair (the smaller of the two counts) costs the whole lab's research 4% (`frictionPerPairPermille`), at most 30% (`maxFrictionPermille`). Mentors lift research by their own share, so the final research factor is 1000 plus mentors minus arguing, on top of the room perks. A lab with only one camp does not argue.

## Who gets them

Only the player's hires roll a trait (Q184). The roll comes from the new person's id plus `traitRollSalt`, not from a dice stream, so no saved dice move. Plan-run labs hire `steady` or `star` as before.

`Traits.Report` gives a screen every sum, the two camp counts and the arguing cost.
