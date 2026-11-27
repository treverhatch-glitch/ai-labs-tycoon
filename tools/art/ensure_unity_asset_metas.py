#!/usr/bin/env python3
"""Create missing .meta for Assets files (graphics lane imports)."""
from __future__ import annotations

import hashlib
import sys
from pathlib import Path

ROOT = Path(__file__).resolve().parents[2] / "unity" / "Assets"
SKIP_NAMES = {".DS_Store"}
MODEL_EXT = {".fbx", ".glb", ".obj"}
TEXTURE_EXT = {".png", ".jpg", ".jpeg"}
JSON_EXT = {".json"}


def guid_for(rel: str) -> str:
    return hashlib.sha256(f"ailabs-meta:{rel}".encode()).hexdigest()[:32]


def model_meta(rel_posix: str) -> str:
    g = guid_for(rel_posix)
    return (
        f"fileFormatVersion: 2\n"
        f"guid: {g}\n"
        f"ModelImporter:\n"
        f"  externalObjects: {{}}\n"
        f"  serializedVersion: 4\n"
        f"  materials:\n"
        f"    materialImportMode: 2\n"
        f"  animations:\n"
        f"    legacyGenerateAnimations: 4\n"
        f"  meshes:\n"
        f"    lODScreenPercentages: []\n"
        f"    globalScale: 1\n"
        f"  tangentSpace:\n"
        f"    normalSmoothAngle: 60\n"
    )


def texture_meta(rel_posix: str) -> str:
    g = guid_for(rel_posix)
    return (
        f"fileFormatVersion: 2\n"
        f"guid: {g}\n"
        f"TextureImporter:\n"
        f"  externalObjects: {{}}\n"
        f"  serializedVersion: 13\n"
        f"  maxTextureSize: 2048\n"
        f"  textureSettings:\n"
        f"    filterMode: 1\n"
        f"    aniso: 1\n"
        f"  spriteMode: 0\n"
        f"  alphaUsage: 1\n"
    )


def default_meta(rel_posix: str) -> str:
    g = guid_for(rel_posix)
    return (
        f"fileFormatVersion: 2\n"
        f"guid: {g}\n"
        f"DefaultImporter:\n"
        f"  externalObjects: {{}}\n"
        f"  userData:\n"
        f"  assetBundleName:\n"
        f"  assetBundleVariant:\n"
    )


def folder_meta(rel_posix: str) -> str:
    g = guid_for(rel_posix + "/")
    return f"fileFormatVersion: 2\n" f"guid: {g}\n" f"folderAsset: yes\n"


def pick_template(path: Path) -> str:
    rel = path.relative_to(ROOT).as_posix()
    if path.is_dir():
        return folder_meta(rel)
    ext = path.suffix.lower()
    if ext in MODEL_EXT:
        return model_meta(rel)
    if ext in TEXTURE_EXT:
        return texture_meta(rel)
    return default_meta(rel)


def main() -> int:
    created = 0
    for path in sorted(ROOT.rglob("*")):
        if path.name in SKIP_NAMES:
            continue
        if path.name.endswith(".meta"):
            continue
        rel = path.relative_to(ROOT)
        meta = path.with_name(path.name + ".meta") if path.is_file() else path / ".meta"
        if path.is_dir():
            meta = path / ".meta"
        if meta.is_file():
            continue
        meta.parent.mkdir(parents=True, exist_ok=True)
        meta.write_text(pick_template(path if path.is_file() else path), encoding="utf-8")
        created += 1
    print(f"OK: created {created} missing meta files")
    return 0


if __name__ == "__main__":
    sys.exit(main())