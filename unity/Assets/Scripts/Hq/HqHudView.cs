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
                string lastWeekGreyChip = HqDisplayQueriesHud.LastWeekGreyChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyChip : status + " · " + lastWeekGreyChip;
                string thisWeekCapitalGrant = HqDisplayQueriesHud.ThisWeekCapitalGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalGrant : status + " · " + thisWeekCapitalGrant;
                string lastWeekTrainingChip = HqDisplayQueriesHud.LastWeekTrainingChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingChip : status + " · " + lastWeekTrainingChip;
                string thisWeekSiteFines = HqDisplayQueriesHud.ThisWeekSiteFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteFines))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteFines : status + " · " + thisWeekSiteFines;
                string thisWeekGrantPatent = HqDisplayQueriesHud.ThisWeekGrantPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantPatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantPatent : status + " · " + thisWeekGrantPatent;
                string lastWeekLaunderGrant = HqDisplayQueriesHud.LastWeekLaunderGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderGrant : status + " · " + lastWeekLaunderGrant;
                string thisWeekChipPower = HqDisplayQueriesHud.ThisWeekChipPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipPower : status + " · " + thisWeekChipPower;
                string lifetimeConstructionPower = HqDisplayQueriesHud.LifetimeConstructionPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionPower : status + " · " + lifetimeConstructionPower;
                string lastWeekHiringGrant = HqDisplayQueriesHud.LastWeekHiringGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringGrant : status + " · " + lastWeekHiringGrant;
                string thisWeekGreyRent = HqDisplayQueriesHud.ThisWeekGreyRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyRent : status + " · " + thisWeekGreyRent;
                string lastWeekSiteGrant = HqDisplayQueriesHud.LastWeekSiteGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteGrant : status + " · " + lastWeekSiteGrant;
                string thisWeekLaunderLand = HqDisplayQueriesHud.ThisWeekLaunderLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderLand : status + " · " + thisWeekLaunderLand;
                string lifetimeRentGrant = HqDisplayQueriesHud.LifetimeRentGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentGrant : status + " · " + lifetimeRentGrant;
                string lastWeekCapitalChip = HqDisplayQueriesHud.LastWeekCapitalChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalChip : status + " · " + lastWeekCapitalChip;
                string lifetimeGreyLand = HqDisplayQueriesHud.LifetimeGreyLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyLand))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyLand : status + " · " + lifetimeGreyLand;
                string thisWeekConstructionLand = HqDisplayQueriesHud.ThisWeekConstructionLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionLand : status + " · " + thisWeekConstructionLand;
                string lastWeekPatentRoom = HqDisplayQueriesHud.LastWeekPatentRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentRoom : status + " · " + lastWeekPatentRoom;
                string thisWeekGrantHardware = HqDisplayQueriesHud.ThisWeekGrantHardwareSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantHardware))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantHardware : status + " · " + thisWeekGrantHardware;
                string lastWeekGreyRent = HqDisplayQueriesHud.LastWeekGreyRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyRent : status + " · " + lastWeekGreyRent;
                string thisWeekChipLand = HqDisplayQueriesHud.ThisWeekChipLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipLand : status + " · " + thisWeekChipLand;
                string thisWeekSiteNet = HqDisplayQueriesHud.ThisWeekSiteNetSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteNet))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteNet : status + " · " + thisWeekSiteNet;
                string lastWeekGrantChip = HqDisplayQueriesHud.LastWeekGrantChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantChip : status + " · " + lastWeekGrantChip;
                string lifetimeLaunderGrant = HqDisplayQueriesHud.LifetimeLaunderGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderGrant : status + " · " + lifetimeLaunderGrant;
                string thisWeekRentTraining = HqDisplayQueriesHud.ThisWeekRentTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentTraining : status + " · " + thisWeekRentTraining;
                string lifetimeRoomChip = HqDisplayQueriesHud.LifetimeRoomChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRoomChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeRoomChip : status + " · " + lifetimeRoomChip;
                string lastWeekFinesChip = HqDisplayQueriesHud.LastWeekFinesChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesChip : status + " · " + lastWeekFinesChip;
                string thisWeekCapitalPower = HqDisplayQueriesHud.ThisWeekCapitalPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalPower : status + " · " + thisWeekCapitalPower;
                string lastWeekSiteLand = HqDisplayQueriesHud.LastWeekSiteLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteLand : status + " · " + lastWeekSiteLand;
                string lastWeekLaunderLand = HqDisplayQueriesHud.LastWeekLaunderLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderLand : status + " · " + lastWeekLaunderLand;
                string thisWeekGrantSite = HqDisplayQueriesHud.ThisWeekGrantSiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantSite))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantSite : status + " · " + thisWeekGrantSite;
                string lifetimeChipLand = HqDisplayQueriesHud.LifetimeChipLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipLand))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipLand : status + " · " + lifetimeChipLand;
                string lastWeekRentTraining = HqDisplayQueriesHud.LastWeekRentTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentTraining))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentTraining : status + " · " + lastWeekRentTraining;
                string lifetimeHiringPower = HqDisplayQueriesHud.LifetimeHiringPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHiringPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeHiringPower : status + " · " + lifetimeHiringPower;
                string thisWeekNetRoom = HqDisplayQueriesHud.ThisWeekNetRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetRoom))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetRoom : status + " · " + thisWeekNetRoom;
                string thisWeekLaunderRoom = HqDisplayQueriesHud.ThisWeekLaunderRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderRoom))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderRoom : status + " · " + thisWeekLaunderRoom;
                string lifetimeRentLand = HqDisplayQueriesHud.LifetimeRentLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentLand))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentLand : status + " · " + lifetimeRentLand;
                string lastWeekSalariesGrant = HqDisplayQueriesHud.LastWeekSalariesGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesGrant : status + " · " + lastWeekSalariesGrant;
                string thisWeekSiteChip = HqDisplayQueriesHud.ThisWeekSiteChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteChip : status + " · " + thisWeekSiteChip;
                string thisWeekCapitalConstruction = HqDisplayQueriesHud.ThisWeekCapitalConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalConstruction : status + " · " + thisWeekCapitalConstruction;
                string lifetimeGrantLand = HqDisplayQueriesHud.LifetimeGrantLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantLand))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantLand : status + " · " + lifetimeGrantLand;
                string lastWeekRoomGrant = HqDisplayQueriesHud.LastWeekRoomGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomGrant : status + " · " + lastWeekRoomGrant;
                string thisWeekPatentRent = HqDisplayQueriesHud.ThisWeekPatentRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentRent : status + " · " + thisWeekPatentRent;
                string thisWeekFinesRent = HqDisplayQueriesHud.ThisWeekFinesRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesRent : status + " · " + thisWeekFinesRent;
                string lifetimeChipRoom = HqDisplayQueriesHud.LifetimeChipRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipRoom))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipRoom : status + " · " + lifetimeChipRoom;
                string lastWeekConstructionFines = HqDisplayQueriesHud.LastWeekConstructionFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionFines : status + " · " + lastWeekConstructionFines;
                string lifetimeSalariesGrant = HqDisplayQueriesHud.LifetimeSalariesGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSalariesGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeSalariesGrant : status + " · " + lifetimeSalariesGrant;
                string lastWeekRentGrant = HqDisplayQueriesHud.LastWeekRentGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentGrant : status + " · " + lastWeekRentGrant;
                string thisWeekLandChip = HqDisplayQueriesHud.ThisWeekLandChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLandChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekLandChip : status + " · " + thisWeekLandChip;
                string lastWeekGreySite = HqDisplayQueriesHud.LastWeekGreySiteSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreySite))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreySite : status + " · " + lastWeekGreySite;
                string thisWeekHardwareLand = HqDisplayQueriesHud.ThisWeekHardwareLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareLand : status + " · " + thisWeekHardwareLand;
                string lastWeekFinesLand = HqDisplayQueriesHud.LastWeekFinesLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesLand : status + " · " + lastWeekFinesLand;
                string lastWeekCapitalGrant = HqDisplayQueriesHud.LastWeekCapitalGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekCapitalGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekCapitalGrant : status + " · " + lastWeekCapitalGrant;
                string thisWeekRoomGrant = HqDisplayQueriesHud.ThisWeekRoomGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRoomGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekRoomGrant : status + " · " + thisWeekRoomGrant;
                string thisWeekConstructionRent = HqDisplayQueriesHud.ThisWeekConstructionRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionRent : status + " · " + thisWeekConstructionRent;
                string thisWeekSalariesLand = HqDisplayQueriesHud.ThisWeekSalariesLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesLand : status + " · " + thisWeekSalariesLand;
                string lastWeekNetRoom = HqDisplayQueriesHud.LastWeekNetRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetRoom))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetRoom : status + " · " + lastWeekNetRoom;
                string lifetimeGrantChip = HqDisplayQueriesHud.LifetimeGrantChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantChip : status + " · " + lifetimeGrantChip;
                string thisWeekGrantLaunder = HqDisplayQueriesHud.ThisWeekGrantLaunderSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantLaunder))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantLaunder : status + " · " + thisWeekGrantLaunder;
                string thisWeekSiteTraining = HqDisplayQueriesHud.ThisWeekSiteTrainingSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteTraining))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteTraining : status + " · " + thisWeekSiteTraining;
                string lifetimeRentChip = HqDisplayQueriesHud.LifetimeRentChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentChip : status + " · " + lifetimeRentChip;
                string lifetimeGreyGrant = HqDisplayQueriesHud.LifetimeGreyGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyGrant : status + " · " + lifetimeGreyGrant;
                string lastWeekChipRent = HqDisplayQueriesHud.LastWeekChipRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipRent : status + " · " + lastWeekChipRent;
                string thisWeekGreyPatent = HqDisplayQueriesHud.ThisWeekGreyPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyPatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyPatent : status + " · " + thisWeekGreyPatent;
                string lifetimeFinesGrant = HqDisplayQueriesHud.LifetimeFinesGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesGrant : status + " · " + lifetimeFinesGrant;
                string lastWeekLandConstruction = HqDisplayQueriesHud.LastWeekLandConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandConstruction))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandConstruction : status + " · " + lastWeekLandConstruction;
                string thisWeekRentPower = HqDisplayQueriesHud.ThisWeekRentPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentPower : status + " · " + thisWeekRentPower;
                string lastWeekGreyFines = HqDisplayQueriesHud.LastWeekGreyFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyFines : status + " · " + lastWeekGreyFines;
                string lifetimePowerLand = HqDisplayQueriesHud.LifetimePowerLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerLand))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerLand : status + " · " + lifetimePowerLand;
                string lastWeekHardwareGrant = HqDisplayQueriesHud.LastWeekHardwareGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareGrant : status + " · " + lastWeekHardwareGrant;
                string lastWeekSiteRent = HqDisplayQueriesHud.LastWeekSiteRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteRent : status + " · " + lastWeekSiteRent;
                string lifetimeConstructionChip = HqDisplayQueriesHud.LifetimeConstructionChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionChip : status + " · " + lifetimeConstructionChip;
                string lastWeekGrantPower = HqDisplayQueriesHud.LastWeekGrantPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantPower : status + " · " + lastWeekGrantPower;
                string thisWeekLandGrant = HqDisplayQueriesHud.ThisWeekLandGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLandGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekLandGrant : status + " · " + thisWeekLandGrant;
                string thisWeekSalariesRent = HqDisplayQueriesHud.ThisWeekSalariesRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesRent : status + " · " + thisWeekSalariesRent;
                string lifetimeLaunderLand = HqDisplayQueriesHud.LifetimeLaunderLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderLand))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderLand : status + " · " + lifetimeLaunderLand;
                string lastWeekSiteFines = HqDisplayQueriesHud.LastWeekSiteFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteFines))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteFines : status + " · " + lastWeekSiteFines;
                string thisWeekCapitalRent = HqDisplayQueriesHud.ThisWeekCapitalRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekCapitalRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekCapitalRent : status + " · " + thisWeekCapitalRent;
                string lifetimeGrantRoom = HqDisplayQueriesHud.LifetimeGrantRoomSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantRoom))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantRoom : status + " · " + lifetimeGrantRoom;
                string lastWeekRentChip = HqDisplayQueriesHud.LastWeekRentChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentChip : status + " · " + lastWeekRentChip;
                string lifetimeLaunderFines = HqDisplayQueriesHud.LifetimeLaunderFinesSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderFines))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderFines : status + " · " + lifetimeLaunderFines;
                string thisWeekLaunderConstruction = HqDisplayQueriesHud.ThisWeekLaunderConstructionSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderConstruction))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderConstruction : status + " · " + thisWeekLaunderConstruction;
                string thisWeekFinesSpendIncome = HqDisplayQueriesHud.ThisWeekFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesSpendIncome : status + " · " + thisWeekFinesSpendIncome;
                string lastWeekFinesSpendIncome = HqDisplayQueriesHud.LastWeekFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesSpendIncome : status + " · " + lastWeekFinesSpendIncome;
                string lifetimeFinesSpendIncome = HqDisplayQueriesHud.LifetimeFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesSpendIncome : status + " · " + lifetimeFinesSpendIncome;
                string thisWeekGrantSpendIncome = HqDisplayQueriesHud.ThisWeekGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantSpendIncome : status + " · " + thisWeekGrantSpendIncome;
                string lastWeekGrantSpendIncome = HqDisplayQueriesHud.LastWeekGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantSpendIncome : status + " · " + lastWeekGrantSpendIncome;
                string lifetimeGrantSpendIncome = HqDisplayQueriesHud.LifetimeGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantSpendIncome : status + " · " + lifetimeGrantSpendIncome;
                string thisWeekChipSpendIncome = HqDisplayQueriesHud.ThisWeekChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipSpendIncome : status + " · " + thisWeekChipSpendIncome;
                string lastWeekChipSpendIncome = HqDisplayQueriesHud.LastWeekChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipSpendIncome : status + " · " + lastWeekChipSpendIncome;
                string lifetimeChipSpendIncome = HqDisplayQueriesHud.LifetimeChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipSpendIncome : status + " · " + lifetimeChipSpendIncome;
                string thisWeekDevSpendIncome = HqDisplayQueriesHud.ThisWeekDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekDevSpendIncome : status + " · " + thisWeekDevSpendIncome;
                string lastWeekDevSpendIncome = HqDisplayQueriesHud.LastWeekDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekDevSpendIncome : status + " · " + lastWeekDevSpendIncome;
                string lifetimeDevSpendIncome = HqDisplayQueriesHud.LifetimeDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeDevSpendIncome : status + " · " + lifetimeDevSpendIncome;
                string thisWeekGpuSpendIncome = HqDisplayQueriesHud.ThisWeekGpuSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuSpendIncome : status + " · " + thisWeekGpuSpendIncome;
                string lastWeekGpuSpendIncome = HqDisplayQueriesHud.LastWeekGpuSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuSpendIncome : status + " · " + lastWeekGpuSpendIncome;
                string lifetimeGpuSpendIncome = HqDisplayQueriesHud.LifetimeGpuSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGpuSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGpuSpendIncome : status + " · " + lifetimeGpuSpendIncome;
                string thisWeekRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueSpendIncome : status + " · " + thisWeekRevenueSpendIncome;
                string lastWeekRevenueSpendIncome = HqDisplayQueriesHud.LastWeekRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueSpendIncome : status + " · " + lastWeekRevenueSpendIncome;
                string lifetimeRevenueSpendIncome = HqDisplayQueriesHud.LifetimeRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueSpendIncome : status + " · " + lifetimeRevenueSpendIncome;
                string thisWeekGrantFinesSpendIncome = HqDisplayQueriesHud.ThisWeekGrantFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantFinesSpendIncome : status + " · " + thisWeekGrantFinesSpendIncome;
                string lastWeekGrantFinesSpendIncome = HqDisplayQueriesHud.LastWeekGrantFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantFinesSpendIncome : status + " · " + lastWeekGrantFinesSpendIncome;
                string lifetimeGrantFinesSpendIncome = HqDisplayQueriesHud.LifetimeGrantFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantFinesSpendIncome : status + " · " + lifetimeGrantFinesSpendIncome;
                string thisWeekChipDevSpendIncome = HqDisplayQueriesHud.ThisWeekChipDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipDevSpendIncome : status + " · " + thisWeekChipDevSpendIncome;
                string lastWeekChipDevSpendIncome = HqDisplayQueriesHud.LastWeekChipDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipDevSpendIncome : status + " · " + lastWeekChipDevSpendIncome;
                string lifetimeChipDevSpendIncome = HqDisplayQueriesHud.LifetimeChipDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipDevSpendIncome : status + " · " + lifetimeChipDevSpendIncome;
                string thisWeekHardwareSiteSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareSiteSpendIncome : status + " · " + thisWeekHardwareSiteSpendIncome;
                string lastWeekHardwareSiteSpendIncome = HqDisplayQueriesHud.LastWeekHardwareSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareSiteSpendIncome : status + " · " + lastWeekHardwareSiteSpendIncome;
                string lifetimeHardwareSiteSpendIncome = HqDisplayQueriesHud.LifetimeHardwareSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareSiteSpendIncome : status + " · " + lifetimeHardwareSiteSpendIncome;
                string thisWeekLandRoomSpendIncome = HqDisplayQueriesHud.ThisWeekLandRoomSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLandRoomSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekLandRoomSpendIncome : status + " · " + thisWeekLandRoomSpendIncome;
                string lastWeekLandRoomSpendIncome = HqDisplayQueriesHud.LastWeekLandRoomSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandRoomSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandRoomSpendIncome : status + " · " + lastWeekLandRoomSpendIncome;
                string lifetimeLandRoomSpendIncome = HqDisplayQueriesHud.LifetimeLandRoomSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLandRoomSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeLandRoomSpendIncome : status + " · " + lifetimeLandRoomSpendIncome;
                string thisWeekRentTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekRentTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentTrainingSpendIncome : status + " · " + thisWeekRentTrainingSpendIncome;
                string lastWeekRentTrainingSpendIncome = HqDisplayQueriesHud.LastWeekRentTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentTrainingSpendIncome : status + " · " + lastWeekRentTrainingSpendIncome;
                string lifetimeRentTrainingSpendIncome = HqDisplayQueriesHud.LifetimeRentTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentTrainingSpendIncome : status + " · " + lifetimeRentTrainingSpendIncome;
                string thisWeekPowerConstructionSpendIncome = HqDisplayQueriesHud.ThisWeekPowerConstructionSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerConstructionSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerConstructionSpendIncome : status + " · " + thisWeekPowerConstructionSpendIncome;
                string lastWeekPowerConstructionSpendIncome = HqDisplayQueriesHud.LastWeekPowerConstructionSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerConstructionSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerConstructionSpendIncome : status + " · " + lastWeekPowerConstructionSpendIncome;
                string lifetimePowerConstructionSpendIncome = HqDisplayQueriesHud.LifetimePowerConstructionSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerConstructionSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerConstructionSpendIncome : status + " · " + lifetimePowerConstructionSpendIncome;
                string thisWeekSalariesHiringSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesHiringSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesHiringSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesHiringSpendIncome : status + " · " + thisWeekSalariesHiringSpendIncome;
                string lastWeekSalariesHiringSpendIncome = HqDisplayQueriesHud.LastWeekSalariesHiringSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesHiringSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesHiringSpendIncome : status + " · " + lastWeekSalariesHiringSpendIncome;
                string lifetimeSalariesHiringSpendIncome = HqDisplayQueriesHud.LifetimeSalariesHiringSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSalariesHiringSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeSalariesHiringSpendIncome : status + " · " + lifetimeSalariesHiringSpendIncome;
                string thisWeekLaunderCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekLaunderCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderCapitalSpendIncome : status + " · " + thisWeekLaunderCapitalSpendIncome;
                string lastWeekLaunderCapitalSpendIncome = HqDisplayQueriesHud.LastWeekLaunderCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderCapitalSpendIncome : status + " · " + lastWeekLaunderCapitalSpendIncome;
                string lifetimeLaunderCapitalSpendIncome = HqDisplayQueriesHud.LifetimeLaunderCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderCapitalSpendIncome : status + " · " + lifetimeLaunderCapitalSpendIncome;
                string thisWeekPatentUpkeepSpendIncome = HqDisplayQueriesHud.ThisWeekPatentUpkeepSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentUpkeepSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentUpkeepSpendIncome : status + " · " + thisWeekPatentUpkeepSpendIncome;
                string lastWeekPatentUpkeepSpendIncome = HqDisplayQueriesHud.LastWeekPatentUpkeepSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentUpkeepSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentUpkeepSpendIncome : status + " · " + lastWeekPatentUpkeepSpendIncome;
                string lifetimePatentUpkeepSpendIncome = HqDisplayQueriesHud.LifetimePatentUpkeepSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentUpkeepSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentUpkeepSpendIncome : status + " · " + lifetimePatentUpkeepSpendIncome;
                string thisWeekNetGreySpendIncome = HqDisplayQueriesHud.ThisWeekNetGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetGreySpendIncome : status + " · " + thisWeekNetGreySpendIncome;
                string lastWeekNetGreySpendIncome = HqDisplayQueriesHud.LastWeekNetGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetGreySpendIncome : status + " · " + lastWeekNetGreySpendIncome;
                string lifetimeNetGreySpendIncome = HqDisplayQueriesHud.LifetimeNetGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetGreySpendIncome : status + " · " + lifetimeNetGreySpendIncome;
                string thisWeekGrantChipSpendIncome = HqDisplayQueriesHud.ThisWeekGrantChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantChipSpendIncome : status + " · " + thisWeekGrantChipSpendIncome;
                string lastWeekGrantChipSpendIncome = HqDisplayQueriesHud.LastWeekGrantChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantChipSpendIncome : status + " · " + lastWeekGrantChipSpendIncome;
                string lifetimeGrantChipSpendIncome = HqDisplayQueriesHud.LifetimeGrantChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantChipSpendIncome : status + " · " + lifetimeGrantChipSpendIncome;
                string thisWeekFinesDevSpendIncome = HqDisplayQueriesHud.ThisWeekFinesDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesDevSpendIncome : status + " · " + thisWeekFinesDevSpendIncome;
                string lastWeekFinesDevSpendIncome = HqDisplayQueriesHud.LastWeekFinesDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesDevSpendIncome : status + " · " + lastWeekFinesDevSpendIncome;
                string lifetimeFinesDevSpendIncome = HqDisplayQueriesHud.LifetimeFinesDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesDevSpendIncome : status + " · " + lifetimeFinesDevSpendIncome;
                string thisWeekHardwareLandSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareLandSpendIncome : status + " · " + thisWeekHardwareLandSpendIncome;
                string lastWeekHardwareLandSpendIncome = HqDisplayQueriesHud.LastWeekHardwareLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareLandSpendIncome : status + " · " + lastWeekHardwareLandSpendIncome;
                string lifetimeHardwareLandSpendIncome = HqDisplayQueriesHud.LifetimeHardwareLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareLandSpendIncome : status + " · " + lifetimeHardwareLandSpendIncome;
                string thisWeekSiteCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekSiteCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteCapitalSpendIncome : status + " · " + thisWeekSiteCapitalSpendIncome;
                string lastWeekSiteCapitalSpendIncome = HqDisplayQueriesHud.LastWeekSiteCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteCapitalSpendIncome : status + " · " + lastWeekSiteCapitalSpendIncome;
                string lifetimeSiteCapitalSpendIncome = HqDisplayQueriesHud.LifetimeSiteCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSiteCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeSiteCapitalSpendIncome : status + " · " + lifetimeSiteCapitalSpendIncome;
                string thisWeekShadyLaunderSpendIncome = HqDisplayQueriesHud.ThisWeekShadyLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekShadyLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekShadyLaunderSpendIncome : status + " · " + thisWeekShadyLaunderSpendIncome;
                string lastWeekShadyLaunderSpendIncome = HqDisplayQueriesHud.LastWeekShadyLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekShadyLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekShadyLaunderSpendIncome : status + " · " + lastWeekShadyLaunderSpendIncome;
                string lifetimeShadyLaunderSpendIncome = HqDisplayQueriesHud.LifetimeShadyLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeShadyLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeShadyLaunderSpendIncome : status + " · " + lifetimeShadyLaunderSpendIncome;
                string thisWeekChipRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekChipRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipRevenueSpendIncome : status + " · " + thisWeekChipRevenueSpendIncome;
                string lastWeekChipRevenueSpendIncome = HqDisplayQueriesHud.LastWeekChipRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipRevenueSpendIncome : status + " · " + lastWeekChipRevenueSpendIncome;
                string lifetimeChipRevenueSpendIncome = HqDisplayQueriesHud.LifetimeChipRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipRevenueSpendIncome : status + " · " + lifetimeChipRevenueSpendIncome;
                string thisWeekPatentGrantSpendIncome = HqDisplayQueriesHud.ThisWeekPatentGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentGrantSpendIncome : status + " · " + thisWeekPatentGrantSpendIncome;
                string lastWeekPatentGrantSpendIncome = HqDisplayQueriesHud.LastWeekPatentGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentGrantSpendIncome : status + " · " + lastWeekPatentGrantSpendIncome;
                string lifetimePatentGrantSpendIncome = HqDisplayQueriesHud.LifetimePatentGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentGrantSpendIncome : status + " · " + lifetimePatentGrantSpendIncome;
                string thisWeekGreyFinesSpendIncome = HqDisplayQueriesHud.ThisWeekGreyFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyFinesSpendIncome : status + " · " + thisWeekGreyFinesSpendIncome;
                string lastWeekGreyFinesSpendIncome = HqDisplayQueriesHud.LastWeekGreyFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyFinesSpendIncome : status + " · " + lastWeekGreyFinesSpendIncome;
                string lifetimeGreyFinesSpendIncome = HqDisplayQueriesHud.LifetimeGreyFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyFinesSpendIncome : status + " · " + lifetimeGreyFinesSpendIncome;
                string thisWeekNetTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekNetTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetTrainingSpendIncome : status + " · " + thisWeekNetTrainingSpendIncome;
                string lastWeekNetTrainingSpendIncome = HqDisplayQueriesHud.LastWeekNetTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetTrainingSpendIncome : status + " · " + lastWeekNetTrainingSpendIncome;
                string lifetimeNetTrainingSpendIncome = HqDisplayQueriesHud.LifetimeNetTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetTrainingSpendIncome : status + " · " + lifetimeNetTrainingSpendIncome;
                string thisWeekGpuPowerSpendIncome = HqDisplayQueriesHud.ThisWeekGpuPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuPowerSpendIncome : status + " · " + thisWeekGpuPowerSpendIncome;
                string lastWeekGpuPowerSpendIncome = HqDisplayQueriesHud.LastWeekGpuPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuPowerSpendIncome : status + " · " + lastWeekGpuPowerSpendIncome;
                string lifetimeGpuPowerSpendIncome = HqDisplayQueriesHud.LifetimeGpuPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGpuPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGpuPowerSpendIncome : status + " · " + lifetimeGpuPowerSpendIncome;
                string thisWeekRevenueGrantsSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueGrantsSpendIncome : status + " · " + thisWeekRevenueGrantsSpendIncome;
                string lastWeekRevenueGrantsSpendIncome = HqDisplayQueriesHud.LastWeekRevenueGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueGrantsSpendIncome : status + " · " + lastWeekRevenueGrantsSpendIncome;
                string lifetimeRevenueGrantsSpendIncome = HqDisplayQueriesHud.LifetimeRevenueGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueGrantsSpendIncome : status + " · " + lifetimeRevenueGrantsSpendIncome;
                string thisWeekConstructionSiteSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionSiteSpendIncome : status + " · " + thisWeekConstructionSiteSpendIncome;
                string lastWeekConstructionSiteSpendIncome = HqDisplayQueriesHud.LastWeekConstructionSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionSiteSpendIncome : status + " · " + lastWeekConstructionSiteSpendIncome;
                string lifetimeConstructionSiteSpendIncome = HqDisplayQueriesHud.LifetimeConstructionSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionSiteSpendIncome : status + " · " + lifetimeConstructionSiteSpendIncome;
                string thisWeekGrantDevSpendIncome = HqDisplayQueriesHud.ThisWeekGrantDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantDevSpendIncome : status + " · " + thisWeekGrantDevSpendIncome;
                string lastWeekGrantDevSpendIncome = HqDisplayQueriesHud.LastWeekGrantDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantDevSpendIncome : status + " · " + lastWeekGrantDevSpendIncome;
                string lifetimeGrantDevSpendIncome = HqDisplayQueriesHud.LifetimeGrantDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantDevSpendIncome : status + " · " + lifetimeGrantDevSpendIncome;
                string thisWeekChipFinesSpendIncome = HqDisplayQueriesHud.ThisWeekChipFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipFinesSpendIncome : status + " · " + thisWeekChipFinesSpendIncome;
                string lastWeekChipFinesSpendIncome = HqDisplayQueriesHud.LastWeekChipFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipFinesSpendIncome : status + " · " + lastWeekChipFinesSpendIncome;
                string lifetimeChipFinesSpendIncome = HqDisplayQueriesHud.LifetimeChipFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipFinesSpendIncome : status + " · " + lifetimeChipFinesSpendIncome;
                string thisWeekShadyGreySpendIncome = HqDisplayQueriesHud.ThisWeekShadyGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekShadyGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekShadyGreySpendIncome : status + " · " + thisWeekShadyGreySpendIncome;
                string lastWeekShadyGreySpendIncome = HqDisplayQueriesHud.LastWeekShadyGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekShadyGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekShadyGreySpendIncome : status + " · " + lastWeekShadyGreySpendIncome;
                string lifetimeShadyGreySpendIncome = HqDisplayQueriesHud.LifetimeShadyGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeShadyGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeShadyGreySpendIncome : status + " · " + lifetimeShadyGreySpendIncome;
                string thisWeekHiringPatentSpendIncome = HqDisplayQueriesHud.ThisWeekHiringPatentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHiringPatentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHiringPatentSpendIncome : status + " · " + thisWeekHiringPatentSpendIncome;
                string lastWeekHiringPatentSpendIncome = HqDisplayQueriesHud.LastWeekHiringPatentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringPatentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringPatentSpendIncome : status + " · " + lastWeekHiringPatentSpendIncome;
                string lifetimeHiringPatentSpendIncome = HqDisplayQueriesHud.LifetimeHiringPatentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHiringPatentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHiringPatentSpendIncome : status + " · " + lifetimeHiringPatentSpendIncome;
                string thisWeekNetGrantSpendIncome = HqDisplayQueriesHud.ThisWeekNetGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetGrantSpendIncome : status + " · " + thisWeekNetGrantSpendIncome;
                string lastWeekNetGrantSpendIncome = HqDisplayQueriesHud.LastWeekNetGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetGrantSpendIncome : status + " · " + lastWeekNetGrantSpendIncome;
                string lifetimeNetGrantSpendIncome = HqDisplayQueriesHud.LifetimeNetGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetGrantSpendIncome : status + " · " + lifetimeNetGrantSpendIncome;
                string thisWeekGreyGrantSpendIncome = HqDisplayQueriesHud.ThisWeekGreyGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyGrantSpendIncome : status + " · " + thisWeekGreyGrantSpendIncome;
                string lastWeekGreyGrantSpendIncome = HqDisplayQueriesHud.LastWeekGreyGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyGrantSpendIncome : status + " · " + lastWeekGreyGrantSpendIncome;
                string lifetimeGreyGrantSpendIncome = HqDisplayQueriesHud.LifetimeGreyGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyGrantSpendIncome : status + " · " + lifetimeGreyGrantSpendIncome;
                string thisWeekConstructionCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionCapitalSpendIncome : status + " · " + thisWeekConstructionCapitalSpendIncome;
                string lastWeekConstructionCapitalSpendIncome = HqDisplayQueriesHud.LastWeekConstructionCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionCapitalSpendIncome : status + " · " + lastWeekConstructionCapitalSpendIncome;
                string lifetimeConstructionCapitalSpendIncome = HqDisplayQueriesHud.LifetimeConstructionCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionCapitalSpendIncome : status + " · " + lifetimeConstructionCapitalSpendIncome;
                string thisWeekRoomTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekRoomTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRoomTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRoomTrainingSpendIncome : status + " · " + thisWeekRoomTrainingSpendIncome;
                string lastWeekRoomTrainingSpendIncome = HqDisplayQueriesHud.LastWeekRoomTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomTrainingSpendIncome : status + " · " + lastWeekRoomTrainingSpendIncome;
                string lifetimeRoomTrainingSpendIncome = HqDisplayQueriesHud.LifetimeRoomTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRoomTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRoomTrainingSpendIncome : status + " · " + lifetimeRoomTrainingSpendIncome;
                string thisWeekGpuRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekGpuRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuRevenueSpendIncome : status + " · " + thisWeekGpuRevenueSpendIncome;
                string lastWeekGpuRevenueSpendIncome = HqDisplayQueriesHud.LastWeekGpuRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuRevenueSpendIncome : status + " · " + lastWeekGpuRevenueSpendIncome;
                string lifetimeGpuRevenueSpendIncome = HqDisplayQueriesHud.LifetimeGpuRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGpuRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGpuRevenueSpendIncome : status + " · " + lifetimeGpuRevenueSpendIncome;
                string thisWeekChipGrantSpendIncome = HqDisplayQueriesHud.ThisWeekChipGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipGrantSpendIncome : status + " · " + thisWeekChipGrantSpendIncome;
                string lastWeekChipGrantSpendIncome = HqDisplayQueriesHud.LastWeekChipGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipGrantSpendIncome : status + " · " + lastWeekChipGrantSpendIncome;
                string lifetimeChipGrantSpendIncome = HqDisplayQueriesHud.LifetimeChipGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipGrantSpendIncome : status + " · " + lifetimeChipGrantSpendIncome;
                string thisWeekLaunderGreySpendIncome = HqDisplayQueriesHud.ThisWeekLaunderGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLaunderGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekLaunderGreySpendIncome : status + " · " + thisWeekLaunderGreySpendIncome;
                string lastWeekLaunderGreySpendIncome = HqDisplayQueriesHud.LastWeekLaunderGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderGreySpendIncome : status + " · " + lastWeekLaunderGreySpendIncome;
                string lifetimeLaunderGreySpendIncome = HqDisplayQueriesHud.LifetimeLaunderGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLaunderGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeLaunderGreySpendIncome : status + " · " + lifetimeLaunderGreySpendIncome;
                string thisWeekPatentFinesSpendIncome = HqDisplayQueriesHud.ThisWeekPatentFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentFinesSpendIncome : status + " · " + thisWeekPatentFinesSpendIncome;
                string lastWeekPatentFinesSpendIncome = HqDisplayQueriesHud.LastWeekPatentFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentFinesSpendIncome : status + " · " + lastWeekPatentFinesSpendIncome;
                string lifetimePatentFinesSpendIncome = HqDisplayQueriesHud.LifetimePatentFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentFinesSpendIncome : status + " · " + lifetimePatentFinesSpendIncome;
                string thisWeekHardwareGrantSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareGrantSpendIncome : status + " · " + thisWeekHardwareGrantSpendIncome;
                string lastWeekHardwareGrantSpendIncome = HqDisplayQueriesHud.LastWeekHardwareGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareGrantSpendIncome : status + " · " + lastWeekHardwareGrantSpendIncome;
                string lifetimeHardwareGrantSpendIncome = HqDisplayQueriesHud.LifetimeHardwareGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareGrantSpendIncome : status + " · " + lifetimeHardwareGrantSpendIncome;
                string thisWeekLandCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekLandCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLandCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekLandCapitalSpendIncome : status + " · " + thisWeekLandCapitalSpendIncome;
                string lastWeekLandCapitalSpendIncome = HqDisplayQueriesHud.LastWeekLandCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLandCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekLandCapitalSpendIncome : status + " · " + lastWeekLandCapitalSpendIncome;
                string lifetimeLandCapitalSpendIncome = HqDisplayQueriesHud.LifetimeLandCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLandCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeLandCapitalSpendIncome : status + " · " + lifetimeLandCapitalSpendIncome;
                string thisWeekRentPowerSpendIncome = HqDisplayQueriesHud.ThisWeekRentPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentPowerSpendIncome : status + " · " + thisWeekRentPowerSpendIncome;
                string lastWeekRentPowerSpendIncome = HqDisplayQueriesHud.LastWeekRentPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentPowerSpendIncome : status + " · " + lastWeekRentPowerSpendIncome;
                string lifetimeRentPowerSpendIncome = HqDisplayQueriesHud.LifetimeRentPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentPowerSpendIncome : status + " · " + lifetimeRentPowerSpendIncome;
                string thisWeekSalariesConstructionSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesConstructionSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesConstructionSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesConstructionSpendIncome : status + " · " + thisWeekSalariesConstructionSpendIncome;
                string lastWeekSalariesConstructionSpendIncome = HqDisplayQueriesHud.LastWeekSalariesConstructionSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesConstructionSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesConstructionSpendIncome : status + " · " + lastWeekSalariesConstructionSpendIncome;
                string lifetimeSalariesConstructionSpendIncome = HqDisplayQueriesHud.LifetimeSalariesConstructionSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSalariesConstructionSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeSalariesConstructionSpendIncome : status + " · " + lifetimeSalariesConstructionSpendIncome;
                string thisWeekNetChipSpendIncome = HqDisplayQueriesHud.ThisWeekNetChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetChipSpendIncome : status + " · " + thisWeekNetChipSpendIncome;
                string lastWeekNetChipSpendIncome = HqDisplayQueriesHud.LastWeekNetChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetChipSpendIncome : status + " · " + lastWeekNetChipSpendIncome;
                string lifetimeNetChipSpendIncome = HqDisplayQueriesHud.LifetimeNetChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetChipSpendIncome : status + " · " + lifetimeNetChipSpendIncome;
                string thisWeekGreyDevSpendIncome = HqDisplayQueriesHud.ThisWeekGreyDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyDevSpendIncome : status + " · " + thisWeekGreyDevSpendIncome;
                string lastWeekGreyDevSpendIncome = HqDisplayQueriesHud.LastWeekGreyDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyDevSpendIncome : status + " · " + lastWeekGreyDevSpendIncome;
                string lifetimeGreyDevSpendIncome = HqDisplayQueriesHud.LifetimeGreyDevSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyDevSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyDevSpendIncome : status + " · " + lifetimeGreyDevSpendIncome;
                string thisWeekShadyGrantSpendIncome = HqDisplayQueriesHud.ThisWeekShadyGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekShadyGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekShadyGrantSpendIncome : status + " · " + thisWeekShadyGrantSpendIncome;
                string lastWeekShadyGrantSpendIncome = HqDisplayQueriesHud.LastWeekShadyGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekShadyGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekShadyGrantSpendIncome : status + " · " + lastWeekShadyGrantSpendIncome;
                string lifetimeShadyGrantSpendIncome = HqDisplayQueriesHud.LifetimeShadyGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeShadyGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeShadyGrantSpendIncome : status + " · " + lifetimeShadyGrantSpendIncome;
                string thisWeekPatentChipSpendIncome = HqDisplayQueriesHud.ThisWeekPatentChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentChipSpendIncome : status + " · " + thisWeekPatentChipSpendIncome;
                string lastWeekPatentChipSpendIncome = HqDisplayQueriesHud.LastWeekPatentChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentChipSpendIncome : status + " · " + lastWeekPatentChipSpendIncome;
                string lifetimePatentChipSpendIncome = HqDisplayQueriesHud.LifetimePatentChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentChipSpendIncome : status + " · " + lifetimePatentChipSpendIncome;
                string thisWeekFinesGrantSpendIncome = HqDisplayQueriesHud.ThisWeekFinesGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesGrantSpendIncome : status + " · " + thisWeekFinesGrantSpendIncome;
                string lastWeekFinesGrantSpendIncome = HqDisplayQueriesHud.LastWeekFinesGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesGrantSpendIncome : status + " · " + lastWeekFinesGrantSpendIncome;
                string lifetimeFinesGrantSpendIncome = HqDisplayQueriesHud.LifetimeFinesGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesGrantSpendIncome : status + " · " + lifetimeFinesGrantSpendIncome;
                string thisWeekGrantRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekGrantRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantRevenueSpendIncome : status + " · " + thisWeekGrantRevenueSpendIncome;
                string lastWeekGrantRevenueSpendIncome = HqDisplayQueriesHud.LastWeekGrantRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantRevenueSpendIncome : status + " · " + lastWeekGrantRevenueSpendIncome;
                string lifetimeGrantRevenueSpendIncome = HqDisplayQueriesHud.LifetimeGrantRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantRevenueSpendIncome : status + " · " + lifetimeGrantRevenueSpendIncome;
                string thisWeekNetFinesSpendIncome = HqDisplayQueriesHud.ThisWeekNetFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekNetFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekNetFinesSpendIncome : status + " · " + thisWeekNetFinesSpendIncome;
                string lastWeekNetFinesSpendIncome = HqDisplayQueriesHud.LastWeekNetFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekNetFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekNetFinesSpendIncome : status + " · " + lastWeekNetFinesSpendIncome;
                string lifetimeNetFinesSpendIncome = HqDisplayQueriesHud.LifetimeNetFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeNetFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeNetFinesSpendIncome : status + " · " + lifetimeNetFinesSpendIncome;
                string thisWeekGreyChipSpendIncome = HqDisplayQueriesHud.ThisWeekGreyChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyChipSpendIncome : status + " · " + thisWeekGreyChipSpendIncome;
                string lastWeekGreyChipSpendIncome = HqDisplayQueriesHud.LastWeekGreyChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyChipSpendIncome : status + " · " + lastWeekGreyChipSpendIncome;
                string lifetimeGreyChipSpendIncome = HqDisplayQueriesHud.LifetimeGreyChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyChipSpendIncome : status + " · " + lifetimeGreyChipSpendIncome;
                string thisWeekGrantPowerSpendIncome = HqDisplayQueriesHud.ThisWeekGrantPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantPowerSpendIncome : status + " · " + thisWeekGrantPowerSpendIncome;
                string lastWeekGrantPowerSpendIncome = HqDisplayQueriesHud.LastWeekGrantPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantPowerSpendIncome : status + " · " + lastWeekGrantPowerSpendIncome;
                string lifetimeGrantPowerSpendIncome = HqDisplayQueriesHud.LifetimeGrantPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantPowerSpendIncome : status + " · " + lifetimeGrantPowerSpendIncome;
                string thisWeekDevFinesSpendIncome = HqDisplayQueriesHud.ThisWeekDevFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekDevFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekDevFinesSpendIncome : status + " · " + thisWeekDevFinesSpendIncome;
                string lastWeekDevFinesSpendIncome = HqDisplayQueriesHud.LastWeekDevFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekDevFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekDevFinesSpendIncome : status + " · " + lastWeekDevFinesSpendIncome;
                string lifetimeDevFinesSpendIncome = HqDisplayQueriesHud.LifetimeDevFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeDevFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeDevFinesSpendIncome : status + " · " + lifetimeDevFinesSpendIncome;
                string thisWeekHardwareCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwareCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwareCapitalSpendIncome : status + " · " + thisWeekHardwareCapitalSpendIncome;
                string lastWeekHardwareCapitalSpendIncome = HqDisplayQueriesHud.LastWeekHardwareCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwareCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwareCapitalSpendIncome : status + " · " + lastWeekHardwareCapitalSpendIncome;
                string lifetimeHardwareCapitalSpendIncome = HqDisplayQueriesHud.LifetimeHardwareCapitalSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwareCapitalSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwareCapitalSpendIncome : status + " · " + lifetimeHardwareCapitalSpendIncome;
                string thisWeekSiteGrantSpendIncome = HqDisplayQueriesHud.ThisWeekSiteGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSiteGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekSiteGrantSpendIncome : status + " · " + thisWeekSiteGrantSpendIncome;
                string lastWeekSiteGrantSpendIncome = HqDisplayQueriesHud.LastWeekSiteGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSiteGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekSiteGrantSpendIncome : status + " · " + lastWeekSiteGrantSpendIncome;
                string lifetimeSiteGrantSpendIncome = HqDisplayQueriesHud.LifetimeSiteGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSiteGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeSiteGrantSpendIncome : status + " · " + lifetimeSiteGrantSpendIncome;
                string thisWeekTrainingGrantSpendIncome = HqDisplayQueriesHud.ThisWeekTrainingGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingGrantSpendIncome : status + " · " + thisWeekTrainingGrantSpendIncome;
                string lastWeekTrainingGrantSpendIncome = HqDisplayQueriesHud.LastWeekTrainingGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingGrantSpendIncome : status + " · " + lastWeekTrainingGrantSpendIncome;
                string lifetimeTrainingGrantSpendIncome = HqDisplayQueriesHud.LifetimeTrainingGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingGrantSpendIncome : status + " · " + lifetimeTrainingGrantSpendIncome;
                string thisWeekRoomGrantSpendIncome = HqDisplayQueriesHud.ThisWeekRoomGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRoomGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRoomGrantSpendIncome : status + " · " + thisWeekRoomGrantSpendIncome;
                string lastWeekRoomGrantSpendIncome = HqDisplayQueriesHud.LastWeekRoomGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRoomGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRoomGrantSpendIncome : status + " · " + lastWeekRoomGrantSpendIncome;
                string lifetimeRoomGrantSpendIncome = HqDisplayQueriesHud.LifetimeRoomGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRoomGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRoomGrantSpendIncome : status + " · " + lifetimeRoomGrantSpendIncome;
                string thisWeekGpuGrantSpendIncome = HqDisplayQueriesHud.ThisWeekGpuGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuGrantSpendIncome : status + " · " + thisWeekGpuGrantSpendIncome;
                string lastWeekGpuGrantSpendIncome = HqDisplayQueriesHud.LastWeekGpuGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuGrantSpendIncome : status + " · " + lastWeekGpuGrantSpendIncome;
                string lifetimeGpuGrantSpendIncome = HqDisplayQueriesHud.LifetimeGpuGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGpuGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGpuGrantSpendIncome : status + " · " + lifetimeGpuGrantSpendIncome;
                string thisWeekRevenueFinesSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueFinesSpendIncome : status + " · " + thisWeekRevenueFinesSpendIncome;
                string lastWeekRevenueFinesSpendIncome = HqDisplayQueriesHud.LastWeekRevenueFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueFinesSpendIncome : status + " · " + lastWeekRevenueFinesSpendIncome;
                string lifetimeRevenueFinesSpendIncome = HqDisplayQueriesHud.LifetimeRevenueFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueFinesSpendIncome : status + " · " + lifetimeRevenueFinesSpendIncome;
                string thisWeekConstructionGrantSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionGrantSpendIncome : status + " · " + thisWeekConstructionGrantSpendIncome;
                string lastWeekConstructionGrantSpendIncome = HqDisplayQueriesHud.LastWeekConstructionGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionGrantSpendIncome : status + " · " + lastWeekConstructionGrantSpendIncome;
                string lifetimeConstructionGrantSpendIncome = HqDisplayQueriesHud.LifetimeConstructionGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionGrantSpendIncome : status + " · " + lifetimeConstructionGrantSpendIncome;
                string thisWeekHiringGrantSpendIncome = HqDisplayQueriesHud.ThisWeekHiringGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHiringGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHiringGrantSpendIncome : status + " · " + thisWeekHiringGrantSpendIncome;
                string lastWeekHiringGrantSpendIncome = HqDisplayQueriesHud.LastWeekHiringGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringGrantSpendIncome : status + " · " + lastWeekHiringGrantSpendIncome;
                string lifetimeHiringGrantSpendIncome = HqDisplayQueriesHud.LifetimeHiringGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHiringGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHiringGrantSpendIncome : status + " · " + lifetimeHiringGrantSpendIncome;
                string thisWeekSalariesRoomSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesRoomSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesRoomSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesRoomSpendIncome : status + " · " + thisWeekSalariesRoomSpendIncome;
                string lastWeekSalariesRoomSpendIncome = HqDisplayQueriesHud.LastWeekSalariesRoomSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesRoomSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesRoomSpendIncome : status + " · " + lastWeekSalariesRoomSpendIncome;
                string lifetimeSalariesRoomSpendIncome = HqDisplayQueriesHud.LifetimeSalariesRoomSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSalariesRoomSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeSalariesRoomSpendIncome : status + " · " + lifetimeSalariesRoomSpendIncome;
                string thisWeekGrantTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekGrantTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantTrainingSpendIncome : status + " · " + thisWeekGrantTrainingSpendIncome;
                string lastWeekGrantTrainingSpendIncome = HqDisplayQueriesHud.LastWeekGrantTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantTrainingSpendIncome : status + " · " + lastWeekGrantTrainingSpendIncome;
                string lifetimeGrantTrainingSpendIncome = HqDisplayQueriesHud.LifetimeGrantTrainingSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantTrainingSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantTrainingSpendIncome : status + " · " + lifetimeGrantTrainingSpendIncome;
                string thisWeekFinesPowerSpendIncome = HqDisplayQueriesHud.ThisWeekFinesPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesPowerSpendIncome : status + " · " + thisWeekFinesPowerSpendIncome;
                string lastWeekFinesPowerSpendIncome = HqDisplayQueriesHud.LastWeekFinesPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesPowerSpendIncome : status + " · " + lastWeekFinesPowerSpendIncome;
                string lifetimeFinesPowerSpendIncome = HqDisplayQueriesHud.LifetimeFinesPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesPowerSpendIncome : status + " · " + lifetimeFinesPowerSpendIncome;
                string thisWeekGpuUpkeepRentSpendIncome = HqDisplayQueriesHud.ThisWeekGpuUpkeepRentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuUpkeepRentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuUpkeepRentSpendIncome : status + " · " + thisWeekGpuUpkeepRentSpendIncome;
                string lastWeekGpuUpkeepRentSpendIncome = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuUpkeepRentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuUpkeepRentSpendIncome : status + " · " + lastWeekGpuUpkeepRentSpendIncome;
                string lifetimeGpuUpkeepRentSpendIncome = HqDisplayQueriesHud.LifetimeGpuUpkeepRentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGpuUpkeepRentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGpuUpkeepRentSpendIncome : status + " · " + lifetimeGpuUpkeepRentSpendIncome;
                string thisWeekTrainingRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekTrainingRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingRevenueSpendIncome : status + " · " + thisWeekTrainingRevenueSpendIncome;
                string lastWeekTrainingRevenueSpendIncome = HqDisplayQueriesHud.LastWeekTrainingRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingRevenueSpendIncome : status + " · " + lastWeekTrainingRevenueSpendIncome;
                string lifetimeTrainingRevenueSpendIncome = HqDisplayQueriesHud.LifetimeTrainingRevenueSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingRevenueSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingRevenueSpendIncome : status + " · " + lifetimeTrainingRevenueSpendIncome;
                string thisWeekChipSalesGrantsSpendIncome = HqDisplayQueriesHud.ThisWeekChipSalesGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipSalesGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipSalesGrantsSpendIncome : status + " · " + thisWeekChipSalesGrantsSpendIncome;
                string lastWeekChipSalesGrantsSpendIncome = HqDisplayQueriesHud.LastWeekChipSalesGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipSalesGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipSalesGrantsSpendIncome : status + " · " + lastWeekChipSalesGrantsSpendIncome;
                string lifetimeChipSalesGrantsSpendIncome = HqDisplayQueriesHud.LifetimeChipSalesGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipSalesGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipSalesGrantsSpendIncome : status + " · " + lifetimeChipSalesGrantsSpendIncome;
                string thisWeekGreyLaunderSpendIncome = HqDisplayQueriesHud.ThisWeekGreyLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyLaunderSpendIncome : status + " · " + thisWeekGreyLaunderSpendIncome;
                string lastWeekGreyLaunderSpendIncome = HqDisplayQueriesHud.LastWeekGreyLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyLaunderSpendIncome : status + " · " + lastWeekGreyLaunderSpendIncome;
                string lifetimeGreyLaunderSpendIncome = HqDisplayQueriesHud.LifetimeGreyLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyLaunderSpendIncome : status + " · " + lifetimeGreyLaunderSpendIncome;
                string thisWeekSalariesGpuSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesGpuSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSalariesGpuSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekSalariesGpuSpendIncome : status + " · " + thisWeekSalariesGpuSpendIncome;
                string lastWeekSalariesGpuSpendIncome = HqDisplayQueriesHud.LastWeekSalariesGpuSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesGpuSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesGpuSpendIncome : status + " · " + lastWeekSalariesGpuSpendIncome;
                string lifetimeSalariesGpuSpendIncome = HqDisplayQueriesHud.LifetimeSalariesGpuSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSalariesGpuSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeSalariesGpuSpendIncome : status + " · " + lifetimeSalariesGpuSpendIncome;
                string thisWeekHardwarePowerSpendIncome = HqDisplayQueriesHud.ThisWeekHardwarePowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwarePowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwarePowerSpendIncome : status + " · " + thisWeekHardwarePowerSpendIncome;
                string lastWeekHardwarePowerSpendIncome = HqDisplayQueriesHud.LastWeekHardwarePowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHardwarePowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHardwarePowerSpendIncome : status + " · " + lastWeekHardwarePowerSpendIncome;
                string lifetimeHardwarePowerSpendIncome = HqDisplayQueriesHud.LifetimeHardwarePowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHardwarePowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHardwarePowerSpendIncome : status + " · " + lifetimeHardwarePowerSpendIncome;
                string thisWeekHiringGreySpendIncome = HqDisplayQueriesHud.ThisWeekHiringGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHiringGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekHiringGreySpendIncome : status + " · " + thisWeekHiringGreySpendIncome;
                string lastWeekHiringGreySpendIncome = HqDisplayQueriesHud.LastWeekHiringGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringGreySpendIncome : status + " · " + lastWeekHiringGreySpendIncome;
                string lifetimeHiringGreySpendIncome = HqDisplayQueriesHud.LifetimeHiringGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHiringGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeHiringGreySpendIncome : status + " · " + lifetimeHiringGreySpendIncome;
                string thisWeekRevenueNetSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRevenueNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRevenueNetSpendIncome : status + " · " + thisWeekRevenueNetSpendIncome;
                string lastWeekRevenueNetSpendIncome = HqDisplayQueriesHud.LastWeekRevenueNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRevenueNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRevenueNetSpendIncome : status + " · " + lastWeekRevenueNetSpendIncome;
                string lifetimeRevenueNetSpendIncome = HqDisplayQueriesHud.LifetimeRevenueNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueNetSpendIncome : status + " · " + lifetimeRevenueNetSpendIncome;
                string thisWeekChipDevLandSpendIncome = HqDisplayQueriesHud.ThisWeekChipDevLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekChipDevLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekChipDevLandSpendIncome : status + " · " + thisWeekChipDevLandSpendIncome;
                string lastWeekChipDevLandSpendIncome = HqDisplayQueriesHud.LastWeekChipDevLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekChipDevLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekChipDevLandSpendIncome : status + " · " + lastWeekChipDevLandSpendIncome;
                string lifetimeChipDevLandSpendIncome = HqDisplayQueriesHud.LifetimeChipDevLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeChipDevLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeChipDevLandSpendIncome : status + " · " + lifetimeChipDevLandSpendIncome;
                string thisWeekConstructionGrantsSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionGrantsSpendIncome : status + " · " + thisWeekConstructionGrantsSpendIncome;
                string lastWeekConstructionGrantsSpendIncome = HqDisplayQueriesHud.LastWeekConstructionGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionGrantsSpendIncome : status + " · " + lastWeekConstructionGrantsSpendIncome;
                string lifetimeConstructionGrantsSpendIncome = HqDisplayQueriesHud.LifetimeConstructionGrantsSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionGrantsSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionGrantsSpendIncome : status + " · " + lifetimeConstructionGrantsSpendIncome;
                string thisWeekTrainingLaunderSpendIncome = HqDisplayQueriesHud.ThisWeekTrainingLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingLaunderSpendIncome : status + " · " + thisWeekTrainingLaunderSpendIncome;
                string lastWeekTrainingLaunderSpendIncome = HqDisplayQueriesHud.LastWeekTrainingLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingLaunderSpendIncome : status + " · " + lastWeekTrainingLaunderSpendIncome;
                string lifetimeTrainingLaunderSpendIncome = HqDisplayQueriesHud.LifetimeTrainingLaunderSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingLaunderSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingLaunderSpendIncome : status + " · " + lifetimeTrainingLaunderSpendIncome;
                string thisWeekPowerChipSpendIncome = HqDisplayQueriesHud.ThisWeekPowerChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerChipSpendIncome : status + " · " + thisWeekPowerChipSpendIncome;
                string lastWeekPowerChipSpendIncome = HqDisplayQueriesHud.LastWeekPowerChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerChipSpendIncome : status + " · " + lastWeekPowerChipSpendIncome;
                string lifetimePowerChipSpendIncome = HqDisplayQueriesHud.LifetimePowerChipSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerChipSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerChipSpendIncome : status + " · " + lifetimePowerChipSpendIncome;
                string thisWeekGrantHardwareSpendIncome = HqDisplayQueriesHud.ThisWeekGrantHardwareSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantHardwareSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantHardwareSpendIncome : status + " · " + thisWeekGrantHardwareSpendIncome;
                string lastWeekGrantHardwareSpendIncome = HqDisplayQueriesHud.LastWeekGrantHardwareSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantHardwareSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantHardwareSpendIncome : status + " · " + lastWeekGrantHardwareSpendIncome;
                string lifetimeGrantHardwareSpendIncome = HqDisplayQueriesHud.LifetimeGrantHardwareSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantHardwareSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantHardwareSpendIncome : status + " · " + lifetimeGrantHardwareSpendIncome;
                string thisWeekPowerFinesSpendIncome = HqDisplayQueriesHud.ThisWeekPowerFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPowerFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPowerFinesSpendIncome : status + " · " + thisWeekPowerFinesSpendIncome;
                string lastWeekPowerFinesSpendIncome = HqDisplayQueriesHud.LastWeekPowerFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerFinesSpendIncome : status + " · " + lastWeekPowerFinesSpendIncome;
                string lifetimePowerFinesSpendIncome = HqDisplayQueriesHud.LifetimePowerFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePowerFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePowerFinesSpendIncome : status + " · " + lifetimePowerFinesSpendIncome;
                string thisWeekGrantsFinesSpendIncome = HqDisplayQueriesHud.ThisWeekGrantsFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantsFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantsFinesSpendIncome : status + " · " + thisWeekGrantsFinesSpendIncome;
                string lastWeekGrantsFinesSpendIncome = HqDisplayQueriesHud.LastWeekGrantsFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantsFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantsFinesSpendIncome : status + " · " + lastWeekGrantsFinesSpendIncome;
                string lifetimeGrantsFinesSpendIncome = HqDisplayQueriesHud.LifetimeGrantsFinesSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantsFinesSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantsFinesSpendIncome : status + " · " + lifetimeGrantsFinesSpendIncome;
                string thisWeekRentGrantSpendIncome = HqDisplayQueriesHud.ThisWeekRentGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRentGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekRentGrantSpendIncome : status + " · " + thisWeekRentGrantSpendIncome;
                string lastWeekRentGrantSpendIncome = HqDisplayQueriesHud.LastWeekRentGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentGrantSpendIncome : status + " · " + lastWeekRentGrantSpendIncome;
                string lifetimeRentGrantSpendIncome = HqDisplayQueriesHud.LifetimeRentGrantSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRentGrantSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeRentGrantSpendIncome : status + " · " + lifetimeRentGrantSpendIncome;
                string thisWeekGrantsNetSpendIncome = HqDisplayQueriesHud.ThisWeekGrantsNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantsNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantsNetSpendIncome : status + " · " + thisWeekGrantsNetSpendIncome;
                string lastWeekGrantsNetSpendIncome = HqDisplayQueriesHud.LastWeekGrantsNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGrantsNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGrantsNetSpendIncome : status + " · " + lastWeekGrantsNetSpendIncome;
                string lifetimeGrantsNetSpendIncome = HqDisplayQueriesHud.LifetimeGrantsNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGrantsNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGrantsNetSpendIncome : status + " · " + lifetimeGrantsNetSpendIncome;
                string thisWeekPatentSiteSpendIncome = HqDisplayQueriesHud.ThisWeekPatentSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentSiteSpendIncome : status + " · " + thisWeekPatentSiteSpendIncome;
                string lastWeekPatentSiteSpendIncome = HqDisplayQueriesHud.LastWeekPatentSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentSiteSpendIncome : status + " · " + lastWeekPatentSiteSpendIncome;
                string lifetimePatentSiteSpendIncome = HqDisplayQueriesHud.LifetimePatentSiteSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentSiteSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentSiteSpendIncome : status + " · " + lifetimePatentSiteSpendIncome;
                string thisWeekGreyLandSpendIncome = HqDisplayQueriesHud.ThisWeekGreyLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGreyLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGreyLandSpendIncome : status + " · " + thisWeekGreyLandSpendIncome;
                string lastWeekGreyLandSpendIncome = HqDisplayQueriesHud.LastWeekGreyLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGreyLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGreyLandSpendIncome : status + " · " + lastWeekGreyLandSpendIncome;
                string lifetimeGreyLandSpendIncome = HqDisplayQueriesHud.LifetimeGreyLandSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyLandSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyLandSpendIncome : status + " · " + lifetimeGreyLandSpendIncome;
                string thisWeekConstructionPatentSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionPatentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionPatentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionPatentSpendIncome : status + " · " + thisWeekConstructionPatentSpendIncome;
                string lastWeekConstructionPatentSpendIncome = HqDisplayQueriesHud.LastWeekConstructionPatentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekConstructionPatentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekConstructionPatentSpendIncome : status + " · " + lastWeekConstructionPatentSpendIncome;
                string lifetimeConstructionPatentSpendIncome = HqDisplayQueriesHud.LifetimeConstructionPatentSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeConstructionPatentSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeConstructionPatentSpendIncome : status + " · " + lifetimeConstructionPatentSpendIncome;
                string thisWeekGpuNetSpendIncome = HqDisplayQueriesHud.ThisWeekGpuNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGpuNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekGpuNetSpendIncome : status + " · " + thisWeekGpuNetSpendIncome;
                string lastWeekGpuNetSpendIncome = HqDisplayQueriesHud.LastWeekGpuNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekGpuNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekGpuNetSpendIncome : status + " · " + lastWeekGpuNetSpendIncome;
                string lifetimeGpuNetSpendIncome = HqDisplayQueriesHud.LifetimeGpuNetSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGpuNetSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeGpuNetSpendIncome : status + " · " + lifetimeGpuNetSpendIncome;
                string thisWeekPatentPowerSpendIncome = HqDisplayQueriesHud.ThisWeekPatentPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentPowerSpendIncome : status + " · " + thisWeekPatentPowerSpendIncome;
                string lastWeekPatentPowerSpendIncome = HqDisplayQueriesHud.LastWeekPatentPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPatentPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekPatentPowerSpendIncome : status + " · " + lastWeekPatentPowerSpendIncome;
                string lifetimePatentPowerSpendIncome = HqDisplayQueriesHud.LifetimePatentPowerSpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentPowerSpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentPowerSpendIncome : status + " · " + lifetimePatentPowerSpendIncome;
                string thisWeekFinesGreySpendIncome = HqDisplayQueriesHud.ThisWeekFinesGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekFinesGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? thisWeekFinesGreySpendIncome : status + " · " + thisWeekFinesGreySpendIncome;
                string lastWeekFinesGreySpendIncome = HqDisplayQueriesHud.LastWeekFinesGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekFinesGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lastWeekFinesGreySpendIncome : status + " · " + lastWeekFinesGreySpendIncome;
                string lifetimeFinesGreySpendIncome = HqDisplayQueriesHud.LifetimeFinesGreySpendIncomeSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeFinesGreySpendIncome))
                    status = string.IsNullOrEmpty(status) ? lifetimeFinesGreySpendIncome : status + " · " + lifetimeFinesGreySpendIncome;
                string thisWeekTrainingPatent = HqDisplayQueriesHud.ThisWeekTrainingPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekTrainingPatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekTrainingPatent : status + " · " + thisWeekTrainingPatent;
                string lastWeekHiringRent = HqDisplayQueriesHud.LastWeekHiringRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekHiringRent))
                    status = string.IsNullOrEmpty(status) ? lastWeekHiringRent : status + " · " + lastWeekHiringRent;
                string lifetimeRevenueGrant = HqDisplayQueriesHud.LifetimeRevenueGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeRevenueGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeRevenueGrant : status + " · " + lifetimeRevenueGrant;
                string thisWeekIncomeChip = HqDisplayQueriesHud.ThisWeekIncomeChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekIncomeChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekIncomeChip : status + " · " + thisWeekIncomeChip;
                string lastWeekUpkeepLand = HqDisplayQueriesHud.LastWeekUpkeepLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekUpkeepLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekUpkeepLand : status + " · " + lastWeekUpkeepLand;
                string thisWeekPatentChip = HqDisplayQueriesHud.ThisWeekPatentChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentChip : status + " · " + thisWeekPatentChip;
                string lastWeekTrainingPatent = HqDisplayQueriesHud.LastWeekTrainingPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekTrainingPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekTrainingPatent : status + " · " + lastWeekTrainingPatent;
                string lifetimeTrainingPatent = HqDisplayQueriesHud.LifetimeTrainingPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeTrainingPatent))
                    status = string.IsNullOrEmpty(status) ? lifetimeTrainingPatent : status + " · " + lifetimeTrainingPatent;
                string thisWeekSpendPower = HqDisplayQueriesHud.ThisWeekSpendPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSpendPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekSpendPower : status + " · " + thisWeekSpendPower;
                string thisWeekDevRent = HqDisplayQueriesHud.ThisWeekDevRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekDevRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekDevRent : status + " · " + thisWeekDevRent;
                string lifetimeSpendGrant = HqDisplayQueriesHud.LifetimeSpendGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSpendGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeSpendGrant : status + " · " + lifetimeSpendGrant;
                string thisWeekIncomeRent = HqDisplayQueriesHud.ThisWeekIncomeRentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekIncomeRent))
                    status = string.IsNullOrEmpty(status) ? thisWeekIncomeRent : status + " · " + thisWeekIncomeRent;
                string lastWeekDevPower = HqDisplayQueriesHud.LastWeekDevPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekDevPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekDevPower : status + " · " + lastWeekDevPower;
                string lifetimeIncomeChip = HqDisplayQueriesHud.LifetimeIncomeChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeIncomeChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeIncomeChip : status + " · " + lifetimeIncomeChip;
                string thisWeekHiringPower = HqDisplayQueriesHud.ThisWeekHiringPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHiringPower))
                    status = string.IsNullOrEmpty(status) ? thisWeekHiringPower : status + " · " + thisWeekHiringPower;
                string lastWeekSpendChip = HqDisplayQueriesHud.LastWeekSpendChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSpendChip))
                    status = string.IsNullOrEmpty(status) ? lastWeekSpendChip : status + " · " + lastWeekSpendChip;
                string lifetimeDevGrant = HqDisplayQueriesHud.LifetimeDevGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeDevGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeDevGrant : status + " · " + lifetimeDevGrant;
                string thisWeekUpkeepChip = HqDisplayQueriesHud.ThisWeekUpkeepChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekUpkeepChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekUpkeepChip : status + " · " + thisWeekUpkeepChip;
                string lastWeekIncomePower = HqDisplayQueriesHud.LastWeekIncomePowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekIncomePower))
                    status = string.IsNullOrEmpty(status) ? lastWeekIncomePower : status + " · " + lastWeekIncomePower;
                string lifetimeHiringGrant = HqDisplayQueriesHud.LifetimeHiringGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeHiringGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeHiringGrant : status + " · " + lifetimeHiringGrant;
                string thisWeekDevLand = HqDisplayQueriesHud.ThisWeekDevLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekDevLand))
                    status = string.IsNullOrEmpty(status) ? thisWeekDevLand : status + " · " + thisWeekDevLand;
                string lastWeekUpkeepGrant = HqDisplayQueriesHud.LastWeekUpkeepGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekUpkeepGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekUpkeepGrant : status + " · " + lastWeekUpkeepGrant;
                string thisWeekLandPatent = HqDisplayQueriesHud.ThisWeekLandPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekLandPatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekLandPatent : status + " · " + thisWeekLandPatent;
                string thisWeekHiringChip = HqDisplayQueriesHud.ThisWeekHiringChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHiringChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekHiringChip : status + " · " + thisWeekHiringChip;
                string lastWeekDevLand = HqDisplayQueriesHud.LastWeekDevLandSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekDevLand))
                    status = string.IsNullOrEmpty(status) ? lastWeekDevLand : status + " · " + lastWeekDevLand;
                string thisWeekGrantDev = HqDisplayQueriesHud.ThisWeekGrantDevSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekGrantDev))
                    status = string.IsNullOrEmpty(status) ? thisWeekGrantDev : status + " · " + thisWeekGrantDev;
                string lifetimeSpendChip = HqDisplayQueriesHud.LifetimeSpendChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSpendChip))
                    status = string.IsNullOrEmpty(status) ? lifetimeSpendChip : status + " · " + lifetimeSpendChip;
                string thisWeekIncomeGrant = HqDisplayQueriesHud.ThisWeekIncomeGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekIncomeGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekIncomeGrant : status + " · " + thisWeekIncomeGrant;
                string lifetimeGreyPatent = HqDisplayQueriesHud.LifetimeGreyPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeGreyPatent))
                    status = string.IsNullOrEmpty(status) ? lifetimeGreyPatent : status + " · " + lifetimeGreyPatent;
                string thisWeekSpendChip = HqDisplayQueriesHud.ThisWeekSpendChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSpendChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekSpendChip : status + " · " + thisWeekSpendChip;
                string lifetimeIncomeGrant = HqDisplayQueriesHud.LifetimeIncomeGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeIncomeGrant))
                    status = string.IsNullOrEmpty(status) ? lifetimeIncomeGrant : status + " · " + lifetimeIncomeGrant;
                string lastWeekIncomeGrant = HqDisplayQueriesHud.LastWeekIncomeGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekIncomeGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekIncomeGrant : status + " · " + lastWeekIncomeGrant;
                string lifetimeSpendPower = HqDisplayQueriesHud.LifetimeSpendPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeSpendPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeSpendPower : status + " · " + lifetimeSpendPower;
                string thisWeekDevGrant = HqDisplayQueriesHud.ThisWeekDevGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekDevGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekDevGrant : status + " · " + thisWeekDevGrant;
                string thisWeekPatentGrant = HqDisplayQueriesHud.ThisWeekPatentGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekPatentGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekPatentGrant : status + " · " + thisWeekPatentGrant;
                string lastWeekSpendPower = HqDisplayQueriesHud.LastWeekSpendPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSpendPower))
                    status = string.IsNullOrEmpty(status) ? lastWeekSpendPower : status + " · " + lastWeekSpendPower;
                string lifetimeDevPower = HqDisplayQueriesHud.LifetimeDevPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeDevPower))
                    status = string.IsNullOrEmpty(status) ? lifetimeDevPower : status + " · " + lifetimeDevPower;
                string lastWeekDevGrant = HqDisplayQueriesHud.LastWeekDevGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekDevGrant))
                    status = string.IsNullOrEmpty(status) ? lastWeekDevGrant : status + " · " + lastWeekDevGrant;
                string lifetimePatentPower = HqDisplayQueriesHud.LifetimePatentPowerSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimePatentPower))
                    status = string.IsNullOrEmpty(status) ? lifetimePatentPower : status + " · " + lifetimePatentPower;
                string thisWeekSpendGrant = HqDisplayQueriesHud.ThisWeekSpendGrantSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekSpendGrant))
                    status = string.IsNullOrEmpty(status) ? thisWeekSpendGrant : status + " · " + thisWeekSpendGrant;
                string lifetimeLandPatent = HqDisplayQueriesHud.LifetimeLandPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lifetimeLandPatent))
                    status = string.IsNullOrEmpty(status) ? lifetimeLandPatent : status + " · " + lifetimeLandPatent;
                string lastWeekRentPatent = HqDisplayQueriesHud.LastWeekRentPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekRentPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekRentPatent : status + " · " + lastWeekRentPatent;
                string thisWeekRoomChip = HqDisplayQueriesHud.ThisWeekRoomChipSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekRoomChip))
                    status = string.IsNullOrEmpty(status) ? thisWeekRoomChip : status + " · " + thisWeekRoomChip;
                string lastWeekPowerPatent = HqDisplayQueriesHud.LastWeekPowerPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekPowerPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekPowerPatent : status + " · " + lastWeekPowerPatent;
                string thisWeekHardwarePatent = HqDisplayQueriesHud.ThisWeekHardwarePatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekHardwarePatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekHardwarePatent : status + " · " + thisWeekHardwarePatent;
                string lastWeekSalariesPatent = HqDisplayQueriesHud.LastWeekSalariesPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekSalariesPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekSalariesPatent : status + " · " + lastWeekSalariesPatent;
                string thisWeekConstructionPatent = HqDisplayQueriesHud.ThisWeekConstructionPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(thisWeekConstructionPatent))
                    status = string.IsNullOrEmpty(status) ? thisWeekConstructionPatent : status + " · " + thisWeekConstructionPatent;
                string lastWeekLaunderPatent = HqDisplayQueriesHud.LastWeekLaunderPatentSummaryLine(lab);
                if (!string.IsNullOrEmpty(lastWeekLaunderPatent))
                    status = string.IsNullOrEmpty(status) ? lastWeekLaunderPatent : status + " · " + lastWeekLaunderPatent;
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