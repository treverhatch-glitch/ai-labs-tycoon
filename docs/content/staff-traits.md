<!-- Generated from Rules/Data/default-config.json (staffPeople). The data file is the source of truth: edit it, then update this page. -->
# Staff traits

Every person on the payroll has one trait. Until now it was `steady` or `star`; the bible's "trait ideas" add ten more. Each does one thing for the lab, worked out from the staff each time. Nothing new is saved: a person's trait id was always in the save.

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
| `posts-through-it` (Posts Through It) | 4% | +12% launch buzz and +12% on what a scandal costs the lab, for each one (see below). |
| `quits-by-blog-post` (Will Quit By Blog Post) | 3% | +5% to their own research. The first week their morale is under 300 they quit, and the post costs the lab 20 reputation. |
| `only-interesting` (Only Works On What Is Interesting) | 4% | Their own research is +20% while a variant run is training and -20% while it is not. |

## Sums and caps

- Training speed and alignment work from traits stay within 30% either way (`maxTraitSwingPermille`).
- Mentors add at most 20% (`maxMentorPermille`); secret interviews add at most 50% to the offer chance (`maxOfferBoostPermille`).
- The incident-catch share from traits adds to the red-teamers' and the rooms' and the whole stays between 0 and 90%.

## Posts, quitting and what is interesting

- **Posts through it.** Launch buzz (`PressSystem.Buzz`) rises by each one's `buzzPermille`, and every reputation loss the press room handles (`PressSystem.Damp`: incidents, hearings, quits) rises by `scandalPermille` of the loss. Both stop at `staffPeople.maxPostsPermille` (300).
- **Will quit by blog post.** `Traits.RunQuits` runs each week after morale moves. Anyone whose trait has `quitBelowMorale` and whose morale is under it leaves the payroll, costs `staffPeople.quitReputation` (20, less what the press room damps, more if a poster is in the lab) and is logged as `EventKind.StaffQuit`. Morale rests at 500, so it takes a run of incidents or a long stretch in debt to get there. An open offer to the person goes with them.
- **Only works on what is interesting.** `Traits.HasSomethingInteresting` is true while any model is training with an approach other than the default (item 38). While it is, `interestingPermille` is added to this person's own research, otherwise taken off. Researchers only, since only they make research.

## Arguing

Put doomers and accelerationists in one lab and they start arguing. Every doomer and accelerationist pair (the smaller of the two counts) costs the whole lab's research 4% (`frictionPerPairPermille`), at most 30% (`maxFrictionPermille`). Mentors lift research by their own share, so the final research factor is 1000 plus mentors minus arguing, on top of the room perks. A lab with only one camp does not argue.

## Who gets them

Only the player's hires roll a trait (Q184). The roll comes from the new person's id plus `traitRollSalt`, not from a dice stream, so no saved dice move. Plan-run labs hire `steady` or `star` as before. Total trait share is now 44% beside the star roll; the new ones come after the old ones in the roll, so no old band moved, but some people who used to be plain now have a new trait (the golden run's staff, for one).

`Traits.Report` gives a screen every sum, the two camp counts and the arguing cost.
