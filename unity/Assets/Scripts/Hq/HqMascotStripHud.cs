using AiLabs.Unity.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    public sealed class HqMascotStripHud : MonoBehaviour
    {
        SimulationHost _host;
        Text _line;
        public static HqMascotStripHud Create(Transform parent, SimulationHost host)
        {
            var go = new GameObject("MascotStrip");
            go.transform.SetParent(parent, false);
            var h = go.AddComponent<HqMascotStripHud>();
            h._host = host;
            h._line = go.AddComponent<Text>();
            host.StateChanged += h.Refresh;
            return h;
        }
        void OnDestroy() { if (_host != null) _host.StateChanged -= Refresh; }
        void Refresh()
        {
            if (_host?.Simulation == null) return;
            var sim = _host.Simulation;
            string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(sim.Player);
            string lastWeekLandPart = string.IsNullOrEmpty(lastWeekLand) ? "" : " · " + lastWeekLand;
            string lastWeekSiteCapital = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(sim.Player);
            string lastWeekSiteCapitalPart = string.IsNullOrEmpty(lastWeekSiteCapital) ? "" : " · " + lastWeekSiteCapital;
            string thisWeekNetTraining = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(sim.Player);
            string thisWeekNetTrainingPart = string.IsNullOrEmpty(thisWeekNetTraining) ? "" : " · " + thisWeekNetTraining;
            string lastWeekTrainingRevenue = HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(sim.Player);
            string lastWeekTrainingRevenuePart = string.IsNullOrEmpty(lastWeekTrainingRevenue) ? "" : " · " + lastWeekTrainingRevenue;
            string lastWeekChipSalesGrants = HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(sim.Player);
            string lastWeekChipSalesGrantsPart = string.IsNullOrEmpty(lastWeekChipSalesGrants) ? "" : " · " + lastWeekChipSalesGrants;
            string lifetimeTrainingLaunder = HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(sim.Player);
            string lifetimeTrainingLaunderPart = string.IsNullOrEmpty(lifetimeTrainingLaunder) ? "" : " · " + lifetimeTrainingLaunder;
            string lastWeekHardwarePower = HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(sim.Player);
            string lastWeekHardwarePowerPart = string.IsNullOrEmpty(lastWeekHardwarePower) ? "" : " · " + lastWeekHardwarePower;
            string thisWeekSalariesGpu = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(sim.Player);
            string thisWeekSalariesGpuPart = string.IsNullOrEmpty(thisWeekSalariesGpu) ? "" : " · " + thisWeekSalariesGpu;
            string thisWeekConstructionGrants = HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(sim.Player);
            string thisWeekConstructionGrantsPart = string.IsNullOrEmpty(thisWeekConstructionGrants) ? "" : " · " + thisWeekConstructionGrants;
            string lastWeekGreyLaunder = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(sim.Player);
            string lastWeekGreyLaunderPart = string.IsNullOrEmpty(lastWeekGreyLaunder) ? "" : " · " + lastWeekGreyLaunder;
            string lifetimePowerFines = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(sim.Player);
            string lifetimePowerFinesPart = string.IsNullOrEmpty(lifetimePowerFines) ? "" : " · " + lifetimePowerFines;
            string thisWeekGrantsFines = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(sim.Player);
            string thisWeekGrantsFinesPart = string.IsNullOrEmpty(thisWeekGrantsFines) ? "" : " · " + thisWeekGrantsFines;
            string lastWeekChipDevLand = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(sim.Player);
            string lastWeekChipDevLandPart = string.IsNullOrEmpty(lastWeekChipDevLand) ? "" : " · " + lastWeekChipDevLand;
            string lastWeekRevenueNet = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(sim.Player);
            string lastWeekRevenueNetPart = string.IsNullOrEmpty(lastWeekRevenueNet) ? "" : " · " + lastWeekRevenueNet;
            string lastWeekGpuUpkeepRent = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(sim.Player);
            string lastWeekGpuUpkeepRentPart = string.IsNullOrEmpty(lastWeekGpuUpkeepRent) ? "" : " · " + lastWeekGpuUpkeepRent;
            string lifetimeRevenueChip = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(sim.Player);
            string lifetimeRevenueChipPart = string.IsNullOrEmpty(lifetimeRevenueChip) ? "" : " · " + lifetimeRevenueChip;
            string lastWeekSalariesRoom = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(sim.Player);
            string lastWeekSalariesRoomPart = string.IsNullOrEmpty(lastWeekSalariesRoom) ? "" : " · " + lastWeekSalariesRoom;
            string thisWeekFinesPower = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(sim.Player);
            string thisWeekFinesPowerPart = string.IsNullOrEmpty(thisWeekFinesPower) ? "" : " · " + thisWeekFinesPower;
            string lifetimeGrantsNet = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(sim.Player);
            string lifetimeGrantsNetPart = string.IsNullOrEmpty(lifetimeGrantsNet) ? "" : " · " + lifetimeGrantsNet;
            string thisWeekChipRevenue = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(sim.Player);
            string thisWeekChipRevenuePart = string.IsNullOrEmpty(thisWeekChipRevenue) ? "" : " · " + thisWeekChipRevenue;
            string lastWeekConstructionPatent = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(sim.Player);
            string lastWeekConstructionPatentPart = string.IsNullOrEmpty(lastWeekConstructionPatent) ? "" : " · " + lastWeekConstructionPatent;
            string lastWeekHiringPatent = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(sim.Player);
            string lastWeekHiringPatentPart = string.IsNullOrEmpty(lastWeekHiringPatent) ? "" : " · " + lastWeekHiringPatent;
            string thisWeekBuild = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(sim.Player);
            string thisWeekBuildPart = string.IsNullOrEmpty(thisWeekBuild) ? "" : " · " + thisWeekBuild;
            _line.text = "Mascots" + lastWeekLandPart + lastWeekSiteCapitalPart + lifetimeGrantsNetPart + thisWeekChipRevenuePart + lastWeekConstructionPatentPart + lastWeekHiringPatentPart + thisWeekBuildPart + thisWeekFinesPowerPart + lastWeekSalariesRoomPart + lifetimeRevenueChipPart + lastWeekGpuUpkeepRentPart + lastWeekRevenueNetPart + thisWeekNetTrainingPart + lastWeekChipDevLandPart + thisWeekGrantsFinesPart + lifetimePowerFinesPart + lastWeekGreyLaunderPart + thisWeekConstructionGrantsPart + lastWeekTrainingRevenuePart + thisWeekSalariesGpuPart + lastWeekHardwarePowerPart + lifetimeTrainingLaunderPart + lastWeekChipSalesGrantsPart;
        }
    }
}
