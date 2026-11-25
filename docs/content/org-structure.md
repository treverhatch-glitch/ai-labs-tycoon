# Organisation structure

How the lab is organised, and what that costs and gains as it grows (`org.*` in the data; Q419). Four saved lab fields (`orgForm`, `orgSince`, `orgDisrupt`, `orgReorgs`; schema 112). Only the player's lab. A lab that never chooses has no structure and none of this applies.

`SetOrg` (1 flat, 2 by function, 3 squads). Changing it starts 8 weeks of disruption (a 25% loss of research and training speed) and cannot be repeated for 26 weeks. The effects are worked out each week from the people the lab has now, and nothing else is saved. Leads are the lab's seniors (see promotions).

| Structure | Loss | Gain |
|---|---|---|
| Flat | 1.5% for each head over 12 | none |
| By function | 8% for each function with people but no senior; 1% for each head over 8 for each function | up to 6% research, in proportion to the functions that have a senior |
| Squads of 5 | 5% for each squad without a senior; 1% for each squad for the meetings between them | up to 12% training speed, in proportion to the squads that have a senior, and only when the lab has 3 different roles |

The loss is capped at 45%. The loss and gain apply to research points a week and to training speed. So a small lab does best flat, a middling one by function once it has promoted a lead for each function, and a large one in squads once it has seniors to spare, and a lab that grows past its structure pays for it every week.

Events: `OrgChanged` (SubjectId the new form, Amount the weeks of disruption), `OrgStrained` (the loss is a quarter of the cap or more; logged every 13 weeks).

Together: a functional lab with a senior safety person prepares full incident reports a week sooner (Q424); squads that are all led halve the cutover loss of a fabric build (Q425).
