# API integrations and deprecation

Customers who build on a model version, and the work of moving them when that version goes (`integrations.*` in the data; Q417). Six saved fields: `integrations`, `apiState`, `apiLeft` and `apiSuccessor` on each model, and `apiMoved` and `apiLost` on each lab (schema 110). Only the player's lab.

| Command | What happens |
|---|---|
| `OpenApi` (model) | the model must be on sale; at most 3 APIs at a time |
| `DeprecateApi` (model, successor, 4 to 52 weeks) | the successor is a model of the lab that has an open API, or 0 for none |
| `CancelDeprecation` (model) | back to an open API; what was lost stays lost |

An open API gains 2 integrations a week and 4% of what it already has, up to 600. Each 10 integrations lift the model's revenue by 0.6%, up to 25%, and cost upkeep: 1.5% of the model's revenue plus 0.2% for each 10 integrations (on the salaries line).

A newer model does not inherit integrations, so a lab that moves on from a version with a big base loses the lift unless it moves them. During a deprecation, each week 12% of what is left moves to the successor and 8% of those are lost on the way. At the deadline whatever is left is stranded: it is lost, and each of the model's sectors loses trust (6 for each 100 stranded, up to 12). If everyone has moved before the deadline the API closes cleanly. The old API keeps costing upkeep until it ends, so a long deadline is kind to customers and dear to the lab.

Events: `ApiOpened`, `ApiDeprecated`, `ApiEnded` (Pick 1 clean, 0 stranded; Amount stranded).
