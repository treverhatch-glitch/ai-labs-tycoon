using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Unity.Sim;
using UnityEngine;

namespace AiLabs.Unity.Hq
{
    /// <summary>Drag a rectangle on the lot; sends BuildRoom commands only.</summary>
    public sealed class RoomBuildController : MonoBehaviour
    {
        SimulationHost _host;
        RoomBuildPanel _panel;
        Camera _camera;
        GameObject _ghost;
        bool _dragging;
        int _anchorX;
        int _anchorY;
        int _lotWidth;
        int _lotHeight;

        public void Bind(SimulationHost host, RoomBuildPanel panel)
        {
            _host = host;
            _panel = panel;
            _camera = Camera.main;
            UpdateLotSize();
            host.StateChanged += OnStateChanged;
        }

        void OnDestroy()
        {
            if (_host != null) _host.StateChanged -= OnStateChanged;
            HideGhost();
        }

        void OnStateChanged()
        {
            UpdateLotSize();
        }

        void UpdateLotSize()
        {
            if (_host?.Simulation == null) return;
            LotConfig lot = _host.Simulation.Config.Lot(_host.Simulation.Player.LotId);
            _lotWidth = lot.Width;
            _lotHeight = lot.Height;
        }

        void Update()
        {
            if (_host?.Simulation == null || _camera == null || _panel == null) return;
            if (Input.GetMouseButtonDown(0) && !UnityEngine.EventSystems.EventSystem.current.IsPointerOverGameObject())
            {
                if (TryPickTile(out _anchorX, out _anchorY))
                {
                    _dragging = true;
                    UpdateGhost(_anchorX, _anchorY, _anchorX, _anchorY);
                }
            }
            else if (_dragging && Input.GetMouseButton(0))
            {
                if (TryPickTile(out int tx, out int ty))
                    UpdateGhost(_anchorX, _anchorY, tx, ty);
            }
            else if (_dragging && Input.GetMouseButtonUp(0))
            {
                _dragging = false;
                TryBuild();
                HideGhost();
            }
        }

        bool TryPickTile(out int x, out int y)
        {
            Ray ray = _camera.ScreenPointToRay(Input.mousePosition);
            var plane = new Plane(Vector3.up, Vector3.zero);
            if (!plane.Raycast(ray, out float enter))
            {
                x = y = -1;
                return false;
            }
            Vector3 hit = ray.GetPoint(enter);
            return HqGridLayout.TryWorldToTile(hit, _lotWidth, _lotHeight, out x, out y);
        }

        void TryBuild()
        {
            if (!TryPickTile(out int bx, out int by)) return;
            RoomTypeConfig type = _host.Simulation.Config.RoomType(_panel.SelectedTypeId);
            if (type == null) return;
            if (!LotTileRect.TryFromDrag(_anchorX, _anchorY, bx, by, type.MinWidth, type.MinHeight,
                    out int x, out int y, out int w, out int h)) return;
            if (!LotTileRect.FitsLot(_lotWidth, _lotHeight, x, y, w, h)) return;

            var cmd = Command.BuildRoom(_host.Simulation.Player.Id, type.Id, x, y, w, h);
            _host.TryApply(cmd);
        }

        void UpdateGhost(int ax, int ay, int bx, int by)
        {
            RoomTypeConfig type = _host.Simulation.Config.RoomType(_panel.SelectedTypeId);
            if (type == null) return;
            if (!LotTileRect.TryFromDrag(ax, ay, bx, by, type.MinWidth, type.MinHeight,
                    out int x, out int y, out int w, out int h)) return;

            EnsureGhost();
            float ww = w * HqGridLayout.TileSize - 0.06f;
            float dd = h * HqGridLayout.TileSize - 0.06f;
            _ghost.transform.localScale = new Vector3(ww, 0.12f, dd);
            _ghost.transform.position = new Vector3(
                x * HqGridLayout.TileSize + ww * 0.5f + 0.03f,
                HqGridLayout.FloorY + 0.08f,
                y * HqGridLayout.TileSize + dd * 0.5f + 0.03f);
        }

        void EnsureGhost()
        {
            if (_ghost != null) return;
            _ghost = GameObject.CreatePrimitive(PrimitiveType.Cube);
            _ghost.name = "BuildGhost";
            var col = _ghost.GetComponent<Collider>();
            if (col != null) Destroy(col);
            var r = _ghost.GetComponent<Renderer>();
            r.material = new Material(HqShaders.Lit()) { color = new Color(0.3f, 0.85f, 0.45f, 0.45f) };
        }

        void HideGhost()
        {
            if (_ghost != null)
            {
                Destroy(_ghost);
                _ghost = null;
            }
        }
    }
}