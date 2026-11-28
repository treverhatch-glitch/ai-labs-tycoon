#!/usr/bin/env python3
"""Procedural rival badge silhouettes (CC0). Shapes only — no likenesses."""
from __future__ import annotations

import json
from pathlib import Path

OUT = Path(__file__).resolve().parents[2] / "unity" / "Assets" / "Art" / "Generated" / "rival-silhouettes"

SHAPES = {
    "openaye": '<circle cx="32" cy="32" r="24" fill="{fill}"/>',
    "anthoptics": '<polygon points="32,8 56,32 32,56 8,32" fill="{fill}"/>',
    "xay": '<polygon points="32,6 54,20 54,44 32,58 10,44 10,20" fill="{fill}"/>',
    "envydia": '<rect x="10" y="14" width="44" height="36" rx="4" fill="{fill}"/>',
}


def svg(lab_id: str, body: str) -> str:
    return (
        f'<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 64 64" role="img" aria-label="{lab_id}">\n'
        f"  {body}\n</svg>\n"
    )


def main() -> None:
    OUT.mkdir(parents=True, exist_ok=True)
    fills = {"openaye": "#5A8CF2", "anthoptics": "#F2808C", "xay": "#E85D4A", "envydia": "#76B900"}
    entries = []
    for lab, tpl in SHAPES.items():
        path = OUT / f"silhouette-{lab}.svg"
        path.write_text(svg(lab, tpl.format(fill=fills.get(lab, "#999999"))), encoding="utf-8")
        entries.append({"id": lab, "path": f"Assets/Art/Generated/rival-silhouettes/silhouette-{lab}.svg"})
    manifest = OUT.parent / "rival-silhouettes-manifest.json"
    manifest.write_text(json.dumps({"silhouettes": entries}, indent=2) + "\n", encoding="utf-8")
    print(f"Wrote {len(entries)} silhouette SVGs")


if __name__ == "__main__":
    main()