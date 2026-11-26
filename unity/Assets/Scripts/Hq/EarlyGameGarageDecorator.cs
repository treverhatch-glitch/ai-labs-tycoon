using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Static garage shell props for Mum's Garage 14×10 (dial-up era block-out).</summary>
    public sealed class EarlyGameGarageDecorator : MonoBehaviour
    {
        const float Tile = HqGridLayout.TileSize;

        Simulation _simulation;
        Transform _propsRoot;

        public void Bind(Simulation simulation)
        {
            _simulation = simulation;
            Rebuild();
        }

        public void Rebuild()
        {
            if (_simulation == null) return;
            if (_simulation.Era.Id != "dial-up-days") return;

            LabState player = _simulation.Player;
            if (player.LotId != "garage") return;

            EnsureRoot();
            ClearChildren(_propsRoot);

            LotConfig lot = _simulation.Config.Lot(player.LotId);
            float w = lot.Width * Tile;
            float h = lot.Height * Tile;
            Color trim = EarlyGameHudArt.TrimOrange;
            Color wall = CampusUiTokens.Hex("#D8C8B0");

            PlaceCatalogProp("garage-roll-door", new Vector3(w * 0.5f, 0f, -0.15f), new Vector3(2f, 2.2f, 0.12f), trim);
            PlaceCatalogProp("garage-server-closet", new Vector3(w - 0.8f, 0f, h - 0.6f), new Vector3(1.1f, 2f, 0.9f), wall);
            PlaceCatalogProp("garage-land-for-sale-sign", new Vector3(-0.4f, 0f, h * 0.5f), new Vector3(0.3f, 1.6f, 0.3f), trim);
            PlaceCatalogProp("garage-desk-crt", new Vector3(1.4f, 0f, 1.2f), new Vector3(1.2f, 0.8f, 0.7f), wall);
            PlaceCatalogProp("garage-voodude-rack", new Vector3(w - 1.2f, 0f, 1.5f), new Vector3(0.6f, 1.8f, 0.75f), CampusUiTokens.Hex("#6A6E78"));
            PlaceCatalogProp("garage-whiteboard-1995", new Vector3(3.2f, 0f, 0.35f), new Vector3(1.4f, 1.2f, 0.1f), Color.white);
            PlaceCatalogProp("garage-coffee-drip", new Vector3(0.8f, 0f, 0.7f), new Vector3(0.3f, 0.6f, 0.25f), trim);

            for (int i = 0; i < EarlyGameStaffCatalog.AllAssetIds.Count && i < 4; i++)
            {
                string id = EarlyGameStaffCatalog.AllAssetIds[i];
                float x = 2.2f + i * 0.55f;
                PlaceStaffProp(id, new Vector3(x, 0f, 2.5f), CampusUiTokens.Hex("#8C66F2"));
            }
        }

        void PlaceCatalogProp(string assetId, Vector3 position, Vector3 scale, Color color)
        {
            EarlyGameArtPaths.LogResolve(EarlyGameGarageCatalog.PathFor(assetId));
            var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.name = "Garage_" + assetId;
            cube.transform.SetParent(_propsRoot, false);
            cube.transform.localPosition = position + Vector3.up * (scale.y * 0.5f);
            cube.transform.localScale = scale;
            var col = cube.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var renderer = cube.GetComponent<Renderer>();
            renderer.material = new Material(HqShaders.Lit()) { color = color };
        }

        void PlaceStaffProp(string roleId, Vector3 position, Color color)
        {
            EarlyGameArtPaths.LogResolve(EarlyGameStaffCatalog.PathFor(roleId));
            var cap = GameObject.CreatePrimitive(PrimitiveType.Capsule);
            cap.name = "Staff_" + roleId;
            cap.transform.SetParent(_propsRoot, false);
            cap.transform.localPosition = position + Vector3.up * 0.85f;
            cap.transform.localScale = new Vector3(0.35f, 0.85f, 0.35f);
            var col = cap.GetComponent<Collider>();
            if (col != null) Destroy(col);
            cap.GetComponent<Renderer>().material = new Material(HqShaders.Lit()) { color = color };
        }

        void EnsureRoot()
        {
            if (_propsRoot != null) return;
            var root = new GameObject("GarageProps");
            root.transform.SetParent(transform, false);
            _propsRoot = root.transform;
        }

        static void ClearChildren(Transform t)
        {
            for (int i = t.childCount - 1; i >= 0; i--)
                Destroy(t.GetChild(i).gameObject);
        }
    }
}