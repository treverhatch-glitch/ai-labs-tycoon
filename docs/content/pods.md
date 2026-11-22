# Project pods

A few of the lab's own people working as one pod, with chemistry that comes from who they are (`pods.*` in the data; Q409). Six saved lab fields (`podIds`, `podWeeks`, `podChem`, `podSwapAfter`, `podPoints`, `podsFormed`; schema 107). Only the player's lab.

| Command | What happens |
|---|---|
| `FormPod` (2 to 5 staff ids) | costs 8,000 cash for each person; one pod at a time |
| `SwapPodMember` (out, in) | swaps one person; the weeks together are halved; 8 weeks before the next swap |
| `DisbandPod` | ends it |

Chemistry (0 to 1000) is worked out each week: it starts at 300, adds 100 for each different role beyond the first (up to three), 100 if the pod has both a senior and a junior, takes 90 off for each pair of members with the same quirk (trait), adds 12 for each week together (up to 240), and adds a fifth of how far the members' average morale is from 500.

The pod adds research points every week: 6 for each member, scaled by how far chemistry is from 500 (so +6 each at 1000, nothing at 500, and -6 each at 0). A pod with bad chemistry takes points away (never below nothing). A pod that loses people (quits, layoffs) below two breaks up.

Events: `PodFormed`, `PodSwapped`, `PodEnded` (Pick 1 disbanded, 0 broke up; Amount the points added over its life).

A pod is sharp when its chemistry is at least 700 (`pods.sharpChem`). A sharp pod takes a week off each research programme stage (never below one week; `pods.programmeWeeksSaved`) and cuts the weekly support cost of pledged models by a quarter (`pledges.podCutPermille`; Q413, Q416).
