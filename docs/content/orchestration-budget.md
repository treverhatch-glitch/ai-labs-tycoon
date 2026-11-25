# Orchestration budget

Weekly cap on how many agent-level models run at full orchestration lift (bible ch. 6; `orchestrationBudget.*`; Q454). Lab fields `orchLevel` and `orchOver` (schema 132). Only the player's lab.

`SetOrchBudget` sets the level 0–3 with a setup cost. Too many agent models accrue `orchOver`, which trims agent revenue and adds human-queue items. Events: `OrchBudgetSet`.