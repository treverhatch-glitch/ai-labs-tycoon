using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Draws the garage lot grid and room blocks from <see cref="GameState"/> only.</summary>
    public sealed class HqGridView : MonoBehaviour
    {

        Simulation _simulation;
        readonly Dictionary<int, RoomBlockView> _roomViews = new Dictionary<int, RoomBlockView>();
        Transform _floorRoot;
        Transform _roomsRoot;
        Material _floorMaterial;

        public void Bind(Simulation simulation)
        {
            _simulation = simulation;
            EnsureRoots();
            Rebuild();
        }

        public void Refresh()
        {
            if (_simulation == null) return;
            Rebuild();
        }

        public void RefreshGlowFromTraining()
        {
            if (_simulation == null || _roomViews.Count == 0) return;
            var glowMap = GlowSpreadMap.Compute(_simulation.Config, _simulation.Player, _simulation.Tick);
            foreach (var pair in _roomViews)
            {
                int glow = glowMap.TryGetValue(pair.Key, out int g) ? g : 0;
                pair.Value.SetGlowPermille(glow);
            }
        }

        public bool AnyModelTraining()
        {
            if (_simulation == null) return false;
            var models = _simulation.Player.Models;
            for (int i = 0; i < models.Count; i++)
            {
                ModelStatus s = models[i].Status;
                if (s == ModelStatus.Training) return true;
            }
            return false;
        }

        void Update()
        {
            if (AnyModelTraining()) RefreshGlowFromTraining();
        }

        void EnsureRoots()
        {
            if (_floorRoot == null)
            {
                var floor = new GameObject("Floor");
                floor.transform.SetParent(transform, false);
                _floorRoot = floor.transform;
            }
            if (_roomsRoot == null)
            {
                var rooms = new GameObject("Rooms");
                rooms.transform.SetParent(transform, false);
                _roomsRoot = rooms.transform;
            }
        }

        void Rebuild()
        {
            ClearChildren(_floorRoot);
            ClearChildren(_roomsRoot);
            _roomViews.Clear();

            LabState player = _simulation.Player;
            LotConfig lot = _simulation.Config.Lot(player.LotId);
            string eraId = _simulation.Era.Id;
            HqVisualPalette.EraSkin skin = HqVisualPalette.EraSkinFor(eraId);
            skin = HqVisualPalette.PolishSkin(skin, HqFinancialHud.FloorPolishPermille(_simulation.Config, player, eraId));

            BuildFloor(lot, skin);
            BuildRooms(player, lot, skin, eraId);
            AlignMainCamera(lot);
        }

        /// <summary>Interim camera helper; graphics batch 1 replaces with <c>CampusPresentationSpec.ApplyGameplayCamera</c> (see docs/art/UNITY-INTEGRATION.md).</summary>
        public static void AlignMainCamera(LotConfig lot, Camera cam = null)
        {
            if (cam == null) cam = Camera.main;
            if (cam == null || lot == null) return;
            float cx = lot.Width * HqGridLayout.TileSize * 0.5f;
            float cz = lot.Height * HqGridLayout.TileSize * 0.5f;
            cam.transform.position = new Vector3(cx, 8.5f, cz - 6f);
            cam.transform.rotation = Quaternion.Euler(52f, 0f, 0f);
        }

        void BuildFloor(LotConfig lot, HqVisualPalette.EraSkin skin)
        {
            if (_floorMaterial == null)
            {
                _floorMaterial = new Material(HqShaders.Lit());
                _floorMaterial.SetFloat("_Smoothness", 0.15f);
            }
            _floorMaterial.color = skin.Floor;

            var plate = GameObject.CreatePrimitive(PrimitiveType.Cube);
            plate.name = "LotPlate";
            plate.transform.SetParent(_floorRoot, false);
            float w = lot.Width * HqGridLayout.TileSize;
            float h = lot.Height * HqGridLayout.TileSize;
            plate.transform.localScale = new Vector3(w, 0.08f, h);
            plate.transform.localPosition = new Vector3(w * 0.5f, HqGridLayout.FloorY - 0.04f, h * 0.5f);
            plate.GetComponent<Renderer>().material = _floorMaterial;

            for (int y = 0; y <= lot.Height; y++)
            {
                var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.name = "GridY" + y;
                line.transform.SetParent(_floorRoot, false);
                line.transform.localScale = new Vector3(w, 0.01f, 0.02f);
                line.transform.localPosition = new Vector3(w * 0.5f, HqGridLayout.FloorY + 0.01f, y * HqGridLayout.TileSize);
                var r = line.GetComponent<Renderer>();
                r.material = new Material(_floorMaterial) { color = skin.GridLine };
            }

            for (int x = 0; x <= lot.Width; x++)
            {
                var line = GameObject.CreatePrimitive(PrimitiveType.Cube);
                line.name = "GridX" + x;
                line.transform.SetParent(_floorRoot, false);
                line.transform.localScale = new Vector3(0.02f, 0.01f, h);
                line.transform.localPosition = new Vector3(x * HqGridLayout.TileSize, HqGridLayout.FloorY + 0.01f, h * 0.5f);
                var r = line.GetComponent<Renderer>();
                r.material = new Material(_floorMaterial) { color = skin.GridLine };
            }
        }

        void BuildRooms(LabState player, LotConfig lot, HqVisualPalette.EraSkin skin, string eraId)
        {
            var glowMap = GlowSpreadMap.Compute(_simulation.Config, player, _simulation.Tick);

            for (int i = 0; i < player.Rooms.Count; i++)
            {
                RoomState room = player.Rooms[i];
                RoomTypeConfig roomType = _simulation.Config.RoomType(room.TypeId);
                string roomEra = roomType != null ? roomType.Era : eraId;
                int evoStep = EraRoomEvolution.Step(_simulation.Config, roomEra, eraId);
                Color color = EraRoomEvolution.EvolvedRoomColor(_simulation.Config, room.TypeId, skin, eraId);

                var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
                block.name = "Room_" + room.Id;
                block.transform.SetParent(_roomsRoot, false);

                float height = HqGridLayout.RoomHeight * EraRoomEvolution.HeightScale(evoStep);
                float w = room.Width * HqGridLayout.TileSize - 0.04f;
                float d = room.Height * HqGridLayout.TileSize - 0.04f;
                block.transform.localScale = new Vector3(w, height, d);
                block.transform.localPosition = new Vector3(
                    room.X * HqGridLayout.TileSize + w * 0.5f + 0.02f,
                    HqGridLayout.FloorY + height * 0.5f,
                    room.Y * HqGridLayout.TileSize + d * 0.5f + 0.02f);

                var view = block.AddComponent<RoomBlockView>();
                view.Bind(room.Id, color);
                if (glowMap.TryGetValue(room.Id, out int glow)) view.SetGlowPermille(glow);
                _roomViews[room.Id] = view;

                // Procedural props until cursor/graphics-pipeline-5032 OBJ kit is merged — see docs/art/UNITY-INTEGRATION.md.
                int staffHere = HqStarterOfficeDressing.StaffForRoom(player, room, roomType);
                HqStarterOfficeDressing.DressRoom(block.transform, room, roomType, staffHere, _floorMaterial);
            }

            // Placeholder accent column for the era skin (future: mascots / glow source).
            var beacon = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
            beacon.name = "EraAccent";
            beacon.transform.SetParent(_roomsRoot, false);
            beacon.transform.localScale = new Vector3(0.25f, 0.5f, 0.25f);
            beacon.transform.localPosition = new Vector3(
                lot.Width * HqGridLayout.TileSize * 0.5f,
                HqGridLayout.FloorY + 0.55f,
                -0.35f);
            beacon.GetComponent<Renderer>().material = new Material(_floorMaterial) { color = skin.Accent };
        }

        static void ClearChildren(Transform root)
        {
            for (int i = root.childCount - 1; i >= 0; i--)
                Destroy(root.GetChild(i).gameObject);
        }

        void OnDestroy()
        {
            if (_floorMaterial != null) Destroy(_floorMaterial);
        }
    }
}