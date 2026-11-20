# studio/

How this repo is wired to the owner's studio harness
([3d-deep-sim-studio-harness](https://github.com/treverhatch-glitch/3d-deep-sim-studio-harness), 2.96.0).

The harness is not in this repo and is not copied into it. A studio is its own folder (a clone of the harness, made
with `npm run new-studio`). This repo is the studio's **engine repo**: the Unity project and the rules it reads.
The only thing a game repo owes the harness is the answer to "what is the game and how is it checked", and that is
one file.

## game.json

[`game.json`](game.json) is the version-controlled copy of the studio's `studio/game.json`. The owner copies it into
the studio and approves it there (it is a protected file in the studio: `npm run approve -- protected`). See
[docs/SETUP-PC.md](../docs/SETUP-PC.md).

| Key | Value | What the harness does with it |
| --- | --- | --- |
| `native` | `unity` | A native-Unity studio: one build per feature, rules in plain code, no web build, no port tasks. |
| `engines` | only `unity` | The one engine track. |
| `engines.unity.folder`, `cwd` | `../ai-labs-tycoon` | Where this repo is, seen from the studio. It assumes the studio sits beside this repo; change it in the studio's copy if not. |
| `engines.unity.target` | `main` | The branch work lands on. |
| `engines.unity.quickCheck` | `dotnet build tools/SimTests --nologo` | A lane's own check at `task done`: the rules and the tests compile. Seconds, no Unity. |
| `engines.unity.rules.test` | `dotnet test tools/SimTests` | The rules check the engine train runs on a batch before any long Unity run. |

Not set yet: `engines.unity.land`. The land command starts Unity on the PC to run Edit Mode and Play Mode tests and
build; it comes with the Unity project (docs/SETUP-PC.md). Until then the harness has nothing to land Unity branches
with, and says so.

A test (`ArchitectureTests`) keeps the file's shape, so the commands the harness is told to run are the ones this
repo has.

This wiring was checked against harness 2.96.0 in a scratch studio: `npm run engine -- native` reports "this studio
is native unity", `npm run engine -- targets` finds this repo, and the harness's own rules check
(`npm run engine -- rules --ref main`) ran `dotnet test tools/SimTests` in its throwaway checkout and passed.

## Beta mode: off, and the owner's to turn on

Beta mode is a switch in the **studio**, not in the game repo: the file `studio/beta.json` in the studio folder,
written by `npm run beta -- on`. Nothing in this repo can turn it on, and a `beta.json` here would do nothing (a test
keeps one from being added).

It is a person's step, so no agent has run it. The exact command, file and signature are in
[docs/SETUP-PC.md](../docs/SETUP-PC.md#beta-mode).
