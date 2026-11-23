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
                string thisWeekTrainingFines = HqDisplayQueriesHud.ThisWeekTrainingFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingFines : status + " · " + thisWeekTrainingFines;
                string lastWeekSalariesSite = HqDisplayQueriesHud.LastWeekSalariesSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesSite : status + " · " + lastWeekSalariesSite;
                string thisWeekRevenueGrey = HqDisplayQueriesHud.ThisWeekRevenueGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueGrey : status + " · " + thisWeekRevenueGrey;
                string lifetimePatentGrants = HqDisplayQueriesHud.LifetimePatentGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentGrants))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentGrants : status + " · " + lifetimePatentGrants;
                string lastWeekTrainingCapital = HqDisplayQueriesHud.LastWeekTrainingCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingCapital : status + " · " + lastWeekTrainingCapital;
                string thisWeekGpuGrant = HqDisplayQueriesHud.ThisWeekGpuGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuGrant : status + " · " + thisWeekGpuGrant;
                string lastWeekGreyLand = HqDisplayQueriesHud.LastWeekGreyLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyLand : status + " · " + lastWeekGreyLand;
                string thisWeekNetConstruction = HqDisplayQueriesHud.ThisWeekNetConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetConstruction : status + " · " + thisWeekNetConstruction;
                string lastWeekRevenueLaunder = HqDisplayQueriesHud.LastWeekRevenueLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueLaunder))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueLaunder : status + " · " + lastWeekRevenueLaunder;
                string lifetimeSiteChip = HqDisplayQueriesHud.LifetimeSiteChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSiteChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeSiteChip : status + " · " + lifetimeSiteChip;
                string thisWeekFinesGrey = HqDisplayQueriesHud.ThisWeekFinesGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesGrey : status + " · " + thisWeekFinesGrey;
                string lastWeekHardwareNet = HqDisplayQueriesHud.LastWeekHardwareNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareNet : status + " · " + lastWeekHardwareNet;
                string thisWeekGrantTraining2 = HqDisplayQueriesHud.ThisWeekGrantTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantTraining2))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantTraining2 : status + " · " + thisWeekGrantTraining2;
                string lastWeekPatentPower = HqDisplayQueriesHud.LastWeekPatentPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentPower : status + " · " + lastWeekPatentPower;
                string thisWeekChipGrant = HqDisplayQueriesHud.ThisWeekChipGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipGrant : status + " · " + thisWeekChipGrant;
                string lifetimeNetChip = HqDisplayQueriesHud.LifetimeNetChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetChip : status + " · " + lifetimeNetChip;
                string lastWeekSiteGrey = HqDisplayQueriesHud.LastWeekSiteGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteGrey : status + " · " + lastWeekSiteGrey;
                string thisWeekRevenueFines = HqDisplayQueriesHud.ThisWeekRevenueFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueFines : status + " · " + thisWeekRevenueFines;
                string lastWeekGrantHardware = HqDisplayQueriesHud.LastWeekGrantHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantHardware : status + " · " + lastWeekGrantHardware;
                string thisWeekRentGrant = HqDisplayQueriesHud.ThisWeekRentGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentGrant : status + " · " + thisWeekRentGrant;
                string lastWeekFinesSite = HqDisplayQueriesHud.LastWeekFinesSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesSite : status + " · " + lastWeekFinesSite;
                string lifetimeGreyPower = HqDisplayQueriesHud.LifetimeGreyPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyPower : status + " · " + lifetimeGreyPower;
                string thisWeekSalariesGrant = HqDisplayQueriesHud.ThisWeekSalariesGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesGrant : status + " · " + thisWeekSalariesGrant;
                string lastWeekChipLand = HqDisplayQueriesHud.LastWeekChipLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipLand : status + " · " + lastWeekChipLand;
                string lastWeekUpkeepGpu = HqDisplayQueriesHud.LastWeekUpkeepGpuSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekUpkeepGpu))
                    status = string.IsNullOrEmpty(status) ? lastWeekUpkeepGpu : status + " · " + lastWeekUpkeepGpu;
                string lastWeekTrainingSite = HqDisplayQueriesHud.LastWeekTrainingSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingSite : status + " · " + lastWeekTrainingSite;
                string thisWeekNetChipGrant = HqDisplayQueriesHud.ThisWeekNetChipGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetChipGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetChipGrant : status + " · " + thisWeekNetChipGrant;
                string lifetimeLaunderChip = HqDisplayQueriesHud.LifetimeLaunderChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderChip : status + " · " + lifetimeLaunderChip;
                string lastWeekRoomPatent = HqDisplayQueriesHud.LastWeekRoomPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomPatent : status + " · " + lastWeekRoomPatent;
                string thisWeekPowerGrey = HqDisplayQueriesHud.ThisWeekPowerGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerGrey : status + " · " + thisWeekPowerGrey;
                string lastWeekRevenueHardware = HqDisplayQueriesHud.LastWeekRevenueHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueHardware : status + " · " + lastWeekRevenueHardware;
                string thisWeekGrantConstruction = HqDisplayQueriesHud.ThisWeekGrantConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantConstruction : status + " · " + thisWeekGrantConstruction;
                string lifetimeFinesChip = HqDisplayQueriesHud.LifetimeFinesChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesChip : status + " · " + lifetimeFinesChip;
                string lastWeekLaunderChip = HqDisplayQueriesHud.LastWeekLaunderChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderChip : status + " · " + lastWeekLaunderChip;
                string lastWeekCapitalLaunder = HqDisplayQueriesHud.LastWeekCapitalLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalLaunder))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalLaunder : status + " · " + lastWeekCapitalLaunder;
                string thisWeekGrantChipNet = HqDisplayQueriesHud.ThisWeekGrantChipNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantChipNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantChipNet : status + " · " + thisWeekGrantChipNet;
                string lastWeekPatentTraining = HqDisplayQueriesHud.LastWeekPatentTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentTraining : status + " · " + lastWeekPatentTraining;
                string lastWeekGreyHardware = HqDisplayQueriesHud.LastWeekGreyHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyHardware : status + " · " + lastWeekGreyHardware;
                string thisWeekNetPower = HqDisplayQueriesHud.ThisWeekNetPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetPower : status + " · " + thisWeekNetPower;
                string lifetimeRevenueNet = HqDisplayQueriesHud.LifetimeRevenueNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueNet))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueNet : status + " · " + lifetimeRevenueNet;
                string lastWeekGrantPatent = HqDisplayQueriesHud.LastWeekGrantPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantPatent : status + " · " + lastWeekGrantPatent;
                string thisWeekChipBuild = HqDisplayQueriesHud.ThisWeekChipBuildSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipBuild))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipBuild : status + " · " + thisWeekChipBuild;
                string lastWeekLaunderGrey = HqDisplayQueriesHud.LastWeekLaunderGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderGrey))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderGrey : status + " · " + lastWeekLaunderGrey;
                string thisWeekGrantRevenue = HqDisplayQueriesHud.ThisWeekGrantRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantRevenue))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantRevenue : status + " · " + thisWeekGrantRevenue;
                string lastWeekSiteRoom = HqDisplayQueriesHud.LastWeekSiteRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteRoom : status + " · " + lastWeekSiteRoom;
                string lifetimeTrainingGrants = HqDisplayQueriesHud.LifetimeTrainingGrantsSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingGrants))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingGrants : status + " · " + lifetimeTrainingGrants;
                string lastWeekCapitalSite = HqDisplayQueriesHud.LastWeekCapitalSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalSite : status + " · " + lastWeekCapitalSite;
                string thisWeekRentChip = HqDisplayQueriesHud.ThisWeekRentChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentChip : status + " · " + thisWeekRentChip;
                string lastWeekFinesGrant = HqDisplayQueriesHud.LastWeekFinesGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesGrant : status + " · " + lastWeekFinesGrant;
                string lastWeekPowerLand = HqDisplayQueriesHud.LastWeekPowerLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerLand : status + " · " + lastWeekPowerLand;
                string thisWeekGrantGrey = HqDisplayQueriesHud.ThisWeekGrantGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantGrey : status + " · " + thisWeekGrantGrey;
                string lifetimeHardwareChip = HqDisplayQueriesHud.LifetimeHardwareChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareChip : status + " · " + lifetimeHardwareChip;
                string lastWeekRoomTraining = HqDisplayQueriesHud.LastWeekRoomTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomTraining : status + " · " + lastWeekRoomTraining;
                string thisWeekTrainingNet = HqDisplayQueriesHud.ThisWeekTrainingNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingNet : status + " · " + thisWeekTrainingNet;
                string lastWeekChipGrant = HqDisplayQueriesHud.LastWeekChipGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipGrant : status + " · " + lastWeekChipGrant;
                string thisWeekGpuPower = HqDisplayQueriesHud.ThisWeekGpuPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuPower : status + " · " + thisWeekGpuPower;
                string lifetimePatentChip = HqDisplayQueriesHud.LifetimePatentChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentChip))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentChip : status + " · " + lifetimePatentChip;
                string lastWeekNetConstruction = HqDisplayQueriesHud.LastWeekNetConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetConstruction : status + " · " + lastWeekNetConstruction;
                string lastWeekHiringHardware = HqDisplayQueriesHud.LastWeekHiringHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringHardware : status + " · " + lastWeekHiringHardware;
                string thisWeekSalariesChip = HqDisplayQueriesHud.ThisWeekSalariesChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesChip : status + " · " + thisWeekSalariesChip;
                string lastWeekGreyPatent = HqDisplayQueriesHud.LastWeekGreyPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyPatent : status + " · " + lastWeekGreyPatent;
                string lastWeekLandGrant = HqDisplayQueriesHud.LastWeekLandGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandGrant : status + " · " + lastWeekLandGrant;
                string thisWeekFinesGrant = HqDisplayQueriesHud.ThisWeekFinesGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesGrant : status + " · " + thisWeekFinesGrant;
                string lifetimeSiteGrey = HqDisplayQueriesHud.LifetimeSiteGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSiteGrey))
                    status = string.IsNullOrEmpty(status) ? lifetimeSiteGrey : status + " · " + lifetimeSiteGrey;
                string lastWeekGpuHardware = HqDisplayQueriesHud.LastWeekGpuHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuHardware))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuHardware : status + " · " + lastWeekGpuHardware;
                string thisWeekChipGreyGrant = HqDisplayQueriesHud.ThisWeekChipGreyGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipGreyGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipGreyGrant : status + " · " + thisWeekChipGreyGrant;
                string lastWeekTrainingHiring = HqDisplayQueriesHud.LastWeekTrainingHiringSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingHiring))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingHiring : status + " · " + lastWeekTrainingHiring;
                string thisWeekRevenueConstruction = HqDisplayQueriesHud.ThisWeekRevenueConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueConstruction : status + " · " + thisWeekRevenueConstruction;
                string lifetimeGrantFines = HqDisplayQueriesHud.LifetimeGrantFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantFines))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantFines : status + " · " + lifetimeGrantFines;
                string thisWeekSalariesGrey = HqDisplayQueriesHud.ThisWeekSalariesGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesGrey : status + " · " + thisWeekSalariesGrey;
                string lastWeekRevenueSite = HqDisplayQueriesHud.LastWeekRevenueSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueSite))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueSite : status + " · " + lastWeekRevenueSite;
                string thisWeekRentFines = HqDisplayQueriesHud.ThisWeekRentFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentFines : status + " · " + thisWeekRentFines;
                string lastWeekConstructionGrant = HqDisplayQueriesHud.LastWeekConstructionGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionGrant : status + " · " + lastWeekConstructionGrant;
                string thisWeekNetSalaries = HqDisplayQueriesHud.ThisWeekNetSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetSalaries))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetSalaries : status + " · " + thisWeekNetSalaries;
                string lifetimeLaunderPower = HqDisplayQueriesHud.LifetimeLaunderPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderPower : status + " · " + lifetimeLaunderPower;
                string lastWeekPatentLand = HqDisplayQueriesHud.LastWeekPatentLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentLand : status + " · " + lastWeekPatentLand;
                string thisWeekPowerConstruction = HqDisplayQueriesHud.ThisWeekPowerConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerConstruction : status + " · " + thisWeekPowerConstruction;
                string lastWeekLaunderCapital = HqDisplayQueriesHud.LastWeekLaunderCapitalSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderCapital))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderCapital : status + " · " + lastWeekLaunderCapital;
                string thisWeekGreySalaries = HqDisplayQueriesHud.ThisWeekGreySalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreySalaries))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreySalaries : status + " · " + thisWeekGreySalaries;
                string lifetimeNetGrant = HqDisplayQueriesHud.LifetimeNetGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetGrant : status + " · " + lifetimeNetGrant;
                string lastWeekHardwareTraining = HqDisplayQueriesHud.LastWeekHardwareTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareTraining : status + " · " + lastWeekHardwareTraining;
                string thisWeekGrantNetChip = HqDisplayQueriesHud.ThisWeekGrantNetChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantNetChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantNetChip : status + " · " + thisWeekGrantNetChip;
                string lastWeekSitePower = HqDisplayQueriesHud.LastWeekSitePowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSitePower))
                    status = string.IsNullOrEmpty(status) ? lastWeekSitePower : status + " · " + lastWeekSitePower;
                string thisWeekGrantFines = HqDisplayQueriesHud.ThisWeekGrantFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantFines : status + " · " + thisWeekGrantFines;
                string lastWeekRoomNet = HqDisplayQueriesHud.LastWeekRoomNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomNet : status + " · " + lastWeekRoomNet;
                string thisWeekChipDev = HqDisplayQueriesHud.ThisWeekChipDevSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipDev))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipDev : status + " · " + thisWeekChipDev;
                string lastWeekFinesGpu = HqDisplayQueriesHud.LastWeekFinesGpuSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesGpu))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesGpu : status + " · " + lastWeekFinesGpu;
                string lifetimeRevenueTraining = HqDisplayQueriesHud.LifetimeRevenueTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueTraining))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueTraining : status + " · " + lifetimeRevenueTraining;
                string thisWeekHiringSalaries = HqDisplayQueriesHud.ThisWeekHiringSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHiringSalaries))
                    status = string.IsNullOrEmpty(status) ? thisWeekHiringSalaries : status + " · " + thisWeekHiringSalaries;
                string lastWeekGrantLand = HqDisplayQueriesHud.LastWeekGrantLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantLand : status + " · " + lastWeekGrantLand;
                string thisWeekPatentGrey = HqDisplayQueriesHud.ThisWeekPatentGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentGrey))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentGrey : status + " · " + thisWeekPatentGrey;
                string lastWeekPowerRent = HqDisplayQueriesHud.LastWeekPowerRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerRent : status + " · " + lastWeekPowerRent;
                string thisWeekCapitalLand = HqDisplayQueriesHud.ThisWeekCapitalLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalLand : status + " · " + thisWeekCapitalLand;
                string lastWeekGreyPower = HqDisplayQueriesHud.LastWeekGreyPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyPower : status + " · " + lastWeekGreyPower;
                string thisWeekSiteRevenue = HqDisplayQueriesHud.ThisWeekSiteRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteRevenue))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteRevenue : status + " · " + thisWeekSiteRevenue;
                string lastWeekNetChip = HqDisplayQueriesHud.LastWeekNetChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetChip : status + " · " + lastWeekNetChip;
                string lifetimeConstructionGrant = HqDisplayQueriesHud.LifetimeConstructionGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionGrant : status + " · " + lifetimeConstructionGrant;
                string thisWeekLaunderFines = HqDisplayQueriesHud.ThisWeekLaunderFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderFines : status + " · " + thisWeekLaunderFines;
                string lastWeekTrainingPower = HqDisplayQueriesHud.LastWeekTrainingPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingPower : status + " · " + lastWeekTrainingPower;
                string lifetimePatentRevenue = HqDisplayQueriesHud.LifetimePatentRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentRevenue))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentRevenue : status + " · " + lifetimePatentRevenue;
                string lastWeekSalariesConstruction = HqDisplayQueriesHud.LastWeekSalariesConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesConstruction : status + " · " + lastWeekSalariesConstruction;
                string thisWeekChipDevGrant = HqDisplayQueriesHud.ThisWeekChipDevGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipDevGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipDevGrant : status + " · " + thisWeekChipDevGrant;
                string lastWeekRentRevenue = HqDisplayQueriesHud.LastWeekRentRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentRevenue))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentRevenue : status + " · " + lastWeekRentRevenue;
                string thisWeekHardwareFines = HqDisplayQueriesHud.ThisWeekHardwareFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareFines : status + " · " + thisWeekHardwareFines;
                string lifetimeRoomGrant = HqDisplayQueriesHud.LifetimeRoomGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRoomGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeRoomGrant : status + " · " + lifetimeRoomGrant;
                string lastWeekChipFines = HqDisplayQueriesHud.LastWeekChipFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipFines : status + " · " + lastWeekChipFines;
                string thisWeekLandTraining = HqDisplayQueriesHud.ThisWeekLandTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLandTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekLandTraining : status + " · " + thisWeekLandTraining;
                string lastWeekGrantGpu = HqDisplayQueriesHud.LastWeekGrantGpuSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantGpu))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantGpu : status + " · " + lastWeekGrantGpu;
                string lifetimeNetPower = HqDisplayQueriesHud.LifetimeNetPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetPower : status + " · " + lifetimeNetPower;
                string thisWeekCapitalTraining = HqDisplayQueriesHud.ThisWeekCapitalTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalTraining : status + " · " + thisWeekCapitalTraining;
                string lastWeekSiteChip = HqDisplayQueriesHud.LastWeekSiteChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteChip : status + " · " + lastWeekSiteChip;
                string lifetimeGreyRoom = HqDisplayQueriesHud.LifetimeGreyRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyRoom))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyRoom : status + " · " + lifetimeGreyRoom;
                string lastWeekGrantRevenue = HqDisplayQueriesHud.LastWeekGrantRevenueSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantRevenue))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantRevenue : status + " · " + lastWeekGrantRevenue;
                string thisWeekLaunderGrant = HqDisplayQueriesHud.ThisWeekLaunderGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderGrant : status + " · " + thisWeekLaunderGrant;
                string lastWeekPatentGrant = HqDisplayQueriesHud.LastWeekPatentGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentGrant : status + " · " + lastWeekPatentGrant;
                string thisWeekChipRoom = HqDisplayQueriesHud.ThisWeekChipRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipRoom))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipRoom : status + " · " + thisWeekChipRoom;
                string lastWeekGreyConstruction = HqDisplayQueriesHud.LastWeekGreyConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyConstruction : status + " · " + lastWeekGreyConstruction;
                string thisWeekNetLand = HqDisplayQueriesHud.ThisWeekNetLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetLand : status + " · " + thisWeekNetLand;
                string lastWeekSalariesRent = HqDisplayQueriesHud.LastWeekSalariesRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesRent : status + " · " + lastWeekSalariesRent;
                string thisWeekGrantLand = HqDisplayQueriesHud.ThisWeekGrantLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantLand : status + " · " + thisWeekGrantLand;
                string lifetimeCapitalGrey = HqDisplayQueriesHud.LifetimeCapitalGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeCapitalGrey))
                    status = string.IsNullOrEmpty(status) ? lifetimeCapitalGrey : status + " · " + lifetimeCapitalGrey;
                string lastWeekRoomFines = HqDisplayQueriesHud.LastWeekRoomFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomFines : status + " · " + lastWeekRoomFines;
                string thisWeekGpuHardware = HqDisplayQueriesHud.ThisWeekGpuHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuHardware))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuHardware : status + " · " + thisWeekGpuHardware;
                string lifetimeFinesRent = HqDisplayQueriesHud.LifetimeFinesRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesRent))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesRent : status + " · " + lifetimeFinesRent;
                string lastWeekCapitalConstruction = HqDisplayQueriesHud.LastWeekCapitalConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalConstruction : status + " · " + lastWeekCapitalConstruction;
                string thisWeekGreyLand = HqDisplayQueriesHud.ThisWeekGreyLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyLand : status + " · " + thisWeekGreyLand;
                string lastWeekLandNet = HqDisplayQueriesHud.LastWeekLandNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandNet : status + " · " + lastWeekLandNet;
                string thisWeekChipPatent = HqDisplayQueriesHud.ThisWeekChipPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipPatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipPatent : status + " · " + thisWeekChipPatent;
                string lifetimeHiringSalaries = HqDisplayQueriesHud.LifetimeHiringSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHiringSalaries))
                    status = string.IsNullOrEmpty(status) ? lifetimeHiringSalaries : status + " · " + lifetimeHiringSalaries;
                string lastWeekHardwareChip = HqDisplayQueriesHud.LastWeekHardwareChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareChip : status + " · " + lastWeekHardwareChip;
                string lifetimeTrainingGrant = HqDisplayQueriesHud.LifetimeTrainingGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingGrant : status + " · " + lifetimeTrainingGrant;
                string lastWeekNetGrant = HqDisplayQueriesHud.LastWeekNetGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetGrant : status + " · " + lastWeekNetGrant;
                string thisWeekSiteGrant = HqDisplayQueriesHud.ThisWeekSiteGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteGrant : status + " · " + thisWeekSiteGrant;
                string lastWeekLaunderRoom = HqDisplayQueriesHud.LastWeekLaunderRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderRoom : status + " · " + lastWeekLaunderRoom;
                string lastWeekHiringChip = HqDisplayQueriesHud.LastWeekHiringChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringChip : status + " · " + lastWeekHiringChip;
                string lifetimeLandGrant = HqDisplayQueriesHud.LifetimeLandGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLandGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeLandGrant : status + " · " + lifetimeLandGrant;
                string thisWeekPowerSalaries = HqDisplayQueriesHud.ThisWeekPowerSalariesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerSalaries))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerSalaries : status + " · " + thisWeekPowerSalaries;
                string thisWeekCapitalChip = HqDisplayQueriesHud.ThisWeekCapitalChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalChip : status + " · " + thisWeekCapitalChip;
                string lifetimeRentGrey = HqDisplayQueriesHud.LifetimeRentGreySummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentGrey))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentGrey : status + " · " + lifetimeRentGrey;
                string lastWeekPatentChip = HqDisplayQueriesHud.LastWeekPatentChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentChip : status + " · " + lastWeekPatentChip;
                string thisWeekGrantRoom = HqDisplayQueriesHud.ThisWeekGrantRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantRoom))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantRoom : status + " · " + thisWeekGrantRoom;
                string thisWeekLaunderChip = HqDisplayQueriesHud.ThisWeekLaunderChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderChip : status + " · " + thisWeekLaunderChip;
                string lifetimeSiteTraining = HqDisplayQueriesHud.LifetimeSiteTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSiteTraining))
                    status = string.IsNullOrEmpty(status) ? lifetimeSiteTraining : status + " · " + lifetimeSiteTraining;
                string lastWeekGpuGrant = HqDisplayQueriesHud.LastWeekGpuGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuGrant : status + " · " + lastWeekGpuGrant;
                string lastWeekConstructionNet = HqDisplayQueriesHud.LastWeekConstructionNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionNet))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionNet : status + " · " + lastWeekConstructionNet;
                string thisWeekHardwareGrant = HqDisplayQueriesHud.ThisWeekHardwareGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareGrant : status + " · " + thisWeekHardwareGrant;
                string lifetimeFinesLand = HqDisplayQueriesHud.LifetimeFinesLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesLand))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesLand : status + " · " + lifetimeFinesLand;
                string lastWeekChipRoom = HqDisplayQueriesHud.LastWeekChipRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipRoom : status + " · " + lastWeekChipRoom;
                string thisWeekPatentLand = HqDisplayQueriesHud.ThisWeekPatentLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentLand : status + " · " + thisWeekPatentLand;
                string lastWeekRentLand = HqDisplayQueriesHud.LastWeekRentLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentLand : status + " · " + lastWeekRentLand;
                string thisWeekRoomTraining = HqDisplayQueriesHud.ThisWeekRoomTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRoomTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekRoomTraining : status + " · " + thisWeekRoomTraining;
                string lifetimeChipGrant = HqDisplayQueriesHud.LifetimeChipGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipGrant : status + " · " + lifetimeChipGrant;
                string lastWeekPowerGrant = HqDisplayQueriesHud.LastWeekPowerGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerGrant : status + " · " + lastWeekPowerGrant;
                string thisWeekRentLand = HqDisplayQueriesHud.ThisWeekRentLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentLand : status + " · " + thisWeekRentLand;
                string lastWeekLaunderTraining = HqDisplayQueriesHud.LastWeekLaunderTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderTraining : status + " · " + lastWeekLaunderTraining;
                string lifetimeNetRoom = HqDisplayQueriesHud.LifetimeNetRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetRoom))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetRoom : status + " · " + lifetimeNetRoom;
                string lifetimePowerGrant = HqDisplayQueriesHud.LifetimePowerGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerGrant : status + " · " + lifetimePowerGrant;
                string lastWeekGreyGrant = HqDisplayQueriesHud.LastWeekGreyGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyGrant : status + " · " + lastWeekGreyGrant;
                string thisWeekFinesLand = HqDisplayQueriesHud.ThisWeekFinesLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesLand : status + " · " + thisWeekFinesLand;
                string lifetimeHardwareRent = HqDisplayQueriesHud.LifetimeHardwareRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareRent))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareRent : status + " · " + lifetimeHardwareRent;
                string lastWeekCapitalPower = HqDisplayQueriesHud.LastWeekCapitalPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalPower : status + " · " + lastWeekCapitalPower;
                string thisWeekPatentTraining = HqDisplayQueriesHud.ThisWeekPatentTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentTraining : status + " · " + thisWeekPatentTraining;
                string thisWeekLaunderPower = HqDisplayQueriesHud.ThisWeekLaunderPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderPower : status + " · " + thisWeekLaunderPower;
                string lifetimeGreyChip = HqDisplayQueriesHud.LifetimeGreyChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyChip : status + " · " + lifetimeGreyChip;
                string lastWeekRentConstruction = HqDisplayQueriesHud.LastWeekRentConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentConstruction : status + " · " + lastWeekRentConstruction;
                string lifetimeSalariesPower = HqDisplayQueriesHud.LifetimeSalariesPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSalariesPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeSalariesPower : status + " · " + lifetimeSalariesPower;
                string lastWeekFinesRoom = HqDisplayQueriesHud.LastWeekFinesRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesRoom : status + " · " + lastWeekFinesRoom;
                string lastWeekLandChip = HqDisplayQueriesHud.LastWeekLandChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandChip : status + " · " + lastWeekLandChip;
                string thisWeekHardwareRoom = HqDisplayQueriesHud.ThisWeekHardwareRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareRoom))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareRoom : status + " · " + thisWeekHardwareRoom;
                string lifetimePatentGrant = HqDisplayQueriesHud.LifetimePatentGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentGrant : status + " · " + lifetimePatentGrant;
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