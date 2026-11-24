# Research experiments

The bible's core loop says every week the player splits compute three ways: training, serving customers and research experiments. Training and
serving already had a slider (`SetTrainingShare`). This adds the third share (Q291).

## The rule

- `SetResearchShare` (player's lab only) sets aside up to `research.maxComputeSharePermille` (300, 30%) of the lab's **general** compute for experiments.
  Anything over the limit, or below zero, is refused.
- The share comes off general compute first (rented, reserved and owned GPUs and training chips; inference chips are not touched). Training takes its
  share of what is left, and customers are served from the rest, so a lab that sets 20% aside has 80% of its general compute to divide.
- Research made by the lab's researchers is multiplied by `1 + share x research.computeShareBonusPermille / 1000` (1200): a 10% share gives +12%
  research, 30% gives +36%. A lab with no researchers gains nothing.

## What it is for

Compute is worth a lot of research only while there are people to use it. The trade is plain: a lab that is behind on the tree can lean on its
researchers' output for a while, at the price of smaller runs and a thinner service. Zero (the start) changes nothing.

## Notes

- Saved per lab as `researchSharePermille` (schema 43; older saves load with 0). The save check reports a share over the limit.
- Rivals never set it (their plans do not issue the command) and the command is refused for them.
- The pinned golden game moved because the lab record has a new saved field; reverting only that field and the schema number gave the old digest back.
