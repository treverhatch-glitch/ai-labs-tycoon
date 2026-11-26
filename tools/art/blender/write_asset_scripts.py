#!/usr/bin/env python3
"""Emit one bpy script per catalogue asset (committed sources)."""
from __future__ import annotations

from pathlib import Path

HEADER = '''#!/usr/bin/env python3
"""Headless Blender asset. Run: blender -b --python tools/art/blender/{name}.py"""
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parent))
import blender_common as bc  # noqa: E402

'''

FOOTER = '''
if __name__ == "__main__":
    bc.run_script(main)
'''

# (asset_id, category, lines of body inside main())
SIMPLE = [
    ("garage-wall-panel", "garage-kit", [
        '    bc.add_cube("Wall", (0.45, 2.2, 0.08), (0, 1.1, 0))',
        '    bc.add_cube("Baseboard", (0.45, 0.12, 0.1), (0, 0.06, 0.02))',
    ]),
    ("garage-exterior-wall", "garage-kit", [
        '    bc.add_cube("Core", (0.45, 2.4, 0.18), (0, 1.2, 0))',
        '    bc.add_cube("Trim", (0.48, 0.08, 0.2), (0, 2.35, 0.01))',
    ]),
    ("garage-roll-door", "garage-kit", [
        '    bc.add_cube("Frame", (2.0, 2.2, 0.12), (0, 1.1, 0))',
        '    for i in range(10):',
        '        bc.add_cube(f"Slat{i}", (1.88, 0.12, 0.04), (0, 0.12 + i * 0.2, 0.04))',
    ]),
    ("garage-server-closet", "garage-kit", [
        '    bc.add_cube("Shell", (1.1, 2.0, 0.9), (0, 1.0, 0))',
        '    bc.add_cube("Door", (0.88, 1.72, 0.06), (0, 0.95, 0.42))',
    ]),
    ("garage-land-for-sale-sign", "garage-kit", [
        '    bc.add_cylinder("Post", 0.05, 1.6, (0, 0.8, 0))',
        '    bc.add_cube("Board", (1.2, 0.7, 0.08), (0, 1.45, 0))',
    ]),
    ("garage-lot-curb", "garage-kit", [
        '    bc.add_cube("Curb", (0.45, 0.14, 0.22), (0, 0.07, 0))',
        '    bc.add_cube("Grass", (0.45, 0.03, 0.28), (0, 0.165, 0.12))',
    ]),
    ("garage-window-frame", "garage-kit", [
        '    bc.add_cube("Frame", (0.9, 0.7, 0.1), (0, 1.5, 0))',
        '    bc.add_cube("Pane", (0.7, 0.5, 0.04), (0, 1.5, 0.03))',
    ]),
    ("garage-crt-tower", "garage-kit", [
        '    bc.add_cube("Body", (0.24, 0.58, 0.44), (0, 0.29, 0))',
        '    bc.add_cube("Floppy", (0.18, 0.06, 0.32), (0, 0.44, 0.2))',
        '    bc.add_cube("Led", (0.04, 0.04, 0.02), (0.05, 0.52, 0.22))',
    ]),
    ("garage-whiteboard-1995", "garage-kit", [
        '    bc.add_cube("Board", (1.4, 0.9, 0.05), (0, 1.1, 0))',
        '    bc.add_cube("Tray", (0.5, 0.04, 0.12), (0, 0.62, 0.04))',
    ]),
    ("garage-coffee-drip", "garage-kit", [
        '    bc.add_cube("Base", (0.28, 0.32, 0.22), (0, 0.16, 0))',
        '    bc.add_cylinder("Pot", 0.1, 0.22, (0, 0.5, 0.05))',
    ]),
    ("garage-cable-spool", "garage-kit", [
        '    bc.add_cylinder("Hub", 0.1, 0.16, (0, 0.08, 0))',
        '    bc.add_cylinder("Coil", 0.16, 0.24, (0, 0.2, 0))',
    ]),
    ("staff-researcher-idle", "staff-kit", [
        '    bc.add_cube("Torso", (0.36, 0.52, 0.24), (0, 0.92, 0))',
        '    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))',
        '    bc.add_cube("Ponytail", (0.08, 0.22, 0.12), (-0.18, 1.42, 0))',
        '    bc.add_cube("Clipboard", (0.18, 0.24, 0.04), (0.22, 0.95, 0.1))',
    ]),
    ("staff-safety-idle", "staff-kit", [
        '    bc.add_cube("Vest", (0.4, 0.45, 0.26), (0, 0.98, 0))',
        '    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))',
        '    bc.add_cube("Clip", (0.14, 0.2, 0.06), (0.24, 0.92, 0.12))',
    ]),
    ("staff-sales-idle", "staff-kit", [
        '    bc.add_cube("Torso", (0.4, 0.52, 0.24), (0, 0.92, 0))',
        '    bc.add_cube("Head", (0.26, 0.26, 0.26), (0, 1.38, 0))',
        '    bc.add_cube("Phone", (0.06, 0.14, 0.05), (0.28, 1.05, 0.1))',
    ]),
    ("staff-recruiter-idle", "staff-kit", [
        '    bc.add_cube("Torso", (0.37, 0.52, 0.24), (0, 0.92, 0))',
        '    bc.add_cube("Folder", (0.22, 0.28, 0.06), (-0.28, 0.96, 0.1))',
        '    bc.add_cube("Tie", (0.06, 0.2, 0.03), (0, 1.05, 0.14))',
    ]),
    ("staff-gpu-tech-idle", "staff-kit", [
        '    bc.add_cylinder("Cap", 0.14, 0.06, (0, 1.48, 0))',
        '    bc.add_cube("Torso", (0.38, 0.52, 0.24), (0, 0.92, 0))',
        '    bc.add_cylinder("Tool", 0.02, 0.2, (0.3, 1.05, 0.05))',
    ]),
    ("staff-founder-idle", "staff-kit", [
        '    bc.add_cube("Torso", (0.38, 0.52, 0.24), (0, 0.92, 0))',
        '    bc.add_cube("Mug", (0.1, 0.12, 0.1), (0.26, 0.98, 0.05))',
    ]),
    ("staff-pose-typing", "staff-kit", [
        '    entry = bc.finish_asset("staff-pose-typing", "staff-kit")',
        '    import importlib.util',
        '    spec = importlib.util.spec_from_file_location("eng", Path(__file__).parent / "staff-engineer-work.py")',
        '    print("OK staff-pose-typing alias engineer")',
        '    return entry',
    ]),
]


def emit(asset_id: str, category: str, body_lines: list[str]) -> None:
    path = Path(__file__).parent / f"{asset_id}.py"
    if asset_id == "staff-pose-typing":
        # alias handled in dedicated file instead
        return
    lines = [
        HEADER.format(name=asset_id),
        f"def main():",
    ]
    if any("return entry" in l for l in body_lines):
        lines.extend(body_lines)
    else:
        lines.extend(body_lines)
        lines.append(f'    entry = bc.finish_asset("{asset_id}", "{category}")')
        lines.append('    print("OK", entry["id"], entry["triangleCount"], "tris")')
        lines.append("    return entry")
    lines.append(FOOTER)
    path.write_text("\n".join(lines) + "\n", encoding="utf-8")


def main() -> None:
    for asset_id, category, body in SIMPLE:
        emit(asset_id, category, body)
    print(f"Emitted {len(SIMPLE)} template scripts")


if __name__ == "__main__":
    main()