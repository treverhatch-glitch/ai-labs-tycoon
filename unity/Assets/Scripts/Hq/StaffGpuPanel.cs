using AiLabs.Rules.Config;
using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Rules.Systems;
using AiLabs.Unity.Sim;
using AiLabs.Unity.UI;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>Hire/fire and GPU rent/buy controls; each click sends one Command.</summary>
    public sealed class StaffGpuPanel : MonoBehaviour
    {
        SimulationHost _host;
        Text _status;
        Text _gpuSummary;
        Text _staffSummary;
        Transform _roleList;

        public static StaffGpuPanel Create(Transform canvasParent, SimulationHost host)
        {
            var panelRoot = HqUiFactory.Panel(canvasParent, "StaffGpuPanel",
                new Vector2(1f, 0.2f), new Vector2(1f, 1f), new Vector2(240f, 0f),
                new Color(0.07f, 0.08f, 0.11f, 0.94f));
            var rect = panelRoot.GetComponent<RectTransform>();
            rect.pivot = new Vector2(1f, 1f);
            rect.anchoredPosition = Vector2.zero;

            var panel = panelRoot.AddComponent<StaffGpuPanel>();
            panel._host = host;
            panel.BuildUi(panelRoot.transform);
            host.StateChanged += panel.Refresh;
            panel.Refresh();
            return panel;
        }

        void OnDestroy()
        {
            if (_host != null) _host.StateChanged -= Refresh;
        }

        void BuildUi(Transform root)
        {
            HqUiFactory.Label(root, "Title", "Staff & GPUs", 18, TextAnchor.UpperLeft)
                .rectTransform.offsetMax = new Vector2(-8f, -32f);

            _staffSummary = HqUiFactory.Label(root, "StaffSum", "", 13, TextAnchor.UpperLeft);
            _staffSummary.rectTransform.offsetMin = new Vector2(8f, -36f);
            _staffSummary.rectTransform.offsetMax = new Vector2(-8f, -56f);

            _gpuSummary = HqUiFactory.Label(root, "GpuSum", "", 13, TextAnchor.UpperLeft);
            _gpuSummary.rectTransform.offsetMin = new Vector2(8f, -58f);
            _gpuSummary.rectTransform.offsetMax = new Vector2(-8f, -78f);

            _roleList = new GameObject("Roles").transform;
            _roleList.SetParent(root, false);
            var listRect = _roleList.gameObject.AddComponent<RectTransform>();
            listRect.anchorMin = new Vector2(0f, 0.35f);
            listRect.anchorMax = new Vector2(1f, 0.88f);
            listRect.offsetMin = new Vector2(8f, 0f);
            listRect.offsetMax = new Vector2(-8f, 0f);
            var layout = _roleList.gameObject.AddComponent<VerticalLayoutGroup>();
            layout.spacing = 4f;
            layout.childControlHeight = true;
            layout.childForceExpandHeight = false;

            var gpuRow = new GameObject("GpuRow");
            gpuRow.transform.SetParent(root, false);
            var gpuRect = gpuRow.AddComponent<RectTransform>();
            gpuRect.anchorMin = new Vector2(0f, 0.2f);
            gpuRect.anchorMax = new Vector2(1f, 0.32f);
            gpuRect.offsetMin = new Vector2(8f, 0f);
            gpuRect.offsetMax = new Vector2(-8f, 0f);
            var gpuLayout = gpuRow.AddComponent<HorizontalLayoutGroup>();
            gpuLayout.spacing = 4f;
            gpuLayout.childForceExpandWidth = true;

            HqUiFactory.SmallButton(gpuRow.transform, "Rent +1", () => AdjustRent(1));
            HqUiFactory.SmallButton(gpuRow.transform, "Rent -1", () => AdjustRent(-1));
            HqUiFactory.SmallButton(gpuRow.transform, "Buy 1 HQ", BuyOneHqGpu);

            _status = HqUiFactory.Label(root, "Status", "", 11, TextAnchor.LowerLeft);
            _status.rectTransform.anchorMin = new Vector2(0f, 0f);
            _status.rectTransform.anchorMax = new Vector2(1f, 0.18f);
        }

        void Refresh()
        {
            if (_host?.Simulation == null) return;
            Simulation sim = _host.Simulation;
            LabState lab = sim.Player;

            _staffSummary.text = "Staff: " + LabQueries.TotalStaff(lab);
            int owned = LabQueries.HqGpus(lab);
            int rented = lab.RentedGpus;
            var gen = LabQueries.CurrentGeneration(sim.Config, sim.Tick);
                        string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;
            string lastWeekSiteCapital = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(lab);
            string lastWeekSiteCapitalPart = string.IsNullOrEmpty(lastWeekSiteCapital) ? "" : " · " + lastWeekSiteCapital;
            string thisWeekNetTraining = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(lab);
            string thisWeekNetTrainingPart = string.IsNullOrEmpty(thisWeekNetTraining) ? "" : " · " + thisWeekNetTraining;
            string lastWeekTrainingRevenue = HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(lab);
            string lastWeekTrainingRevenuePart = string.IsNullOrEmpty(lastWeekTrainingRevenue) ? "" : " · " + lastWeekTrainingRevenue;
            string thisWeekPowerChip = HqDisplayQueriesHud.ThisWeekPowerChipSummaryLine(lab);
            string thisWeekPowerChipPart = string.IsNullOrEmpty(thisWeekPowerChip) ? "" : " · " + thisWeekPowerChip;
            string lastWeekRoomHiring = HqDisplayQueriesHud.LastWeekRoomHiringSummaryLine(lab);
            string lastWeekRoomHiringPart = string.IsNullOrEmpty(lastWeekRoomHiring) ? "" : " · " + lastWeekRoomHiring;
            string lastWeekConstructionLand = HqDisplayQueriesHud.LastWeekConstructionLandSummaryLine(lab);
            string lastWeekConstructionLandPart = string.IsNullOrEmpty(lastWeekConstructionLand) ? "" : " · " + lastWeekConstructionLand;
            string thisWeekSalariesFines = HqDisplayQueriesHud.ThisWeekSalariesFinesSummaryLine(lab);
            string thisWeekSalariesFinesPart = string.IsNullOrEmpty(thisWeekSalariesFines) ? "" : " · " + thisWeekSalariesFines;
            string lastWeekChipGrey = HqDisplayQueriesHud.LastWeekChipGreySummaryLine(lab);
            string lastWeekChipGreyPart = string.IsNullOrEmpty(lastWeekChipGrey) ? "" : " · " + lastWeekChipGrey;
            string lastWeekCapitalPatent = HqDisplayQueriesHud.LastWeekCapitalPatentSummaryLine(lab);
            string lastWeekCapitalPatentPart = string.IsNullOrEmpty(lastWeekCapitalPatent) ? "" : " · " + lastWeekCapitalPatent;
            string thisWeekGrantsChip = HqDisplayQueriesHud.ThisWeekGrantsChipSummaryLine(lab);
            string thisWeekGrantsChipPart = string.IsNullOrEmpty(thisWeekGrantsChip) ? "" : " · " + thisWeekGrantsChip;
            string lastWeekSiteHardware = HqDisplayQueriesHud.LastWeekSiteHardwareSummaryLine(lab);
            string lastWeekSiteHardwarePart = string.IsNullOrEmpty(lastWeekSiteHardware) ? "" : " · " + lastWeekSiteHardware;
            string lastWeekNetGrey = HqDisplayQueriesHud.LastWeekNetGreySummaryLine(lab);
            string lastWeekNetGreyPart = string.IsNullOrEmpty(lastWeekNetGrey) ? "" : " · " + lastWeekNetGrey;
            string lastWeekRoomPower = HqDisplayQueriesHud.LastWeekRoomPowerSummaryLine(lab);
            string lastWeekRoomPowerPart = string.IsNullOrEmpty(lastWeekRoomPower) ? "" : " · " + lastWeekRoomPower;
            string thisWeekNetChip = HqDisplayQueriesHud.ThisWeekNetChipSummaryLine(lab);
            string thisWeekNetChipPart = string.IsNullOrEmpty(thisWeekNetChip) ? "" : " · " + thisWeekNetChip;
            string lastWeekFinesCapital = HqDisplayQueriesHud.LastWeekFinesCapitalSummaryLine(lab);
            string lastWeekFinesCapitalPart = string.IsNullOrEmpty(lastWeekFinesCapital) ? "" : " · " + lastWeekFinesCapital;
            string thisWeekChipFines = HqDisplayQueriesHud.ThisWeekChipFinesSummaryLine(lab);
            string thisWeekChipFinesPart = string.IsNullOrEmpty(thisWeekChipFines) ? "" : " · " + thisWeekChipFines;
            string lastWeekPowerNet = HqDisplayQueriesHud.LastWeekPowerNetSummaryLine(lab);
            string lastWeekPowerNetPart = string.IsNullOrEmpty(lastWeekPowerNet) ? "" : " · " + lastWeekPowerNet;
            string thisWeekTrainingGrant = HqDisplayQueriesHud.ThisWeekTrainingGrantSummaryLine(lab);
            string thisWeekTrainingGrantPart = string.IsNullOrEmpty(thisWeekTrainingGrant) ? "" : " · " + thisWeekTrainingGrant;
            string lastWeekChipTraining = HqDisplayQueriesHud.LastWeekChipTrainingSummaryLine(lab);
            string lastWeekChipTrainingPart = string.IsNullOrEmpty(lastWeekChipTraining) ? "" : " · " + lastWeekChipTraining;
            string thisWeekChipRent = HqDisplayQueriesHud.ThisWeekChipRentSummaryLine(lab);
            string thisWeekChipRentPart = string.IsNullOrEmpty(thisWeekChipRent) ? "" : " · " + thisWeekChipRent;
            string thisWeekGrantChip = HqDisplayQueriesHud.ThisWeekGrantChipSummaryLine(lab);
            string thisWeekGrantChipPart = string.IsNullOrEmpty(thisWeekGrantChip) ? "" : " · " + thisWeekGrantChip;
            string lastWeekTrainingGrey = HqDisplayQueriesHud.LastWeekTrainingGreySummaryLine(lab);
            string lastWeekTrainingGreyPart = string.IsNullOrEmpty(lastWeekTrainingGrey) ? "" : " · " + lastWeekTrainingGrey;
            string lastWeekGrantSite = HqDisplayQueriesHud.LastWeekGrantSiteSummaryLine(lab);
            string lastWeekGrantSitePart = string.IsNullOrEmpty(lastWeekGrantSite) ? "" : " · " + lastWeekGrantSite;
            string thisWeekTrainingFines = HqDisplayQueriesHud.ThisWeekTrainingFinesSummaryLine(lab);
            string thisWeekTrainingFinesPart = string.IsNullOrEmpty(thisWeekTrainingFines) ? "" : " · " + thisWeekTrainingFines;
            string lastWeekGreyLand = HqDisplayQueriesHud.LastWeekGreyLandSummaryLine(lab);
            string lastWeekGreyLandPart = string.IsNullOrEmpty(lastWeekGreyLand) ? "" : " · " + lastWeekGreyLand;
            string thisWeekGrantTraining2 = HqDisplayQueriesHud.ThisWeekGrantTrainingSummaryLine(lab);
            string thisWeekGrantTraining2Part = string.IsNullOrEmpty(thisWeekGrantTraining2) ? "" : " · " + thisWeekGrantTraining2;
            string lastWeekGrantHardware = HqDisplayQueriesHud.LastWeekGrantHardwareSummaryLine(lab);
            string lastWeekGrantHardwarePart = string.IsNullOrEmpty(lastWeekGrantHardware) ? "" : " · " + lastWeekGrantHardware;
            string lastWeekUpkeepGpu = HqDisplayQueriesHud.LastWeekUpkeepGpuSummaryLine(lab);
            string lastWeekUpkeepGpuPart = string.IsNullOrEmpty(lastWeekUpkeepGpu) ? "" : " · " + lastWeekUpkeepGpu;
            string lastWeekRevenueHardware = HqDisplayQueriesHud.LastWeekRevenueHardwareSummaryLine(lab);
            string lastWeekRevenueHardwarePart = string.IsNullOrEmpty(lastWeekRevenueHardware) ? "" : " · " + lastWeekRevenueHardware;
            string lastWeekPatentTraining = HqDisplayQueriesHud.LastWeekPatentTrainingSummaryLine(lab);
            string lastWeekPatentTrainingPart = string.IsNullOrEmpty(lastWeekPatentTraining) ? "" : " · " + lastWeekPatentTraining;
            string lastWeekLaunderGrey = HqDisplayQueriesHud.LastWeekLaunderGreySummaryLine(lab);
            string lastWeekLaunderGreyPart = string.IsNullOrEmpty(lastWeekLaunderGrey) ? "" : " · " + lastWeekLaunderGrey;
            string lastWeekFinesGrant = HqDisplayQueriesHud.LastWeekFinesGrantSummaryLine(lab);
            string lastWeekFinesGrantPart = string.IsNullOrEmpty(lastWeekFinesGrant) ? "" : " · " + lastWeekFinesGrant;
            string lastWeekChipGrant = HqDisplayQueriesHud.LastWeekChipGrantSummaryLine(lab);
            string lastWeekChipGrantPart = string.IsNullOrEmpty(lastWeekChipGrant) ? "" : " · " + lastWeekChipGrant;
            string lastWeekNetConstruction = HqDisplayQueriesHud.LastWeekNetConstructionSummaryLine(lab);
            string lastWeekNetConstructionPart = string.IsNullOrEmpty(lastWeekNetConstruction) ? "" : " · " + lastWeekNetConstruction;
            string lifetimePatentChip = HqDisplayQueriesHud.LifetimePatentChipSummaryLine(lab);
            string lifetimePatentChipPart = string.IsNullOrEmpty(lifetimePatentChip) ? "" : " · " + lifetimePatentChip;
            string thisWeekGpuPower = HqDisplayQueriesHud.ThisWeekGpuPowerSummaryLine(lab);
            string thisWeekGpuPowerPart = string.IsNullOrEmpty(thisWeekGpuPower) ? "" : " · " + thisWeekGpuPower;
            string thisWeekTrainingNet = HqDisplayQueriesHud.ThisWeekTrainingNetSummaryLine(lab);
            string thisWeekTrainingNetPart = string.IsNullOrEmpty(thisWeekTrainingNet) ? "" : " · " + thisWeekTrainingNet;
            string lastWeekRoomTraining = HqDisplayQueriesHud.LastWeekRoomTrainingSummaryLine(lab);
            string lastWeekRoomTrainingPart = string.IsNullOrEmpty(lastWeekRoomTraining) ? "" : " · " + lastWeekRoomTraining;
            string lifetimeHardwareChip = HqDisplayQueriesHud.LifetimeHardwareChipSummaryLine(lab);
            string lifetimeHardwareChipPart = string.IsNullOrEmpty(lifetimeHardwareChip) ? "" : " · " + lifetimeHardwareChip;
            string thisWeekGrantGrey = HqDisplayQueriesHud.ThisWeekGrantGreySummaryLine(lab);
            string thisWeekGrantGreyPart = string.IsNullOrEmpty(thisWeekGrantGrey) ? "" : " · " + thisWeekGrantGrey;
            string lastWeekPowerLand = HqDisplayQueriesHud.LastWeekPowerLandSummaryLine(lab);
            string lastWeekPowerLandPart = string.IsNullOrEmpty(lastWeekPowerLand) ? "" : " · " + lastWeekPowerLand;
            string thisWeekRentChip = HqDisplayQueriesHud.ThisWeekRentChipSummaryLine(lab);
            string thisWeekRentChipPart = string.IsNullOrEmpty(thisWeekRentChip) ? "" : " · " + thisWeekRentChip;
            string lastWeekCapitalSite = HqDisplayQueriesHud.LastWeekCapitalSiteSummaryLine(lab);
            string lastWeekCapitalSitePart = string.IsNullOrEmpty(lastWeekCapitalSite) ? "" : " · " + lastWeekCapitalSite;
            string lifetimeTrainingGrants = HqDisplayQueriesHud.LifetimeTrainingGrantsSummaryLine(lab);
            string lifetimeTrainingGrantsPart = string.IsNullOrEmpty(lifetimeTrainingGrants) ? "" : " · " + lifetimeTrainingGrants;
            string lastWeekSiteRoom = HqDisplayQueriesHud.LastWeekSiteRoomSummaryLine(lab);
            string lastWeekSiteRoomPart = string.IsNullOrEmpty(lastWeekSiteRoom) ? "" : " · " + lastWeekSiteRoom;
            string thisWeekGrantRevenue = HqDisplayQueriesHud.ThisWeekGrantRevenueSummaryLine(lab);
            string thisWeekGrantRevenuePart = string.IsNullOrEmpty(thisWeekGrantRevenue) ? "" : " · " + thisWeekGrantRevenue;
            string thisWeekChipBuild = HqDisplayQueriesHud.ThisWeekChipBuildSummaryLine(lab);
            string thisWeekChipBuildPart = string.IsNullOrEmpty(thisWeekChipBuild) ? "" : " · " + thisWeekChipBuild;
            string lastWeekGrantPatent = HqDisplayQueriesHud.LastWeekGrantPatentSummaryLine(lab);
            string lastWeekGrantPatentPart = string.IsNullOrEmpty(lastWeekGrantPatent) ? "" : " · " + lastWeekGrantPatent;
            string lifetimeRevenueNet = HqDisplayQueriesHud.LifetimeRevenueNetSummaryLine(lab);
            string lifetimeRevenueNetPart = string.IsNullOrEmpty(lifetimeRevenueNet) ? "" : " · " + lifetimeRevenueNet;
            string thisWeekNetPower = HqDisplayQueriesHud.ThisWeekNetPowerSummaryLine(lab);
            string thisWeekNetPowerPart = string.IsNullOrEmpty(thisWeekNetPower) ? "" : " · " + thisWeekNetPower;
            string lastWeekGreyHardware = HqDisplayQueriesHud.LastWeekGreyHardwareSummaryLine(lab);
            string lastWeekGreyHardwarePart = string.IsNullOrEmpty(lastWeekGreyHardware) ? "" : " · " + lastWeekGreyHardware;
            string thisWeekGrantChipNet = HqDisplayQueriesHud.ThisWeekGrantChipNetSummaryLine(lab);
            string thisWeekGrantChipNetPart = string.IsNullOrEmpty(thisWeekGrantChipNet) ? "" : " · " + thisWeekGrantChipNet;
            string lastWeekCapitalLaunder = HqDisplayQueriesHud.LastWeekCapitalLaunderSummaryLine(lab);
            string lastWeekCapitalLaunderPart = string.IsNullOrEmpty(lastWeekCapitalLaunder) ? "" : " · " + lastWeekCapitalLaunder;
            string lastWeekLaunderChip = HqDisplayQueriesHud.LastWeekLaunderChipSummaryLine(lab);
            string lastWeekLaunderChipPart = string.IsNullOrEmpty(lastWeekLaunderChip) ? "" : " · " + lastWeekLaunderChip;
            string lifetimeFinesChip = HqDisplayQueriesHud.LifetimeFinesChipSummaryLine(lab);
            string lifetimeFinesChipPart = string.IsNullOrEmpty(lifetimeFinesChip) ? "" : " · " + lifetimeFinesChip;
            string thisWeekGrantConstruction = HqDisplayQueriesHud.ThisWeekGrantConstructionSummaryLine(lab);
            string thisWeekGrantConstructionPart = string.IsNullOrEmpty(thisWeekGrantConstruction) ? "" : " · " + thisWeekGrantConstruction;
            string thisWeekPowerGrey = HqDisplayQueriesHud.ThisWeekPowerGreySummaryLine(lab);
            string thisWeekPowerGreyPart = string.IsNullOrEmpty(thisWeekPowerGrey) ? "" : " · " + thisWeekPowerGrey;
            string lastWeekRoomPatent = HqDisplayQueriesHud.LastWeekRoomPatentSummaryLine(lab);
            string lastWeekRoomPatentPart = string.IsNullOrEmpty(lastWeekRoomPatent) ? "" : " · " + lastWeekRoomPatent;
            string lifetimeLaunderChip = HqDisplayQueriesHud.LifetimeLaunderChipSummaryLine(lab);
            string lifetimeLaunderChipPart = string.IsNullOrEmpty(lifetimeLaunderChip) ? "" : " · " + lifetimeLaunderChip;
            string thisWeekNetChipGrant = HqDisplayQueriesHud.ThisWeekNetChipGrantSummaryLine(lab);
            string thisWeekNetChipGrantPart = string.IsNullOrEmpty(thisWeekNetChipGrant) ? "" : " · " + thisWeekNetChipGrant;
            string lastWeekTrainingSite = HqDisplayQueriesHud.LastWeekTrainingSiteSummaryLine(lab);
            string lastWeekTrainingSitePart = string.IsNullOrEmpty(lastWeekTrainingSite) ? "" : " · " + lastWeekTrainingSite;
            string lastWeekChipLand = HqDisplayQueriesHud.LastWeekChipLandSummaryLine(lab);
            string lastWeekChipLandPart = string.IsNullOrEmpty(lastWeekChipLand) ? "" : " · " + lastWeekChipLand;
            string thisWeekSalariesGrant = HqDisplayQueriesHud.ThisWeekSalariesGrantSummaryLine(lab);
            string thisWeekSalariesGrantPart = string.IsNullOrEmpty(thisWeekSalariesGrant) ? "" : " · " + thisWeekSalariesGrant;
            string lifetimeGreyPower = HqDisplayQueriesHud.LifetimeGreyPowerSummaryLine(lab);
            string lifetimeGreyPowerPart = string.IsNullOrEmpty(lifetimeGreyPower) ? "" : " · " + lifetimeGreyPower;
            string lastWeekFinesSite = HqDisplayQueriesHud.LastWeekFinesSiteSummaryLine(lab);
            string lastWeekFinesSitePart = string.IsNullOrEmpty(lastWeekFinesSite) ? "" : " · " + lastWeekFinesSite;
            string thisWeekRentGrant = HqDisplayQueriesHud.ThisWeekRentGrantSummaryLine(lab);
            string thisWeekRentGrantPart = string.IsNullOrEmpty(thisWeekRentGrant) ? "" : " · " + thisWeekRentGrant;
            string thisWeekRevenueFines = HqDisplayQueriesHud.ThisWeekRevenueFinesSummaryLine(lab);
            string thisWeekRevenueFinesPart = string.IsNullOrEmpty(thisWeekRevenueFines) ? "" : " · " + thisWeekRevenueFines;
            string lastWeekSiteGrey = HqDisplayQueriesHud.LastWeekSiteGreySummaryLine(lab);
            string lastWeekSiteGreyPart = string.IsNullOrEmpty(lastWeekSiteGrey) ? "" : " · " + lastWeekSiteGrey;
            string lifetimeNetChip = HqDisplayQueriesHud.LifetimeNetChipSummaryLine(lab);
            string lifetimeNetChipPart = string.IsNullOrEmpty(lifetimeNetChip) ? "" : " · " + lifetimeNetChip;
            string thisWeekChipGrant = HqDisplayQueriesHud.ThisWeekChipGrantSummaryLine(lab);
            string thisWeekChipGrantPart = string.IsNullOrEmpty(thisWeekChipGrant) ? "" : " · " + thisWeekChipGrant;
            string lastWeekPatentPower = HqDisplayQueriesHud.LastWeekPatentPowerSummaryLine(lab);
            string lastWeekPatentPowerPart = string.IsNullOrEmpty(lastWeekPatentPower) ? "" : " · " + lastWeekPatentPower;
            string lastWeekHardwareNet = HqDisplayQueriesHud.LastWeekHardwareNetSummaryLine(lab);
            string lastWeekHardwareNetPart = string.IsNullOrEmpty(lastWeekHardwareNet) ? "" : " · " + lastWeekHardwareNet;
            string thisWeekFinesGrey = HqDisplayQueriesHud.ThisWeekFinesGreySummaryLine(lab);
            string thisWeekFinesGreyPart = string.IsNullOrEmpty(thisWeekFinesGrey) ? "" : " · " + thisWeekFinesGrey;
            string lifetimeSiteChip = HqDisplayQueriesHud.LifetimeSiteChipSummaryLine(lab);
            string lifetimeSiteChipPart = string.IsNullOrEmpty(lifetimeSiteChip) ? "" : " · " + lifetimeSiteChip;
            string lastWeekRevenueLaunder = HqDisplayQueriesHud.LastWeekRevenueLaunderSummaryLine(lab);
            string lastWeekRevenueLaunderPart = string.IsNullOrEmpty(lastWeekRevenueLaunder) ? "" : " · " + lastWeekRevenueLaunder;
            string thisWeekNetConstruction = HqDisplayQueriesHud.ThisWeekNetConstructionSummaryLine(lab);
            string thisWeekNetConstructionPart = string.IsNullOrEmpty(thisWeekNetConstruction) ? "" : " · " + thisWeekNetConstruction;
            string thisWeekGpuGrant = HqDisplayQueriesHud.ThisWeekGpuGrantSummaryLine(lab);
            string thisWeekGpuGrantPart = string.IsNullOrEmpty(thisWeekGpuGrant) ? "" : " · " + thisWeekGpuGrant;
            string lastWeekTrainingCapital = HqDisplayQueriesHud.LastWeekTrainingCapitalSummaryLine(lab);
            string lastWeekTrainingCapitalPart = string.IsNullOrEmpty(lastWeekTrainingCapital) ? "" : " · " + lastWeekTrainingCapital;
            string lifetimePatentGrants = HqDisplayQueriesHud.LifetimePatentGrantsSummaryLine(lab);
            string lifetimePatentGrantsPart = string.IsNullOrEmpty(lifetimePatentGrants) ? "" : " · " + lifetimePatentGrants;
            string thisWeekRevenueGrey = HqDisplayQueriesHud.ThisWeekRevenueGreySummaryLine(lab);
            string thisWeekRevenueGreyPart = string.IsNullOrEmpty(thisWeekRevenueGrey) ? "" : " · " + thisWeekRevenueGrey;
            string lastWeekSalariesSite = HqDisplayQueriesHud.LastWeekSalariesSiteSummaryLine(lab);
            string lastWeekSalariesSitePart = string.IsNullOrEmpty(lastWeekSalariesSite) ? "" : " · " + lastWeekSalariesSite;
            string lastWeekChipPower = HqDisplayQueriesHud.LastWeekChipPowerSummaryLine(lab);
            string lastWeekChipPowerPart = string.IsNullOrEmpty(lastWeekChipPower) ? "" : " · " + lastWeekChipPower;
            string thisWeekGreyGrant = HqDisplayQueriesHud.ThisWeekGreyGrantSummaryLine(lab);
            string thisWeekGreyGrantPart = string.IsNullOrEmpty(thisWeekGreyGrant) ? "" : " · " + thisWeekGreyGrant;
            string lifetimeHardwareTraining = HqDisplayQueriesHud.LifetimeHardwareTrainingSummaryLine(lab);
            string lifetimeHardwareTrainingPart = string.IsNullOrEmpty(lifetimeHardwareTraining) ? "" : " · " + lifetimeHardwareTraining;
            string lastWeekNetLand = HqDisplayQueriesHud.LastWeekNetLandSummaryLine(lab);
            string lastWeekNetLandPart = string.IsNullOrEmpty(lastWeekNetLand) ? "" : " · " + lastWeekNetLand;
            string thisWeekConstructionChip = HqDisplayQueriesHud.ThisWeekConstructionChipSummaryLine(lab);
            string thisWeekConstructionChipPart = string.IsNullOrEmpty(thisWeekConstructionChip) ? "" : " · " + thisWeekConstructionChip;
            string thisWeekSalariesPower = HqDisplayQueriesHud.ThisWeekSalariesPowerSummaryLine(lab);
            string thisWeekSalariesPowerPart = string.IsNullOrEmpty(thisWeekSalariesPower) ? "" : " · " + thisWeekSalariesPower;
            string lastWeekFinesHardware = HqDisplayQueriesHud.LastWeekFinesHardwareSummaryLine(lab);
            string lastWeekFinesHardwarePart = string.IsNullOrEmpty(lastWeekFinesHardware) ? "" : " · " + lastWeekFinesHardware;
            string lifetimeLaunderNet = HqDisplayQueriesHud.LifetimeLaunderNetSummaryLine(lab);
            string lifetimeLaunderNetPart = string.IsNullOrEmpty(lifetimeLaunderNet) ? "" : " · " + lifetimeLaunderNet;
            string thisWeekNetGrant = HqDisplayQueriesHud.ThisWeekNetGrantSummaryLine(lab);
            string thisWeekNetGrantPart = string.IsNullOrEmpty(thisWeekNetGrant) ? "" : " · " + thisWeekNetGrant;
            string lastWeekRevenueCapital = HqDisplayQueriesHud.LastWeekRevenueCapitalSummaryLine(lab);
            string lastWeekRevenueCapitalPart = string.IsNullOrEmpty(lastWeekRevenueCapital) ? "" : " · " + lastWeekRevenueCapital;
            string thisWeekGrantPower = HqDisplayQueriesHud.ThisWeekGrantPowerSummaryLine(lab);
            string thisWeekGrantPowerPart = string.IsNullOrEmpty(thisWeekGrantPower) ? "" : " · " + thisWeekGrantPower;
            string lastWeekGrantLaunder = HqDisplayQueriesHud.LastWeekGrantLaunderSummaryLine(lab);
            string lastWeekGrantLaunderPart = string.IsNullOrEmpty(lastWeekGrantLaunder) ? "" : " · " + lastWeekGrantLaunder;
            string lifetimeGreyTraining = HqDisplayQueriesHud.LifetimeGreyTrainingSummaryLine(lab);
            string lifetimeGreyTrainingPart = string.IsNullOrEmpty(lifetimeGreyTraining) ? "" : " · " + lifetimeGreyTraining;
            string thisWeekFinesConstruction = HqDisplayQueriesHud.ThisWeekFinesConstructionSummaryLine(lab);
            string thisWeekFinesConstructionPart = string.IsNullOrEmpty(thisWeekFinesConstruction) ? "" : " · " + thisWeekFinesConstruction;
            string lastWeekHardwareSite = HqDisplayQueriesHud.LastWeekHardwareSiteSummaryLine(lab);
            string lastWeekHardwareSitePart = string.IsNullOrEmpty(lastWeekHardwareSite) ? "" : " · " + lastWeekHardwareSite;
            string lastWeekSalariesGrey = HqDisplayQueriesHud.LastWeekSalariesGreySummaryLine(lab);
            string lastWeekSalariesGreyPart = string.IsNullOrEmpty(lastWeekSalariesGrey) ? "" : " · " + lastWeekSalariesGrey;
            string lastWeekConstructionPower = HqDisplayQueriesHud.LastWeekConstructionPowerSummaryLine(lab);
            string lastWeekConstructionPowerPart = string.IsNullOrEmpty(lastWeekConstructionPower) ? "" : " · " + lastWeekConstructionPower;
            string lifetimeRevenueLaunder = HqDisplayQueriesHud.LifetimeRevenueLaunderSummaryLine(lab);
            string lifetimeRevenueLaunderPart = string.IsNullOrEmpty(lifetimeRevenueLaunder) ? "" : " · " + lifetimeRevenueLaunder;
            string thisWeekRentNet = HqDisplayQueriesHud.ThisWeekRentNetSummaryLine(lab);
            string thisWeekRentNetPart = string.IsNullOrEmpty(thisWeekRentNet) ? "" : " · " + thisWeekRentNet;
            string lastWeekPatentGrey = HqDisplayQueriesHud.LastWeekPatentGreySummaryLine(lab);
            string lastWeekPatentGreyPart = string.IsNullOrEmpty(lastWeekPatentGrey) ? "" : " · " + lastWeekPatentGrey;
            string lastWeekHiringSite = HqDisplayQueriesHud.LastWeekHiringSiteSummaryLine(lab);
            string lastWeekHiringSitePart = string.IsNullOrEmpty(lastWeekHiringSite) ? "" : " · " + lastWeekHiringSite;
            string thisWeekNetRevenue = HqDisplayQueriesHud.ThisWeekNetRevenueSummaryLine(lab);
            string thisWeekNetRevenuePart = string.IsNullOrEmpty(thisWeekNetRevenue) ? "" : " · " + thisWeekNetRevenue;
            string lifetimeFinesTraining = HqDisplayQueriesHud.LifetimeFinesTrainingSummaryLine(lab);
            string lifetimeFinesTrainingPart = string.IsNullOrEmpty(lifetimeFinesTraining) ? "" : " · " + lifetimeFinesTraining;
            string lastWeekRoomLand = HqDisplayQueriesHud.LastWeekRoomLandSummaryLine(lab);
            string lastWeekRoomLandPart = string.IsNullOrEmpty(lastWeekRoomLand) ? "" : " · " + lastWeekRoomLand;
            string thisWeekGrantsPower = HqDisplayQueriesHud.ThisWeekGrantsPowerSummaryLine(lab);
            string thisWeekGrantsPowerPart = string.IsNullOrEmpty(thisWeekGrantsPower) ? "" : " · " + thisWeekGrantsPower;
            string thisWeekSalariesConstruction = HqDisplayQueriesHud.ThisWeekSalariesConstructionSummaryLine(lab);
            string thisWeekSalariesConstructionPart = string.IsNullOrEmpty(thisWeekSalariesConstruction) ? "" : " · " + thisWeekSalariesConstruction;
            string lastWeekLaunderPower = HqDisplayQueriesHud.LastWeekLaunderPowerSummaryLine(lab);
            string lastWeekLaunderPowerPart = string.IsNullOrEmpty(lastWeekLaunderPower) ? "" : " · " + lastWeekLaunderPower;
            string lifetimeGrantsChip = HqDisplayQueriesHud.LifetimeGrantsChipSummaryLine(lab);
            string lifetimeGrantsChipPart = string.IsNullOrEmpty(lifetimeGrantsChip) ? "" : " · " + lifetimeGrantsChip;
            string thisWeekFinesNet = HqDisplayQueriesHud.ThisWeekFinesNetSummaryLine(lab);
            string thisWeekFinesNetPart = string.IsNullOrEmpty(thisWeekFinesNet) ? "" : " · " + thisWeekFinesNet;
            string lastWeekCapitalHardware = HqDisplayQueriesHud.LastWeekCapitalHardwareSummaryLine(lab);
            string lastWeekCapitalHardwarePart = string.IsNullOrEmpty(lastWeekCapitalHardware) ? "" : " · " + lastWeekCapitalHardware;
            string lastWeekPatentNet = HqDisplayQueriesHud.LastWeekPatentNetSummaryLine(lab);
            string lastWeekPatentNetPart = string.IsNullOrEmpty(lastWeekPatentNet) ? "" : " · " + lastWeekPatentNet;
            string thisWeekGpuGrey = HqDisplayQueriesHud.ThisWeekGpuGreySummaryLine(lab);
            string thisWeekGpuGreyPart = string.IsNullOrEmpty(thisWeekGpuGrey) ? "" : " · " + thisWeekGpuGrey;
            string lifetimePowerChip = HqDisplayQueriesHud.LifetimePowerChipSummaryLine(lab);
            string lifetimePowerChipPart = string.IsNullOrEmpty(lifetimePowerChip) ? "" : " · " + lifetimePowerChip;
            string lastWeekGpuLand = HqDisplayQueriesHud.LastWeekGpuLandSummaryLine(lab);
            string lastWeekGpuLandPart = string.IsNullOrEmpty(lastWeekGpuLand) ? "" : " · " + lastWeekGpuLand;
            string thisWeekConstructionGrey = HqDisplayQueriesHud.ThisWeekConstructionGreySummaryLine(lab);
            string thisWeekConstructionGreyPart = string.IsNullOrEmpty(thisWeekConstructionGrey) ? "" : " · " + thisWeekConstructionGrey;
            string thisWeekGreyChip = HqDisplayQueriesHud.ThisWeekGreyChipSummaryLine(lab);
            string thisWeekGreyChipPart = string.IsNullOrEmpty(thisWeekGreyChip) ? "" : " · " + thisWeekGreyChip;
            string lastWeekSiteTraining = HqDisplayQueriesHud.LastWeekSiteTrainingSummaryLine(lab);
            string lastWeekSiteTrainingPart = string.IsNullOrEmpty(lastWeekSiteTraining) ? "" : " · " + lastWeekSiteTraining;
            string lifetimeNetLaunder = HqDisplayQueriesHud.LifetimeNetLaunderSummaryLine(lab);
            string lifetimeNetLaunderPart = string.IsNullOrEmpty(lifetimeNetLaunder) ? "" : " · " + lifetimeNetLaunder;
            string thisWeekRevenueSalaries = HqDisplayQueriesHud.ThisWeekRevenueSalariesSummaryLine(lab);
            string thisWeekRevenueSalariesPart = string.IsNullOrEmpty(thisWeekRevenueSalaries) ? "" : " · " + thisWeekRevenueSalaries;
            string lastWeekHardwareHiring = HqDisplayQueriesHud.LastWeekHardwareHiringSummaryLine(lab);
            string lastWeekHardwareHiringPart = string.IsNullOrEmpty(lastWeekHardwareHiring) ? "" : " · " + lastWeekHardwareHiring;
            string lastWeekRoomGrey = HqDisplayQueriesHud.LastWeekRoomGreySummaryLine(lab);
            string lastWeekRoomGreyPart = string.IsNullOrEmpty(lastWeekRoomGrey) ? "" : " · " + lastWeekRoomGrey;
            string thisWeekConstructionNet = HqDisplayQueriesHud.ThisWeekConstructionNetSummaryLine(lab);
            string thisWeekConstructionNetPart = string.IsNullOrEmpty(thisWeekConstructionNet) ? "" : " · " + thisWeekConstructionNet;
            string lifetimeLaunderGrants = HqDisplayQueriesHud.LifetimeLaunderGrantsSummaryLine(lab);
            string lifetimeLaunderGrantsPart = string.IsNullOrEmpty(lifetimeLaunderGrants) ? "" : " · " + lifetimeLaunderGrants;
            string lastWeekLandPatent = HqDisplayQueriesHud.LastWeekLandPatentSummaryLine(lab);
            string lastWeekLandPatentPart = string.IsNullOrEmpty(lastWeekLandPatent) ? "" : " · " + lastWeekLandPatent;
            string thisWeekGpuSalaries = HqDisplayQueriesHud.ThisWeekGpuSalariesSummaryLine(lab);
            string thisWeekGpuSalariesPart = string.IsNullOrEmpty(thisWeekGpuSalaries) ? "" : " · " + thisWeekGpuSalaries;
            string thisWeekGreyTraining = HqDisplayQueriesHud.ThisWeekGreyTrainingSummaryLine(lab);
            string thisWeekGreyTrainingPart = string.IsNullOrEmpty(thisWeekGreyTraining) ? "" : " · " + thisWeekGreyTraining;
            string lastWeekConstructionChip = HqDisplayQueriesHud.LastWeekConstructionChipSummaryLine(lab);
            string lastWeekConstructionChipPart = string.IsNullOrEmpty(lastWeekConstructionChip) ? "" : " · " + lastWeekConstructionChip;
            string lifetimeChipDev = HqDisplayQueriesHud.LifetimeChipDevSummaryLine(lab);
            string lifetimeChipDevPart = string.IsNullOrEmpty(lifetimeChipDev) ? "" : " · " + lifetimeChipDev;
            string thisWeekRevenuePower = HqDisplayQueriesHud.ThisWeekRevenuePowerSummaryLine(lab);
            string thisWeekRevenuePowerPart = string.IsNullOrEmpty(thisWeekRevenuePower) ? "" : " · " + thisWeekRevenuePower;
            string lastWeekGrantTraining = HqDisplayQueriesHud.LastWeekGrantTrainingSummaryLine(lab);
            string lastWeekGrantTrainingPart = string.IsNullOrEmpty(lastWeekGrantTraining) ? "" : " · " + lastWeekGrantTraining;
            string lastWeekGpuGrey = HqDisplayQueriesHud.LastWeekGpuGreySummaryLine(lab);
            string lastWeekGpuGreyPart = string.IsNullOrEmpty(lastWeekGpuGrey) ? "" : " · " + lastWeekGpuGrey;
            string thisWeekFinesTraining = HqDisplayQueriesHud.ThisWeekFinesTrainingSummaryLine(lab);
            string thisWeekFinesTrainingPart = string.IsNullOrEmpty(thisWeekFinesTraining) ? "" : " · " + thisWeekFinesTraining;
            string lifetimeRevenueGrants = HqDisplayQueriesHud.LifetimeRevenueGrantsSummaryLine(lab);
            string lifetimeRevenueGrantsPart = string.IsNullOrEmpty(lifetimeRevenueGrants) ? "" : " · " + lifetimeRevenueGrants;
            string lastWeekChipPatent = HqDisplayQueriesHud.LastWeekChipPatentSummaryLine(lab);
            string lastWeekChipPatentPart = string.IsNullOrEmpty(lastWeekChipPatent) ? "" : " · " + lastWeekChipPatent;
            string thisWeekGreyConstruction = HqDisplayQueriesHud.ThisWeekGreyConstructionSummaryLine(lab);
            string thisWeekGreyConstructionPart = string.IsNullOrEmpty(thisWeekGreyConstruction) ? "" : " · " + thisWeekGreyConstruction;
            string lastWeekHiringCapital = HqDisplayQueriesHud.LastWeekHiringCapitalSummaryLine(lab);
            string lastWeekHiringCapitalPart = string.IsNullOrEmpty(lastWeekHiringCapital) ? "" : " · " + lastWeekHiringCapital;
            string thisWeekChipGpu = HqDisplayQueriesHud.ThisWeekChipGpuSummaryLine(lab);
            string thisWeekChipGpuPart = string.IsNullOrEmpty(thisWeekChipGpu) ? "" : " · " + thisWeekChipGpu;
            string lifetimeFinesGrey = HqDisplayQueriesHud.LifetimeFinesGreySummaryLine(lab);
            string lifetimeFinesGreyPart = string.IsNullOrEmpty(lifetimeFinesGrey) ? "" : " · " + lifetimeFinesGrey;
            string lastWeekTrainingConstruction = HqDisplayQueriesHud.LastWeekTrainingConstructionSummaryLine(lab);
            string lastWeekTrainingConstructionPart = string.IsNullOrEmpty(lastWeekTrainingConstruction) ? "" : " · " + lastWeekTrainingConstruction;
            string thisWeekGrantNet = HqDisplayQueriesHud.ThisWeekGrantNetSummaryLine(lab);
            string thisWeekGrantNetPart = string.IsNullOrEmpty(thisWeekGrantNet) ? "" : " · " + thisWeekGrantNet;
            string thisWeekSalariesGrants = HqDisplayQueriesHud.ThisWeekSalariesGrantsSummaryLine(lab);
            string thisWeekSalariesGrantsPart = string.IsNullOrEmpty(thisWeekSalariesGrants) ? "" : " · " + thisWeekSalariesGrants;
            string lastWeekPowerChipDev = HqDisplayQueriesHud.LastWeekPowerChipDevSummaryLine(lab);
            string lastWeekPowerChipDevPart = string.IsNullOrEmpty(lastWeekPowerChipDev) ? "" : " · " + lastWeekPowerChipDev;
            string thisWeekRevenueTraining = HqDisplayQueriesHud.ThisWeekRevenueTrainingSummaryLine(lab);
            string thisWeekRevenueTrainingPart = string.IsNullOrEmpty(thisWeekRevenueTraining) ? "" : " · " + thisWeekRevenueTraining;
            string lifetimeTrainingChip = HqDisplayQueriesHud.LifetimeTrainingChipSummaryLine(lab);
            string lifetimeTrainingChipPart = string.IsNullOrEmpty(lifetimeTrainingChip) ? "" : " · " + lifetimeTrainingChip;
            string lastWeekLaunderFines = HqDisplayQueriesHud.LastWeekLaunderFinesSummaryLine(lab);
            string lastWeekLaunderFinesPart = string.IsNullOrEmpty(lastWeekLaunderFines) ? "" : " · " + lastWeekLaunderFines;
            string thisWeekGpuConstruction = HqDisplayQueriesHud.ThisWeekGpuConstructionSummaryLine(lab);
            string thisWeekGpuConstructionPart = string.IsNullOrEmpty(thisWeekGpuConstruction) ? "" : " · " + thisWeekGpuConstruction;
            string lastWeekGreyGrants = HqDisplayQueriesHud.LastWeekGreyGrantsSummaryLine(lab);
            string lastWeekGreyGrantsPart = string.IsNullOrEmpty(lastWeekGreyGrants) ? "" : " · " + lastWeekGreyGrants;
            string lifetimeChipPower = HqDisplayQueriesHud.LifetimeChipPowerSummaryLine(lab);
            string lifetimeChipPowerPart = string.IsNullOrEmpty(lifetimeChipPower) ? "" : " · " + lifetimeChipPower;
            string thisWeekFinesChip = HqDisplayQueriesHud.ThisWeekFinesChipSummaryLine(lab);
            string thisWeekFinesChipPart = string.IsNullOrEmpty(thisWeekFinesChip) ? "" : " · " + thisWeekFinesChip;
            string lastWeekNetPower = HqDisplayQueriesHud.LastWeekNetPowerSummaryLine(lab);
            string lastWeekNetPowerPart = string.IsNullOrEmpty(lastWeekNetPower) ? "" : " · " + lastWeekNetPower;
            string lastWeekSalariesHiring = HqDisplayQueriesHud.LastWeekSalariesHiringSummaryLine(lab);
            string lastWeekSalariesHiringPart = string.IsNullOrEmpty(lastWeekSalariesHiring) ? "" : " · " + lastWeekSalariesHiring;
            string thisWeekConstructionPower = HqDisplayQueriesHud.ThisWeekConstructionPowerSummaryLine(lab);
            string thisWeekConstructionPowerPart = string.IsNullOrEmpty(thisWeekConstructionPower) ? "" : " · " + thisWeekConstructionPower;
            string lifetimeGreyNet = HqDisplayQueriesHud.LifetimeGreyNetSummaryLine(lab);
            string lifetimeGreyNetPart = string.IsNullOrEmpty(lifetimeGreyNet) ? "" : " · " + lifetimeGreyNet;
            string lastWeekRevenueChip = HqDisplayQueriesHud.LastWeekRevenueChipSummaryLine(lab);
            string lastWeekRevenueChipPart = string.IsNullOrEmpty(lastWeekRevenueChip) ? "" : " · " + lastWeekRevenueChip;
            string thisWeekChipTraining = HqDisplayQueriesHud.ThisWeekChipTrainingSummaryLine(lab);
            string thisWeekChipTrainingPart = string.IsNullOrEmpty(thisWeekChipTraining) ? "" : " · " + thisWeekChipTraining;
            string thisWeekSalariesNet = HqDisplayQueriesHud.ThisWeekSalariesNetSummaryLine(lab);
            string thisWeekSalariesNetPart = string.IsNullOrEmpty(thisWeekSalariesNet) ? "" : " · " + thisWeekSalariesNet;
            string lastWeekGpuFines = HqDisplayQueriesHud.LastWeekGpuFinesSummaryLine(lab);
            string lastWeekGpuFinesPart = string.IsNullOrEmpty(lastWeekGpuFines) ? "" : " · " + lastWeekGpuFines;
            string lifetimeRevenuePower = HqDisplayQueriesHud.LifetimeRevenuePowerSummaryLine(lab);
            string lifetimeRevenuePowerPart = string.IsNullOrEmpty(lifetimeRevenuePower) ? "" : " · " + lifetimeRevenuePower;
            string lastWeekConstructionGrey = HqDisplayQueriesHud.LastWeekConstructionGreySummaryLine(lab);
            string lastWeekConstructionGreyPart = string.IsNullOrEmpty(lastWeekConstructionGrey) ? "" : " · " + lastWeekConstructionGrey;
            string thisWeekGpuFinesGrey = HqDisplayQueriesHud.ThisWeekGpuFinesGreySummaryLine(lab);
            string thisWeekGpuFinesGreyPart = string.IsNullOrEmpty(thisWeekGpuFinesGrey) ? "" : " · " + thisWeekGpuFinesGrey;
            string thisWeekTrainingPower = HqDisplayQueriesHud.ThisWeekTrainingPowerSummaryLine(lab);
            string thisWeekTrainingPowerPart = string.IsNullOrEmpty(thisWeekTrainingPower) ? "" : " · " + thisWeekTrainingPower;
            string lastWeekSalariesPower = HqDisplayQueriesHud.LastWeekSalariesPowerSummaryLine(lab);
            string lastWeekSalariesPowerPart = string.IsNullOrEmpty(lastWeekSalariesPower) ? "" : " · " + lastWeekSalariesPower;
            string lifetimeGrantsLaunder = HqDisplayQueriesHud.LifetimeGrantsLaunderSummaryLine(lab);
            string lifetimeGrantsLaunderPart = string.IsNullOrEmpty(lifetimeGrantsLaunder) ? "" : " · " + lifetimeGrantsLaunder;
            string thisWeekGreyPower = HqDisplayQueriesHud.ThisWeekGreyPowerSummaryLine(lab);
            string thisWeekGreyPowerPart = string.IsNullOrEmpty(thisWeekGreyPower) ? "" : " · " + thisWeekGreyPower;
            string lastWeekNetFines = HqDisplayQueriesHud.LastWeekNetFinesSummaryLine(lab);
            string lastWeekNetFinesPart = string.IsNullOrEmpty(lastWeekNetFines) ? "" : " · " + lastWeekNetFines;
            string thisWeekChipConstruction = HqDisplayQueriesHud.ThisWeekChipConstructionSummaryLine(lab);
            string thisWeekChipConstructionPart = string.IsNullOrEmpty(thisWeekChipConstruction) ? "" : " · " + thisWeekChipConstruction;
            string lastWeekPatentHiring = HqDisplayQueriesHud.LastWeekPatentHiringSummaryLine(lab);
            string lastWeekPatentHiringPart = string.IsNullOrEmpty(lastWeekPatentHiring) ? "" : " · " + lastWeekPatentHiring;
            string thisWeekGpuNet = HqDisplayQueriesHud.ThisWeekGpuNetSummaryLine(lab);
            string thisWeekGpuNetPart = string.IsNullOrEmpty(thisWeekGpuNet) ? "" : " · " + thisWeekGpuNet;
            string lifetimeFinesPower = HqDisplayQueriesHud.LifetimeFinesPowerSummaryLine(lab);
            string lifetimeFinesPowerPart = string.IsNullOrEmpty(lifetimeFinesPower) ? "" : " · " + lifetimeFinesPower;
            string lastWeekGrantsRevenue = HqDisplayQueriesHud.LastWeekGrantsRevenueSummaryLine(lab);
            string lastWeekGrantsRevenuePart = string.IsNullOrEmpty(lastWeekGrantsRevenue) ? "" : " · " + lastWeekGrantsRevenue;
            string thisWeekRevenueGrants = HqDisplayQueriesHud.ThisWeekRevenueGrantsSummaryLine(lab);
            string thisWeekRevenueGrantsPart = string.IsNullOrEmpty(thisWeekRevenueGrants) ? "" : " · " + thisWeekRevenueGrants;
            string lastWeekChipDevSales = HqDisplayQueriesHud.LastWeekChipDevSalesSummaryLine(lab);
            string lastWeekChipDevSalesPart = string.IsNullOrEmpty(lastWeekChipDevSales) ? "" : " · " + lastWeekChipDevSales;
            string lastWeekTrainingGpuUpkeep = HqDisplayQueriesHud.LastWeekTrainingGpuUpkeepSummaryLine(lab);
            string lastWeekTrainingGpuUpkeepPart = string.IsNullOrEmpty(lastWeekTrainingGpuUpkeep) ? "" : " · " + lastWeekTrainingGpuUpkeep;
            string lifetimeChipGrey = HqDisplayQueriesHud.LifetimeChipGreySummaryLine(lab);
            string lifetimeChipGreyPart = string.IsNullOrEmpty(lifetimeChipGrey) ? "" : " · " + lifetimeChipGrey;
            string lastWeekCapitalGrey = HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(lab);
            string lastWeekCapitalGreyPart = string.IsNullOrEmpty(lastWeekCapitalGrey) ? "" : " · " + lastWeekCapitalGrey;
            string thisWeekGreyNet = HqDisplayQueriesHud.ThisWeekGreyNetSummaryLine(lab);
            string thisWeekGreyNetPart = string.IsNullOrEmpty(thisWeekGreyNet) ? "" : " · " + thisWeekGreyNet;
            string lastWeekFinesTraining = HqDisplayQueriesHud.LastWeekFinesTrainingSummaryLine(lab);
            string lastWeekFinesTrainingPart = string.IsNullOrEmpty(lastWeekFinesTraining) ? "" : " · " + lastWeekFinesTraining;
            string lifetimeNetTraining = HqDisplayQueriesHud.LifetimeNetTrainingSummaryLine(lab);
            string lifetimeNetTrainingPart = string.IsNullOrEmpty(lifetimeNetTraining) ? "" : " · " + lifetimeNetTraining;
            string thisWeekTrainingChip = HqDisplayQueriesHud.ThisWeekTrainingChipSummaryLine(lab);
            string thisWeekTrainingChipPart = string.IsNullOrEmpty(thisWeekTrainingChip) ? "" : " · " + thisWeekTrainingChip;
            string lastWeekPatentSite = HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(lab);
            string lastWeekPatentSitePart = string.IsNullOrEmpty(lastWeekPatentSite) ? "" : " · " + lastWeekPatentSite;
            string lastWeekHiringGrey = HqDisplayQueriesHud.LastWeekHiringGreySummaryLine(lab);
            string lastWeekHiringGreyPart = string.IsNullOrEmpty(lastWeekHiringGrey) ? "" : " · " + lastWeekHiringGrey;
            string lastWeekChipSalesGrants = HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(lab);
            string lastWeekChipSalesGrantsPart = string.IsNullOrEmpty(lastWeekChipSalesGrants) ? "" : " · " + lastWeekChipSalesGrants;
            string lifetimeTrainingLaunder = HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(lab);
            string lifetimeTrainingLaunderPart = string.IsNullOrEmpty(lifetimeTrainingLaunder) ? "" : " · " + lifetimeTrainingLaunder;
            string lastWeekHardwarePower = HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(lab);
            string lastWeekHardwarePowerPart = string.IsNullOrEmpty(lastWeekHardwarePower) ? "" : " · " + lastWeekHardwarePower;
            string thisWeekSalariesGpu = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(lab);
            string thisWeekSalariesGpuPart = string.IsNullOrEmpty(thisWeekSalariesGpu) ? "" : " · " + thisWeekSalariesGpu;
            string thisWeekConstructionGrants = HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(lab);
            string thisWeekConstructionGrantsPart = string.IsNullOrEmpty(thisWeekConstructionGrants) ? "" : " · " + thisWeekConstructionGrants;
            string lastWeekGreyLaunder = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(lab);
            string lastWeekGreyLaunderPart = string.IsNullOrEmpty(lastWeekGreyLaunder) ? "" : " · " + lastWeekGreyLaunder;
            string lifetimePowerFines = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(lab);
            string lifetimePowerFinesPart = string.IsNullOrEmpty(lifetimePowerFines) ? "" : " · " + lifetimePowerFines;
            string thisWeekGrantsFines = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(lab);
            string thisWeekGrantsFinesPart = string.IsNullOrEmpty(thisWeekGrantsFines) ? "" : " · " + thisWeekGrantsFines;
            string lastWeekChipDevLand = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(lab);
            string lastWeekChipDevLandPart = string.IsNullOrEmpty(lastWeekChipDevLand) ? "" : " · " + lastWeekChipDevLand;
            string lastWeekRevenueNet = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(lab);
            string lastWeekRevenueNetPart = string.IsNullOrEmpty(lastWeekRevenueNet) ? "" : " · " + lastWeekRevenueNet;
            string lastWeekGpuUpkeepRent = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(lab);
            string lastWeekGpuUpkeepRentPart = string.IsNullOrEmpty(lastWeekGpuUpkeepRent) ? "" : " · " + lastWeekGpuUpkeepRent;
            string lifetimeRevenueChip = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(lab);
            string lifetimeRevenueChipPart = string.IsNullOrEmpty(lifetimeRevenueChip) ? "" : " · " + lifetimeRevenueChip;
            string lastWeekSalariesRoom = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(lab);
            string lastWeekSalariesRoomPart = string.IsNullOrEmpty(lastWeekSalariesRoom) ? "" : " · " + lastWeekSalariesRoom;
            string thisWeekFinesPower = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(lab);
            string thisWeekFinesPowerPart = string.IsNullOrEmpty(thisWeekFinesPower) ? "" : " · " + thisWeekFinesPower;
            string lifetimeGrantsNet = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(lab);
            string lifetimeGrantsNetPart = string.IsNullOrEmpty(lifetimeGrantsNet) ? "" : " · " + lifetimeGrantsNet;
            string thisWeekChipRevenue = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(lab);
            string thisWeekChipRevenuePart = string.IsNullOrEmpty(thisWeekChipRevenue) ? "" : " · " + thisWeekChipRevenue;
            string lastWeekConstructionPatent = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(lab);
            string lastWeekConstructionPatentPart = string.IsNullOrEmpty(lastWeekConstructionPatent) ? "" : " · " + lastWeekConstructionPatent;
            string lastWeekHiringPatent = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(lab);
            string lastWeekHiringPatentPart = string.IsNullOrEmpty(lastWeekHiringPatent) ? "" : " · " + lastWeekHiringPatent;
            string thisWeekBuild = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(lab);
            string thisWeekBuildPart = string.IsNullOrEmpty(thisWeekBuild) ? "" : " · " + thisWeekBuild;
            _gpuSummary.text = "GPUs: " + rented + " rented, " + owned + " owned (" + gen.Name + ")" + lastWeekLandPart + lastWeekSiteCapitalPart + thisWeekNetTrainingPart + lastWeekTrainingRevenuePart + thisWeekPowerChipPart + lastWeekRoomHiringPart + lastWeekConstructionLandPart + thisWeekSalariesFinesPart + lastWeekChipGreyPart + lastWeekCapitalPatentPart + thisWeekGrantsChipPart + lastWeekSiteHardwarePart + lastWeekNetGreyPart + lastWeekRoomPowerPart + thisWeekNetChipPart + lastWeekFinesCapitalPart + thisWeekChipFinesPart + lastWeekPowerNetPart + thisWeekTrainingGrantPart + lastWeekChipTrainingPart + thisWeekChipRentPart + thisWeekGrantChipPart + lastWeekTrainingGreyPart + lastWeekGrantSitePart + thisWeekTrainingFinesPart + lastWeekGreyLandPart + thisWeekGrantTraining2Part + lastWeekGrantHardwarePart + lastWeekUpkeepGpuPart + lastWeekRevenueHardwarePart + lastWeekPatentTrainingPart + thisWeekGrantChipNetPart + lastWeekCapitalLaunderPart + lastWeekLaunderChipPart + lifetimeFinesChipPart + thisWeekGrantConstructionPart + thisWeekPowerGreyPart + lastWeekRoomPatentPart + lifetimeLaunderChipPart + thisWeekNetChipGrantPart + lastWeekTrainingSitePart + lastWeekChipLandPart + thisWeekSalariesGrantPart + lifetimeGreyPowerPart + lastWeekFinesSitePart + thisWeekRentGrantPart + thisWeekRevenueFinesPart + lastWeekSiteGreyPart + lifetimeNetChipPart + thisWeekChipGrantPart + lastWeekPatentPowerPart + lastWeekHardwareNetPart + thisWeekFinesGreyPart + lifetimeSiteChipPart + lastWeekRevenueLaunderPart + thisWeekNetConstructionPart + thisWeekGpuGrantPart + lastWeekTrainingCapitalPart + lifetimePatentGrantsPart + thisWeekRevenueGreyPart + lastWeekSalariesSitePart + lastWeekChipPowerPart + thisWeekGreyGrantPart + lifetimeHardwareTrainingPart + lastWeekNetLandPart + thisWeekConstructionChipPart + thisWeekSalariesPowerPart + lastWeekFinesHardwarePart + lifetimeLaunderNetPart + thisWeekNetGrantPart + lastWeekRevenueCapitalPart + thisWeekGrantPowerPart + lastWeekGrantLaunderPart + lifetimeGreyTrainingPart + thisWeekFinesConstructionPart + lastWeekHardwareSitePart + lastWeekSalariesGreyPart + lastWeekConstructionPowerPart + lifetimeRevenueLaunderPart + thisWeekRentNetPart + lastWeekPatentGreyPart + lastWeekHiringSitePart + thisWeekNetRevenuePart + lifetimeFinesTrainingPart + lastWeekRoomLandPart + thisWeekGrantsPowerPart + thisWeekSalariesConstructionPart + lastWeekLaunderPowerPart + lifetimeGrantsChipPart + thisWeekFinesNetPart + lastWeekCapitalHardwarePart + lastWeekPatentNetPart + thisWeekGpuGreyPart + lifetimePowerChipPart + lastWeekGpuLandPart + thisWeekConstructionGreyPart + thisWeekGreyChipPart + lastWeekSiteTrainingPart + lifetimeNetLaunderPart + thisWeekRevenueSalariesPart + lastWeekHardwareHiringPart + lastWeekRoomGreyPart + thisWeekConstructionNetPart + lifetimeLaunderGrantsPart + lastWeekLandPatentPart + thisWeekGpuSalariesPart + thisWeekGreyTrainingPart + lastWeekConstructionChipPart + lifetimeChipDevPart + thisWeekRevenuePowerPart + lastWeekGrantTrainingPart + lastWeekGpuGreyPart + thisWeekFinesTrainingPart + lifetimeRevenueGrantsPart + lastWeekChipPatentPart + thisWeekGreyConstructionPart + lastWeekHiringCapitalPart + thisWeekChipGpuPart + lifetimeFinesGreyPart + lastWeekTrainingConstructionPart + thisWeekGrantNetPart + thisWeekSalariesGrantsPart + lastWeekPowerChipDevPart + thisWeekRevenueTrainingPart + lifetimeTrainingChipPart + lastWeekLaunderFinesPart + thisWeekGpuConstructionPart + lastWeekGreyGrantsPart + lifetimeChipPowerPart + thisWeekFinesChipPart + lastWeekNetPowerPart + lastWeekSalariesHiringPart + thisWeekConstructionPowerPart + lifetimeGreyNetPart + lastWeekRevenueChipPart + thisWeekChipTrainingPart + thisWeekSalariesNetPart + lastWeekGpuFinesPart + lifetimeRevenuePowerPart + lastWeekConstructionGreyPart + thisWeekGpuFinesGreyPart + thisWeekTrainingPowerPart + lastWeekSalariesPowerPart + lifetimeGrantsLaunderPart + thisWeekGreyPowerPart + lastWeekNetFinesPart + thisWeekChipConstructionPart + lastWeekPatentHiringPart + thisWeekGpuNetPart + lifetimeFinesPowerPart + lastWeekGrantsRevenuePart + thisWeekRevenueGrantsPart + lastWeekChipDevSalesPart + lastWeekTrainingGpuUpkeepPart + lifetimeChipGreyPart + lastWeekCapitalGreyPart + thisWeekGreyNetPart + lastWeekFinesTrainingPart + lifetimeNetTrainingPart + thisWeekTrainingChipPart + lastWeekPatentSitePart + lastWeekHiringGreyPart + lastWeekChipSalesGrantsPart + lifetimeTrainingLaunderPart + lastWeekHardwarePowerPart + thisWeekSalariesGpuPart + thisWeekConstructionGrantsPart + lastWeekGreyLaunderPart + lifetimePowerFinesPart + thisWeekGrantsFinesPart + lastWeekChipDevLandPart + lastWeekRevenueNetPart + lastWeekGpuUpkeepRentPart + lifetimeRevenueChipPart + lastWeekSalariesRoomPart + thisWeekFinesPowerPart + lifetimeGrantsNetPart + thisWeekChipRevenuePart + lastWeekConstructionPatentPart + lastWeekHiringPatentPart + thisWeekBuildPart + lastWeekGreyHardwarePart + thisWeekNetPowerPart + lifetimeRevenueNetPart + lastWeekGrantPatentPart + thisWeekChipBuildPart + lastWeekLaunderGreyPart + thisWeekGrantRevenuePart + lastWeekSiteRoomPart + lifetimeTrainingGrantsPart + lastWeekCapitalSitePart + thisWeekRentChipPart + lastWeekFinesGrantPart + lastWeekPowerLandPart + thisWeekGrantGreyPart + lifetimeHardwareChipPart + lastWeekRoomTrainingPart + thisWeekTrainingNetPart + lastWeekChipGrantPart + thisWeekGpuPowerPart + lifetimePatentChipPart + lastWeekNetConstructionPart;

            for (int i = _roleList.childCount - 1; i >= 0; i--)
                Destroy(_roleList.GetChild(i).gameObject);

            for (int i = 0; i < sim.Config.Roles.Count; i++)
            {
                RoleConfig role = sim.Config.Roles[i];
                if (role.Function == RoleFunction.Infrastructure || role.Function == RoleFunction.Policy) continue;

                var row = new GameObject(role.Id);
                row.transform.SetParent(_roleList, false);
                var rowRect = row.AddComponent<RectTransform>();
                rowRect.sizeDelta = new Vector2(0f, 36f);
                var rowLayout = row.AddComponent<HorizontalLayoutGroup>();
                rowLayout.spacing = 2f;
                rowLayout.childForceExpandWidth = true;

                int count = LabQueries.StaffCount(lab, role.Id);
                var label = HqUiFactory.Label(row.transform, "C", role.Name + ": " + count, 12, TextAnchor.MiddleLeft);
                label.rectTransform.sizeDelta = new Vector2(90f, 32f);

                string roleId = role.Id;
                HqUiFactory.SmallButton(row.transform, "+", () => Hire(roleId, 1));
                HqUiFactory.SmallButton(row.transform, "−", () => Fire(roleId, 1));
            }

            _status.text = string.IsNullOrEmpty(_host.StatusMessage)
                ? "Hire needs desks in the matching room."
                : _host.StatusMessage;
        }

        void Hire(string roleId, int count)
        {
            _host.TryApply(Command.Hire(_host.Simulation.Player.Id, roleId, count));
        }

        void Fire(string roleId, int count)
        {
            _host.TryApply(Command.Fire(_host.Simulation.Player.Id, roleId, count));
        }

        void AdjustRent(int delta)
        {
            int next = _host.Simulation.Player.RentedGpus + delta;
            if (next < 0) next = 0;
            _host.TryApply(Command.SetRentedGpus(_host.Simulation.Player.Id, next));
        }

        void BuyOneHqGpu()
        {
            _host.TryApply(Command.BuyGpus(_host.Simulation.Player.Id, 1));
        }
    }
}