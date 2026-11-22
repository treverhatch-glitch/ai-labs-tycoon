# Graphics GO packet template

```
SHA: <full commit>
Parent: <parent full>
Author/committer: 342607508+treverhatch-glitch@users.noreply.github.com
Co-authored-by: (must be empty — use commit-tree if hooks inject)
Prepush: python3 tools/art/print_art_summary.py
Remote parent before push: <short>
Spacing: ≥30m since last push on this branch
```

Plain push: `git push -u origin cursor/graphics-pipeline-5032` (no force).