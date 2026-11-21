<!-- Generated from Rules/Data/default-config.json (rush). The data file is the source of truth: edit it, then update this page. -->
# Rushing a release

The bible's chapter 6 says "you also pick how long to test before release. Rushing wins the launch window and raises the odds of a public incident", and chapter 10 lists "rushing a release to beat a rival" among the things that erode control. A normal release is a tested one. The player can instead skip some of the testing (see QUESTIONS Q252 to Q254).

## `RushRelease`

It takes the model, the weeks of testing to skip (1 to **6**, `maxWeeks`) and the sectors, and does everything `ReleaseModel` does for a model that has not been on sale before. It is refused, changing nothing, for anything `ReleaseModel` refuses (the lab's own gate, the law's bar, a sector that is closed or too demanding), for a model that is not freshly trained, and for a number of weeks outside 1 to 6.

For each week skipped:

| What | Effect (shipped data) |
|---|---|
| Launch window | The model's appeal is **+4%** for the next **13 weeks** (`launchAppealPermillePerWeek`, `launchWeeks`) |
| Public incident | The weekly incident chance is **+0.3 points** for the next **26 weeks** (`incidentPermillePerWeek` 3 permille, `incidentWeeks`), never past certain |
| Culture | Safety culture **-3** points, once, at launch (`culturePointsPerWeek`) |

So a six-week rush is +24% appeal for a quarter, +1.8 points of incident chance for half a year and -18 culture. The appeal gain is its own line in the appeal breakdown (`LaunchEffect`) and the incident chance has its own line (`FromRush`) in the incident breakdown.

## Notes

- Saved per model: `rushWeeks` (0 for a tested launch), schema 36.
- A launch that was already made normally cannot be rushed afterwards, and rushing does not apply to a model replacing an older one in a new sector later: only the first launch counts.
- Rivals and the autopilot never rush, so their numbers do not change. `EventKind.ModelRushed` is logged and the company chat has a line for it.
