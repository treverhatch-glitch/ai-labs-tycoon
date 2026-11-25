# Governance

How the company is controlled (bible ch. 5, "Control of your own company"; `governance.*` in the data; Q429). Five saved lab fields (`govForm`, `govSince`, `govDirectors`, `govSearch`, `govResigned`; schema 117). Only the player's lab. A lab that changes nothing is an ordinary company, and nothing here applies.

- **Dual-class shares** (`SetGovernance` 1). A founder share counts for four votes, so the board can only oust the founders when four times their share is at or below the control line. Allowed only while the lab has raised at most 1 round. Every later round brings 15% less cash.
- **Non-profit parent** (`SetGovernance` 2). The founders cannot be outvoted at all. Allowed only before any round. Every later round brings 40% less cash.
- Each is permanent: a lab cannot change back or take the other.
- **Independent directors** (`SeatDirector`). A search costs 150,000 and takes 12 weeks, one at a time, up to 3, and needs reputation 300. Each seated director costs 3,000 a week (on the salaries line) and takes a quarter off the yearly chance of board drama. If the lab cannot pay a week, one director resigns.

Events: `GovernanceChanged`, `DirectorSeated`, `DirectorResigned`.
