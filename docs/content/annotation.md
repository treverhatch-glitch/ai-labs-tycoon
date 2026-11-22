# Annotation vendors

Who labels the data (bible ch. 6, labelling and human feedback; `annotation.*` in the data; Q438). Six saved fields on the lab (`annVendor`, `annLeft`, `annExposure`, `annStrikes`, `annAuditWeek`, `annCleaned`; schema 121). Only the player's lab; needs the data pipeline.

`ContractAnnotators` (vendor 1 volume, 2 licensed, 3 expert) takes one vendor for a 26 week term. Only one at a time.

- **Fee:** paid every week whether or not any data is cleaned.
- **Yield:** each cleaning job no bigger than the vendor's size gets +15%, +20% or +30% of its units (volume 200 units, licensed 120, expert 60). A bigger job gets nothing.
- **Exposure:** the volume vendor adds 14 each week (of 1000) where nobody sees it; licensed and expert add none. Each week the chance of a scandal is the exposure divided by 10 per thousand.
- **Scandal:** fine of 60,000, 15 trust lost in every sector the lab sells in, the contract ends and the exposure drops to 150.
- `AuditVendor` costs 12,000 and takes 60% of the exposure away. It can be done every 8 weeks.
- `EndAnnotators` ends the contract early for 60% of the fees still to run.

- A scandal adds 150 board tension when the lab holds board meetings (Q444).

Events: `AnnotatorsContracted`, `AnnotatorsEnded` (Pick 1 term, 0 early; a scandal ends the contract with its own event), `VendorAudited`, `VendorScandal`.
