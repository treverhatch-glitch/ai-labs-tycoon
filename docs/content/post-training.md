# Post-training

What a lab can do to a model between training and sale (bible ch. 6, preference tuning and safety tuning; `postTraining.*` in the data; Q439). Four saved fields on each model (`postKind`, `postLeft`, `postPref`, `postSafety`; schema 122). Only the player's lab, from the chat era on, and only on a trained model that has not gone on sale.

`PostTrain` (model, kind) starts a 3 week run that costs 30,000 and uses up 40 clean data units. The model cannot go on sale while it runs (`AbandonPostTraining` stops it, and the data is gone).

- **Preference tuning (1)** uses clean user conversation data. When it is done the model earns 7% more, the next one 4.5%, then 3%. Every preference run adds 1.4% to the model's weekly chance of failing in front of customers: it has learned to please.
- **Safety tuning (2)** uses licensed clean data first, then scraped. When it is done the true and the shown alignment rise by 110, the next by 77, then 54. Every safety run takes 3.5% of the model's revenue: it refuses what customers ask.
- A model takes three runs of each kind. The net change in revenue never falls below minus half.
- Training also wants clean data (the data bonus), and so does the user-data trade: what a run uses is not there for those.

Events: `PostTrainingStarted`, `PostTrainingDone`, `PostTrainingAbandoned`.
