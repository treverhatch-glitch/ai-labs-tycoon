<!-- Generated from Rules/Data/default-config.json (leads). The data file is the source of truth: edit it, then update this page. -->
# Research leads

The bible's chapter 6: "Results have variance. Sometimes a model is better than it has any right to be and nobody knows why. That mystery becomes a research lead." Only the player's labs find leads. They are saved as a list per lab and one speed number (save schema 32; see QUESTIONS Q236 to Q238).

## Finding one

A finished training run already lands within 8% of its aim. A run that lands **6% or more above** it (`minLuckPermille` 60) leaves a lead for that model (a `LeadFound` event). The desk holds at most **3**; a new lead is not found while it is full. A lead nobody follows goes cold after **52** weeks (`LeadWentCold`).

## Following one

`FollowLead` costs **6 weeks** of the lab's research output (never under 100 points) and takes **8 weeks**. The lab looks into one lead at a time. It is refused, and changes nothing, when the lead has gone, is already being followed, another is being followed, or the lab has too few research points.

## What comes of it

The ending is picked by weight from the lead itself, so the same lead always ends the same way and no dice are used.

| Outcome | Weight | Effect | |
| --- | --- | --- | --- |
| `real-insight` A real insight | 4 | 20 weeks of research output paid out | It was not luck. It was a good idea that nobody had written down. |
| `training-trick` A training trick | 3 | +3% training speed for good (all tricks together capped at +15%) | A learning-rate schedule that looks like a typo. It works. Nobody is allowed to fix the typo. |
| `data-quirk` A data quirk | 2 | 8 weeks of research output paid out | A mislabelled folder of very good data. It has been labelled correctly now, to everyone's regret. |
| `the-cat` The cat | 1 | nothing | The office cat walked across a keyboard. The cat has been thanked and has not noticed. |
| `tuesday` Tuesday | 1 | nothing | The model is nicer on Tuesdays. Nobody knows why. The dashboard has been asked to stop asking. |

## Notes

- `ResearchLeads.Rows` gives a screen each lead with its luck, whether it is being followed, the weeks left and the price.
- A lead's id is the id of its model (one lead per model).
- The autopilot and rivals never follow leads; rivals never find them.
- Save schema 32 adds `researchLeads` and `leadSpeedPermille` to every lab. An older save loads with none.
