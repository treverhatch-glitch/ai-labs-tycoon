"""Researcher hero self-check: mesh basics + 0° front render coat coverage."""
from __future__ import annotations

import os
import sys
import tempfile
from pathlib import Path

import bpy

sys.path.insert(0, str(Path(__file__).resolve().parent))
sys.path.insert(0, str(Path(__file__).resolve().parents[1]))
import blender_common as bc  # noqa: E402
import kenney_researcher_lib as kr  # noqa: E402
from researcher_hero_render_selfcheck import assert_front_coat_render  # noqa: E402


def _assert_mesh_basics() -> None:
    coat = bpy.data.objects.get("ResearcherLabCoat")
    if coat is None or len(coat.data.polygons) < 24:
        n = 0 if coat is None else len(coat.data.polygons)
        raise RuntimeError(f"FAIL researcher_hero_selfcheck lab_coat_faces={n} want>=24")
    glasses = bpy.data.objects.get("ResearcherGlasses")
    if glasses is None or len(glasses.data.vertices) < 80:
        gv = 0 if glasses is None else len(glasses.data.vertices)
        raise RuntimeError(f"FAIL researcher_hero_selfcheck glasses_verts={gv} want>=80")
    print(f"OK researcher_hero_selfcheck mesh coat_faces={len(coat.data.polygons)} glasses_verts={len(glasses.data.vertices)}")


def _render_front_hero() -> Path:
    meshes = kr.collect_researcher_scene_meshes()
    os.environ["EARLY_ART_PREVIEW_RIG"] = "1"
    os.environ["STAFF_PREVIEW_YAW_DEG"] = "0"
    os.environ["STAFF_PREVIEW_ELEV_DEG"] = "8"
    os.environ["STAFF_PREVIEW_MARGIN"] = "1.35"
    os.environ["STAFF_PREVIEW_ASSET"] = "staff-researcher-idle"
    out = Path(tempfile.gettempdir()) / "researcher_hero_selfcheck_front.png"
    for stale in (out, out.with_name(out.stem + ".mask.png")):
        if stale.is_file():
            stale.unlink()
    bc.render_preview(meshes, out, asset_id="staff-researcher-idle")
    mask = out.with_name(out.stem + ".mask.png")
    if not mask.is_file():
        raise RuntimeError(f"FAIL researcher_hero_selfcheck missing_mask {mask}")
    return out


def assert_researcher_hero_geometry(_body: bpy.types.Object | None = None) -> None:
    _assert_mesh_basics()
    png = _render_front_hero()
    assert_front_coat_render(png)


def main() -> None:
    os.environ["RESEARCHER_STOCK_RECOLOR"] = "1"
    kr.build_researcher_kenney()
    body = bpy.data.objects.get("KenneyResearcherStock")
    if body is None:
        raise RuntimeError("FAIL researcher_hero_selfcheck missing_body")
    assert_researcher_hero_geometry(body)
    raise SystemExit(0)


if __name__ == "__main__":
    main()