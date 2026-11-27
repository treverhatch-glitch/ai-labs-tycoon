#!/usr/bin/env python3
"""Emit minimal Unity .meta files for Cc0/compute-kit (deterministic guids from paths)."""
from __future__ import annotations

import hashlib
from pathlib import Path

ROOT = Path(__file__).resolve().parents[3]
KIT = ROOT / "unity/Assets/Art/Cc0/compute-kit"


def guid_for(rel: str) -> str:
    return hashlib.md5(f"cc0-compute::{rel}".encode()).hexdigest()


def folder_meta(path: Path, rel: str) -> None:
    meta = path / ".meta"
    if meta.exists():
        return
    g = guid_for(rel)
    meta.write_text(
        f"fileFormatVersion: 2\n"
        f"guid: {g}\n"
        f"folderAsset: yes\n"
        f"DefaultImporter:\n"
        f"  externalObjects: {{}}\n"
        f"  userData:\n"
        f"  assetBundleName:\n"
        f"  assetBundleVariant:\n",
        encoding="utf-8",
    )


def file_meta(path: Path, rel: str, importer: str = "DefaultImporter") -> None:
    meta = Path(str(path) + ".meta")
    if meta.exists():
        return
    g = guid_for(rel)
    body = (
        f"fileFormatVersion: 2\n"
        f"guid: {g}\n"
        f"{importer}:\n"
        f"  externalObjects: {{}}\n"
        f"  userData:\n"
        f"  assetBundleName:\n"
        f"  assetBundleVariant:\n"
    )
    if importer == "ModelImporter":
        body = (
            f"fileFormatVersion: 2\n"
            f"guid: {g}\n"
            f"ModelImporter:\n"
            f"  externalObjects: {{}}\n"
            f"  materials:\n"
            f"  animations:\n"
            f"  userData:\n"
            f"  assetBundleName:\n"
            f"  assetBundleVariant:\n"
        )
    elif importer == "TextureImporter":
        body = (
            f"fileFormatVersion: 2\n"
            f"guid: {g}\n"
            f"TextureImporter:\n"
            f"  externalObjects: {{}}\n"
            f"  userData:\n"
            f"  assetBundleName:\n"
            f"  assetBundleVariant:\n"
        )
    meta.write_text(body, encoding="utf-8")


def walk() -> None:
    if not KIT.exists():
        raise SystemExit(f"missing {KIT}")
    folder_meta(KIT.parent, "Assets/Art/Cc0")
    folder_meta(KIT, "Assets/Art/Cc0/compute-kit")
    for path in sorted(KIT.rglob("*")):
        if path.name.endswith(".meta"):
            continue
        rel = str(path.relative_to(ROOT / "unity"))
        if path.is_dir():
            folder_meta(path, rel)
        elif path.suffix == ".glb":
            file_meta(path, rel, "ModelImporter")
        elif path.suffix == ".png":
            file_meta(path, rel, "TextureImporter")
        elif path.suffix == ".json":
            file_meta(path, rel, "TextScriptImporter" if False else "DefaultImporter")


if __name__ == "__main__":
    walk()
    print("OK: unity metas for compute-kit")