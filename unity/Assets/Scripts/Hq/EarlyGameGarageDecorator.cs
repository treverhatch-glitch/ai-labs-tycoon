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

            ApplyFloorTileTexture();
            PlacePropRoot("garage-roll-door", new Vector3(w * 0.5f, 0f, -0.15f), trim);
            PlacePropRoot("garage-server-closet", new Vector3(w - 0.8f, 0f, h - 0.6f), wall);
            PlacePropRoot("garage-land-for-sale-sign", new Vector3(-0.4f, 0f, h * 0.5f), trim);
            PlacePropRoot("garage-desk-crt", new Vector3(1.4f, 0f, 1.2f), wall);
            PlacePropRoot("garage-voodude-rack", new Vector3(w - 1.2f, 0f, 1.5f), CampusUiTokens.Hex("#6A6E78"));
            PlacePropRoot("garage-whiteboard-1995", new Vector3(3.2f, 0f, 0.35f), Color.white);
            PlacePropRoot("garage-coffee-drip", new Vector3(0.8f, 0f, 0.7f), trim);

            for (int i = 0; i < EarlyGameStaffCatalog.AllAssetIds.Count && i < 4; i++)
            {
                string id = EarlyGameStaffCatalog.AllAssetIds[i];
                float x = 2.2f + i * 0.55f;
                PlaceStaffRoot(id, new Vector3(x, 0f, 2.5f));
            }
        }

        void ApplyFloorTileTexture()
        {
            var tex = EarlyGameTextureLoader.LoadAlbedo("tex-garage-carpet");
            if (tex == null) return;
            EarlyGameArtPaths.LogResolve(EarlyGameTextureCatalog.PathFor("tex-garage-carpet"));
        }

        void PlacePropRoot(string assetId, Vector3 position, Color _)
        {
            var root = new GameObject("Garage_" + assetId);
            root.transform.SetParent(_propsRoot, false);
            root.transform.localPosition = position;
            EarlyGamePropStandIn.Spawn(root.transform, assetId);
        }

        void PlaceStaffRoot(string roleId, Vector3 position)
        {
            var root = new GameObject("Staff_" + roleId);
            root.transform.SetParent(_propsRoot, false);
            root.transform.localPosition = position;
            EarlyGamePropStandIn.SpawnStaff(root.transform, roleId);
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