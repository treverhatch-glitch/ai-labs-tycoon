# Incident floors

Everything that makes an incident cheaper or rarer is taken off one after the other: the on-call rota and insurance cut the fine, a chief safety officer cuts the weekly chance, and other plans can be added. Without a floor a lab that buys them all would pay almost nothing and almost never have an incident. The `incidentFloor` data sets two floors (Q327). With the shipped data neither floor binds: they only matter when the data, or another branch, adds more cuts.

## The fine floor (`fineFloorPermille`, 150)

The gross fine is the fine after the rung, the sector, the risk class and the era. The lab always pays at least 15% of it itself.

1. The rota cuts the gross fine, but never below the floor (`IncidentFloor.CutFine`).
2. The insurer may pay at most what leaves the floor (`IncidentFloor.MaxPayout`); the claim still counts toward the lab's claim record and the claim event shows what was actually paid.
3. Sector make-good costs are added after and are not part of it.

With the shipped data a lab with insurance level 3 (75% cover) and rota level 3 (30% cut) pays 17.5% of the gross fine, so the floor does not bind. With 100% cover and a 90% rota cut, it pays the floor, 15%. With the floor at 0 it would pay nothing, and with the floor at 1000 no cut does anything.

## The chance floor (`chanceFloorPermille`, 400)

The weekly chance after every cut never falls below 40% of the uncut chance (the model's own chance, plus overload, crunch and what optimisation adds, capped at 1000). The chief safety officer's cut passes through `IncidentFloor.Chance`.

## For anything added later

Any new cut to a fine must call `IncidentFloor.CutFine` and tell the insurer `IncidentFloor.MaxPayout`; any new cut to the weekly chance must call `IncidentFloor.Chance` with the uncut and the cut value. See Q327 for how this meets the branches that are not merged yet.
