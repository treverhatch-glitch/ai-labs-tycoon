using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
using System.Text;
using AiLabs.Unity.Sim;
using AiLabs.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>Pick a room type and see build refusals from the rules layer.</summary>
    public sealed class RoomBuildPanel : MonoBehaviour
    {
        SimulationHost _host;
        string _selectedTypeId = "research-lab";
        Text _status;
        Text _selection;
        readonly List<string> _openTypes = new List<string>();

        public string SelectedTypeId => _selectedTypeId;

        public static RoomBuildPanel Create(Transform parent, SimulationHost host)
        {
            var panelRoot = HqUiFactory.Panel(parent, "RoomBuildPanel",
                new Vector2(0f, 0.35f), new Vector2(0f, 1f), new Vector2(220f, 0f));
            var panel = panelRoot.AddComponent<RoomBuildPanel>();
            panel._host = host;
            panel.BuildUi(panelRoot.transform);
            panel.RefreshTypes();
            host.StateChanged += panel.OnStateChanged;
            return panel;
        }

        void OnDestroy()
        {
            if (_host != null) _host.StateChanged -= OnStateChanged;
        }

        void OnStateChanged()
        {
            RefreshTypes();
            if (_status != null && !string.IsNullOrEmpty(_host.StatusMessage))
                _status.text = _host.StatusMessage;
            else if (_status != null && string.IsNullOrEmpty(_host.StatusMessage))
                _status.text = "Drag on the lot to place a room.";
        }

        void BuildUi(Transform root)
        {
            var title = HqUiFactory.Label(root, "Title", "Build room", 18, TextAnchor.UpperLeft);
            title.rectTransform.offsetMin = new Vector2(8f, -8f);
            title.rectTransform.offsetMax = new Vector2(-8f, -32f);

            _selection = HqUiFactory.Label(root, "Selected", "", 14, TextAnchor.UpperLeft);
            _selection.rectTransform.offsetMin = new Vector2(8f, -36f);
            _selection.rectTransform.offsetMax = new Vector2(-8f, -56f);

            var list = new GameObject("TypeList");
            list.transform.SetParent(root, false);
            var listRect = list.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0f, 0.2f);
            listRect.anchorMax = new Vector2(1f, 0.85f);
            listRect.offsetMin = new Vector2(8f, 8f);
            listRect.offsetMax = new Vector2(-8f, -8f);
            var layout = list.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.childForceExpandHeight = false;
            layout.childControlHeight = true;
            list.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;

            _status = HqUiFactory.Label(root, "Status", "Drag on the lot to place a room.", 12, TextAnchor.LowerLeft);
            _status.color = new Color(0.85f, 0.9f, 0.95f);
            _status.rectTransform.anchorMin = new Vector2(0f, 0f);
            _status.rectTransform.anchorMax = new Vector2(1f, 0.18f);

            _typeListRoot = list.transform;
        }

        Transform _typeListRoot;

        void RefreshTypes()
        {
            if (_host?.Simulation == null || _typeListRoot == null) return;
            Simulation sim = _host.Simulation;
            bool draw = GarageDrawRoom.Active(sim.Config, sim.Player);
            _openTypes.Clear();
            for (int i = _typeListRoot.childCount - 1; i >= 0; i--)
                Destroy(_typeListRoot.GetChild(i).gameObject);
            for (int i = 0; i < sim.Config.RoomTypes.Count; i++)
            {
                RoomTypeConfig type = sim.Config.RoomTypes[i];
                if (!EraSystem.RoomOpen(sim.Config, type, sim.Tick)) continue;
                bool open = !draw || GarageDrawRoom.DayOneType(sim.Config, type.Id);
                string id = type.Id;
                string label = open ? type.Name : type.Name + " (locked)";
                HqUiFactory.SmallButton(_typeListRoot, label, () => { if (open) SelectType(id); });
                if (open) _openTypes.Add(id);
            }

            if (_openTypes.Count > 0 && !_openTypes.Contains(_selectedTypeId))
                SelectType(_openTypes[0]);
            else
                UpdateSelectionLabel();
        }

        public void SetDragCostReadout(int tiles, long cost, int seats, int overcrowd)
        {
            if (_selection == null) return;
            RoomTypeConfig type = _host?.Simulation?.Config.RoomType(_selectedTypeId);
            if (type == null) return;
            var sb = new StringBuilder();
            sb.Append(type.Name);
            sb.Append(" · $");
            sb.Append(cost);
            sb.Append(" (");
            sb.Append(tiles);
            sb.Append(" tiles)");
            if (seats > 0)
            {
                sb.Append(" · ~");
                sb.Append(seats);
                sb.Append(" desks");
                if (overcrowd > seats) sb.Append(", crowded above ").Append(overcrowd - 1);
            }
            _selection.text = sb.ToString();
        }

        void SelectType(string id)
        {
            _selectedTypeId = id;
            UpdateSelectionLabel();
        }

        void UpdateSelectionLabel()
        {
            if (_selection == null || _host?.Simulation == null) return;
            RoomTypeConfig type = _host.Simulation.Config.RoomType(_selectedTypeId);
            _selection.text = type != null
                ? type.Name + " (" + type.MinWidth + "×" + type.MinHeight + " min)"
                : "";
        }
    }
}