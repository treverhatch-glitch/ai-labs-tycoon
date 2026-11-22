# Training-data pipeline

Where a model's data comes from (bible ch. 6, "Data"; `datasets.*` in the data; Q427). Thirteen saved lab fields (`dataRaw1` to `dataRaw4`, `dataClean1` to `dataClean4`, `dataJobKind`, `dataJobUnits`, `dataJobLeft`, `dataSuits`, `dataBackfilled`; schema 115). Only the player's lab. A lab that never sources data has no corpus and nothing here applies.

## Three stages

1. **Sourcing** (`AcquireData`, a source and 1 to 200 units): 1 scraped (400 a unit), 2 licensed archives (6,000), 3 synthetic (150; needs a model on sale to write it). Source 4, the lab's own users, is only filled by the user-data flywheel. The lab holds at most 5,000 units, raw and clean.
2. **Cleaning** (`CleanData`, a source and up to 100 raw units): one job at a time, 3 weeks, 300 a unit, and 1 engineer for each 25 units. The units leave the raw stock when the job starts. When it ends a share comes through as clean units: scraped 60%, licensed 95%, synthetic 80%, users 70%.
3. **Licensing** (`LicenseScraped`, 2,500 a unit): scraped units, clean first and then raw, become licensed ones after the fact. A unit in a cleaning job cannot be licensed.

## What the corpus does

Only clean units count. Each is worth a share of a unit by source (scraped 80%, licensed 100%, synthetic 60%, users 90%). Every 100 worth of clean units adds 2.5% of a line's base capability to each new model, up to 30%. It is added to the tech part of the capability, and read when a run finishes, so data bought after a run starts counts only if it is clean by the end.

- **Synthetic data** makes models strange when it is more than 40% of the clean corpus: the gain loses 2 for each permille over, up to 60% of it.
- **Claims** (bible: "Free now. Lawsuits arrive two eras later"). From era 4 (chat boom) on, every 26 weeks a claim lands on a lab that holds any scraped units (raw, clean or in a job): a fine of 800 for each unit, and all of them are taken. Licensed, synthetic and users' units are never taken. Licensing scraped units before the claim is the way to keep them.

Events: `DataAcquired`, `DataCleaned`, `DataLicensed`, `DataSuit`.
