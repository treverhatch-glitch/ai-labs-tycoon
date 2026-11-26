#!/usr/bin/env python3
"""Write stable Unity .meta files for tracked assets (run locally; not part of the game)."""
import hashlib
import os
import sys

REPO = os.path.dirname(os.path.dirname(os.path.abspath(__file__)))


def guid_for(rel_path: str) -> str:
    return hashlib.sha256(("ailabs-meta:" + rel_path).encode()).hexdigest()[:32]


def write_meta(path: str, body: str) -> None:
    meta = path + ".meta"
    if os.path.exists(meta):
        return
    with open(meta, "w", encoding="utf-8") as f:
        f.write(body)


def folder_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
folderAsset: yes
DefaultImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def mono_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
MonoImporter:
  externalObjects: {{}}
  serializedVersion: 2
  defaultReferences: []
  executionOrder: 0
  icon: {{instanceID: 0}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def asmdef_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
AssemblyDefinitionImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def text_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
TextScriptImporter:
  externalObjects: {{}}
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def json_meta(guid: str) -> str:
    return f"""fileFormatVersion: 2
guid: {guid}
NativeFormatImporter:
  externalObjects: {{}}
  mainObjectFileID: 0
  userData: 
  assetBundleName: 
  assetBundleVariant: 
"""


def walk(root: str, rel_prefix: str) -> None:
    for dirpath, dirnames, filenames in os.walk(root):
        rel_dir = os.path.join(rel_prefix, os.path.relpath(dirpath, root)).replace("\\", "/")
        if rel_dir.endswith("/."):
            rel_dir = rel_prefix
        write_meta(dirpath, folder_meta(guid_for(rel_dir + "/")))
        for name in filenames:
            if name.endswith(".meta"):
                continue
            full = os.path.join(dirpath, name)
            rel = os.path.join(rel_prefix, os.path.relpath(full, REPO)).replace("\\", "/")
            g = guid_for(rel)
            if name.endswith(".cs"):
                write_meta(full, mono_meta(g))
            elif name.endswith(".asmdef"):
                write_meta(full, asmdef_meta(g))
            elif name.endswith(".json") and "Rules/Data" in rel:
                write_meta(full, text_meta(g))
            elif name == "package.json":
                write_meta(full, json_meta(g))


def main() -> int:
    walk(os.path.join(REPO, "unity", "Assets"), "unity/Assets")
    rules = os.path.join(REPO, "Rules")
    walk(rules, "Rules")
    print("Meta generation complete (skipped existing .meta files).")
    return 0


if __name__ == "__main__":
    sys.exit(main())