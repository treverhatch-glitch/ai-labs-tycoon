using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Multi-part primitive stand-ins matching OBJ material groups (until mesh import).</summary>
    public static class EarlyGamePropStandIn
    {
        public static void Spawn(Transform root, string assetId)
        {
            EarlyGameArtPaths.LogResolve(EarlyGameGarageCatalog.PathFor(assetId));
            switch (assetId)
            {
                case "garage-desk-crt":
                    Part(root, "top", new Vector3(0, 0.74f, 0), new Vector3(1.15f, 0.06f, 0.68f), CampusUiTokens.Hex("#8C6E48"));
                    Part(root, "crt", new Vector3(0.14f, 0.95f, -0.02f), new Vector3(0.48f, 0.42f, 0.46f), CampusUiTokens.Hex("#9E9E9E"));
                    Part(root, "screen", new Vector3(0.14f, 0.92f, 0.12f), new Vector3(0.38f, 0.3f, 0.04f), CampusUiTokens.Hex("#3A8C50"));
                    Part(root, "keyboard", new Vector3(0, 0.78f, 0.14f), new Vector3(0.44f, 0.04f, 0.2f), CampusUiTokens.Hex("#2E2E32"));
                    break;
                case "garage-voodude-rack":
                    Part(root, "frame", new Vector3(0, 0.92f, 0), new Vector3(0.62f, 1.85f, 0.78f), CampusUiTokens.Hex("#6A6E78"));
                    for (int i = 0; i < 4; i++)
                        Part(root, "u" + i, new Vector3(0, 0.35f + i * 0.35f, 0.05f), new Vector3(0.5f, 0.18f, 0.5f), CampusUiTokens.Hex("#4A8C58"));
                    break;
                case "garage-folding-chair":
                    Part(root, "seat", new Vector3(0, 0.48f, 0), new Vector3(0.52f, 0.04f, 0.5f), CampusUiTokens.Hex("#5A5888"));
                    Part(root, "back", new Vector3(0, 0.72f, -0.2f), new Vector3(0.5f, 0.45f, 0.05f), CampusUiTokens.Hex("#4A4878"));
                    break;
                default:
                    Part(root, "body", new Vector3(0, 0.5f, 0), new Vector3(0.6f, 1f, 0.6f), EarlyGameHudArt.TrimOrange);
                    break;
            }
        }

        public static void SpawnStaff(Transform root, string roleId)
        {
            EarlyGameArtPaths.LogResolve(EarlyGameStaffCatalog.PathFor(roleId));
            Color shirt = roleId.Contains("engineer") || roleId.Contains("gpu")
                ? CampusUiTokens.Hex("#6A6E78")
                : roleId.Contains("safety")
                    ? EarlyGameHudArt.TrimOrange
                    : CampusUiTokens.Hex("#8C66F2");
            Part(root, "torso", new Vector3(0, 0.95f, 0), new Vector3(0.38f, 0.5f, 0.24f), shirt);
            Part(root, "head", new Vector3(0, 1.38f, 0), new Vector3(0.26f, 0.26f, 0.26f), CampusUiTokens.UiPaper);
            if (roleId.Contains("researcher"))
                Part(root, "hair", new Vector3(-0.16f, 1.45f, 0), new Vector3(0.1f, 0.2f, 0.12f), CampusUiTokens.Hex("#5A4030"));
            if (roleId.Contains("recruiter"))
                Part(root, "folder", new Vector3(-0.28f, 0.96f, 0.1f), new Vector3(0.22f, 0.28f, 0.06f), CampusUiTokens.Hex("#E8D858"));
            if (roleId.Contains("sales"))
                Part(root, "phone", new Vector3(0.28f, 1.05f, 0.1f), new Vector3(0.06f, 0.14f, 0.05f), CampusUiTokens.Hex("#2E2E32"));
        }

        static void Part(Transform root, string name, Vector3 pos, Vector3 scale, Color color)
        {
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = name;
            cube.transform.SetParent(root, false);
            cube.transform.localPosition = pos;
            cube.transform.localScale = scale;
            var col = cube.GetComponent<Collider>();
            if (col != null) Object.Destroy(col);
            cube.GetComponent<Renderer>().material = new Material(HqShaders.Lit()) { color = color };
        }
    }
}