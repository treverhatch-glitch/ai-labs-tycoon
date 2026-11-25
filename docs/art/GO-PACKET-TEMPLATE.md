# Graphics GO packet template

```
SHA: <full commit>
Parent: <parent full>
Author/committer: 342607508+treverhatch-glitch@users.noreply.github.com
Co-authored-by: (must be empty — use commit-tree if hooks inject)
Prepush: python3 tools/art/print_art_summary.py (includes verify_handoff_lists)
Remote parent before push: <short>
Spacing: ≥30m since last push on this branch
```

Plain push: `git push -u origin cursor/graphics-pipeline-5032` (no force).

**Never** `git push --force` / `--force-with-lease`. Do **not** rewrite pushed SHAs to strip `Co-authored-by`; only new commits must be clean (`prepush.sh` grep).

**Commits:** always `git commit-tree` (or merge-no-coauthor on `main`); grep message before push; **no empty commits** to retrigger CI — use workflow_dispatch / Re-run jobs.

Store policy: project store [preferences.md](/cursor/stores/bc-65240cc3-45b5-52f3-98ce-af719f90ff1e/preferences.md) (Hatch ~16:01 ICT).

**Index lane (batches 50+):** store packet `internal/graphics-batch{N}-go-packet.md`; hub [PRESENTATION-INDEX-HUB.md](PRESENTATION-INDEX-HUB.md). Constants: `CampusGoPacketTemplatePresentationSpec` (batch 63).

Post-60 refresh: [GO-PACKET-INDEX-LANE-REFRESH.md](GO-PACKET-INDEX-LANE-REFRESH.md) (batch 77).