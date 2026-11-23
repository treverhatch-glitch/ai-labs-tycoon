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
    /// <summary>Start training runs and show progress for models in training.</summary>
    public sealed class TrainingRunPanel : MonoBehaviour
    {
        const int DefaultAlignmentShare = 300;

        SimulationHost _host;
        Transform _activeRuns;
        Text _status;
        Text _summary;

        public static TrainingRunPanel Create(Transform canvasParent, SimulationHost host)
        {
            var panelRoot = HqUiFactory.Panel(canvasParent, "TrainingRunPanel",
                new Vector2(0.22f, 0f), new Vector2(0.78f, 0.18f), new Vector2(0f, 120f));
            var panel = panelRoot.AddComponent<TrainingRunPanel>();
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
            HqUiFactory.Label(root, "Title", "Training runs", 17, TextAnchor.UpperLeft)
                .rectTransform.offsetMax = new Vector2(-8f, -28f);

            var startRow = new GameObject("StartRow");
            startRow.transform.SetParent(root, false);
            var startRect = startRow.AddComponent<RectTransform>();
            startRect.anchorMin = new Vector2(0f, 0.55f);
            startRect.anchorMax = new Vector2(1f, 1f);
            startRect.offsetMin = new Vector2(8f, 0f);
            startRect.offsetMax = new Vector2(-8f, -32f);
            var startLayout = startRow.AddComponent<HorizontalLayoutGroup>();
            startLayout.spacing = 6f;
            startLayout.childForceExpandWidth = false;

            HqUiFactory.SmallButton(startRow.transform, "Train Mini", () => StartLine("mini"));
            HqUiFactory.SmallButton(startRow.transform, "Train Mid", () => StartLine("mid"));
            HqUiFactory.SmallButton(startRow.transform, "Train Flagship", () => StartLine("flagship"));

            _activeRuns = new GameObject("Active").transform;
            _activeRuns.SetParent(root, false);
            var runsRect = _activeRuns.gameObject.AddComponent<RectTransform>();
            runsRect.anchorMin = new Vector2(0f, 0.22f);
            runsRect.anchorMax = new Vector2(1f, 0.52f);
            runsRect.offsetMin = new Vector2(8f, 0f);
            runsRect.offsetMax = new Vector2(-8f, 0f);
            var runsLayout = _activeRuns.gameObject.AddComponent<VerticalLayoutGroup>();
            runsLayout.spacing = 2f;

            _summary = HqUiFactory.Label(root, "Summary", "", 11, TextAnchor.UpperLeft);
            _summary.rectTransform.anchorMin = new Vector2(0f, 0.22f);
            _summary.rectTransform.anchorMax = new Vector2(1f, 0.38f);
            _status = HqUiFactory.Label(root, "Status", "", 11, TextAnchor.LowerLeft);
            _status.rectTransform.anchorMin = new Vector2(0f, 0f);
            _status.rectTransform.anchorMax = new Vector2(1f, 0.2f);
        }

        void StartLine(string lineId)
        {
            var cmd = Command.StartTraining(_host.Simulation.Player.Id, lineId, DefaultAlignmentShare);
            _host.TryApply(cmd);
        }

        void Refresh()
        {
            if (_host?.Simulation == null) return;
            Simulation sim = _host.Simulation;
            LabState lab = sim.Player;

            for (int i = _activeRuns.childCount - 1; i >= 0; i--)
                Destroy(_activeRuns.GetChild(i).gameObject);

            bool any = false;
            for (int i = 0; i < lab.Models.Count; i++)
            {
                ModelState model = lab.Models[i];
                if (model.Status != ModelStatus.Training) continue;
                any = true;
                AddRunRow(model);
            }
            if (!any)
            {
                var empty = HqUiFactory.Label(_activeRuns, "Empty", "No active runs", 12, TextAnchor.MiddleLeft);
                empty.color = new Color(0.75f, 0.8f, 0.85f);
            }

            int training = 0;
            for (int i = 0; i < lab.Models.Count; i++)
                if (lab.Models[i].Status == ModelStatus.Training) training++;
            _summary.text = training > 0 ? "Active runs: " + training + " · Models: " + lab.Models.Count : "No active runs";
                    string lastWeekLand = HqDisplayQueriesHud.LastWeekLandHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLand))
                        _summary.text += " · " + lastWeekLand;
                    string lastWeekSiteCapital = HqDisplayQueriesHud.LastWeekSiteCapitalSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteCapital))
                        _summary.text += " · " + lastWeekSiteCapital;
                    string lifetimeGrantsNet = HqDisplayQueriesHud.LifetimeGrantsNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantsNet))
                        _summary.text += " · " + lifetimeGrantsNet;
                    string thisWeekChipRevenue = HqDisplayQueriesHud.ThisWeekChipRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipRevenue))
                        _summary.text += " · " + thisWeekChipRevenue;
                    string lastWeekConstructionPatent = HqDisplayQueriesHud.LastWeekConstructionPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionPatent))
                        _summary.text += " · " + lastWeekConstructionPatent;
                    string lastWeekHiringPatent = HqDisplayQueriesHud.LastWeekHiringPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringPatent))
                        _summary.text += " · " + lastWeekHiringPatent;
                    string thisWeekBuild = HqDisplayQueriesHud.ThisWeekConstructionSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekBuild))
                        _summary.text += " · " + thisWeekBuild;
                    string thisWeekFinesPower = HqDisplayQueriesHud.ThisWeekFinesPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesPower))
                        _summary.text += " · " + thisWeekFinesPower;
                    string lastWeekChipDevLand = HqDisplayQueriesHud.LastWeekChipDevLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipDevLand))
                        _summary.text += " · " + lastWeekChipDevLand;
                    string thisWeekSalariesGpu = HqDisplayQueriesHud.ThisWeekSalariesGpuSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesGpu))
                        _summary.text += " · " + thisWeekSalariesGpu;
                    string lastWeekPatentSite = HqDisplayQueriesHud.LastWeekPatentSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentSite))
                        _summary.text += " · " + lastWeekPatentSite;
                    string lastWeekCapitalGrey = HqDisplayQueriesHud.LastWeekCapitalGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalGrey))
                        _summary.text += " · " + lastWeekCapitalGrey;
                    string lastWeekGrantsRevenue = HqDisplayQueriesHud.LastWeekGrantsRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantsRevenue))
                        _summary.text += " · " + lastWeekGrantsRevenue;
                    string thisWeekGpuFinesGrey = HqDisplayQueriesHud.ThisWeekGpuFinesGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuFinesGrey))
                        _summary.text += " · " + thisWeekGpuFinesGrey;
                    string lastWeekSalariesRoom = HqDisplayQueriesHud.LastWeekSalariesRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesRoom))
                        _summary.text += " · " + lastWeekSalariesRoom;
                    string lifetimeRevenueChip = HqDisplayQueriesHud.LifetimeRevenueChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueChip))
                        _summary.text += " · " + lifetimeRevenueChip;
                    string lastWeekGpuUpkeepRent = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuUpkeepRent))
                        _summary.text += " · " + lastWeekGpuUpkeepRent;
                    string lastWeekRevenueNet = HqDisplayQueriesHud.LastWeekRevenueNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueNet))
                        _summary.text += " · " + lastWeekRevenueNet;
                    string thisWeekNetTraining = HqDisplayQueriesHud.ThisWeekNetTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetTraining))
                        _summary.text += " · " + thisWeekNetTraining;
                    string thisWeekGrantsFines = HqDisplayQueriesHud.ThisWeekGrantsFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantsFines))
                        _summary.text += " · " + thisWeekGrantsFines;
                    string lifetimePowerFines = HqDisplayQueriesHud.LifetimePowerFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerFines))
                        _summary.text += " · " + lifetimePowerFines;
                    string lastWeekGreyLaunder = HqDisplayQueriesHud.LastWeekGreyLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyLaunder))
                        _summary.text += " · " + lastWeekGreyLaunder;
                    string thisWeekConstructionGrants = HqDisplayQueriesHud.ThisWeekConstructionGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionGrants))
                        _summary.text += " · " + thisWeekConstructionGrants;
                    string lastWeekTrainingRevenue = HqDisplayQueriesHud.LastWeekTrainingRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingRevenue))
                        _summary.text += " · " + lastWeekTrainingRevenue;
                    string lastWeekHardwarePower = HqDisplayQueriesHud.LastWeekHardwarePowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwarePower))
                        _summary.text += " · " + lastWeekHardwarePower;
                    string lifetimeTrainingLaunder = HqDisplayQueriesHud.LifetimeTrainingLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingLaunder))
                        _summary.text += " · " + lifetimeTrainingLaunder;
                    string lastWeekChipSalesGrants = HqDisplayQueriesHud.LastWeekChipSalesGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipSalesGrants))
                        _summary.text += " · " + lastWeekChipSalesGrants;
                    string lastWeekHiringGrey = HqDisplayQueriesHud.LastWeekHiringGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringGrey))
                        _summary.text += " · " + lastWeekHiringGrey;
                    string thisWeekPowerChip = HqDisplayQueriesHud.ThisWeekPowerChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerChip))
                        _summary.text += " · " + thisWeekPowerChip;
                    string thisWeekTrainingChip = HqDisplayQueriesHud.ThisWeekTrainingChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingChip))
                        _summary.text += " · " + thisWeekTrainingChip;
                    string lifetimeNetTraining = HqDisplayQueriesHud.LifetimeNetTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetTraining))
                        _summary.text += " · " + lifetimeNetTraining;
                    string lastWeekFinesTraining = HqDisplayQueriesHud.LastWeekFinesTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesTraining))
                        _summary.text += " · " + lastWeekFinesTraining;
                    string thisWeekGreyNet = HqDisplayQueriesHud.ThisWeekGreyNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyNet))
                        _summary.text += " · " + thisWeekGreyNet;
                    string lastWeekRoomHiring = HqDisplayQueriesHud.LastWeekRoomHiringSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomHiring))
                        _summary.text += " · " + lastWeekRoomHiring;
                    string lifetimeChipGrey = HqDisplayQueriesHud.LifetimeChipGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipGrey))
                        _summary.text += " · " + lifetimeChipGrey;
                    string lastWeekTrainingGpuUpkeep = HqDisplayQueriesHud.LastWeekTrainingGpuUpkeepSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingGpuUpkeep))
                        _summary.text += " · " + lastWeekTrainingGpuUpkeep;
                    string lastWeekChipDevSales = HqDisplayQueriesHud.LastWeekChipDevSalesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipDevSales))
                        _summary.text += " · " + lastWeekChipDevSales;
                    string thisWeekRevenueGrants = HqDisplayQueriesHud.ThisWeekRevenueGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueGrants))
                        _summary.text += " · " + thisWeekRevenueGrants;
                    string lastWeekConstructionLand = HqDisplayQueriesHud.LastWeekConstructionLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionLand))
                        _summary.text += " · " + lastWeekConstructionLand;
                    string lifetimeFinesPower = HqDisplayQueriesHud.LifetimeFinesPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesPower))
                        _summary.text += " · " + lifetimeFinesPower;
                    string thisWeekGpuNet = HqDisplayQueriesHud.ThisWeekGpuNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuNet))
                        _summary.text += " · " + thisWeekGpuNet;
                    string lastWeekPatentHiring = HqDisplayQueriesHud.LastWeekPatentHiringSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentHiring))
                        _summary.text += " · " + lastWeekPatentHiring;
                    string thisWeekChipConstruction = HqDisplayQueriesHud.ThisWeekChipConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipConstruction))
                        _summary.text += " · " + thisWeekChipConstruction;
                    string thisWeekSalariesFines = HqDisplayQueriesHud.ThisWeekSalariesFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesFines))
                        _summary.text += " · " + thisWeekSalariesFines;
                    string lastWeekNetFines = HqDisplayQueriesHud.LastWeekNetFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetFines))
                        _summary.text += " · " + lastWeekNetFines;
                    string thisWeekGreyPower = HqDisplayQueriesHud.ThisWeekGreyPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyPower))
                        _summary.text += " · " + thisWeekGreyPower;
                    string lifetimeGrantsLaunder = HqDisplayQueriesHud.LifetimeGrantsLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantsLaunder))
                        _summary.text += " · " + lifetimeGrantsLaunder;
                    string lastWeekSalariesPower = HqDisplayQueriesHud.LastWeekSalariesPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesPower))
                        _summary.text += " · " + lastWeekSalariesPower;
                    string thisWeekTrainingPower = HqDisplayQueriesHud.ThisWeekTrainingPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingPower))
                        _summary.text += " · " + thisWeekTrainingPower;
                    string lastWeekChipGrey = HqDisplayQueriesHud.LastWeekChipGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipGrey))
                        _summary.text += " · " + lastWeekChipGrey;
                    string lastWeekConstructionGrey = HqDisplayQueriesHud.LastWeekConstructionGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionGrey))
                        _summary.text += " · " + lastWeekConstructionGrey;
                    string lifetimeRevenuePower = HqDisplayQueriesHud.LifetimeRevenuePowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenuePower))
                        _summary.text += " · " + lifetimeRevenuePower;
                    string lastWeekGpuFines = HqDisplayQueriesHud.LastWeekGpuFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuFines))
                        _summary.text += " · " + lastWeekGpuFines;
                    string thisWeekSalariesNet = HqDisplayQueriesHud.ThisWeekSalariesNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesNet))
                        _summary.text += " · " + thisWeekSalariesNet;
                    string lastWeekCapitalPatent = HqDisplayQueriesHud.LastWeekCapitalPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalPatent))
                        _summary.text += " · " + lastWeekCapitalPatent;
                    string thisWeekChipTraining = HqDisplayQueriesHud.ThisWeekChipTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipTraining))
                        _summary.text += " · " + thisWeekChipTraining;
                    string lastWeekRevenueChip = HqDisplayQueriesHud.LastWeekRevenueChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueChip))
                        _summary.text += " · " + lastWeekRevenueChip;
                    string lifetimeGreyNet = HqDisplayQueriesHud.LifetimeGreyNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyNet))
                        _summary.text += " · " + lifetimeGreyNet;
                    string thisWeekConstructionPower = HqDisplayQueriesHud.ThisWeekConstructionPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionPower))
                        _summary.text += " · " + thisWeekConstructionPower;
                    string lastWeekSalariesHiring = HqDisplayQueriesHud.LastWeekSalariesHiringSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesHiring))
                        _summary.text += " · " + lastWeekSalariesHiring;
                    string thisWeekGrantsChip = HqDisplayQueriesHud.ThisWeekGrantsChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantsChip))
                        _summary.text += " · " + thisWeekGrantsChip;
                    string lastWeekNetPower = HqDisplayQueriesHud.LastWeekNetPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetPower))
                        _summary.text += " · " + lastWeekNetPower;
                    string thisWeekFinesChip = HqDisplayQueriesHud.ThisWeekFinesChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesChip))
                        _summary.text += " · " + thisWeekFinesChip;
                    string lifetimeChipPower = HqDisplayQueriesHud.LifetimeChipPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipPower))
                        _summary.text += " · " + lifetimeChipPower;
                    string lastWeekGreyGrants = HqDisplayQueriesHud.LastWeekGreyGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyGrants))
                        _summary.text += " · " + lastWeekGreyGrants;
                    string thisWeekGpuConstruction = HqDisplayQueriesHud.ThisWeekGpuConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuConstruction))
                        _summary.text += " · " + thisWeekGpuConstruction;
                    string lastWeekSiteHardware = HqDisplayQueriesHud.LastWeekSiteHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteHardware))
                        _summary.text += " · " + lastWeekSiteHardware;
                    string lastWeekLaunderFines = HqDisplayQueriesHud.LastWeekLaunderFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderFines))
                        _summary.text += " · " + lastWeekLaunderFines;
                    string lifetimeTrainingChip = HqDisplayQueriesHud.LifetimeTrainingChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingChip))
                        _summary.text += " · " + lifetimeTrainingChip;
                    string thisWeekRevenueTraining = HqDisplayQueriesHud.ThisWeekRevenueTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueTraining))
                        _summary.text += " · " + thisWeekRevenueTraining;
                    string lastWeekPowerChipDev = HqDisplayQueriesHud.LastWeekPowerChipDevSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerChipDev))
                        _summary.text += " · " + lastWeekPowerChipDev;
                    string thisWeekSalariesGrants = HqDisplayQueriesHud.ThisWeekSalariesGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesGrants))
                        _summary.text += " · " + thisWeekSalariesGrants;
                    string lastWeekNetGrey = HqDisplayQueriesHud.LastWeekNetGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetGrey))
                        _summary.text += " · " + lastWeekNetGrey;
                    string thisWeekGrantNet = HqDisplayQueriesHud.ThisWeekGrantNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantNet))
                        _summary.text += " · " + thisWeekGrantNet;
                    string lastWeekTrainingConstruction = HqDisplayQueriesHud.LastWeekTrainingConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingConstruction))
                        _summary.text += " · " + lastWeekTrainingConstruction;
                    string lifetimeFinesGrey = HqDisplayQueriesHud.LifetimeFinesGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesGrey))
                        _summary.text += " · " + lifetimeFinesGrey;
                    string thisWeekChipGpu = HqDisplayQueriesHud.ThisWeekChipGpuSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipGpu))
                        _summary.text += " · " + thisWeekChipGpu;
                    string lastWeekHiringCapital = HqDisplayQueriesHud.LastWeekHiringCapitalSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringCapital))
                        _summary.text += " · " + lastWeekHiringCapital;
                    string lastWeekRoomPower = HqDisplayQueriesHud.LastWeekRoomPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomPower))
                        _summary.text += " · " + lastWeekRoomPower;
                    string thisWeekGreyConstruction = HqDisplayQueriesHud.ThisWeekGreyConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyConstruction))
                        _summary.text += " · " + thisWeekGreyConstruction;
                    string lastWeekChipPatent = HqDisplayQueriesHud.LastWeekChipPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipPatent))
                        _summary.text += " · " + lastWeekChipPatent;
                    string lifetimeRevenueGrants = HqDisplayQueriesHud.LifetimeRevenueGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueGrants))
                        _summary.text += " · " + lifetimeRevenueGrants;
                    string thisWeekFinesTraining = HqDisplayQueriesHud.ThisWeekFinesTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesTraining))
                        _summary.text += " · " + thisWeekFinesTraining;
                    string lastWeekGpuGrey = HqDisplayQueriesHud.LastWeekGpuGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuGrey))
                        _summary.text += " · " + lastWeekGpuGrey;
                    string thisWeekNetChip = HqDisplayQueriesHud.ThisWeekNetChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetChip))
                        _summary.text += " · " + thisWeekNetChip;
                    string lastWeekGrantTraining = HqDisplayQueriesHud.LastWeekGrantTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantTraining))
                        _summary.text += " · " + lastWeekGrantTraining;
                    string thisWeekRevenuePower = HqDisplayQueriesHud.ThisWeekRevenuePowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenuePower))
                        _summary.text += " · " + thisWeekRevenuePower;
                    string lifetimeChipDev = HqDisplayQueriesHud.LifetimeChipDevSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipDev))
                        _summary.text += " · " + lifetimeChipDev;
                    string lastWeekConstructionChip = HqDisplayQueriesHud.LastWeekConstructionChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionChip))
                        _summary.text += " · " + lastWeekConstructionChip;
                    string thisWeekGreyTraining = HqDisplayQueriesHud.ThisWeekGreyTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyTraining))
                        _summary.text += " · " + thisWeekGreyTraining;
                    string lastWeekFinesCapital = HqDisplayQueriesHud.LastWeekFinesCapitalSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesCapital))
                        _summary.text += " · " + lastWeekFinesCapital;
                    string thisWeekGpuSalaries = HqDisplayQueriesHud.ThisWeekGpuSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuSalaries))
                        _summary.text += " · " + thisWeekGpuSalaries;
                    string lastWeekLandPatent = HqDisplayQueriesHud.LastWeekLandPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandPatent))
                        _summary.text += " · " + lastWeekLandPatent;
                    string lifetimeLaunderGrants = HqDisplayQueriesHud.LifetimeLaunderGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderGrants))
                        _summary.text += " · " + lifetimeLaunderGrants;
                    string thisWeekConstructionNet = HqDisplayQueriesHud.ThisWeekConstructionNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionNet))
                        _summary.text += " · " + thisWeekConstructionNet;
                    string lastWeekRoomGrey = HqDisplayQueriesHud.LastWeekRoomGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomGrey))
                        _summary.text += " · " + lastWeekRoomGrey;
                    string thisWeekChipFines = HqDisplayQueriesHud.ThisWeekChipFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipFines))
                        _summary.text += " · " + thisWeekChipFines;
                    string lastWeekHardwareHiring = HqDisplayQueriesHud.LastWeekHardwareHiringSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareHiring))
                        _summary.text += " · " + lastWeekHardwareHiring;
                    string thisWeekRevenueSalaries = HqDisplayQueriesHud.ThisWeekRevenueSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueSalaries))
                        _summary.text += " · " + thisWeekRevenueSalaries;
                    string lifetimeNetLaunder = HqDisplayQueriesHud.LifetimeNetLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetLaunder))
                        _summary.text += " · " + lifetimeNetLaunder;
                    string lastWeekSiteTraining = HqDisplayQueriesHud.LastWeekSiteTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteTraining))
                        _summary.text += " · " + lastWeekSiteTraining;
                    string thisWeekGreyChip = HqDisplayQueriesHud.ThisWeekGreyChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyChip))
                        _summary.text += " · " + thisWeekGreyChip;
                    string lastWeekPowerNet = HqDisplayQueriesHud.LastWeekPowerNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerNet))
                        _summary.text += " · " + lastWeekPowerNet;
                    string thisWeekConstructionGrey = HqDisplayQueriesHud.ThisWeekConstructionGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionGrey))
                        _summary.text += " · " + thisWeekConstructionGrey;
                    string lastWeekGpuLand = HqDisplayQueriesHud.LastWeekGpuLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuLand))
                        _summary.text += " · " + lastWeekGpuLand;
                    string lifetimePowerChip = HqDisplayQueriesHud.LifetimePowerChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerChip))
                        _summary.text += " · " + lifetimePowerChip;
                    string thisWeekGpuGrey = HqDisplayQueriesHud.ThisWeekGpuGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuGrey))
                        _summary.text += " · " + thisWeekGpuGrey;
                    string lastWeekPatentNet = HqDisplayQueriesHud.LastWeekPatentNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentNet))
                        _summary.text += " · " + lastWeekPatentNet;
                    string thisWeekTrainingGrant = HqDisplayQueriesHud.ThisWeekTrainingGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingGrant))
                        _summary.text += " · " + thisWeekTrainingGrant;
                    string lastWeekCapitalHardware = HqDisplayQueriesHud.LastWeekCapitalHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalHardware))
                        _summary.text += " · " + lastWeekCapitalHardware;
                    string thisWeekFinesNet = HqDisplayQueriesHud.ThisWeekFinesNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesNet))
                        _summary.text += " · " + thisWeekFinesNet;
                    string lifetimeGrantsChip = HqDisplayQueriesHud.LifetimeGrantsChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantsChip))
                        _summary.text += " · " + lifetimeGrantsChip;
                    string lastWeekLaunderPower = HqDisplayQueriesHud.LastWeekLaunderPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderPower))
                        _summary.text += " · " + lastWeekLaunderPower;
                    string thisWeekSalariesConstruction = HqDisplayQueriesHud.ThisWeekSalariesConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesConstruction))
                        _summary.text += " · " + thisWeekSalariesConstruction;
                    string lastWeekChipTraining = HqDisplayQueriesHud.LastWeekChipTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipTraining))
                        _summary.text += " · " + lastWeekChipTraining;
                    string thisWeekGrantsPower = HqDisplayQueriesHud.ThisWeekGrantsPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantsPower))
                        _summary.text += " · " + thisWeekGrantsPower;
                    string lastWeekRoomLand = HqDisplayQueriesHud.LastWeekRoomLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomLand))
                        _summary.text += " · " + lastWeekRoomLand;
                    string lifetimeFinesTraining = HqDisplayQueriesHud.LifetimeFinesTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesTraining))
                        _summary.text += " · " + lifetimeFinesTraining;
                    string thisWeekNetRevenue = HqDisplayQueriesHud.ThisWeekNetRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetRevenue))
                        _summary.text += " · " + thisWeekNetRevenue;
                    string lastWeekHiringSite = HqDisplayQueriesHud.LastWeekHiringSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringSite))
                        _summary.text += " · " + lastWeekHiringSite;
                    string thisWeekChipRent = HqDisplayQueriesHud.ThisWeekChipRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipRent))
                        _summary.text += " · " + thisWeekChipRent;
                    string lastWeekPatentGrey = HqDisplayQueriesHud.LastWeekPatentGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentGrey))
                        _summary.text += " · " + lastWeekPatentGrey;
                    string thisWeekRentNet = HqDisplayQueriesHud.ThisWeekRentNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentNet))
                        _summary.text += " · " + thisWeekRentNet;
                    string lifetimeRevenueLaunder = HqDisplayQueriesHud.LifetimeRevenueLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueLaunder))
                        _summary.text += " · " + lifetimeRevenueLaunder;
                    string lastWeekConstructionPower = HqDisplayQueriesHud.LastWeekConstructionPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionPower))
                        _summary.text += " · " + lastWeekConstructionPower;
                    string lastWeekSalariesGrey = HqDisplayQueriesHud.LastWeekSalariesGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesGrey))
                        _summary.text += " · " + lastWeekSalariesGrey;
                    string thisWeekGrantChip = HqDisplayQueriesHud.ThisWeekGrantChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantChip))
                        _summary.text += " · " + thisWeekGrantChip;
                    string lastWeekHardwareSite = HqDisplayQueriesHud.LastWeekHardwareSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareSite))
                        _summary.text += " · " + lastWeekHardwareSite;
                    string thisWeekFinesConstruction = HqDisplayQueriesHud.ThisWeekFinesConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesConstruction))
                        _summary.text += " · " + thisWeekFinesConstruction;
                    string lifetimeGreyTraining = HqDisplayQueriesHud.LifetimeGreyTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyTraining))
                        _summary.text += " · " + lifetimeGreyTraining;
                    string lastWeekGrantLaunder = HqDisplayQueriesHud.LastWeekGrantLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantLaunder))
                        _summary.text += " · " + lastWeekGrantLaunder;
                    string thisWeekGrantPower = HqDisplayQueriesHud.ThisWeekGrantPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantPower))
                        _summary.text += " · " + thisWeekGrantPower;
                    string lastWeekTrainingGrey = HqDisplayQueriesHud.LastWeekTrainingGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingGrey))
                        _summary.text += " · " + lastWeekTrainingGrey;
                    string lastWeekRevenueCapital = HqDisplayQueriesHud.LastWeekRevenueCapitalSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueCapital))
                        _summary.text += " · " + lastWeekRevenueCapital;
                    string thisWeekNetGrant = HqDisplayQueriesHud.ThisWeekNetGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetGrant))
                        _summary.text += " · " + thisWeekNetGrant;
                    string lifetimeLaunderNet = HqDisplayQueriesHud.LifetimeLaunderNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderNet))
                        _summary.text += " · " + lifetimeLaunderNet;
                    string lastWeekFinesHardware = HqDisplayQueriesHud.LastWeekFinesHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesHardware))
                        _summary.text += " · " + lastWeekFinesHardware;
                    string thisWeekSalariesPower = HqDisplayQueriesHud.ThisWeekSalariesPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesPower))
                        _summary.text += " · " + thisWeekSalariesPower;
                    string lastWeekGrantSite = HqDisplayQueriesHud.LastWeekGrantSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantSite))
                        _summary.text += " · " + lastWeekGrantSite;
                    string thisWeekConstructionChip = HqDisplayQueriesHud.ThisWeekConstructionChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionChip))
                        _summary.text += " · " + thisWeekConstructionChip;
                    string lastWeekNetLand = HqDisplayQueriesHud.LastWeekNetLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetLand))
                        _summary.text += " · " + lastWeekNetLand;
                    string lifetimeHardwareTraining = HqDisplayQueriesHud.LifetimeHardwareTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareTraining))
                        _summary.text += " · " + lifetimeHardwareTraining;
                    string thisWeekGreyGrant = HqDisplayQueriesHud.ThisWeekGreyGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyGrant))
                        _summary.text += " · " + thisWeekGreyGrant;
                    string lastWeekChipPower = HqDisplayQueriesHud.LastWeekChipPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipPower))
                        _summary.text += " · " + lastWeekChipPower;
                    string thisWeekTrainingFines = HqDisplayQueriesHud.ThisWeekTrainingFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingFines))
                        _summary.text += " · " + thisWeekTrainingFines;
                    string lastWeekSalariesSite = HqDisplayQueriesHud.LastWeekSalariesSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesSite))
                        _summary.text += " · " + lastWeekSalariesSite;
                    string thisWeekRevenueGrey = HqDisplayQueriesHud.ThisWeekRevenueGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueGrey))
                        _summary.text += " · " + thisWeekRevenueGrey;
                    string lifetimePatentGrants = HqDisplayQueriesHud.LifetimePatentGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentGrants))
                        _summary.text += " · " + lifetimePatentGrants;
                    string lastWeekTrainingCapital = HqDisplayQueriesHud.LastWeekTrainingCapitalSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingCapital))
                        _summary.text += " · " + lastWeekTrainingCapital;
                    string thisWeekGpuGrant = HqDisplayQueriesHud.ThisWeekGpuGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuGrant))
                        _summary.text += " · " + thisWeekGpuGrant;
                    string lastWeekGreyLand = HqDisplayQueriesHud.LastWeekGreyLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyLand))
                        _summary.text += " · " + lastWeekGreyLand;
                    string thisWeekNetConstruction = HqDisplayQueriesHud.ThisWeekNetConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetConstruction))
                        _summary.text += " · " + thisWeekNetConstruction;
                    string lastWeekRevenueLaunder = HqDisplayQueriesHud.LastWeekRevenueLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueLaunder))
                        _summary.text += " · " + lastWeekRevenueLaunder;
                    string lifetimeSiteChip = HqDisplayQueriesHud.LifetimeSiteChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSiteChip))
                        _summary.text += " · " + lifetimeSiteChip;
                    string thisWeekFinesGrey = HqDisplayQueriesHud.ThisWeekFinesGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesGrey))
                        _summary.text += " · " + thisWeekFinesGrey;
                    string lastWeekHardwareNet = HqDisplayQueriesHud.LastWeekHardwareNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareNet))
                        _summary.text += " · " + lastWeekHardwareNet;
                    string thisWeekGrantTraining2 = HqDisplayQueriesHud.ThisWeekGrantTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantTraining2))
                        _summary.text += " · " + thisWeekGrantTraining2;
                    string lastWeekPatentPower = HqDisplayQueriesHud.LastWeekPatentPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentPower))
                        _summary.text += " · " + lastWeekPatentPower;
                    string thisWeekChipGrant = HqDisplayQueriesHud.ThisWeekChipGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipGrant))
                        _summary.text += " · " + thisWeekChipGrant;
                    string lifetimeNetChip = HqDisplayQueriesHud.LifetimeNetChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetChip))
                        _summary.text += " · " + lifetimeNetChip;
                    string lastWeekSiteGrey = HqDisplayQueriesHud.LastWeekSiteGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteGrey))
                        _summary.text += " · " + lastWeekSiteGrey;
                    string thisWeekRevenueFines = HqDisplayQueriesHud.ThisWeekRevenueFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueFines))
                        _summary.text += " · " + thisWeekRevenueFines;
                    string lastWeekGrantHardware = HqDisplayQueriesHud.LastWeekGrantHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantHardware))
                        _summary.text += " · " + lastWeekGrantHardware;
                    string thisWeekRentGrant = HqDisplayQueriesHud.ThisWeekRentGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentGrant))
                        _summary.text += " · " + thisWeekRentGrant;
                    string lastWeekFinesSite = HqDisplayQueriesHud.LastWeekFinesSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesSite))
                        _summary.text += " · " + lastWeekFinesSite;
                    string lifetimeGreyPower = HqDisplayQueriesHud.LifetimeGreyPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyPower))
                        _summary.text += " · " + lifetimeGreyPower;
                    string thisWeekSalariesGrant = HqDisplayQueriesHud.ThisWeekSalariesGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesGrant))
                        _summary.text += " · " + thisWeekSalariesGrant;
                    string lastWeekChipLand = HqDisplayQueriesHud.LastWeekChipLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipLand))
                        _summary.text += " · " + lastWeekChipLand;
                    string lastWeekUpkeepGpu = HqDisplayQueriesHud.LastWeekUpkeepGpuSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekUpkeepGpu))
                        _summary.text += " · " + lastWeekUpkeepGpu;
                    string lastWeekTrainingSite = HqDisplayQueriesHud.LastWeekTrainingSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingSite))
                        _summary.text += " · " + lastWeekTrainingSite;
                    string thisWeekNetChipGrant = HqDisplayQueriesHud.ThisWeekNetChipGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetChipGrant))
                        _summary.text += " · " + thisWeekNetChipGrant;
                    string lifetimeLaunderChip = HqDisplayQueriesHud.LifetimeLaunderChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderChip))
                        _summary.text += " · " + lifetimeLaunderChip;
                    string lastWeekRoomPatent = HqDisplayQueriesHud.LastWeekRoomPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomPatent))
                        _summary.text += " · " + lastWeekRoomPatent;
                    string thisWeekPowerGrey = HqDisplayQueriesHud.ThisWeekPowerGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerGrey))
                        _summary.text += " · " + thisWeekPowerGrey;
                    string lastWeekRevenueHardware = HqDisplayQueriesHud.LastWeekRevenueHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueHardware))
                        _summary.text += " · " + lastWeekRevenueHardware;
                    string weeklyCompute = HqDisplayQueriesHud.WeeklyComputeLine(sim.Config, sim.State, lab);
                    if (!string.IsNullOrEmpty(weeklyCompute))
                        _summary.text += " · " + weeklyCompute;
            _status.text = string.IsNullOrEmpty(_host.StatusMessage)
                ? "Start a run after researching the line."
                : _host.StatusMessage;
        }

        void AddRunRow(ModelState model)
        {
            var row = new GameObject("Run_" + model.Id);
            row.transform.SetParent(_activeRuns, false);
            var rect = row.AddComponent<RectTransform>();
            rect.sizeDelta = new Vector2(0f, 22f);

            string label = TrainingRunFormatter.LineLabel(_host.Simulation.Config, model) + " " +
                             TrainingRunFormatter.ProgressPermille(model) + "‰";
            HqUiFactory.Label(row.transform, "Label", label, 12, TextAnchor.MiddleLeft);
        }
    }
}