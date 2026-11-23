using AiLabs.Rules.Sim;
using AiLabs.Rules.State;
using AiLabs.Unity.Sim;
using UnityEngine;
using UnityEngine.UI;

namespace AiLabs.Unity.Hq
{
    /// <summary>Cash, calendar date, era label, and the advance-week control.</summary>
    public sealed class HqHudView : MonoBehaviour
    {
        SimulationHost _host;
        Image _barImage;
        Text _cashLabel;
        Text _dateLabel;
        Text _eraLabel;
        Text _phaseLabel;
        Text _researchLabel;
        Text _statusLabel;

        public void Bind(SimulationHost host)
        {
            _host = host;
            _host.StateChanged += OnStateChanged;
            OnStateChanged();
        }

        void OnDestroy()
        {
            if (_host != null) _host.StateChanged -= OnStateChanged;
        }

        void OnStateChanged()
        {
            if (_host?.Simulation == null) return;
            Simulation sim = _host.Simulation;
            LabState lab = sim.Player;
            if (_cashLabel != null)
            {
                _cashLabel.text = "Cash: $" + lab.Cash.ToString("N0");
                _cashLabel.color = HqFinancialHud.CashColor(lab);
            }
            if (_dateLabel != null) _dateLabel.text = sim.DateLabel;
            if (_researchLabel != null)
                _researchLabel.text = "Research: " + lab.ResearchPoints.ToString("N0") + " pts · Staff: " + lab.Staff.Count;
            if (_eraLabel != null) _eraLabel.text = sim.Era.Name;
            if (_phaseLabel != null)
                _phaseLabel.text = HqCampusPhase.LabelForEra(sim.Config, sim.Era.Id, lab);
            if (_statusLabel != null)
            {
                string status = HqFinancialHud.StatusLine(lab);
                string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekLand : status + " · " + lastWeekLand;
                string lastWeekSiteCapital = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteCapital : status + " · " + lastWeekSiteCapital;
                string lifetimeGrantsNet = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantsNet))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantsNet : status + " · " + lifetimeGrantsNet;
                string thisWeekChipRevenue = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipRevenue))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipRevenue : status + " · " + thisWeekChipRevenue;
                string lastWeekConstructionPatent = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionPatent : status + " · " + lastWeekConstructionPatent;
                string lastWeekHiringPatent = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringPatent : status + " · " + lastWeekHiringPatent;
                string thisWeekBuild = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekBuild))
                    status = string.IsNullOrEmpty(status) ? thisWeekBuild : status + " · " + thisWeekBuild;
                string thisWeekFinesPower = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesPower : status + " · " + thisWeekFinesPower;
                string lastWeekChipDevLand = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipDevLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipDevLand : status + " · " + lastWeekChipDevLand;
                string thisWeekSalariesGpu = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesGpu))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesGpu : status + " · " + thisWeekSalariesGpu;
                string lastWeekPatentSite = HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentSite : status + " · " + lastWeekPatentSite;
                string lastWeekCapitalGrey = HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalGrey : status + " · " + lastWeekCapitalGrey;
                string lastWeekGrantsRevenue = HqDisplayQueriesHud.LastWeekGrantsRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantsRevenue))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantsRevenue : status + " · " + lastWeekGrantsRevenue;
                string thisWeekGpuFinesGrey = HqDisplayQueriesHud.ThisWeekGpuFinesGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuFinesGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuFinesGrey : status + " · " + thisWeekGpuFinesGrey;
                string lastWeekSalariesRoom = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesRoom : status + " · " + lastWeekSalariesRoom;
                string lifetimeRevenueChip = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueChip : status + " · " + lifetimeRevenueChip;
                string lastWeekGpuUpkeepRent = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuUpkeepRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuUpkeepRent : status + " · " + lastWeekGpuUpkeepRent;
                string lastWeekRevenueNet = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueNet : status + " · " + lastWeekRevenueNet;
                string thisWeekNetTraining = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetTraining : status + " · " + thisWeekNetTraining;
                string thisWeekGrantsFines = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantsFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantsFines : status + " · " + thisWeekGrantsFines;
                string lifetimePowerFines = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerFines))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerFines : status + " · " + lifetimePowerFines;
                string lastWeekGreyLaunder = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyLaunder))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyLaunder : status + " · " + lastWeekGreyLaunder;
                string thisWeekConstructionGrants = HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionGrants))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionGrants : status + " · " + thisWeekConstructionGrants;
                string lastWeekTrainingRevenue = HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingRevenue))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingRevenue : status + " · " + lastWeekTrainingRevenue;
                string lastWeekHardwarePower = HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwarePower))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwarePower : status + " · " + lastWeekHardwarePower;
                string lifetimeTrainingLaunder = HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingLaunder))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingLaunder : status + " · " + lifetimeTrainingLaunder;
                string lastWeekChipSalesGrants = HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipSalesGrants))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipSalesGrants : status + " · " + lastWeekChipSalesGrants;
                string lastWeekHiringGrey = HqDisplayQueriesHud.LastWeekHiringGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringGrey : status + " · " + lastWeekHiringGrey;
                string thisWeekPowerChip = HqDisplayQueriesHud.ThisWeekPowerChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerChip : status + " · " + thisWeekPowerChip;
                string thisWeekTrainingChip = HqDisplayQueriesHud.ThisWeekTrainingChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingChip : status + " · " + thisWeekTrainingChip;
                string lifetimeNetTraining = HqDisplayQueriesHud.LifetimeNetTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetTraining))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetTraining : status + " · " + lifetimeNetTraining;
                string lastWeekFinesTraining = HqDisplayQueriesHud.LastWeekFinesTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesTraining : status + " · " + lastWeekFinesTraining;
                string thisWeekGreyNet = HqDisplayQueriesHud.ThisWeekGreyNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyNet : status + " · " + thisWeekGreyNet;
                string lastWeekRoomHiring = HqDisplayQueriesHud.LastWeekRoomHiringSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomHiring))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomHiring : status + " · " + lastWeekRoomHiring;
                string lifetimeChipGrey = HqDisplayQueriesHud.LifetimeChipGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipGrey))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipGrey : status + " · " + lifetimeChipGrey;
                string lastWeekTrainingGpuUpkeep = HqDisplayQueriesHud.LastWeekTrainingGpuUpkeepSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingGpuUpkeep))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingGpuUpkeep : status + " · " + lastWeekTrainingGpuUpkeep;
                string lastWeekChipDevSales = HqDisplayQueriesHud.LastWeekChipDevSalesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipDevSales))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipDevSales : status + " · " + lastWeekChipDevSales;
                string thisWeekRevenueGrants = HqDisplayQueriesHud.ThisWeekRevenueGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueGrants))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueGrants : status + " · " + thisWeekRevenueGrants;
                string lastWeekConstructionLand = HqDisplayQueriesHud.LastWeekConstructionLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionLand : status + " · " + lastWeekConstructionLand;
                string lifetimeFinesPower = HqDisplayQueriesHud.LifetimeFinesPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesPower : status + " · " + lifetimeFinesPower;
                string thisWeekGpuNet = HqDisplayQueriesHud.ThisWeekGpuNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuNet : status + " · " + thisWeekGpuNet;
                string lastWeekPatentHiring = HqDisplayQueriesHud.LastWeekPatentHiringSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentHiring))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentHiring : status + " · " + lastWeekPatentHiring;
                string thisWeekChipConstruction = HqDisplayQueriesHud.ThisWeekChipConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipConstruction : status + " · " + thisWeekChipConstruction;
                string thisWeekSalariesFines = HqDisplayQueriesHud.ThisWeekSalariesFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesFines : status + " · " + thisWeekSalariesFines;
                string lastWeekNetFines = HqDisplayQueriesHud.LastWeekNetFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetFines : status + " · " + lastWeekNetFines;
                string thisWeekGreyPower = HqDisplayQueriesHud.ThisWeekGreyPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyPower : status + " · " + thisWeekGreyPower;
                string lifetimeGrantsLaunder = HqDisplayQueriesHud.LifetimeGrantsLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantsLaunder))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantsLaunder : status + " · " + lifetimeGrantsLaunder;
                string lastWeekSalariesPower = HqDisplayQueriesHud.LastWeekSalariesPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesPower : status + " · " + lastWeekSalariesPower;
                string thisWeekTrainingPower = HqDisplayQueriesHud.ThisWeekTrainingPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingPower : status + " · " + thisWeekTrainingPower;
                string lastWeekChipGrey = HqDisplayQueriesHud.LastWeekChipGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipGrey : status + " · " + lastWeekChipGrey;
                string lastWeekConstructionGrey = HqDisplayQueriesHud.LastWeekConstructionGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionGrey : status + " · " + lastWeekConstructionGrey;
                string lifetimeRevenuePower = HqDisplayQueriesHud.LifetimeRevenuePowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenuePower))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenuePower : status + " · " + lifetimeRevenuePower;
                string lastWeekGpuFines = HqDisplayQueriesHud.LastWeekGpuFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuFines : status + " · " + lastWeekGpuFines;
                string thisWeekSalariesNet = HqDisplayQueriesHud.ThisWeekSalariesNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesNet : status + " · " + thisWeekSalariesNet;
                string lastWeekCapitalPatent = HqDisplayQueriesHud.LastWeekCapitalPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalPatent : status + " · " + lastWeekCapitalPatent;
                string thisWeekChipTraining = HqDisplayQueriesHud.ThisWeekChipTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipTraining : status + " · " + thisWeekChipTraining;
                string lastWeekRevenueChip = HqDisplayQueriesHud.LastWeekRevenueChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueChip : status + " · " + lastWeekRevenueChip;
                string lifetimeGreyNet = HqDisplayQueriesHud.LifetimeGreyNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyNet))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyNet : status + " · " + lifetimeGreyNet;
                string thisWeekConstructionPower = HqDisplayQueriesHud.ThisWeekConstructionPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionPower : status + " · " + thisWeekConstructionPower;
                string lastWeekSalariesHiring = HqDisplayQueriesHud.LastWeekSalariesHiringSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesHiring))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesHiring : status + " · " + lastWeekSalariesHiring;
                string thisWeekGrantsChip = HqDisplayQueriesHud.ThisWeekGrantsChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantsChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantsChip : status + " · " + thisWeekGrantsChip;
                string lastWeekNetPower = HqDisplayQueriesHud.LastWeekNetPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetPower : status + " · " + lastWeekNetPower;
                string thisWeekFinesChip = HqDisplayQueriesHud.ThisWeekFinesChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesChip : status + " · " + thisWeekFinesChip;
                string lifetimeChipPower = HqDisplayQueriesHud.LifetimeChipPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipPower : status + " · " + lifetimeChipPower;
                string lastWeekGreyGrants = HqDisplayQueriesHud.LastWeekGreyGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyGrants))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyGrants : status + " · " + lastWeekGreyGrants;
                string thisWeekGpuConstruction = HqDisplayQueriesHud.ThisWeekGpuConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuConstruction : status + " · " + thisWeekGpuConstruction;
                string lastWeekSiteHardware = HqDisplayQueriesHud.LastWeekSiteHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteHardware : status + " · " + lastWeekSiteHardware;
                string lastWeekLaunderFines = HqDisplayQueriesHud.LastWeekLaunderFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderFines : status + " · " + lastWeekLaunderFines;
                string lifetimeTrainingChip = HqDisplayQueriesHud.LifetimeTrainingChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingChip : status + " · " + lifetimeTrainingChip;
                string thisWeekRevenueTraining = HqDisplayQueriesHud.ThisWeekRevenueTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueTraining : status + " · " + thisWeekRevenueTraining;
                string lastWeekPowerChipDev = HqDisplayQueriesHud.LastWeekPowerChipDevSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerChipDev))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerChipDev : status + " · " + lastWeekPowerChipDev;
                string thisWeekSalariesGrants = HqDisplayQueriesHud.ThisWeekSalariesGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesGrants))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesGrants : status + " · " + thisWeekSalariesGrants;
                string lastWeekNetGrey = HqDisplayQueriesHud.LastWeekNetGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetGrey : status + " · " + lastWeekNetGrey;
                string thisWeekGrantNet = HqDisplayQueriesHud.ThisWeekGrantNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantNet : status + " · " + thisWeekGrantNet;
                string lastWeekTrainingConstruction = HqDisplayQueriesHud.LastWeekTrainingConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingConstruction : status + " · " + lastWeekTrainingConstruction;
                string lifetimeFinesGrey = HqDisplayQueriesHud.LifetimeFinesGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesGrey))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesGrey : status + " · " + lifetimeFinesGrey;
                string thisWeekChipGpu = HqDisplayQueriesHud.ThisWeekChipGpuSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipGpu))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipGpu : status + " · " + thisWeekChipGpu;
                string lastWeekHiringCapital = HqDisplayQueriesHud.LastWeekHiringCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringCapital : status + " · " + lastWeekHiringCapital;
                string lastWeekRoomPower = HqDisplayQueriesHud.LastWeekRoomPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomPower : status + " · " + lastWeekRoomPower;
                string thisWeekGreyConstruction = HqDisplayQueriesHud.ThisWeekGreyConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyConstruction : status + " · " + thisWeekGreyConstruction;
                string lastWeekChipPatent = HqDisplayQueriesHud.LastWeekChipPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipPatent : status + " · " + lastWeekChipPatent;
                string lifetimeRevenueGrants = HqDisplayQueriesHud.LifetimeRevenueGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueGrants))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueGrants : status + " · " + lifetimeRevenueGrants;
                string thisWeekFinesTraining = HqDisplayQueriesHud.ThisWeekFinesTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesTraining : status + " · " + thisWeekFinesTraining;
                string lastWeekGpuGrey = HqDisplayQueriesHud.LastWeekGpuGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuGrey : status + " · " + lastWeekGpuGrey;
                string thisWeekNetChip = HqDisplayQueriesHud.ThisWeekNetChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetChip : status + " · " + thisWeekNetChip;
                string lastWeekGrantTraining = HqDisplayQueriesHud.LastWeekGrantTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantTraining : status + " · " + lastWeekGrantTraining;
                string thisWeekRevenuePower = HqDisplayQueriesHud.ThisWeekRevenuePowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenuePower))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenuePower : status + " · " + thisWeekRevenuePower;
                string lifetimeChipDev = HqDisplayQueriesHud.LifetimeChipDevSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipDev))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipDev : status + " · " + lifetimeChipDev;
                string lastWeekConstructionChip = HqDisplayQueriesHud.LastWeekConstructionChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionChip : status + " · " + lastWeekConstructionChip;
                string thisWeekGreyTraining = HqDisplayQueriesHud.ThisWeekGreyTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyTraining : status + " · " + thisWeekGreyTraining;
                string lastWeekFinesCapital = HqDisplayQueriesHud.LastWeekFinesCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesCapital : status + " · " + lastWeekFinesCapital;
                string thisWeekGpuSalaries = HqDisplayQueriesHud.ThisWeekGpuSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuSalaries))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuSalaries : status + " · " + thisWeekGpuSalaries;
                string lastWeekLandPatent = HqDisplayQueriesHud.LastWeekLandPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandPatent : status + " · " + lastWeekLandPatent;
                string lifetimeLaunderGrants = HqDisplayQueriesHud.LifetimeLaunderGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderGrants))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderGrants : status + " · " + lifetimeLaunderGrants;
                string thisWeekConstructionNet = HqDisplayQueriesHud.ThisWeekConstructionNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionNet : status + " · " + thisWeekConstructionNet;
                string lastWeekRoomGrey = HqDisplayQueriesHud.LastWeekRoomGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomGrey : status + " · " + lastWeekRoomGrey;
                string thisWeekChipFines = HqDisplayQueriesHud.ThisWeekChipFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipFines : status + " · " + thisWeekChipFines;
                string lastWeekHardwareHiring = HqDisplayQueriesHud.LastWeekHardwareHiringSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareHiring))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareHiring : status + " · " + lastWeekHardwareHiring;
                string thisWeekRevenueSalaries = HqDisplayQueriesHud.ThisWeekRevenueSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueSalaries))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueSalaries : status + " · " + thisWeekRevenueSalaries;
                string lifetimeNetLaunder = HqDisplayQueriesHud.LifetimeNetLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetLaunder))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetLaunder : status + " · " + lifetimeNetLaunder;
                string lastWeekSiteTraining = HqDisplayQueriesHud.LastWeekSiteTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteTraining : status + " · " + lastWeekSiteTraining;
                string thisWeekGreyChip = HqDisplayQueriesHud.ThisWeekGreyChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyChip : status + " · " + thisWeekGreyChip;
                string lastWeekPowerNet = HqDisplayQueriesHud.LastWeekPowerNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerNet : status + " · " + lastWeekPowerNet;
                string thisWeekConstructionGrey = HqDisplayQueriesHud.ThisWeekConstructionGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionGrey : status + " · " + thisWeekConstructionGrey;
                string lastWeekGpuLand = HqDisplayQueriesHud.LastWeekGpuLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuLand : status + " · " + lastWeekGpuLand;
                string lifetimePowerChip = HqDisplayQueriesHud.LifetimePowerChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerChip))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerChip : status + " · " + lifetimePowerChip;
                string thisWeekGpuGrey = HqDisplayQueriesHud.ThisWeekGpuGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuGrey : status + " · " + thisWeekGpuGrey;
                string lastWeekPatentNet = HqDisplayQueriesHud.LastWeekPatentNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentNet : status + " · " + lastWeekPatentNet;
                string thisWeekTrainingGrant = HqDisplayQueriesHud.ThisWeekTrainingGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingGrant : status + " · " + thisWeekTrainingGrant;
                string lastWeekCapitalHardware = HqDisplayQueriesHud.LastWeekCapitalHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalHardware : status + " · " + lastWeekCapitalHardware;
                string thisWeekFinesNet = HqDisplayQueriesHud.ThisWeekFinesNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesNet : status + " · " + thisWeekFinesNet;
                string lifetimeGrantsChip = HqDisplayQueriesHud.LifetimeGrantsChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantsChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantsChip : status + " · " + lifetimeGrantsChip;
                string lastWeekLaunderPower = HqDisplayQueriesHud.LastWeekLaunderPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderPower : status + " · " + lastWeekLaunderPower;
                string thisWeekSalariesConstruction = HqDisplayQueriesHud.ThisWeekSalariesConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesConstruction : status + " · " + thisWeekSalariesConstruction;
                string lastWeekChipTraining = HqDisplayQueriesHud.LastWeekChipTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipTraining : status + " · " + lastWeekChipTraining;
                string thisWeekGrantsPower = HqDisplayQueriesHud.ThisWeekGrantsPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantsPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantsPower : status + " · " + thisWeekGrantsPower;
                string lastWeekRoomLand = HqDisplayQueriesHud.LastWeekRoomLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomLand : status + " · " + lastWeekRoomLand;
                string lifetimeFinesTraining = HqDisplayQueriesHud.LifetimeFinesTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesTraining))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesTraining : status + " · " + lifetimeFinesTraining;
                string thisWeekNetRevenue = HqDisplayQueriesHud.ThisWeekNetRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetRevenue))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetRevenue : status + " · " + thisWeekNetRevenue;
                string lastWeekHiringSite = HqDisplayQueriesHud.LastWeekHiringSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringSite : status + " · " + lastWeekHiringSite;
                string thisWeekChipRent = HqDisplayQueriesHud.ThisWeekChipRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipRent : status + " · " + thisWeekChipRent;
                string lastWeekPatentGrey = HqDisplayQueriesHud.LastWeekPatentGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentGrey : status + " · " + lastWeekPatentGrey;
                string thisWeekRentNet = HqDisplayQueriesHud.ThisWeekRentNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentNet : status + " · " + thisWeekRentNet;
                string lifetimeRevenueLaunder = HqDisplayQueriesHud.LifetimeRevenueLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueLaunder))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueLaunder : status + " · " + lifetimeRevenueLaunder;
                string lastWeekConstructionPower = HqDisplayQueriesHud.LastWeekConstructionPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionPower : status + " · " + lastWeekConstructionPower;
                string lastWeekSalariesGrey = HqDisplayQueriesHud.LastWeekSalariesGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesGrey : status + " · " + lastWeekSalariesGrey;
                string thisWeekGrantChip = HqDisplayQueriesHud.ThisWeekGrantChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantChip : status + " · " + thisWeekGrantChip;
                string lastWeekHardwareSite = HqDisplayQueriesHud.LastWeekHardwareSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareSite : status + " · " + lastWeekHardwareSite;
                string thisWeekFinesConstruction = HqDisplayQueriesHud.ThisWeekFinesConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesConstruction : status + " · " + thisWeekFinesConstruction;
                string lifetimeGreyTraining = HqDisplayQueriesHud.LifetimeGreyTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyTraining))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyTraining : status + " · " + lifetimeGreyTraining;
                string lastWeekGrantLaunder = HqDisplayQueriesHud.LastWeekGrantLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantLaunder))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantLaunder : status + " · " + lastWeekGrantLaunder;
                string thisWeekGrantPower = HqDisplayQueriesHud.ThisWeekGrantPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantPower : status + " · " + thisWeekGrantPower;
                string lastWeekTrainingGrey = HqDisplayQueriesHud.LastWeekTrainingGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingGrey : status + " · " + lastWeekTrainingGrey;
                string lastWeekRevenueCapital = HqDisplayQueriesHud.LastWeekRevenueCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueCapital : status + " · " + lastWeekRevenueCapital;
                string thisWeekNetGrant = HqDisplayQueriesHud.ThisWeekNetGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetGrant : status + " · " + thisWeekNetGrant;
                string lifetimeLaunderNet = HqDisplayQueriesHud.LifetimeLaunderNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderNet))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderNet : status + " · " + lifetimeLaunderNet;
                string lastWeekFinesHardware = HqDisplayQueriesHud.LastWeekFinesHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesHardware : status + " · " + lastWeekFinesHardware;
                string thisWeekSalariesPower = HqDisplayQueriesHud.ThisWeekSalariesPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesPower : status + " · " + thisWeekSalariesPower;
                string lastWeekGrantSite = HqDisplayQueriesHud.LastWeekGrantSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantSite : status + " · " + lastWeekGrantSite;
                string thisWeekConstructionChip = HqDisplayQueriesHud.ThisWeekConstructionChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionChip : status + " · " + thisWeekConstructionChip;
                string lastWeekNetLand = HqDisplayQueriesHud.LastWeekNetLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetLand : status + " · " + lastWeekNetLand;
                string lifetimeHardwareTraining = HqDisplayQueriesHud.LifetimeHardwareTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareTraining))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareTraining : status + " · " + lifetimeHardwareTraining;
                string thisWeekGreyGrant = HqDisplayQueriesHud.ThisWeekGreyGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyGrant : status + " · " + thisWeekGreyGrant;
                string lastWeekChipPower = HqDisplayQueriesHud.LastWeekChipPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipPower : status + " · " + lastWeekChipPower;
                string vendorCourt = HqDisplayQueriesHud.LatestVendorCourtLine(sim.State, sim.Tick);
                if (!string.IsNullOrEmpty(vendorCourt))
                    status = string.IsNullOrEmpty(status) ? vendorCourt : status + " · " + vendorCourt;
                _statusLabel.text = status;
                _statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(status));
            }
            if (_barImage != null) _barImage.color = HqFinancialHud.BarTint(lab);
        }

        public static HqHudView Create(Transform parent, SimulationHost host)
        {
            var canvasGo = new GameObject("HUD");
            canvasGo.transform.SetParent(parent, false);
            var canvas = canvasGo.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasGo.AddComponent<CanvasScaler>().uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasGo.AddComponent<GraphicRaycaster>();

            var panel = new GameObject("TopBar");
            panel.transform.SetParent(canvasGo.transform, false);
            var panelImage = panel.AddComponent<Image>();
            panelImage.color = new Color(0.08f, 0.09f, 0.12f, 0.92f);
            var panelRect = panel.GetComponent<RectTransform>();
            panelRect.anchorMin = new Vector2(0f, 1f);
            panelRect.anchorMax = new Vector2(1f, 1f);
            panelRect.pivot = new Vector2(0.5f, 1f);
            panelRect.sizeDelta = new Vector2(0f, 96f);
            panelRect.anchoredPosition = Vector2.zero;

            var hud = panel.AddComponent<HqHudView>();
            hud._barImage = panelImage;
            hud._cashLabel = CreateLabel(panel.transform, "CashLabel", new Vector2(16f, -12f), TextAnchor.UpperLeft);
            hud._dateLabel = CreateLabel(panel.transform, "DateLabel", new Vector2(220f, -12f), TextAnchor.UpperLeft);
            hud._researchLabel = CreateLabel(panel.transform, "ResearchLabel", new Vector2(16f, -40f), TextAnchor.UpperLeft);
            hud._researchLabel.fontSize = 18;
            hud._statusLabel = CreateLabel(panel.transform, "StatusLabel", new Vector2(16f, -68f), TextAnchor.UpperLeft);
            hud._statusLabel.fontSize = 16;
            hud._statusLabel.color = new Color(1f, 0.7f, 0.45f);
            hud._phaseLabel = CreateLabel(panel.transform, "PhaseLabel", new Vector2(16f, -40f), TextAnchor.UpperRight);
            var phaseRect = hud._phaseLabel.GetComponent<RectTransform>();
            phaseRect.anchorMin = new Vector2(1f, 1f);
            phaseRect.anchorMax = new Vector2(1f, 1f);
            phaseRect.pivot = new Vector2(1f, 1f);
            phaseRect.anchoredPosition = new Vector2(-16f, -12f);
            phaseRect.sizeDelta = new Vector2(280f, 28f);
            hud._phaseLabel.alignment = TextAnchor.UpperRight;

            hud._eraLabel = CreateLabel(panel.transform, "EraLabel", new Vector2(-16f, -40f), TextAnchor.UpperRight);

            CreateTimeControls(canvasGo.transform, hud);
            CreateSaveLoadControls(canvasGo.transform, hud);
            hud.Bind(host);
            return hud;
        }

        static void CreateSaveLoadControls(Transform parent, HqHudView hud)
        {
            AddHudButton(parent, "Save game", new Vector2(24f, 24f), new Vector2(0f, 0f), () =>
                HqSaveLoad.SaveToFile(hud._host, HqSaveLoad.DefaultPath));
            AddHudButton(parent, "Load game", new Vector2(184f, 24f), new Vector2(0f, 0f), () =>
                HqSaveLoad.LoadFromFile(hud._host, HqSaveLoad.DefaultPath));
        }

        static void AddHudButton(Transform parent, string caption, Vector2 anchoredPos, Vector2 anchor, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(caption);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = anchor;
            rect.anchorMax = anchor;
            rect.pivot = new Vector2(0f, 0f);
            rect.sizeDelta = new Vector2(148f, 44f);
            rect.anchoredPosition = anchoredPos;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.2f, 0.45f, 0.35f, 1f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var label = labelGo.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 15;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = caption;
        }

        static Text CreateLabel(Transform parent, string name, Vector2 anchoredPos, TextAnchor align)
        {
            var go = new GameObject(name);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = align == TextAnchor.UpperRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.anchorMax = align == TextAnchor.UpperRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.pivot = align == TextAnchor.UpperRight ? new Vector2(1f, 1f) : new Vector2(0f, 1f);
            rect.sizeDelta = new Vector2(420f, 28f);
            rect.anchoredPosition = anchoredPos;
            var text = go.AddComponent<Text>();
            text.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            text.fontSize = 20;
            text.color = Color.white;
            text.alignment = align;
            return text;
        }

        static void CreateTimeControls(Transform parent, HqHudView hud)
        {
            float y = 24f;
            AddTimeButton(parent, hud, "Advance week", new Vector2(-24f, y), () => hud._host.StepWeek());
            AddTimeButton(parent, hud, "Advance month", new Vector2(-240f, y), () =>
            {
                int weeks = hud._host.Simulation.Config.Calendar.WeeksPerMonth;
                hud._host.StepWeeks(weeks);
            });
            AddTimeButton(parent, hud, "Advance year", new Vector2(-456f, y), () =>
            {
                int weeks = hud._host.Simulation.Config.Calendar.WeeksPerYear;
                hud._host.StepWeeks(weeks);
            });
        }

        static void AddTimeButton(Transform parent, HqHudView hud, string caption, Vector2 anchoredPos, UnityEngine.Events.UnityAction onClick)
        {
            var go = new GameObject(caption);
            go.transform.SetParent(parent, false);
            var rect = go.AddComponent<RectTransform>();
            rect.anchorMin = new Vector2(1f, 0f);
            rect.anchorMax = new Vector2(1f, 0f);
            rect.pivot = new Vector2(1f, 0f);
            rect.sizeDelta = new Vector2(200f, 48f);
            rect.anchoredPosition = anchoredPos;

            var image = go.AddComponent<Image>();
            image.color = new Color(0.25f, 0.55f, 0.85f, 1f);
            var button = go.AddComponent<Button>();
            button.targetGraphic = image;
            button.onClick.AddListener(onClick);

            var labelGo = new GameObject("Label");
            labelGo.transform.SetParent(go.transform, false);
            var labelRect = labelGo.AddComponent<RectTransform>();
            labelRect.anchorMin = Vector2.zero;
            labelRect.anchorMax = Vector2.one;
            labelRect.offsetMin = Vector2.zero;
            labelRect.offsetMax = Vector2.zero;
            var label = labelGo.AddComponent<Text>();
            label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
            label.fontSize = 16;
            label.alignment = TextAnchor.MiddleCenter;
            label.color = Color.white;
            label.text = caption;
        }
    }
}