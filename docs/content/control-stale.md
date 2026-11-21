<!-- Generated from Rules/Data/default-config.json (control). The data file is the source of truth: edit it, then update this page. -->
# Control goes stale

The bible's chapter 10 lists what erodes control: "each new era: control techniques go stale as models change", and "firing or ignoring the safety team". Only the player's labs go stale; rivals keep their techniques at full strength (see QUESTIONS Q233 to Q235). Nothing is saved: it is worked out from the lab's techs and the date.

## Going stale

The lab's newest **safety tech** is the newest tech it owns that adds to alignment work. The number of eras that have begun since that tech's era is the lab's **stale eras**. The first one is free (`graceEras` 1); each after it takes a tenth (`lossPerEraPermille` 100) of the summed bonus of all the lab's safety techs, up to half (`maxLossPermille` 500).

| Eras since the newest safety tech | Share of the bonus kept |
| --- | --- |
| 0 or 1 | 100% |
| 2 | 90% |
| 3 | 80% |
| 4 | 70% |
| 5 | 60% |
| 6 or more | 50% |

A lab with no safety tech has nothing to lose. Researching any new safety tech makes it the newest, and every older safety tech is back at full strength.

## What it changes

The alignment work a training run does (`TrainingSystem.AlignmentWorkPermille`, which now takes the date) uses the stale bonus in place of the full one. Staff, containment, role perks and traits add to it as before. Models already finished keep what they got.

## Hearing about it

- `ControlStale` is written when a new era leaves a lab with a bigger loss than before (Amount is the permille lost, Ref the newest safety tech). It stops once the cap is reached.
- `ControlRefreshed` is written when new safety research wins ground back (Amount is the permille won back).
- Both have log text, and the safety people say a line about each in the company chat.
- `ControlStale.Of` gives a screen the era, the newest safety tech, the stale eras, the loss, and the bonus before and after.

## Letting the listeners go

Each person fired from a role in `culture.listeningRoles` (safety, red-teamer, alignment-fellow) takes `firedListenerPoints` (15) off culture. Other roles cost nothing. Culture stays between 0 and 1000.

## Notes

- No save change. The pinned golden game did not move.
