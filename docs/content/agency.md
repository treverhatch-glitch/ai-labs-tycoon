# Agent autonomy

How much freedom a released model gives its agents (bible ch. 6, "Agency: tools and long tasks", and the agents era; `agency.*` in the data; Q437). Four saved fields on each model (`agentLevel`, `agentSince`, `agentStreak`, `agentBlowups`) and one on the lab (`agentEarned`; schema 120). Only the player's lab. A model nobody sets earns and risks what it did.

`SetAutonomy` (model, level 0 to 3): none, supervised, delegated, autonomous. Needs a released model and the agents era.

- Each level added costs 40,000. A raise has to wait 4 weeks after the last change. Lowering is free and clears the streak.
- Delegated needs one containment wing, autonomous needs two and an alignment of 600.
- **Revenue:** the agents add 8%, 18% or 32% of what that model earns each week.
- **Incidents:** they add 1%, 3% or 7% to the model's weekly incident chance.
- **Blow-ups:** at delegated and autonomous each week an agent run can go wrong (4% or 9%, less 25% for each containment wing, at most 75% off). It refunds two weeks of the model's revenue, costs 12 trust in each sector it sells in, and resets the streak.
- **Clean streak:** every 26 weeks without a blow-up earns 8 trust in each sector.
- A model that leaves sale loses its agents.

- **Knowledge gap:** a model with a knowledge gap adds up to 6% to the blow-up chance of its agents, scaled by the gap (Q443). A blow-up adds 100 tension to a board that meets (Q445).

Events: `AutonomySet`, `AgentBlowup`, `AgentStreak`.
