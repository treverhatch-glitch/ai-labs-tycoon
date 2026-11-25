# Internships

A cohort of interns in one role (`internships.*` in the data; Q365). Four saved fields on the lab (`internRole`, `internCount`, `internStart`, `internDue`; schema 77); older saves have none.

## `StartInternships` and `EndInternships`

`StartInternships(labId, roleId, count)`: from 1 to `maxCohort` (4) interns, one cohort at a time. The whole stipend (`stipendPermille`, 25% of the role's hiring fee per intern) must be in the bank now and is paid evenly over `weeks` (26). Interns hold no seat and do no work. Refused for a rival, a game with `enabled` 0, an unknown role, a bad size, a cohort already in the lab and a lab short of the stipend (`InvalidArgument`, `UnknownId`, `NotEnoughCash`).

At the end each intern is offered the job for `convertPermille` (30%) of the hiring fee, so the whole road costs 55% of a plain hire and takes half a year, if there is a free seat in the role and the cash. They arrive with the settled morale plus `convertMorale` (+100) and the default trait. Anyone who cannot be taken on joins the alumni pool on good terms (50% rehire fee).

`EndInternships(labId)` sends the cohort home early: nobody is offered a job, all go to the alumni pool.

## Notes

- Events: `InternshipStarted` (Amount interns, Pick weeks, Ref role) and `InternshipsEnded` (SubjectId interns, Amount taken on, Pick 1 ran its course or 0 cut short).
- No random numbers. The validator keeps stipend plus conversion under the full fee, or interns would cost more than hiring.
