# AI Labs Tycoon - Hatch's decisions (Nov 20, 2026) - overrides the v0.1 bible where they differ
- Engine: game rules in plain C# with NO Unity types (netstandard2.1 / net8 class library), tested with `dotnet test`, no editor needed. Unity is for scenes/visuals only. NOT browser-first.
- Building: Mad Games Tycoon 2 style top-down cutaway rooms, each with a purpose; buy more land/buildings to expand mid/late game. Two scales: HQ room-building + world map of data center sites. Rooms evolve with eras; the AI visibly glows through the building.
- Multiple models per lab (like Fable/Opus/Sonnet lines), each with its own visible growth, state and safety meter.
- Rival cast: PARODY near-miss names (AntHopticsAI with model "Clause", Jam Saltmen, Elton Husk, Envydia with leather-jacket boss Jensun Hwang, etc). Original character designs, no real logos.
- Start year: selectable start era, recommended default ~1995 (dot-com), 2009 option, 1956 hardcore unlock (still open).
- Endless: endings do NOT end the game; player can keep playing like MGT2.
- Hardware: core "console" analog = designing your own GPUs/AI accelerators (like Google TPUs) that feed back into training; consumer devices (smart glasses, robots) secondary; brain-implant chips very late game.
- Real dark/shady path in v1 (hacking, stealing, copying rivals), circular financing + bubble that can crash the economy, China/government deals, yearly model tournament, sectors to sell models into, shares/monopoly.
- Tone: funny, goofy, cute and alive.
