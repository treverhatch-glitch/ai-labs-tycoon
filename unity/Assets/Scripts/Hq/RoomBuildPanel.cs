using System.Collections.Generic;
using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.Systems;
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
        Text _buildSummary;
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
            if (_buildSummary != null && _host?.Simulation != null)
            {
                _buildSummary.text = "Build"
                                 + " · " + HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHiringGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPowerChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekTrainingChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeNetTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomHiringSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeChipGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingGpuUpkeepSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipDevSalesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenueGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekConstructionLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantsRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeFinesPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentHiringSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGrantsLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekTrainingPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuFinesGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekConstructionGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRevenuePowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGpuFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRevenueChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGreyNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesHiringSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantsChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeChipPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeTrainingChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenueTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPowerChipDevSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeFinesGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipGpuSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHiringCapitalSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRevenueGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGpuGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenuePowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeChipDevSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekConstructionChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesCapitalSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLandPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeLaunderGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHardwareHiringSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenueSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeNetLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPowerNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGpuLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePowerChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekTrainingGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGrantsChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantsPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeFinesTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHiringSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqOperationsHud.RunningCostLine(_host.Simulation);
            }
            if (_status != null && !string.IsNullOrEmpty(_host.StatusMessage))
                _status.text = _host.StatusMessage;
            else if (_status != null)
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

            _buildSummary = HqUiFactory.Label(root, "Summary", "Build", 11, TextAnchor.UpperLeft);
            _buildSummary.rectTransform.offsetMin = new Vector2(8f, -58f);
            _buildSummary.rectTransform.offsetMax = new Vector2(-8f, -74f);

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
            _openTypes.Clear();
            for (int i = 0; i < sim.Config.RoomTypes.Count; i++)
            {
                RoomTypeConfig type = sim.Config.RoomTypes[i];
                if (EraSystem.RoomOpen(sim.Config, type, sim.Tick))
                    _openTypes.Add(type.Id);
            }
            for (int i = _typeListRoot.childCount - 1; i >= 0; i--)
                Destroy(_typeListRoot.GetChild(i).gameObject);

            for (int i = 0; i < _openTypes.Count; i++)
            {
                string id = _openTypes[i];
                RoomTypeConfig type = sim.Config.RoomType(id);
                HqUiFactory.SmallButton(_typeListRoot, type.Name, () => SelectType(id));
            }

            if (_openTypes.Count > 0 && !_openTypes.Contains(_selectedTypeId))
                SelectType(_openTypes[0]);
            else
                UpdateSelectionLabel();
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