"""Researcher hero — Kenney CC0 stock + recolour (lab coat, hair cap, glasses)."""
from __future__ import annotations

import os


def build_researcher_kenney() -> None:
    if os.environ.get("RESEARCHER_STOCK_RECOLOR", "").strip() == "1":
        from kenney_researcher_from_founder import load_researcher_from_founder_stock

        load_researcher_from_founder_stock()
        return
    import cc0_staff_lib as c0

    c0.build_researcher_cc0()


def collect_researcher_scene_meshes() -> list:
    import bpy

    if os.environ.get("RESEARCHER_STOCK_RECOLOR", "").strip() == "1":
        from kenney_researcher_from_founder import collect_researcher_founder_meshes

        return collect_researcher_founder_meshes()
    import cc0_staff_lib as c0

    return c0.collect_researcher_scene_meshes()