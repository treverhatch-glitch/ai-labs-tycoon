# unity/

The Unity project lives here (Unity 6, scenes, the building, mascots and UI). It is a placeholder until the project
is created on a machine with Unity: see [docs/SETUP-PC.md](../docs/SETUP-PC.md).

The one rule: no game rules in this folder. Rules are plain C# in [`Rules/`](../Rules), which this project imports as
a local package and only reads and draws:

```json
{ "dependencies": { "com.hatchtag.ailabs.rules": "file:../../Rules" } }
```

(in `unity/Packages/manifest.json`; the path is relative to `unity/Packages/`).
