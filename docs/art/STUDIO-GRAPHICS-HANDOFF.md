# Studio landing (graphics PR #41)

Per `docs/NEXT.md` Unity item 5 and `SETUP-PC.md`:

| Step | Action |
| --- | --- |
| Branch | `cursor/graphics-pipeline-5032` on `treverhatch-glitch/ai-labs-tycoon` |
| Gate before merge | `./tools/art/prepush.sh` |
| Unity verify | `UNITY-INTEGRATION.md` on owner PC |

Constants: `CampusGraphicsStudioHandoffSpec` (branch, prepush path, integration doc path).

**Index lane (batch 64+):** After `UNITY-INTEGRATION.md` § Index lane — hub docs `PresentationHubDoc` / `HandoffRollupDoc` on the studio spec (batch 65, Q514).

Graphics does not edit `studio/game.json` in prepush batches; studio thread owns land script entries. See also [HANDOFF-INDEX.md](HANDOFF-INDEX.md).