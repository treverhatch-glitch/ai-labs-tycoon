# AI Labs Tycoon

A goofy AI-lab tycoon game (working title): Mad Games Tycoon 2, but you run an AI lab. You start in a garage in
1995 selling enemy AI to video game studios, and climb through the eras while three parody rivals (OpenAye,
AntHopticsAI and xAy) do the same.

C# rules core plus Unity. All game rules are plain C# with no Unity types, so they build and test anywhere in
seconds; Unity only draws.

## Try it

Needs the .NET 8 SDK. No Unity.

```bash
dotnet test tools/SimTests                                   # the rules tests
dotnet run --project tools/SimCli -- run --years 8           # watch a whole game play itself
dotnet run --project tools/SimCli -- run --years 8 --events  # ...with the headlines
dotnet run --project tools/SimCli -- soak --games 60         # balance over many seeds
```

## Where things are

| Path | What |
| --- | --- |
| [`docs/DESIGN.md`](docs/DESIGN.md) | The design in short, the owner's decisions, and how the rules are built. Read this first. |
| [`docs/NEXT.md`](docs/NEXT.md) | The roadmap. |
| [`docs/SETUP-PC.md`](docs/SETUP-PC.md) | What needs the owner's PC: Unity, the studio, keys. |
| [`docs/reference/`](docs/reference) | The Game Bible v0.2 and the owner's decisions. |
| [`Rules/`](Rules) | The rules library (netstandard2.1), also a Unity package. Numbers are in `Rules/Data/default-config.json`. |
| [`tools/SimTests/`](tools/SimTests) | The tests (net8, xunit). |
| [`tools/SimCli/`](tools/SimCli) | The headless game. |
| [`unity/`](unity) | The Unity project (placeholder). |
| [`studio/`](studio) | The studio harness wiring. |

## Rules of the repo

- No game rules in Unity scripts, and no Unity types in `Rules/`.
- The rules are deterministic: whole numbers only, seeded dice, no clock. Tests enforce it.
- This repo is public. No secrets, licence files, keys or personal emails in it, ever. Unity credentials live in
  GitHub Actions secrets only.
- Workflows run on pull requests and by hand. Few, large pushes; one pull request per piece of work.
- The rivals are parodies: near-miss names, original designs, no real names or logos.
