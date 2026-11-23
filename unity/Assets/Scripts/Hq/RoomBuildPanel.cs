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
                                 + " · " + HqDisplayQueriesHud.ThisWeekRentNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRevenueLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekConstructionPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHardwareSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGreyTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRevenueCapitalSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeLaunderNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeHardwareTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekTrainingFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenueGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePatentGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingCapitalSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRevenueLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeSiteChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHardwareNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeNetChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenueFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRentGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGreyPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekUpkeepGpuSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetChipGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeLaunderChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPowerGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRevenueHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeFinesChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalLaunderSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantChipNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRevenueNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipBuildSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeTrainingGrantsSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRentChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPowerLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeHardwareChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekTrainingNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePatentChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHiringHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLandGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeSiteGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGpuHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipGreyGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingHiringSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRevenueConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGrantFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSalariesGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRevenueSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRentFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekConstructionGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeLaunderPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPowerConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderCapitalSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreySalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeNetGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHardwareTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantNetChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSitePowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipDevSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesGpuSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRevenueTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekHiringSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPatentGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPowerRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekCapitalLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSiteRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeConstructionGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLaunderFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePatentRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipDevGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRentRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekHardwareFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRoomGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLandTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantGpuSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeNetPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekCapitalTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGreyRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantRevenueSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLaunderGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeCapitalGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGpuHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeFinesRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLandNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeHiringSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHardwareChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeTrainingGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekNetGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSiteGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHiringChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeLandGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPowerSalariesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekCapitalChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRentGreySummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLaunderChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeSiteTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGpuGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekConstructionNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekHardwareGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeFinesLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekChipRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPatentLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRentLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRoomTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeChipGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPowerGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRentLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeNetRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePowerGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeHardwareRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPatentTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLaunderPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGreyChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRentConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeSalariesPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLandChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekHardwareRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimePatentGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekCapitalGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekTrainingChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSiteFinesSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantPatentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeConstructionPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekHiringGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGreyRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLaunderLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRentGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekCapitalChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGreyLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekConstructionLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekPatentRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantHardwareSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGreyRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekChipLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSiteNetSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekGrantChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeLaunderGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekRentTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRoomChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekFinesChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekCapitalPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSiteLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekLaunderLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekGrantSiteSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeChipLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRentTrainingSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeHiringPowerSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekNetRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekLaunderRoomSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeRentLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekSalariesGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekSiteChipSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekCapitalConstructionSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LifetimeGrantLandSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.LastWeekRoomGrantSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekPatentRentSummaryLine(_host.Simulation.Player)
                                 + " · " + HqDisplayQueriesHud.ThisWeekFinesRentSummaryLine(_host.Simulation.Player)
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