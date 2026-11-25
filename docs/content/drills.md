# Incident drills

Tabletop drills that build preparedness before a model fails in public (bible ch. 10; `drills.*` in the data; Q447). Five saved fields on the lab (`drillPrep`, `drillLeft`, `drillDue`, `drillPassed`, `drillFailed`; schema 125). Only the player's lab. Nothing happens until the lab starts a drill.

`StartDrill` costs cash, needs the configured number of safety people free, and runs for a few weeks. When it ends the team passes or fails; passing raises preparedness, failing lowers it. Preparedness fades each week and faster if the lab goes too long without another drill. High preparedness cuts how often models fail in front of customers and how much reputation an incident costs, beside what on-call already does.

Events: `DrillStarted`, `DrillFinished` (Pick 1 passed, 0 failed).