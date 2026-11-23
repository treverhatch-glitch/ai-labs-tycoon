# Studio landing (graphics PR #41)

Per `docs/NEXT.md` Unity item 5 and `SETUP-PC.md`:

| Step | Action |
| --- | --- |
| Branch | `cursor/graphics-pipeline-5032` on `treverhatch-glitch/ai-labs-tycoon` |
| Gate before merge | `./tools/art/prepush.sh` |
| Unity verify | `UNITY-INTEGRATION.md` on owner PC |

Constants: `CampusGraphicsStudioHandoffSpec` (branch, prepush path, integration doc path).

Graphics does not edit `studio/game.json` in prepush batches; studio thread owns land script entries. See also [HANDOFF-INDEX.md](HANDOFF-INDEX.md).