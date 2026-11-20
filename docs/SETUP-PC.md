# Setup on the owner's PC

What could not be done from a cloud agent, in the order to do it. Everything else (the rules, the tests, the
headless game, CI) works from a fresh clone with only the .NET 8 SDK.

| Step | Why it needs the PC |
| --- | --- |
| [1. Tools](#1-tools) | Installs |
| [2. The Unity project](#2-the-unity-project) | Needs the Unity editor and a signed-in licence |
| [3. The studio](#3-the-studio) | A studio is a folder on the machine the agents run on, with its own keys |
| [4. The owner key](#4-the-owner-key) | Made by the owner, on a device no agent runs on |
| [Beta mode](#beta-mode) | The owner's switch; an agent must not flip it |
| [5. The land script](#5-the-land-script) | Runs Unity |

Never commit a licence file (`.ulf`), a key (`.pem`), a `.env` file, a password or a personal email address. The
repo is public. `.gitignore` covers the usual names and a test fails if one is tracked, but a pasted secret inside
an ordinary file is only caught by reading the diff.

## 1. Tools

- .NET 8 SDK (`dotnet --version` prints 8.x).
- Node 22.3 or newer, for the studio harness.
- Unity Hub with Unity 6000.3 LTS, signed in. Unity Personal is free under Unity's revenue limit; check the current
  terms.
- Git, with Git LFS if the Unity project starts tracking large art.

Check the rules on the PC:

```bash
dotnet test tools/SimTests
dotnet run --project tools/SimCli -- run --years 8
```

## 2. The Unity project

`unity/` is a placeholder. Create the project in it:

1. Unity Hub > New project > Unity 6000.3 > a 3D (URP) template > location: this repo, project name `unity`, so
   that `unity/ProjectSettings/ProjectVersion.txt` exists.
2. Add the rules as a local package. In `unity/Packages/manifest.json`, under `dependencies`:

   ```json
   "com.hatchtag.ailabs.rules": "file:../../Rules"
   ```

   Unity compiles the same sources the tests do, as the assembly `AiLabs.Rules` (`Rules/AiLabs.Rules.asmdef`; it has
   `noEngineReferences` on, so a Unity type in the rules is a compile error in Unity too).
3. Unity writes a `.meta` file beside each rules file on the first import. Commit them: they hold the IDs Unity
   uses, and they must be the same on every machine.
4. `Rules/Data/default-config.json` is the game's data. Load it as text (a `TextAsset` from the package) and hand
   it to `GameConfig.FromJson`. Do not copy the numbers into a scene or a ScriptableObject.
5. The first script is small: `Simulation.NewGame(config, seed)`, a button that calls `Step()`, and labels that
   read `sim.State`. Unity sends `Command`s and draws state; it holds no rules.

The root `.gitignore` already ignores `unity/Library/`, `unity/Temp/`, `unity/Logs/`, `unity/UserSettings/` and
builds.

### The Unity workflow on GitHub

`.github/workflows/unity.yml` runs GameCI (Edit Mode and Play Mode tests, and a Windows build when asked). It is
manual: Actions > unity > Run workflow, on `main`. It uses the repository secrets `UNITY_LICENSE`, `UNITY_EMAIL` and
`UNITY_PASSWORD`, which are already set. While `unity/` holds no project it prints a notice and passes without
starting Unity.

## 3. The studio

The studio harness is the owner's repo
[3d-deep-sim-studio-harness](https://github.com/treverhatch-glitch/3d-deep-sim-studio-harness) (2.96.0 when this was
written). A studio is its own folder; this repo is the game it works on. Nothing from the harness is copied into
this repo.

1. Make the studio beside this repo. From an existing studio or a clone of the harness:

   ```bash
   npm run new-studio -- --name "AI Labs Tycoon" --dir ../ai-labs-tycoon-studio
   ```

   The folders should end up side by side:

   ```
   ai-labs-tycoon/          this repo (the Unity project and the rules)
   ai-labs-tycoon-studio/   the studio (the harness, the board, the agents)
   ```

2. Copy this repo's [`studio/game.json`](../studio/game.json) to `ai-labs-tycoon-studio/studio/game.json`. If the
   folders are not side by side, change `folder` and `cwd` in the copy.
3. `studio/game.json` is a protected file in a studio, so the owner approves the change there:

   ```bash
   npm run approve -- protected --note "AI Labs Tycoon: native Unity, rules checked with dotnet test"
   ```

4. Check the wiring from the studio folder:

   ```bash
   npm run engine -- native          # "this studio is native unity"
   npm run engine -- targets         # lists unity, target main, this repo's folder
   npm run engine -- rules show      # dotnet test tools/SimTests (needs step 5's land command first)
   ```

5. In the studio's `.env` (never committed):

   ```bash
   STUDIO_UNITY=C:\Program Files\Unity\Hub\Editor\<version>\Editor\Unity.exe
   STUDIO_UNITY_PIN=6000.3
   STUDIO_DOTNET=dotnet
   ```

What each key in `game.json` does is in [studio/README.md](../studio/README.md).

## 4. The owner key

Made once, by the owner, on a device no agent runs on:

```bash
npm run approve -- keygen
```

Commit only the public key, `studio/keys/owner-ed25519.pub.pem`, to the studio's main branch. The private key never
goes on the PC the agents use and never into any repo. Until the studio's main branch has the public key, the
harness's checks warn instead of enforcing.

## Beta mode

**State: off.** The owner asked for beta mode. It is a person-only step, so it was left off and is written down
here instead.

What it is: one switch in the studio (harness 2.76.0 and later, `docs/AUTONOMY.md`, "Beta mode") that lets named
outside agents and launched Producer and Director sessions act as the owner while the game is being built.

Where it lives: the file `studio/beta.json` **in the studio folder** (`ai-labs-tycoon-studio/studio/beta.json`). It
is a protected file there and is read as committed in the studio's main checkout. There is no beta setting in a
game repo: nothing in this repo's `studio/game.json` or anywhere else here turns it on.

To turn it on, the owner runs this in their own terminal, in the studio folder:

```bash
npm run beta -- on --by <owner name> --agents <agent name> --why "building AI Labs Tycoon"
npm run beta -- status
```

- `--agents` names the outside agents that may act as the owner (for example `opus-1`). Leave it out to cover only
  the owner proxy's agent and launched Producer and Director sessions.
- **The signature.** If the studio has an owner key (step 4), `beta.json` is signed with the owner's Ed25519
  private key, so the command has to run where that key is: the owner's own terminal. If the studio has no owner key
  yet, an unsigned `beta.json` counts only when the commit is a person's (no `Studio-Agent` trailer).
- The command writes and commits `studio/beta.json`. Push it to the studio's main branch.
- To turn it off: `npm run beta -- off --by <owner name>`, also from the owner's terminal.

The only other supported way is the owner proxy: the owner first grants one agent a standing proxy, signed where
the key is,

```bash
npm run delegate -- proxy <agent name> --by <owner name> --days 30
```

and that agent may then run `npm run beta -- on --by <owner name> --permission "<the owner's yes, in their words>"`
from its own shell. No such grant exists, so no agent can do this today.

An agent must not run `npm run beta -- on` for the owner, write `studio/beta.json` by hand, or commit one without
its agent trailer, even from a shell where the harness would not stop it. The harness refuses the command in a shell
that carries a coding agent's marker (`CLAUDECODE`, `AI_AGENT`, `CODEX_SANDBOX`, `GEMINI_CLI`); a Cursor cloud agent's
shell carries none of those (it sets `CURSOR_AGENT`), so the harness would not have refused it there. That is a gap
in the harness's list worth closing, not a permission.

## 5. The land script

The harness never starts Unity itself. It calls the game's land command, which (on the PC) checks a branch out in
its own worktree, runs the Edit Mode and Play Mode tests and a build, and lands the branch on `main` when green.
Until it exists the harness cannot land Unity branches; the rules check and the quick check work without it.

When the Unity project exists, add to the studio's `studio/game.json` under `engines.unity` (and approve it as in
step 3), and mirror the change into this repo's `studio/game.json`:

```json
"land": "<the land script> {branch}",
"gateWorktree": "../ai-labs-tycoon-gate",
"timeoutMin": 120
```

The contract for the command is in the harness's `docs/UNITY-TRACK.md` ("The land-command contract for a train").
