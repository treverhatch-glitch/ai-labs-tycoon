# Era materials (P3 handoff)

Unity thread builds URP Lit materials from:

| Input | Use |
| --- | --- |
| `CampusFallback.mat` | Default OBJ import |
| `era-skin-tokens.json` | Per-era floor hex → `HqVisualPalette.EraSkin` migration |
| `CampusMaterialTokens` | Smoothness/metallic constants |
| `MATERIAL-TOKENS.json` | UI chrome colours |

**Cross-fade:** `CampusEraSkinTokens.EraTransitionSeconds` when `Simulation.Era` changes.