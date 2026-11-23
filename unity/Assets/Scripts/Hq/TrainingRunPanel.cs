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
                    string thisWeekGrantConstruction = HqDisplayQueriesHud.ThisWeekGrantConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantConstruction))
                        _summary.text += " · " + thisWeekGrantConstruction;
                    string lifetimeFinesChip = HqDisplayQueriesHud.LifetimeFinesChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesChip))
                        _summary.text += " · " + lifetimeFinesChip;
                    string lastWeekLaunderChip = HqDisplayQueriesHud.LastWeekLaunderChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderChip))
                        _summary.text += " · " + lastWeekLaunderChip;
                    string lastWeekCapitalLaunder = HqDisplayQueriesHud.LastWeekCapitalLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalLaunder))
                        _summary.text += " · " + lastWeekCapitalLaunder;
                    string thisWeekGrantChipNet = HqDisplayQueriesHud.ThisWeekGrantChipNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantChipNet))
                        _summary.text += " · " + thisWeekGrantChipNet;
                    string lastWeekPatentTraining = HqDisplayQueriesHud.LastWeekPatentTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentTraining))
                        _summary.text += " · " + lastWeekPatentTraining;
                    string lastWeekGreyHardware = HqDisplayQueriesHud.LastWeekGreyHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyHardware))
                        _summary.text += " · " + lastWeekGreyHardware;
                    string thisWeekNetPower = HqDisplayQueriesHud.ThisWeekNetPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetPower))
                        _summary.text += " · " + thisWeekNetPower;
                    string lifetimeRevenueNet = HqDisplayQueriesHud.LifetimeRevenueNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueNet))
                        _summary.text += " · " + lifetimeRevenueNet;
                    string lastWeekGrantPatent = HqDisplayQueriesHud.LastWeekGrantPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantPatent))
                        _summary.text += " · " + lastWeekGrantPatent;
                    string thisWeekChipBuild = HqDisplayQueriesHud.ThisWeekChipBuildSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipBuild))
                        _summary.text += " · " + thisWeekChipBuild;
                    string lastWeekLaunderGrey = HqDisplayQueriesHud.LastWeekLaunderGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderGrey))
                        _summary.text += " · " + lastWeekLaunderGrey;
                    string thisWeekGrantRevenue = HqDisplayQueriesHud.ThisWeekGrantRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantRevenue))
                        _summary.text += " · " + thisWeekGrantRevenue;
                    string lastWeekSiteRoom = HqDisplayQueriesHud.LastWeekSiteRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteRoom))
                        _summary.text += " · " + lastWeekSiteRoom;
                    string lifetimeTrainingGrants = HqDisplayQueriesHud.LifetimeTrainingGrantsSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingGrants))
                        _summary.text += " · " + lifetimeTrainingGrants;
                    string lastWeekCapitalSite = HqDisplayQueriesHud.LastWeekCapitalSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalSite))
                        _summary.text += " · " + lastWeekCapitalSite;
                    string thisWeekRentChip = HqDisplayQueriesHud.ThisWeekRentChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentChip))
                        _summary.text += " · " + thisWeekRentChip;
                    string lastWeekFinesGrant = HqDisplayQueriesHud.LastWeekFinesGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesGrant))
                        _summary.text += " · " + lastWeekFinesGrant;
                    string lastWeekPowerLand = HqDisplayQueriesHud.LastWeekPowerLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerLand))
                        _summary.text += " · " + lastWeekPowerLand;
                    string thisWeekGrantGrey = HqDisplayQueriesHud.ThisWeekGrantGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantGrey))
                        _summary.text += " · " + thisWeekGrantGrey;
                    string lifetimeHardwareChip = HqDisplayQueriesHud.LifetimeHardwareChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareChip))
                        _summary.text += " · " + lifetimeHardwareChip;
                    string lastWeekRoomTraining = HqDisplayQueriesHud.LastWeekRoomTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomTraining))
                        _summary.text += " · " + lastWeekRoomTraining;
                    string thisWeekTrainingNet = HqDisplayQueriesHud.ThisWeekTrainingNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingNet))
                        _summary.text += " · " + thisWeekTrainingNet;
                    string lastWeekChipGrant = HqDisplayQueriesHud.LastWeekChipGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipGrant))
                        _summary.text += " · " + lastWeekChipGrant;
                    string thisWeekGpuPower = HqDisplayQueriesHud.ThisWeekGpuPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuPower))
                        _summary.text += " · " + thisWeekGpuPower;
                    string lifetimePatentChip = HqDisplayQueriesHud.LifetimePatentChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentChip))
                        _summary.text += " · " + lifetimePatentChip;
                    string lastWeekNetConstruction = HqDisplayQueriesHud.LastWeekNetConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetConstruction))
                        _summary.text += " · " + lastWeekNetConstruction;
                    string lastWeekHiringHardware = HqDisplayQueriesHud.LastWeekHiringHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringHardware))
                        _summary.text += " · " + lastWeekHiringHardware;
                    string thisWeekSalariesChip = HqDisplayQueriesHud.ThisWeekSalariesChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesChip))
                        _summary.text += " · " + thisWeekSalariesChip;
                    string lastWeekGreyPatent = HqDisplayQueriesHud.LastWeekGreyPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyPatent))
                        _summary.text += " · " + lastWeekGreyPatent;
                    string lastWeekLandGrant = HqDisplayQueriesHud.LastWeekLandGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandGrant))
                        _summary.text += " · " + lastWeekLandGrant;
                    string thisWeekFinesGrant = HqDisplayQueriesHud.ThisWeekFinesGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesGrant))
                        _summary.text += " · " + thisWeekFinesGrant;
                    string lifetimeSiteGrey = HqDisplayQueriesHud.LifetimeSiteGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSiteGrey))
                        _summary.text += " · " + lifetimeSiteGrey;
                    string lastWeekGpuHardware = HqDisplayQueriesHud.LastWeekGpuHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuHardware))
                        _summary.text += " · " + lastWeekGpuHardware;
                    string thisWeekChipGreyGrant = HqDisplayQueriesHud.ThisWeekChipGreyGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipGreyGrant))
                        _summary.text += " · " + thisWeekChipGreyGrant;
                    string lastWeekTrainingHiring = HqDisplayQueriesHud.LastWeekTrainingHiringSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingHiring))
                        _summary.text += " · " + lastWeekTrainingHiring;
                    string thisWeekRevenueConstruction = HqDisplayQueriesHud.ThisWeekRevenueConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueConstruction))
                        _summary.text += " · " + thisWeekRevenueConstruction;
                    string lifetimeGrantFines = HqDisplayQueriesHud.LifetimeGrantFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantFines))
                        _summary.text += " · " + lifetimeGrantFines;
                    string thisWeekSalariesGrey = HqDisplayQueriesHud.ThisWeekSalariesGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesGrey))
                        _summary.text += " · " + thisWeekSalariesGrey;
                    string lastWeekRevenueSite = HqDisplayQueriesHud.LastWeekRevenueSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueSite))
                        _summary.text += " · " + lastWeekRevenueSite;
                    string thisWeekRentFines = HqDisplayQueriesHud.ThisWeekRentFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentFines))
                        _summary.text += " · " + thisWeekRentFines;
                    string lastWeekConstructionGrant = HqDisplayQueriesHud.LastWeekConstructionGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionGrant))
                        _summary.text += " · " + lastWeekConstructionGrant;
                    string thisWeekNetSalaries = HqDisplayQueriesHud.ThisWeekNetSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetSalaries))
                        _summary.text += " · " + thisWeekNetSalaries;
                    string lifetimeLaunderPower = HqDisplayQueriesHud.LifetimeLaunderPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderPower))
                        _summary.text += " · " + lifetimeLaunderPower;
                    string lastWeekPatentLand = HqDisplayQueriesHud.LastWeekPatentLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentLand))
                        _summary.text += " · " + lastWeekPatentLand;
                    string thisWeekPowerConstruction = HqDisplayQueriesHud.ThisWeekPowerConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerConstruction))
                        _summary.text += " · " + thisWeekPowerConstruction;
                    string lastWeekLaunderCapital = HqDisplayQueriesHud.LastWeekLaunderCapitalSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderCapital))
                        _summary.text += " · " + lastWeekLaunderCapital;
                    string thisWeekGreySalaries = HqDisplayQueriesHud.ThisWeekGreySalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreySalaries))
                        _summary.text += " · " + thisWeekGreySalaries;
                    string lifetimeNetGrant = HqDisplayQueriesHud.LifetimeNetGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetGrant))
                        _summary.text += " · " + lifetimeNetGrant;
                    string lastWeekHardwareTraining = HqDisplayQueriesHud.LastWeekHardwareTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareTraining))
                        _summary.text += " · " + lastWeekHardwareTraining;
                    string thisWeekGrantNetChip = HqDisplayQueriesHud.ThisWeekGrantNetChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantNetChip))
                        _summary.text += " · " + thisWeekGrantNetChip;
                    string lastWeekSitePower = HqDisplayQueriesHud.LastWeekSitePowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSitePower))
                        _summary.text += " · " + lastWeekSitePower;
                    string thisWeekGrantFines = HqDisplayQueriesHud.ThisWeekGrantFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantFines))
                        _summary.text += " · " + thisWeekGrantFines;
                    string lastWeekRoomNet = HqDisplayQueriesHud.LastWeekRoomNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomNet))
                        _summary.text += " · " + lastWeekRoomNet;
                    string thisWeekChipDev = HqDisplayQueriesHud.ThisWeekChipDevSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipDev))
                        _summary.text += " · " + thisWeekChipDev;
                    string lastWeekFinesGpu = HqDisplayQueriesHud.LastWeekFinesGpuSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesGpu))
                        _summary.text += " · " + lastWeekFinesGpu;
                    string lifetimeRevenueTraining = HqDisplayQueriesHud.LifetimeRevenueTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueTraining))
                        _summary.text += " · " + lifetimeRevenueTraining;
                    string thisWeekHiringSalaries = HqDisplayQueriesHud.ThisWeekHiringSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHiringSalaries))
                        _summary.text += " · " + thisWeekHiringSalaries;
                    string lastWeekGrantLand = HqDisplayQueriesHud.LastWeekGrantLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantLand))
                        _summary.text += " · " + lastWeekGrantLand;
                    string thisWeekPatentGrey = HqDisplayQueriesHud.ThisWeekPatentGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentGrey))
                        _summary.text += " · " + thisWeekPatentGrey;
                    string lastWeekPowerRent = HqDisplayQueriesHud.LastWeekPowerRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerRent))
                        _summary.text += " · " + lastWeekPowerRent;
                    string thisWeekCapitalLand = HqDisplayQueriesHud.ThisWeekCapitalLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalLand))
                        _summary.text += " · " + thisWeekCapitalLand;
                    string lastWeekGreyPower = HqDisplayQueriesHud.LastWeekGreyPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyPower))
                        _summary.text += " · " + lastWeekGreyPower;
                    string thisWeekSiteRevenue = HqDisplayQueriesHud.ThisWeekSiteRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteRevenue))
                        _summary.text += " · " + thisWeekSiteRevenue;
                    string lastWeekNetChip = HqDisplayQueriesHud.LastWeekNetChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetChip))
                        _summary.text += " · " + lastWeekNetChip;
                    string lifetimeConstructionGrant = HqDisplayQueriesHud.LifetimeConstructionGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionGrant))
                        _summary.text += " · " + lifetimeConstructionGrant;
                    string thisWeekLaunderFines = HqDisplayQueriesHud.ThisWeekLaunderFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderFines))
                        _summary.text += " · " + thisWeekLaunderFines;
                    string lastWeekTrainingPower = HqDisplayQueriesHud.LastWeekTrainingPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingPower))
                        _summary.text += " · " + lastWeekTrainingPower;
                    string lifetimePatentRevenue = HqDisplayQueriesHud.LifetimePatentRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentRevenue))
                        _summary.text += " · " + lifetimePatentRevenue;
                    string lastWeekSalariesConstruction = HqDisplayQueriesHud.LastWeekSalariesConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesConstruction))
                        _summary.text += " · " + lastWeekSalariesConstruction;
                    string thisWeekChipDevGrant = HqDisplayQueriesHud.ThisWeekChipDevGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipDevGrant))
                        _summary.text += " · " + thisWeekChipDevGrant;
                    string lastWeekRentRevenue = HqDisplayQueriesHud.LastWeekRentRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentRevenue))
                        _summary.text += " · " + lastWeekRentRevenue;
                    string thisWeekHardwareFines = HqDisplayQueriesHud.ThisWeekHardwareFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareFines))
                        _summary.text += " · " + thisWeekHardwareFines;
                    string lifetimeRoomGrant = HqDisplayQueriesHud.LifetimeRoomGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRoomGrant))
                        _summary.text += " · " + lifetimeRoomGrant;
                    string lastWeekChipFines = HqDisplayQueriesHud.LastWeekChipFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipFines))
                        _summary.text += " · " + lastWeekChipFines;
                    string thisWeekLandTraining = HqDisplayQueriesHud.ThisWeekLandTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLandTraining))
                        _summary.text += " · " + thisWeekLandTraining;
                    string lastWeekGrantGpu = HqDisplayQueriesHud.LastWeekGrantGpuSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantGpu))
                        _summary.text += " · " + lastWeekGrantGpu;
                    string lifetimeNetPower = HqDisplayQueriesHud.LifetimeNetPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetPower))
                        _summary.text += " · " + lifetimeNetPower;
                    string thisWeekCapitalTraining = HqDisplayQueriesHud.ThisWeekCapitalTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalTraining))
                        _summary.text += " · " + thisWeekCapitalTraining;
                    string lastWeekSiteChip = HqDisplayQueriesHud.LastWeekSiteChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteChip))
                        _summary.text += " · " + lastWeekSiteChip;
                    string lifetimeGreyRoom = HqDisplayQueriesHud.LifetimeGreyRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyRoom))
                        _summary.text += " · " + lifetimeGreyRoom;
                    string lastWeekGrantRevenue = HqDisplayQueriesHud.LastWeekGrantRevenueSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantRevenue))
                        _summary.text += " · " + lastWeekGrantRevenue;
                    string thisWeekLaunderGrant = HqDisplayQueriesHud.ThisWeekLaunderGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderGrant))
                        _summary.text += " · " + thisWeekLaunderGrant;
                    string lastWeekPatentGrant = HqDisplayQueriesHud.LastWeekPatentGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentGrant))
                        _summary.text += " · " + lastWeekPatentGrant;
                    string thisWeekChipRoom = HqDisplayQueriesHud.ThisWeekChipRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipRoom))
                        _summary.text += " · " + thisWeekChipRoom;
                    string lastWeekGreyConstruction = HqDisplayQueriesHud.LastWeekGreyConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyConstruction))
                        _summary.text += " · " + lastWeekGreyConstruction;
                    string thisWeekNetLand = HqDisplayQueriesHud.ThisWeekNetLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetLand))
                        _summary.text += " · " + thisWeekNetLand;
                    string lastWeekSalariesRent = HqDisplayQueriesHud.LastWeekSalariesRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesRent))
                        _summary.text += " · " + lastWeekSalariesRent;
                    string thisWeekGrantLand = HqDisplayQueriesHud.ThisWeekGrantLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantLand))
                        _summary.text += " · " + thisWeekGrantLand;
                    string lifetimeCapitalGrey = HqDisplayQueriesHud.LifetimeCapitalGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeCapitalGrey))
                        _summary.text += " · " + lifetimeCapitalGrey;
                    string lastWeekRoomFines = HqDisplayQueriesHud.LastWeekRoomFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomFines))
                        _summary.text += " · " + lastWeekRoomFines;
                    string thisWeekGpuHardware = HqDisplayQueriesHud.ThisWeekGpuHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuHardware))
                        _summary.text += " · " + thisWeekGpuHardware;
                    string lifetimeFinesRent = HqDisplayQueriesHud.LifetimeFinesRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesRent))
                        _summary.text += " · " + lifetimeFinesRent;
                    string lastWeekCapitalConstruction = HqDisplayQueriesHud.LastWeekCapitalConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalConstruction))
                        _summary.text += " · " + lastWeekCapitalConstruction;
                    string thisWeekGreyLand = HqDisplayQueriesHud.ThisWeekGreyLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyLand))
                        _summary.text += " · " + thisWeekGreyLand;
                    string lastWeekLandNet = HqDisplayQueriesHud.LastWeekLandNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandNet))
                        _summary.text += " · " + lastWeekLandNet;
                    string thisWeekChipPatent = HqDisplayQueriesHud.ThisWeekChipPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipPatent))
                        _summary.text += " · " + thisWeekChipPatent;
                    string lifetimeHiringSalaries = HqDisplayQueriesHud.LifetimeHiringSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHiringSalaries))
                        _summary.text += " · " + lifetimeHiringSalaries;
                    string lastWeekHardwareChip = HqDisplayQueriesHud.LastWeekHardwareChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareChip))
                        _summary.text += " · " + lastWeekHardwareChip;
                    string lifetimeTrainingGrant = HqDisplayQueriesHud.LifetimeTrainingGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingGrant))
                        _summary.text += " · " + lifetimeTrainingGrant;
                    string lastWeekNetGrant = HqDisplayQueriesHud.LastWeekNetGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetGrant))
                        _summary.text += " · " + lastWeekNetGrant;
                    string thisWeekSiteGrant = HqDisplayQueriesHud.ThisWeekSiteGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteGrant))
                        _summary.text += " · " + thisWeekSiteGrant;
                    string lastWeekLaunderRoom = HqDisplayQueriesHud.LastWeekLaunderRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderRoom))
                        _summary.text += " · " + lastWeekLaunderRoom;
                    string lastWeekHiringChip = HqDisplayQueriesHud.LastWeekHiringChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringChip))
                        _summary.text += " · " + lastWeekHiringChip;
                    string lifetimeLandGrant = HqDisplayQueriesHud.LifetimeLandGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLandGrant))
                        _summary.text += " · " + lifetimeLandGrant;
                    string thisWeekPowerSalaries = HqDisplayQueriesHud.ThisWeekPowerSalariesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerSalaries))
                        _summary.text += " · " + thisWeekPowerSalaries;
                    string thisWeekCapitalChip = HqDisplayQueriesHud.ThisWeekCapitalChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalChip))
                        _summary.text += " · " + thisWeekCapitalChip;
                    string lifetimeRentGrey = HqDisplayQueriesHud.LifetimeRentGreySummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentGrey))
                        _summary.text += " · " + lifetimeRentGrey;
                    string lastWeekPatentChip = HqDisplayQueriesHud.LastWeekPatentChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentChip))
                        _summary.text += " · " + lastWeekPatentChip;
                    string thisWeekGrantRoom = HqDisplayQueriesHud.ThisWeekGrantRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantRoom))
                        _summary.text += " · " + thisWeekGrantRoom;
                    string thisWeekLaunderChip = HqDisplayQueriesHud.ThisWeekLaunderChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderChip))
                        _summary.text += " · " + thisWeekLaunderChip;
                    string lifetimeSiteTraining = HqDisplayQueriesHud.LifetimeSiteTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSiteTraining))
                        _summary.text += " · " + lifetimeSiteTraining;
                    string lastWeekGpuGrant = HqDisplayQueriesHud.LastWeekGpuGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuGrant))
                        _summary.text += " · " + lastWeekGpuGrant;
                    string lastWeekConstructionNet = HqDisplayQueriesHud.LastWeekConstructionNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionNet))
                        _summary.text += " · " + lastWeekConstructionNet;
                    string thisWeekHardwareGrant = HqDisplayQueriesHud.ThisWeekHardwareGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareGrant))
                        _summary.text += " · " + thisWeekHardwareGrant;
                    string lifetimeFinesLand = HqDisplayQueriesHud.LifetimeFinesLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesLand))
                        _summary.text += " · " + lifetimeFinesLand;
                    string lastWeekChipRoom = HqDisplayQueriesHud.LastWeekChipRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipRoom))
                        _summary.text += " · " + lastWeekChipRoom;
                    string thisWeekPatentLand = HqDisplayQueriesHud.ThisWeekPatentLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentLand))
                        _summary.text += " · " + thisWeekPatentLand;
                    string lastWeekRentLand = HqDisplayQueriesHud.LastWeekRentLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentLand))
                        _summary.text += " · " + lastWeekRentLand;
                    string thisWeekRoomTraining = HqDisplayQueriesHud.ThisWeekRoomTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRoomTraining))
                        _summary.text += " · " + thisWeekRoomTraining;
                    string lifetimeChipGrant = HqDisplayQueriesHud.LifetimeChipGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipGrant))
                        _summary.text += " · " + lifetimeChipGrant;
                    string lastWeekPowerGrant = HqDisplayQueriesHud.LastWeekPowerGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerGrant))
                        _summary.text += " · " + lastWeekPowerGrant;
                    string thisWeekRentLand = HqDisplayQueriesHud.ThisWeekRentLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentLand))
                        _summary.text += " · " + thisWeekRentLand;
                    string lastWeekLaunderTraining = HqDisplayQueriesHud.LastWeekLaunderTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderTraining))
                        _summary.text += " · " + lastWeekLaunderTraining;
                    string lifetimeNetRoom = HqDisplayQueriesHud.LifetimeNetRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetRoom))
                        _summary.text += " · " + lifetimeNetRoom;
                    string lifetimePowerGrant = HqDisplayQueriesHud.LifetimePowerGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerGrant))
                        _summary.text += " · " + lifetimePowerGrant;
                    string lastWeekGreyGrant = HqDisplayQueriesHud.LastWeekGreyGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyGrant))
                        _summary.text += " · " + lastWeekGreyGrant;
                    string thisWeekFinesLand = HqDisplayQueriesHud.ThisWeekFinesLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesLand))
                        _summary.text += " · " + thisWeekFinesLand;
                    string lifetimeHardwareRent = HqDisplayQueriesHud.LifetimeHardwareRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareRent))
                        _summary.text += " · " + lifetimeHardwareRent;
                    string lastWeekCapitalPower = HqDisplayQueriesHud.LastWeekCapitalPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalPower))
                        _summary.text += " · " + lastWeekCapitalPower;
                    string thisWeekPatentTraining = HqDisplayQueriesHud.ThisWeekPatentTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentTraining))
                        _summary.text += " · " + thisWeekPatentTraining;
                    string thisWeekLaunderPower = HqDisplayQueriesHud.ThisWeekLaunderPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderPower))
                        _summary.text += " · " + thisWeekLaunderPower;
                    string lifetimeGreyChip = HqDisplayQueriesHud.LifetimeGreyChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyChip))
                        _summary.text += " · " + lifetimeGreyChip;
                    string lastWeekRentConstruction = HqDisplayQueriesHud.LastWeekRentConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentConstruction))
                        _summary.text += " · " + lastWeekRentConstruction;
                    string lifetimeSalariesPower = HqDisplayQueriesHud.LifetimeSalariesPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSalariesPower))
                        _summary.text += " · " + lifetimeSalariesPower;
                    string lastWeekFinesRoom = HqDisplayQueriesHud.LastWeekFinesRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesRoom))
                        _summary.text += " · " + lastWeekFinesRoom;
                    string lastWeekLandChip = HqDisplayQueriesHud.LastWeekLandChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandChip))
                        _summary.text += " · " + lastWeekLandChip;
                    string thisWeekHardwareRoom = HqDisplayQueriesHud.ThisWeekHardwareRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareRoom))
                        _summary.text += " · " + thisWeekHardwareRoom;
                    string lifetimePatentGrant = HqDisplayQueriesHud.LifetimePatentGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentGrant))
                        _summary.text += " · " + lifetimePatentGrant;
                    string lastWeekGreyChip = HqDisplayQueriesHud.LastWeekGreyChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyChip))
                        _summary.text += " · " + lastWeekGreyChip;
                    string thisWeekCapitalGrant = HqDisplayQueriesHud.ThisWeekCapitalGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalGrant))
                        _summary.text += " · " + thisWeekCapitalGrant;
                    string lastWeekTrainingChip = HqDisplayQueriesHud.LastWeekTrainingChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingChip))
                        _summary.text += " · " + lastWeekTrainingChip;
                    string thisWeekSiteFines = HqDisplayQueriesHud.ThisWeekSiteFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteFines))
                        _summary.text += " · " + thisWeekSiteFines;
                    string thisWeekGrantPatent = HqDisplayQueriesHud.ThisWeekGrantPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantPatent))
                        _summary.text += " · " + thisWeekGrantPatent;
                    string lastWeekLaunderGrant = HqDisplayQueriesHud.LastWeekLaunderGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderGrant))
                        _summary.text += " · " + lastWeekLaunderGrant;
                    string thisWeekChipPower = HqDisplayQueriesHud.ThisWeekChipPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipPower))
                        _summary.text += " · " + thisWeekChipPower;
                    string lifetimeConstructionPower = HqDisplayQueriesHud.LifetimeConstructionPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionPower))
                        _summary.text += " · " + lifetimeConstructionPower;
                    string lastWeekHiringGrant = HqDisplayQueriesHud.LastWeekHiringGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringGrant))
                        _summary.text += " · " + lastWeekHiringGrant;
                    string thisWeekGreyRent = HqDisplayQueriesHud.ThisWeekGreyRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyRent))
                        _summary.text += " · " + thisWeekGreyRent;
                    string lastWeekSiteGrant = HqDisplayQueriesHud.LastWeekSiteGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteGrant))
                        _summary.text += " · " + lastWeekSiteGrant;
                    string thisWeekLaunderLand = HqDisplayQueriesHud.ThisWeekLaunderLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderLand))
                        _summary.text += " · " + thisWeekLaunderLand;
                    string lifetimeRentGrant = HqDisplayQueriesHud.LifetimeRentGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentGrant))
                        _summary.text += " · " + lifetimeRentGrant;
                    string lastWeekCapitalChip = HqDisplayQueriesHud.LastWeekCapitalChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalChip))
                        _summary.text += " · " + lastWeekCapitalChip;
                    string lifetimeGreyLand = HqDisplayQueriesHud.LifetimeGreyLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyLand))
                        _summary.text += " · " + lifetimeGreyLand;
                    string thisWeekConstructionLand = HqDisplayQueriesHud.ThisWeekConstructionLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionLand))
                        _summary.text += " · " + thisWeekConstructionLand;
                    string lastWeekPatentRoom = HqDisplayQueriesHud.LastWeekPatentRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentRoom))
                        _summary.text += " · " + lastWeekPatentRoom;
                    string thisWeekGrantHardware = HqDisplayQueriesHud.ThisWeekGrantHardwareSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantHardware))
                        _summary.text += " · " + thisWeekGrantHardware;
                    string lastWeekGreyRent = HqDisplayQueriesHud.LastWeekGreyRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyRent))
                        _summary.text += " · " + lastWeekGreyRent;
                    string thisWeekChipLand = HqDisplayQueriesHud.ThisWeekChipLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipLand))
                        _summary.text += " · " + thisWeekChipLand;
                    string thisWeekSiteNet = HqDisplayQueriesHud.ThisWeekSiteNetSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteNet))
                        _summary.text += " · " + thisWeekSiteNet;
                    string lastWeekGrantChip = HqDisplayQueriesHud.LastWeekGrantChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantChip))
                        _summary.text += " · " + lastWeekGrantChip;
                    string lifetimeLaunderGrant = HqDisplayQueriesHud.LifetimeLaunderGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderGrant))
                        _summary.text += " · " + lifetimeLaunderGrant;
                    string thisWeekRentTraining = HqDisplayQueriesHud.ThisWeekRentTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentTraining))
                        _summary.text += " · " + thisWeekRentTraining;
                    string lifetimeRoomChip = HqDisplayQueriesHud.LifetimeRoomChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRoomChip))
                        _summary.text += " · " + lifetimeRoomChip;
                    string lastWeekFinesChip = HqDisplayQueriesHud.LastWeekFinesChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesChip))
                        _summary.text += " · " + lastWeekFinesChip;
                    string thisWeekCapitalPower = HqDisplayQueriesHud.ThisWeekCapitalPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalPower))
                        _summary.text += " · " + thisWeekCapitalPower;
                    string lastWeekSiteLand = HqDisplayQueriesHud.LastWeekSiteLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteLand))
                        _summary.text += " · " + lastWeekSiteLand;
                    string lastWeekLaunderLand = HqDisplayQueriesHud.LastWeekLaunderLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderLand))
                        _summary.text += " · " + lastWeekLaunderLand;
                    string thisWeekGrantSite = HqDisplayQueriesHud.ThisWeekGrantSiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantSite))
                        _summary.text += " · " + thisWeekGrantSite;
                    string lifetimeChipLand = HqDisplayQueriesHud.LifetimeChipLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipLand))
                        _summary.text += " · " + lifetimeChipLand;
                    string lastWeekRentTraining = HqDisplayQueriesHud.LastWeekRentTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentTraining))
                        _summary.text += " · " + lastWeekRentTraining;
                    string lifetimeHiringPower = HqDisplayQueriesHud.LifetimeHiringPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHiringPower))
                        _summary.text += " · " + lifetimeHiringPower;
                    string thisWeekNetRoom = HqDisplayQueriesHud.ThisWeekNetRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetRoom))
                        _summary.text += " · " + thisWeekNetRoom;
                    string thisWeekLaunderRoom = HqDisplayQueriesHud.ThisWeekLaunderRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderRoom))
                        _summary.text += " · " + thisWeekLaunderRoom;
                    string lifetimeRentLand = HqDisplayQueriesHud.LifetimeRentLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentLand))
                        _summary.text += " · " + lifetimeRentLand;
                    string lastWeekSalariesGrant = HqDisplayQueriesHud.LastWeekSalariesGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesGrant))
                        _summary.text += " · " + lastWeekSalariesGrant;
                    string thisWeekSiteChip = HqDisplayQueriesHud.ThisWeekSiteChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteChip))
                        _summary.text += " · " + thisWeekSiteChip;
                    string thisWeekCapitalConstruction = HqDisplayQueriesHud.ThisWeekCapitalConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalConstruction))
                        _summary.text += " · " + thisWeekCapitalConstruction;
                    string lifetimeGrantLand = HqDisplayQueriesHud.LifetimeGrantLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantLand))
                        _summary.text += " · " + lifetimeGrantLand;
                    string lastWeekRoomGrant = HqDisplayQueriesHud.LastWeekRoomGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomGrant))
                        _summary.text += " · " + lastWeekRoomGrant;
                    string thisWeekPatentRent = HqDisplayQueriesHud.ThisWeekPatentRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentRent))
                        _summary.text += " · " + thisWeekPatentRent;
                    string thisWeekFinesRent = HqDisplayQueriesHud.ThisWeekFinesRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesRent))
                        _summary.text += " · " + thisWeekFinesRent;
                    string lifetimeChipRoom = HqDisplayQueriesHud.LifetimeChipRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipRoom))
                        _summary.text += " · " + lifetimeChipRoom;
                    string lastWeekConstructionFines = HqDisplayQueriesHud.LastWeekConstructionFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionFines))
                        _summary.text += " · " + lastWeekConstructionFines;
                    string lifetimeSalariesGrant = HqDisplayQueriesHud.LifetimeSalariesGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSalariesGrant))
                        _summary.text += " · " + lifetimeSalariesGrant;
                    string lastWeekRentGrant = HqDisplayQueriesHud.LastWeekRentGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentGrant))
                        _summary.text += " · " + lastWeekRentGrant;
                    string thisWeekLandChip = HqDisplayQueriesHud.ThisWeekLandChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLandChip))
                        _summary.text += " · " + thisWeekLandChip;
                    string lastWeekGreySite = HqDisplayQueriesHud.LastWeekGreySiteSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreySite))
                        _summary.text += " · " + lastWeekGreySite;
                    string thisWeekHardwareLand = HqDisplayQueriesHud.ThisWeekHardwareLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareLand))
                        _summary.text += " · " + thisWeekHardwareLand;
                    string lastWeekFinesLand = HqDisplayQueriesHud.LastWeekFinesLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesLand))
                        _summary.text += " · " + lastWeekFinesLand;
                    string lastWeekCapitalGrant = HqDisplayQueriesHud.LastWeekCapitalGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekCapitalGrant))
                        _summary.text += " · " + lastWeekCapitalGrant;
                    string thisWeekRoomGrant = HqDisplayQueriesHud.ThisWeekRoomGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRoomGrant))
                        _summary.text += " · " + thisWeekRoomGrant;
                    string thisWeekConstructionRent = HqDisplayQueriesHud.ThisWeekConstructionRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionRent))
                        _summary.text += " · " + thisWeekConstructionRent;
                    string thisWeekSalariesLand = HqDisplayQueriesHud.ThisWeekSalariesLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesLand))
                        _summary.text += " · " + thisWeekSalariesLand;
                    string lastWeekNetRoom = HqDisplayQueriesHud.LastWeekNetRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetRoom))
                        _summary.text += " · " + lastWeekNetRoom;
                    string lifetimeGrantChip = HqDisplayQueriesHud.LifetimeGrantChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantChip))
                        _summary.text += " · " + lifetimeGrantChip;
                    string thisWeekGrantLaunder = HqDisplayQueriesHud.ThisWeekGrantLaunderSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantLaunder))
                        _summary.text += " · " + thisWeekGrantLaunder;
                    string thisWeekSiteTraining = HqDisplayQueriesHud.ThisWeekSiteTrainingSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteTraining))
                        _summary.text += " · " + thisWeekSiteTraining;
                    string lifetimeRentChip = HqDisplayQueriesHud.LifetimeRentChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentChip))
                        _summary.text += " · " + lifetimeRentChip;
                    string lifetimeGreyGrant = HqDisplayQueriesHud.LifetimeGreyGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyGrant))
                        _summary.text += " · " + lifetimeGreyGrant;
                    string lastWeekChipRent = HqDisplayQueriesHud.LastWeekChipRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipRent))
                        _summary.text += " · " + lastWeekChipRent;
                    string thisWeekGreyPatent = HqDisplayQueriesHud.ThisWeekGreyPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyPatent))
                        _summary.text += " · " + thisWeekGreyPatent;
                    string lifetimeFinesGrant = HqDisplayQueriesHud.LifetimeFinesGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesGrant))
                        _summary.text += " · " + lifetimeFinesGrant;
                    string lastWeekLandConstruction = HqDisplayQueriesHud.LastWeekLandConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandConstruction))
                        _summary.text += " · " + lastWeekLandConstruction;
                    string thisWeekRentPower = HqDisplayQueriesHud.ThisWeekRentPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentPower))
                        _summary.text += " · " + thisWeekRentPower;
                    string lastWeekGreyFines = HqDisplayQueriesHud.LastWeekGreyFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyFines))
                        _summary.text += " · " + lastWeekGreyFines;
                    string lifetimePowerLand = HqDisplayQueriesHud.LifetimePowerLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerLand))
                        _summary.text += " · " + lifetimePowerLand;
                    string lastWeekHardwareGrant = HqDisplayQueriesHud.LastWeekHardwareGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareGrant))
                        _summary.text += " · " + lastWeekHardwareGrant;
                    string lastWeekSiteRent = HqDisplayQueriesHud.LastWeekSiteRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteRent))
                        _summary.text += " · " + lastWeekSiteRent;
                    string lifetimeConstructionChip = HqDisplayQueriesHud.LifetimeConstructionChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionChip))
                        _summary.text += " · " + lifetimeConstructionChip;
                    string lastWeekGrantPower = HqDisplayQueriesHud.LastWeekGrantPowerSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantPower))
                        _summary.text += " · " + lastWeekGrantPower;
                    string thisWeekLandGrant = HqDisplayQueriesHud.ThisWeekLandGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLandGrant))
                        _summary.text += " · " + thisWeekLandGrant;
                    string thisWeekSalariesRent = HqDisplayQueriesHud.ThisWeekSalariesRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesRent))
                        _summary.text += " · " + thisWeekSalariesRent;
                    string lifetimeLaunderLand = HqDisplayQueriesHud.LifetimeLaunderLandSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderLand))
                        _summary.text += " · " + lifetimeLaunderLand;
                    string lastWeekSiteFines = HqDisplayQueriesHud.LastWeekSiteFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteFines))
                        _summary.text += " · " + lastWeekSiteFines;
                    string thisWeekCapitalRent = HqDisplayQueriesHud.ThisWeekCapitalRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekCapitalRent))
                        _summary.text += " · " + thisWeekCapitalRent;
                    string lifetimeGrantRoom = HqDisplayQueriesHud.LifetimeGrantRoomSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantRoom))
                        _summary.text += " · " + lifetimeGrantRoom;
                    string lastWeekRentChip = HqDisplayQueriesHud.LastWeekRentChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentChip))
                        _summary.text += " · " + lastWeekRentChip;
                    string lifetimeLaunderFines = HqDisplayQueriesHud.LifetimeLaunderFinesSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderFines))
                        _summary.text += " · " + lifetimeLaunderFines;
                    string thisWeekLaunderConstruction = HqDisplayQueriesHud.ThisWeekLaunderConstructionSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderConstruction))
                        _summary.text += " · " + thisWeekLaunderConstruction;
                    string thisWeekFinesSpendIncome = HqDisplayQueriesHud.ThisWeekFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesSpendIncome))
                        _summary.text += " · " + thisWeekFinesSpendIncome;
                    string lastWeekFinesSpendIncome = HqDisplayQueriesHud.LastWeekFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesSpendIncome))
                        _summary.text += " · " + lastWeekFinesSpendIncome;
                    string lifetimeFinesSpendIncome = HqDisplayQueriesHud.LifetimeFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesSpendIncome))
                        _summary.text += " · " + lifetimeFinesSpendIncome;
                    string thisWeekGrantSpendIncome = HqDisplayQueriesHud.ThisWeekGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantSpendIncome))
                        _summary.text += " · " + thisWeekGrantSpendIncome;
                    string lastWeekGrantSpendIncome = HqDisplayQueriesHud.LastWeekGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantSpendIncome))
                        _summary.text += " · " + lastWeekGrantSpendIncome;
                    string lifetimeGrantSpendIncome = HqDisplayQueriesHud.LifetimeGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantSpendIncome))
                        _summary.text += " · " + lifetimeGrantSpendIncome;
                    string thisWeekChipSpendIncome = HqDisplayQueriesHud.ThisWeekChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipSpendIncome))
                        _summary.text += " · " + thisWeekChipSpendIncome;
                    string lastWeekChipSpendIncome = HqDisplayQueriesHud.LastWeekChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipSpendIncome))
                        _summary.text += " · " + lastWeekChipSpendIncome;
                    string lifetimeChipSpendIncome = HqDisplayQueriesHud.LifetimeChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipSpendIncome))
                        _summary.text += " · " + lifetimeChipSpendIncome;
                    string thisWeekDevSpendIncome = HqDisplayQueriesHud.ThisWeekDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekDevSpendIncome))
                        _summary.text += " · " + thisWeekDevSpendIncome;
                    string lastWeekDevSpendIncome = HqDisplayQueriesHud.LastWeekDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekDevSpendIncome))
                        _summary.text += " · " + lastWeekDevSpendIncome;
                    string lifetimeDevSpendIncome = HqDisplayQueriesHud.LifetimeDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeDevSpendIncome))
                        _summary.text += " · " + lifetimeDevSpendIncome;
                    string thisWeekGpuSpendIncome = HqDisplayQueriesHud.ThisWeekGpuSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuSpendIncome))
                        _summary.text += " · " + thisWeekGpuSpendIncome;
                    string lastWeekGpuSpendIncome = HqDisplayQueriesHud.LastWeekGpuSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuSpendIncome))
                        _summary.text += " · " + lastWeekGpuSpendIncome;
                    string lifetimeGpuSpendIncome = HqDisplayQueriesHud.LifetimeGpuSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGpuSpendIncome))
                        _summary.text += " · " + lifetimeGpuSpendIncome;
                    string thisWeekRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueSpendIncome))
                        _summary.text += " · " + thisWeekRevenueSpendIncome;
                    string lastWeekRevenueSpendIncome = HqDisplayQueriesHud.LastWeekRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueSpendIncome))
                        _summary.text += " · " + lastWeekRevenueSpendIncome;
                    string lifetimeRevenueSpendIncome = HqDisplayQueriesHud.LifetimeRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueSpendIncome))
                        _summary.text += " · " + lifetimeRevenueSpendIncome;
                    string thisWeekGrantFinesSpendIncome = HqDisplayQueriesHud.ThisWeekGrantFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantFinesSpendIncome))
                        _summary.text += " · " + thisWeekGrantFinesSpendIncome;
                    string lastWeekGrantFinesSpendIncome = HqDisplayQueriesHud.LastWeekGrantFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantFinesSpendIncome))
                        _summary.text += " · " + lastWeekGrantFinesSpendIncome;
                    string lifetimeGrantFinesSpendIncome = HqDisplayQueriesHud.LifetimeGrantFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantFinesSpendIncome))
                        _summary.text += " · " + lifetimeGrantFinesSpendIncome;
                    string thisWeekChipDevSpendIncome = HqDisplayQueriesHud.ThisWeekChipDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipDevSpendIncome))
                        _summary.text += " · " + thisWeekChipDevSpendIncome;
                    string lastWeekChipDevSpendIncome = HqDisplayQueriesHud.LastWeekChipDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipDevSpendIncome))
                        _summary.text += " · " + lastWeekChipDevSpendIncome;
                    string lifetimeChipDevSpendIncome = HqDisplayQueriesHud.LifetimeChipDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipDevSpendIncome))
                        _summary.text += " · " + lifetimeChipDevSpendIncome;
                    string thisWeekHardwareSiteSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareSiteSpendIncome))
                        _summary.text += " · " + thisWeekHardwareSiteSpendIncome;
                    string lastWeekHardwareSiteSpendIncome = HqDisplayQueriesHud.LastWeekHardwareSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareSiteSpendIncome))
                        _summary.text += " · " + lastWeekHardwareSiteSpendIncome;
                    string lifetimeHardwareSiteSpendIncome = HqDisplayQueriesHud.LifetimeHardwareSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareSiteSpendIncome))
                        _summary.text += " · " + lifetimeHardwareSiteSpendIncome;
                    string thisWeekLandRoomSpendIncome = HqDisplayQueriesHud.ThisWeekLandRoomSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLandRoomSpendIncome))
                        _summary.text += " · " + thisWeekLandRoomSpendIncome;
                    string lastWeekLandRoomSpendIncome = HqDisplayQueriesHud.LastWeekLandRoomSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandRoomSpendIncome))
                        _summary.text += " · " + lastWeekLandRoomSpendIncome;
                    string lifetimeLandRoomSpendIncome = HqDisplayQueriesHud.LifetimeLandRoomSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLandRoomSpendIncome))
                        _summary.text += " · " + lifetimeLandRoomSpendIncome;
                    string thisWeekRentTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekRentTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentTrainingSpendIncome))
                        _summary.text += " · " + thisWeekRentTrainingSpendIncome;
                    string lastWeekRentTrainingSpendIncome = HqDisplayQueriesHud.LastWeekRentTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentTrainingSpendIncome))
                        _summary.text += " · " + lastWeekRentTrainingSpendIncome;
                    string lifetimeRentTrainingSpendIncome = HqDisplayQueriesHud.LifetimeRentTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentTrainingSpendIncome))
                        _summary.text += " · " + lifetimeRentTrainingSpendIncome;
                    string thisWeekPowerConstructionSpendIncome = HqDisplayQueriesHud.ThisWeekPowerConstructionSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerConstructionSpendIncome))
                        _summary.text += " · " + thisWeekPowerConstructionSpendIncome;
                    string lastWeekPowerConstructionSpendIncome = HqDisplayQueriesHud.LastWeekPowerConstructionSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerConstructionSpendIncome))
                        _summary.text += " · " + lastWeekPowerConstructionSpendIncome;
                    string lifetimePowerConstructionSpendIncome = HqDisplayQueriesHud.LifetimePowerConstructionSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerConstructionSpendIncome))
                        _summary.text += " · " + lifetimePowerConstructionSpendIncome;
                    string thisWeekSalariesHiringSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesHiringSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesHiringSpendIncome))
                        _summary.text += " · " + thisWeekSalariesHiringSpendIncome;
                    string lastWeekSalariesHiringSpendIncome = HqDisplayQueriesHud.LastWeekSalariesHiringSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesHiringSpendIncome))
                        _summary.text += " · " + lastWeekSalariesHiringSpendIncome;
                    string lifetimeSalariesHiringSpendIncome = HqDisplayQueriesHud.LifetimeSalariesHiringSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSalariesHiringSpendIncome))
                        _summary.text += " · " + lifetimeSalariesHiringSpendIncome;
                    string thisWeekLaunderCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekLaunderCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderCapitalSpendIncome))
                        _summary.text += " · " + thisWeekLaunderCapitalSpendIncome;
                    string lastWeekLaunderCapitalSpendIncome = HqDisplayQueriesHud.LastWeekLaunderCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderCapitalSpendIncome))
                        _summary.text += " · " + lastWeekLaunderCapitalSpendIncome;
                    string lifetimeLaunderCapitalSpendIncome = HqDisplayQueriesHud.LifetimeLaunderCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderCapitalSpendIncome))
                        _summary.text += " · " + lifetimeLaunderCapitalSpendIncome;
                    string thisWeekPatentUpkeepSpendIncome = HqDisplayQueriesHud.ThisWeekPatentUpkeepSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentUpkeepSpendIncome))
                        _summary.text += " · " + thisWeekPatentUpkeepSpendIncome;
                    string lastWeekPatentUpkeepSpendIncome = HqDisplayQueriesHud.LastWeekPatentUpkeepSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentUpkeepSpendIncome))
                        _summary.text += " · " + lastWeekPatentUpkeepSpendIncome;
                    string lifetimePatentUpkeepSpendIncome = HqDisplayQueriesHud.LifetimePatentUpkeepSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentUpkeepSpendIncome))
                        _summary.text += " · " + lifetimePatentUpkeepSpendIncome;
                    string thisWeekNetGreySpendIncome = HqDisplayQueriesHud.ThisWeekNetGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetGreySpendIncome))
                        _summary.text += " · " + thisWeekNetGreySpendIncome;
                    string lastWeekNetGreySpendIncome = HqDisplayQueriesHud.LastWeekNetGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetGreySpendIncome))
                        _summary.text += " · " + lastWeekNetGreySpendIncome;
                    string lifetimeNetGreySpendIncome = HqDisplayQueriesHud.LifetimeNetGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetGreySpendIncome))
                        _summary.text += " · " + lifetimeNetGreySpendIncome;
                    string thisWeekGrantChipSpendIncome = HqDisplayQueriesHud.ThisWeekGrantChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantChipSpendIncome))
                        _summary.text += " · " + thisWeekGrantChipSpendIncome;
                    string lastWeekGrantChipSpendIncome = HqDisplayQueriesHud.LastWeekGrantChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantChipSpendIncome))
                        _summary.text += " · " + lastWeekGrantChipSpendIncome;
                    string lifetimeGrantChipSpendIncome = HqDisplayQueriesHud.LifetimeGrantChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantChipSpendIncome))
                        _summary.text += " · " + lifetimeGrantChipSpendIncome;
                    string thisWeekFinesDevSpendIncome = HqDisplayQueriesHud.ThisWeekFinesDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesDevSpendIncome))
                        _summary.text += " · " + thisWeekFinesDevSpendIncome;
                    string lastWeekFinesDevSpendIncome = HqDisplayQueriesHud.LastWeekFinesDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesDevSpendIncome))
                        _summary.text += " · " + lastWeekFinesDevSpendIncome;
                    string lifetimeFinesDevSpendIncome = HqDisplayQueriesHud.LifetimeFinesDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesDevSpendIncome))
                        _summary.text += " · " + lifetimeFinesDevSpendIncome;
                    string thisWeekHardwareLandSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareLandSpendIncome))
                        _summary.text += " · " + thisWeekHardwareLandSpendIncome;
                    string lastWeekHardwareLandSpendIncome = HqDisplayQueriesHud.LastWeekHardwareLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareLandSpendIncome))
                        _summary.text += " · " + lastWeekHardwareLandSpendIncome;
                    string lifetimeHardwareLandSpendIncome = HqDisplayQueriesHud.LifetimeHardwareLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareLandSpendIncome))
                        _summary.text += " · " + lifetimeHardwareLandSpendIncome;
                    string thisWeekSiteCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekSiteCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteCapitalSpendIncome))
                        _summary.text += " · " + thisWeekSiteCapitalSpendIncome;
                    string lastWeekSiteCapitalSpendIncome = HqDisplayQueriesHud.LastWeekSiteCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteCapitalSpendIncome))
                        _summary.text += " · " + lastWeekSiteCapitalSpendIncome;
                    string lifetimeSiteCapitalSpendIncome = HqDisplayQueriesHud.LifetimeSiteCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSiteCapitalSpendIncome))
                        _summary.text += " · " + lifetimeSiteCapitalSpendIncome;
                    string thisWeekShadyLaunderSpendIncome = HqDisplayQueriesHud.ThisWeekShadyLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekShadyLaunderSpendIncome))
                        _summary.text += " · " + thisWeekShadyLaunderSpendIncome;
                    string lastWeekShadyLaunderSpendIncome = HqDisplayQueriesHud.LastWeekShadyLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekShadyLaunderSpendIncome))
                        _summary.text += " · " + lastWeekShadyLaunderSpendIncome;
                    string lifetimeShadyLaunderSpendIncome = HqDisplayQueriesHud.LifetimeShadyLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeShadyLaunderSpendIncome))
                        _summary.text += " · " + lifetimeShadyLaunderSpendIncome;
                    string thisWeekChipRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekChipRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipRevenueSpendIncome))
                        _summary.text += " · " + thisWeekChipRevenueSpendIncome;
                    string lastWeekChipRevenueSpendIncome = HqDisplayQueriesHud.LastWeekChipRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipRevenueSpendIncome))
                        _summary.text += " · " + lastWeekChipRevenueSpendIncome;
                    string lifetimeChipRevenueSpendIncome = HqDisplayQueriesHud.LifetimeChipRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipRevenueSpendIncome))
                        _summary.text += " · " + lifetimeChipRevenueSpendIncome;
                    string thisWeekPatentGrantSpendIncome = HqDisplayQueriesHud.ThisWeekPatentGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentGrantSpendIncome))
                        _summary.text += " · " + thisWeekPatentGrantSpendIncome;
                    string lastWeekPatentGrantSpendIncome = HqDisplayQueriesHud.LastWeekPatentGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentGrantSpendIncome))
                        _summary.text += " · " + lastWeekPatentGrantSpendIncome;
                    string lifetimePatentGrantSpendIncome = HqDisplayQueriesHud.LifetimePatentGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentGrantSpendIncome))
                        _summary.text += " · " + lifetimePatentGrantSpendIncome;
                    string thisWeekGreyFinesSpendIncome = HqDisplayQueriesHud.ThisWeekGreyFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyFinesSpendIncome))
                        _summary.text += " · " + thisWeekGreyFinesSpendIncome;
                    string lastWeekGreyFinesSpendIncome = HqDisplayQueriesHud.LastWeekGreyFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyFinesSpendIncome))
                        _summary.text += " · " + lastWeekGreyFinesSpendIncome;
                    string lifetimeGreyFinesSpendIncome = HqDisplayQueriesHud.LifetimeGreyFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyFinesSpendIncome))
                        _summary.text += " · " + lifetimeGreyFinesSpendIncome;
                    string thisWeekNetTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekNetTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetTrainingSpendIncome))
                        _summary.text += " · " + thisWeekNetTrainingSpendIncome;
                    string lastWeekNetTrainingSpendIncome = HqDisplayQueriesHud.LastWeekNetTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetTrainingSpendIncome))
                        _summary.text += " · " + lastWeekNetTrainingSpendIncome;
                    string lifetimeNetTrainingSpendIncome = HqDisplayQueriesHud.LifetimeNetTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetTrainingSpendIncome))
                        _summary.text += " · " + lifetimeNetTrainingSpendIncome;
                    string thisWeekGpuPowerSpendIncome = HqDisplayQueriesHud.ThisWeekGpuPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuPowerSpendIncome))
                        _summary.text += " · " + thisWeekGpuPowerSpendIncome;
                    string lastWeekGpuPowerSpendIncome = HqDisplayQueriesHud.LastWeekGpuPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuPowerSpendIncome))
                        _summary.text += " · " + lastWeekGpuPowerSpendIncome;
                    string lifetimeGpuPowerSpendIncome = HqDisplayQueriesHud.LifetimeGpuPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGpuPowerSpendIncome))
                        _summary.text += " · " + lifetimeGpuPowerSpendIncome;
                    string thisWeekRevenueGrantsSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueGrantsSpendIncome))
                        _summary.text += " · " + thisWeekRevenueGrantsSpendIncome;
                    string lastWeekRevenueGrantsSpendIncome = HqDisplayQueriesHud.LastWeekRevenueGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueGrantsSpendIncome))
                        _summary.text += " · " + lastWeekRevenueGrantsSpendIncome;
                    string lifetimeRevenueGrantsSpendIncome = HqDisplayQueriesHud.LifetimeRevenueGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueGrantsSpendIncome))
                        _summary.text += " · " + lifetimeRevenueGrantsSpendIncome;
                    string thisWeekConstructionSiteSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionSiteSpendIncome))
                        _summary.text += " · " + thisWeekConstructionSiteSpendIncome;
                    string lastWeekConstructionSiteSpendIncome = HqDisplayQueriesHud.LastWeekConstructionSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionSiteSpendIncome))
                        _summary.text += " · " + lastWeekConstructionSiteSpendIncome;
                    string lifetimeConstructionSiteSpendIncome = HqDisplayQueriesHud.LifetimeConstructionSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionSiteSpendIncome))
                        _summary.text += " · " + lifetimeConstructionSiteSpendIncome;
                    string thisWeekGrantDevSpendIncome = HqDisplayQueriesHud.ThisWeekGrantDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantDevSpendIncome))
                        _summary.text += " · " + thisWeekGrantDevSpendIncome;
                    string lastWeekGrantDevSpendIncome = HqDisplayQueriesHud.LastWeekGrantDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantDevSpendIncome))
                        _summary.text += " · " + lastWeekGrantDevSpendIncome;
                    string lifetimeGrantDevSpendIncome = HqDisplayQueriesHud.LifetimeGrantDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantDevSpendIncome))
                        _summary.text += " · " + lifetimeGrantDevSpendIncome;
                    string thisWeekChipFinesSpendIncome = HqDisplayQueriesHud.ThisWeekChipFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipFinesSpendIncome))
                        _summary.text += " · " + thisWeekChipFinesSpendIncome;
                    string lastWeekChipFinesSpendIncome = HqDisplayQueriesHud.LastWeekChipFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipFinesSpendIncome))
                        _summary.text += " · " + lastWeekChipFinesSpendIncome;
                    string lifetimeChipFinesSpendIncome = HqDisplayQueriesHud.LifetimeChipFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipFinesSpendIncome))
                        _summary.text += " · " + lifetimeChipFinesSpendIncome;
                    string thisWeekShadyGreySpendIncome = HqDisplayQueriesHud.ThisWeekShadyGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekShadyGreySpendIncome))
                        _summary.text += " · " + thisWeekShadyGreySpendIncome;
                    string lastWeekShadyGreySpendIncome = HqDisplayQueriesHud.LastWeekShadyGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekShadyGreySpendIncome))
                        _summary.text += " · " + lastWeekShadyGreySpendIncome;
                    string lifetimeShadyGreySpendIncome = HqDisplayQueriesHud.LifetimeShadyGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeShadyGreySpendIncome))
                        _summary.text += " · " + lifetimeShadyGreySpendIncome;
                    string thisWeekHiringPatentSpendIncome = HqDisplayQueriesHud.ThisWeekHiringPatentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHiringPatentSpendIncome))
                        _summary.text += " · " + thisWeekHiringPatentSpendIncome;
                    string lastWeekHiringPatentSpendIncome = HqDisplayQueriesHud.LastWeekHiringPatentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringPatentSpendIncome))
                        _summary.text += " · " + lastWeekHiringPatentSpendIncome;
                    string lifetimeHiringPatentSpendIncome = HqDisplayQueriesHud.LifetimeHiringPatentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHiringPatentSpendIncome))
                        _summary.text += " · " + lifetimeHiringPatentSpendIncome;
                    string thisWeekNetGrantSpendIncome = HqDisplayQueriesHud.ThisWeekNetGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetGrantSpendIncome))
                        _summary.text += " · " + thisWeekNetGrantSpendIncome;
                    string lastWeekNetGrantSpendIncome = HqDisplayQueriesHud.LastWeekNetGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetGrantSpendIncome))
                        _summary.text += " · " + lastWeekNetGrantSpendIncome;
                    string lifetimeNetGrantSpendIncome = HqDisplayQueriesHud.LifetimeNetGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetGrantSpendIncome))
                        _summary.text += " · " + lifetimeNetGrantSpendIncome;
                    string thisWeekGreyGrantSpendIncome = HqDisplayQueriesHud.ThisWeekGreyGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyGrantSpendIncome))
                        _summary.text += " · " + thisWeekGreyGrantSpendIncome;
                    string lastWeekGreyGrantSpendIncome = HqDisplayQueriesHud.LastWeekGreyGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyGrantSpendIncome))
                        _summary.text += " · " + lastWeekGreyGrantSpendIncome;
                    string lifetimeGreyGrantSpendIncome = HqDisplayQueriesHud.LifetimeGreyGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyGrantSpendIncome))
                        _summary.text += " · " + lifetimeGreyGrantSpendIncome;
                    string thisWeekConstructionCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionCapitalSpendIncome))
                        _summary.text += " · " + thisWeekConstructionCapitalSpendIncome;
                    string lastWeekConstructionCapitalSpendIncome = HqDisplayQueriesHud.LastWeekConstructionCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionCapitalSpendIncome))
                        _summary.text += " · " + lastWeekConstructionCapitalSpendIncome;
                    string lifetimeConstructionCapitalSpendIncome = HqDisplayQueriesHud.LifetimeConstructionCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionCapitalSpendIncome))
                        _summary.text += " · " + lifetimeConstructionCapitalSpendIncome;
                    string thisWeekRoomTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekRoomTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRoomTrainingSpendIncome))
                        _summary.text += " · " + thisWeekRoomTrainingSpendIncome;
                    string lastWeekRoomTrainingSpendIncome = HqDisplayQueriesHud.LastWeekRoomTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomTrainingSpendIncome))
                        _summary.text += " · " + lastWeekRoomTrainingSpendIncome;
                    string lifetimeRoomTrainingSpendIncome = HqDisplayQueriesHud.LifetimeRoomTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRoomTrainingSpendIncome))
                        _summary.text += " · " + lifetimeRoomTrainingSpendIncome;
                    string thisWeekGpuRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekGpuRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuRevenueSpendIncome))
                        _summary.text += " · " + thisWeekGpuRevenueSpendIncome;
                    string lastWeekGpuRevenueSpendIncome = HqDisplayQueriesHud.LastWeekGpuRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuRevenueSpendIncome))
                        _summary.text += " · " + lastWeekGpuRevenueSpendIncome;
                    string lifetimeGpuRevenueSpendIncome = HqDisplayQueriesHud.LifetimeGpuRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGpuRevenueSpendIncome))
                        _summary.text += " · " + lifetimeGpuRevenueSpendIncome;
                    string thisWeekChipGrantSpendIncome = HqDisplayQueriesHud.ThisWeekChipGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipGrantSpendIncome))
                        _summary.text += " · " + thisWeekChipGrantSpendIncome;
                    string lastWeekChipGrantSpendIncome = HqDisplayQueriesHud.LastWeekChipGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipGrantSpendIncome))
                        _summary.text += " · " + lastWeekChipGrantSpendIncome;
                    string lifetimeChipGrantSpendIncome = HqDisplayQueriesHud.LifetimeChipGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipGrantSpendIncome))
                        _summary.text += " · " + lifetimeChipGrantSpendIncome;
                    string thisWeekLaunderGreySpendIncome = HqDisplayQueriesHud.ThisWeekLaunderGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLaunderGreySpendIncome))
                        _summary.text += " · " + thisWeekLaunderGreySpendIncome;
                    string lastWeekLaunderGreySpendIncome = HqDisplayQueriesHud.LastWeekLaunderGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLaunderGreySpendIncome))
                        _summary.text += " · " + lastWeekLaunderGreySpendIncome;
                    string lifetimeLaunderGreySpendIncome = HqDisplayQueriesHud.LifetimeLaunderGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLaunderGreySpendIncome))
                        _summary.text += " · " + lifetimeLaunderGreySpendIncome;
                    string thisWeekPatentFinesSpendIncome = HqDisplayQueriesHud.ThisWeekPatentFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentFinesSpendIncome))
                        _summary.text += " · " + thisWeekPatentFinesSpendIncome;
                    string lastWeekPatentFinesSpendIncome = HqDisplayQueriesHud.LastWeekPatentFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentFinesSpendIncome))
                        _summary.text += " · " + lastWeekPatentFinesSpendIncome;
                    string lifetimePatentFinesSpendIncome = HqDisplayQueriesHud.LifetimePatentFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentFinesSpendIncome))
                        _summary.text += " · " + lifetimePatentFinesSpendIncome;
                    string thisWeekHardwareGrantSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareGrantSpendIncome))
                        _summary.text += " · " + thisWeekHardwareGrantSpendIncome;
                    string lastWeekHardwareGrantSpendIncome = HqDisplayQueriesHud.LastWeekHardwareGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareGrantSpendIncome))
                        _summary.text += " · " + lastWeekHardwareGrantSpendIncome;
                    string lifetimeHardwareGrantSpendIncome = HqDisplayQueriesHud.LifetimeHardwareGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareGrantSpendIncome))
                        _summary.text += " · " + lifetimeHardwareGrantSpendIncome;
                    string thisWeekLandCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekLandCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekLandCapitalSpendIncome))
                        _summary.text += " · " + thisWeekLandCapitalSpendIncome;
                    string lastWeekLandCapitalSpendIncome = HqDisplayQueriesHud.LastWeekLandCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekLandCapitalSpendIncome))
                        _summary.text += " · " + lastWeekLandCapitalSpendIncome;
                    string lifetimeLandCapitalSpendIncome = HqDisplayQueriesHud.LifetimeLandCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeLandCapitalSpendIncome))
                        _summary.text += " · " + lifetimeLandCapitalSpendIncome;
                    string thisWeekRentPowerSpendIncome = HqDisplayQueriesHud.ThisWeekRentPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentPowerSpendIncome))
                        _summary.text += " · " + thisWeekRentPowerSpendIncome;
                    string lastWeekRentPowerSpendIncome = HqDisplayQueriesHud.LastWeekRentPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentPowerSpendIncome))
                        _summary.text += " · " + lastWeekRentPowerSpendIncome;
                    string lifetimeRentPowerSpendIncome = HqDisplayQueriesHud.LifetimeRentPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentPowerSpendIncome))
                        _summary.text += " · " + lifetimeRentPowerSpendIncome;
                    string thisWeekSalariesConstructionSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesConstructionSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesConstructionSpendIncome))
                        _summary.text += " · " + thisWeekSalariesConstructionSpendIncome;
                    string lastWeekSalariesConstructionSpendIncome = HqDisplayQueriesHud.LastWeekSalariesConstructionSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesConstructionSpendIncome))
                        _summary.text += " · " + lastWeekSalariesConstructionSpendIncome;
                    string lifetimeSalariesConstructionSpendIncome = HqDisplayQueriesHud.LifetimeSalariesConstructionSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSalariesConstructionSpendIncome))
                        _summary.text += " · " + lifetimeSalariesConstructionSpendIncome;
                    string thisWeekNetChipSpendIncome = HqDisplayQueriesHud.ThisWeekNetChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetChipSpendIncome))
                        _summary.text += " · " + thisWeekNetChipSpendIncome;
                    string lastWeekNetChipSpendIncome = HqDisplayQueriesHud.LastWeekNetChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetChipSpendIncome))
                        _summary.text += " · " + lastWeekNetChipSpendIncome;
                    string lifetimeNetChipSpendIncome = HqDisplayQueriesHud.LifetimeNetChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetChipSpendIncome))
                        _summary.text += " · " + lifetimeNetChipSpendIncome;
                    string thisWeekGreyDevSpendIncome = HqDisplayQueriesHud.ThisWeekGreyDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyDevSpendIncome))
                        _summary.text += " · " + thisWeekGreyDevSpendIncome;
                    string lastWeekGreyDevSpendIncome = HqDisplayQueriesHud.LastWeekGreyDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyDevSpendIncome))
                        _summary.text += " · " + lastWeekGreyDevSpendIncome;
                    string lifetimeGreyDevSpendIncome = HqDisplayQueriesHud.LifetimeGreyDevSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyDevSpendIncome))
                        _summary.text += " · " + lifetimeGreyDevSpendIncome;
                    string thisWeekShadyGrantSpendIncome = HqDisplayQueriesHud.ThisWeekShadyGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekShadyGrantSpendIncome))
                        _summary.text += " · " + thisWeekShadyGrantSpendIncome;
                    string lastWeekShadyGrantSpendIncome = HqDisplayQueriesHud.LastWeekShadyGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekShadyGrantSpendIncome))
                        _summary.text += " · " + lastWeekShadyGrantSpendIncome;
                    string lifetimeShadyGrantSpendIncome = HqDisplayQueriesHud.LifetimeShadyGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeShadyGrantSpendIncome))
                        _summary.text += " · " + lifetimeShadyGrantSpendIncome;
                    string thisWeekPatentChipSpendIncome = HqDisplayQueriesHud.ThisWeekPatentChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentChipSpendIncome))
                        _summary.text += " · " + thisWeekPatentChipSpendIncome;
                    string lastWeekPatentChipSpendIncome = HqDisplayQueriesHud.LastWeekPatentChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentChipSpendIncome))
                        _summary.text += " · " + lastWeekPatentChipSpendIncome;
                    string lifetimePatentChipSpendIncome = HqDisplayQueriesHud.LifetimePatentChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentChipSpendIncome))
                        _summary.text += " · " + lifetimePatentChipSpendIncome;
                    string thisWeekFinesGrantSpendIncome = HqDisplayQueriesHud.ThisWeekFinesGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesGrantSpendIncome))
                        _summary.text += " · " + thisWeekFinesGrantSpendIncome;
                    string lastWeekFinesGrantSpendIncome = HqDisplayQueriesHud.LastWeekFinesGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesGrantSpendIncome))
                        _summary.text += " · " + lastWeekFinesGrantSpendIncome;
                    string lifetimeFinesGrantSpendIncome = HqDisplayQueriesHud.LifetimeFinesGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesGrantSpendIncome))
                        _summary.text += " · " + lifetimeFinesGrantSpendIncome;
                    string thisWeekGrantRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekGrantRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantRevenueSpendIncome))
                        _summary.text += " · " + thisWeekGrantRevenueSpendIncome;
                    string lastWeekGrantRevenueSpendIncome = HqDisplayQueriesHud.LastWeekGrantRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantRevenueSpendIncome))
                        _summary.text += " · " + lastWeekGrantRevenueSpendIncome;
                    string lifetimeGrantRevenueSpendIncome = HqDisplayQueriesHud.LifetimeGrantRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantRevenueSpendIncome))
                        _summary.text += " · " + lifetimeGrantRevenueSpendIncome;
                    string thisWeekNetFinesSpendIncome = HqDisplayQueriesHud.ThisWeekNetFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekNetFinesSpendIncome))
                        _summary.text += " · " + thisWeekNetFinesSpendIncome;
                    string lastWeekNetFinesSpendIncome = HqDisplayQueriesHud.LastWeekNetFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekNetFinesSpendIncome))
                        _summary.text += " · " + lastWeekNetFinesSpendIncome;
                    string lifetimeNetFinesSpendIncome = HqDisplayQueriesHud.LifetimeNetFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeNetFinesSpendIncome))
                        _summary.text += " · " + lifetimeNetFinesSpendIncome;
                    string thisWeekGreyChipSpendIncome = HqDisplayQueriesHud.ThisWeekGreyChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyChipSpendIncome))
                        _summary.text += " · " + thisWeekGreyChipSpendIncome;
                    string lastWeekGreyChipSpendIncome = HqDisplayQueriesHud.LastWeekGreyChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyChipSpendIncome))
                        _summary.text += " · " + lastWeekGreyChipSpendIncome;
                    string lifetimeGreyChipSpendIncome = HqDisplayQueriesHud.LifetimeGreyChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyChipSpendIncome))
                        _summary.text += " · " + lifetimeGreyChipSpendIncome;
                    string thisWeekGrantPowerSpendIncome = HqDisplayQueriesHud.ThisWeekGrantPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantPowerSpendIncome))
                        _summary.text += " · " + thisWeekGrantPowerSpendIncome;
                    string lastWeekGrantPowerSpendIncome = HqDisplayQueriesHud.LastWeekGrantPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantPowerSpendIncome))
                        _summary.text += " · " + lastWeekGrantPowerSpendIncome;
                    string lifetimeGrantPowerSpendIncome = HqDisplayQueriesHud.LifetimeGrantPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantPowerSpendIncome))
                        _summary.text += " · " + lifetimeGrantPowerSpendIncome;
                    string thisWeekDevFinesSpendIncome = HqDisplayQueriesHud.ThisWeekDevFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekDevFinesSpendIncome))
                        _summary.text += " · " + thisWeekDevFinesSpendIncome;
                    string lastWeekDevFinesSpendIncome = HqDisplayQueriesHud.LastWeekDevFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekDevFinesSpendIncome))
                        _summary.text += " · " + lastWeekDevFinesSpendIncome;
                    string lifetimeDevFinesSpendIncome = HqDisplayQueriesHud.LifetimeDevFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeDevFinesSpendIncome))
                        _summary.text += " · " + lifetimeDevFinesSpendIncome;
                    string thisWeekHardwareCapitalSpendIncome = HqDisplayQueriesHud.ThisWeekHardwareCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwareCapitalSpendIncome))
                        _summary.text += " · " + thisWeekHardwareCapitalSpendIncome;
                    string lastWeekHardwareCapitalSpendIncome = HqDisplayQueriesHud.LastWeekHardwareCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwareCapitalSpendIncome))
                        _summary.text += " · " + lastWeekHardwareCapitalSpendIncome;
                    string lifetimeHardwareCapitalSpendIncome = HqDisplayQueriesHud.LifetimeHardwareCapitalSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwareCapitalSpendIncome))
                        _summary.text += " · " + lifetimeHardwareCapitalSpendIncome;
                    string thisWeekSiteGrantSpendIncome = HqDisplayQueriesHud.ThisWeekSiteGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSiteGrantSpendIncome))
                        _summary.text += " · " + thisWeekSiteGrantSpendIncome;
                    string lastWeekSiteGrantSpendIncome = HqDisplayQueriesHud.LastWeekSiteGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSiteGrantSpendIncome))
                        _summary.text += " · " + lastWeekSiteGrantSpendIncome;
                    string lifetimeSiteGrantSpendIncome = HqDisplayQueriesHud.LifetimeSiteGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSiteGrantSpendIncome))
                        _summary.text += " · " + lifetimeSiteGrantSpendIncome;
                    string thisWeekTrainingGrantSpendIncome = HqDisplayQueriesHud.ThisWeekTrainingGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingGrantSpendIncome))
                        _summary.text += " · " + thisWeekTrainingGrantSpendIncome;
                    string lastWeekTrainingGrantSpendIncome = HqDisplayQueriesHud.LastWeekTrainingGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingGrantSpendIncome))
                        _summary.text += " · " + lastWeekTrainingGrantSpendIncome;
                    string lifetimeTrainingGrantSpendIncome = HqDisplayQueriesHud.LifetimeTrainingGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingGrantSpendIncome))
                        _summary.text += " · " + lifetimeTrainingGrantSpendIncome;
                    string thisWeekRoomGrantSpendIncome = HqDisplayQueriesHud.ThisWeekRoomGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRoomGrantSpendIncome))
                        _summary.text += " · " + thisWeekRoomGrantSpendIncome;
                    string lastWeekRoomGrantSpendIncome = HqDisplayQueriesHud.LastWeekRoomGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRoomGrantSpendIncome))
                        _summary.text += " · " + lastWeekRoomGrantSpendIncome;
                    string lifetimeRoomGrantSpendIncome = HqDisplayQueriesHud.LifetimeRoomGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRoomGrantSpendIncome))
                        _summary.text += " · " + lifetimeRoomGrantSpendIncome;
                    string thisWeekGpuGrantSpendIncome = HqDisplayQueriesHud.ThisWeekGpuGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuGrantSpendIncome))
                        _summary.text += " · " + thisWeekGpuGrantSpendIncome;
                    string lastWeekGpuGrantSpendIncome = HqDisplayQueriesHud.LastWeekGpuGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuGrantSpendIncome))
                        _summary.text += " · " + lastWeekGpuGrantSpendIncome;
                    string lifetimeGpuGrantSpendIncome = HqDisplayQueriesHud.LifetimeGpuGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGpuGrantSpendIncome))
                        _summary.text += " · " + lifetimeGpuGrantSpendIncome;
                    string thisWeekRevenueFinesSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueFinesSpendIncome))
                        _summary.text += " · " + thisWeekRevenueFinesSpendIncome;
                    string lastWeekRevenueFinesSpendIncome = HqDisplayQueriesHud.LastWeekRevenueFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueFinesSpendIncome))
                        _summary.text += " · " + lastWeekRevenueFinesSpendIncome;
                    string lifetimeRevenueFinesSpendIncome = HqDisplayQueriesHud.LifetimeRevenueFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueFinesSpendIncome))
                        _summary.text += " · " + lifetimeRevenueFinesSpendIncome;
                    string thisWeekConstructionGrantSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionGrantSpendIncome))
                        _summary.text += " · " + thisWeekConstructionGrantSpendIncome;
                    string lastWeekConstructionGrantSpendIncome = HqDisplayQueriesHud.LastWeekConstructionGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionGrantSpendIncome))
                        _summary.text += " · " + lastWeekConstructionGrantSpendIncome;
                    string lifetimeConstructionGrantSpendIncome = HqDisplayQueriesHud.LifetimeConstructionGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionGrantSpendIncome))
                        _summary.text += " · " + lifetimeConstructionGrantSpendIncome;
                    string thisWeekHiringGrantSpendIncome = HqDisplayQueriesHud.ThisWeekHiringGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHiringGrantSpendIncome))
                        _summary.text += " · " + thisWeekHiringGrantSpendIncome;
                    string lastWeekHiringGrantSpendIncome = HqDisplayQueriesHud.LastWeekHiringGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringGrantSpendIncome))
                        _summary.text += " · " + lastWeekHiringGrantSpendIncome;
                    string lifetimeHiringGrantSpendIncome = HqDisplayQueriesHud.LifetimeHiringGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHiringGrantSpendIncome))
                        _summary.text += " · " + lifetimeHiringGrantSpendIncome;
                    string thisWeekSalariesRoomSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesRoomSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesRoomSpendIncome))
                        _summary.text += " · " + thisWeekSalariesRoomSpendIncome;
                    string lastWeekSalariesRoomSpendIncome = HqDisplayQueriesHud.LastWeekSalariesRoomSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesRoomSpendIncome))
                        _summary.text += " · " + lastWeekSalariesRoomSpendIncome;
                    string lifetimeSalariesRoomSpendIncome = HqDisplayQueriesHud.LifetimeSalariesRoomSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSalariesRoomSpendIncome))
                        _summary.text += " · " + lifetimeSalariesRoomSpendIncome;
                    string thisWeekGrantTrainingSpendIncome = HqDisplayQueriesHud.ThisWeekGrantTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantTrainingSpendIncome))
                        _summary.text += " · " + thisWeekGrantTrainingSpendIncome;
                    string lastWeekGrantTrainingSpendIncome = HqDisplayQueriesHud.LastWeekGrantTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantTrainingSpendIncome))
                        _summary.text += " · " + lastWeekGrantTrainingSpendIncome;
                    string lifetimeGrantTrainingSpendIncome = HqDisplayQueriesHud.LifetimeGrantTrainingSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantTrainingSpendIncome))
                        _summary.text += " · " + lifetimeGrantTrainingSpendIncome;
                    string thisWeekFinesPowerSpendIncome = HqDisplayQueriesHud.ThisWeekFinesPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesPowerSpendIncome))
                        _summary.text += " · " + thisWeekFinesPowerSpendIncome;
                    string lastWeekFinesPowerSpendIncome = HqDisplayQueriesHud.LastWeekFinesPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesPowerSpendIncome))
                        _summary.text += " · " + lastWeekFinesPowerSpendIncome;
                    string lifetimeFinesPowerSpendIncome = HqDisplayQueriesHud.LifetimeFinesPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesPowerSpendIncome))
                        _summary.text += " · " + lifetimeFinesPowerSpendIncome;
                    string thisWeekGpuUpkeepRentSpendIncome = HqDisplayQueriesHud.ThisWeekGpuUpkeepRentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuUpkeepRentSpendIncome))
                        _summary.text += " · " + thisWeekGpuUpkeepRentSpendIncome;
                    string lastWeekGpuUpkeepRentSpendIncome = HqDisplayQueriesHud.LastWeekGpuUpkeepRentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuUpkeepRentSpendIncome))
                        _summary.text += " · " + lastWeekGpuUpkeepRentSpendIncome;
                    string lifetimeGpuUpkeepRentSpendIncome = HqDisplayQueriesHud.LifetimeGpuUpkeepRentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGpuUpkeepRentSpendIncome))
                        _summary.text += " · " + lifetimeGpuUpkeepRentSpendIncome;
                    string thisWeekTrainingRevenueSpendIncome = HqDisplayQueriesHud.ThisWeekTrainingRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingRevenueSpendIncome))
                        _summary.text += " · " + thisWeekTrainingRevenueSpendIncome;
                    string lastWeekTrainingRevenueSpendIncome = HqDisplayQueriesHud.LastWeekTrainingRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingRevenueSpendIncome))
                        _summary.text += " · " + lastWeekTrainingRevenueSpendIncome;
                    string lifetimeTrainingRevenueSpendIncome = HqDisplayQueriesHud.LifetimeTrainingRevenueSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingRevenueSpendIncome))
                        _summary.text += " · " + lifetimeTrainingRevenueSpendIncome;
                    string thisWeekChipSalesGrantsSpendIncome = HqDisplayQueriesHud.ThisWeekChipSalesGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipSalesGrantsSpendIncome))
                        _summary.text += " · " + thisWeekChipSalesGrantsSpendIncome;
                    string lastWeekChipSalesGrantsSpendIncome = HqDisplayQueriesHud.LastWeekChipSalesGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipSalesGrantsSpendIncome))
                        _summary.text += " · " + lastWeekChipSalesGrantsSpendIncome;
                    string lifetimeChipSalesGrantsSpendIncome = HqDisplayQueriesHud.LifetimeChipSalesGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipSalesGrantsSpendIncome))
                        _summary.text += " · " + lifetimeChipSalesGrantsSpendIncome;
                    string thisWeekGreyLaunderSpendIncome = HqDisplayQueriesHud.ThisWeekGreyLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyLaunderSpendIncome))
                        _summary.text += " · " + thisWeekGreyLaunderSpendIncome;
                    string lastWeekGreyLaunderSpendIncome = HqDisplayQueriesHud.LastWeekGreyLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyLaunderSpendIncome))
                        _summary.text += " · " + lastWeekGreyLaunderSpendIncome;
                    string lifetimeGreyLaunderSpendIncome = HqDisplayQueriesHud.LifetimeGreyLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyLaunderSpendIncome))
                        _summary.text += " · " + lifetimeGreyLaunderSpendIncome;
                    string thisWeekSalariesGpuSpendIncome = HqDisplayQueriesHud.ThisWeekSalariesGpuSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekSalariesGpuSpendIncome))
                        _summary.text += " · " + thisWeekSalariesGpuSpendIncome;
                    string lastWeekSalariesGpuSpendIncome = HqDisplayQueriesHud.LastWeekSalariesGpuSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekSalariesGpuSpendIncome))
                        _summary.text += " · " + lastWeekSalariesGpuSpendIncome;
                    string lifetimeSalariesGpuSpendIncome = HqDisplayQueriesHud.LifetimeSalariesGpuSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeSalariesGpuSpendIncome))
                        _summary.text += " · " + lifetimeSalariesGpuSpendIncome;
                    string thisWeekHardwarePowerSpendIncome = HqDisplayQueriesHud.ThisWeekHardwarePowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHardwarePowerSpendIncome))
                        _summary.text += " · " + thisWeekHardwarePowerSpendIncome;
                    string lastWeekHardwarePowerSpendIncome = HqDisplayQueriesHud.LastWeekHardwarePowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHardwarePowerSpendIncome))
                        _summary.text += " · " + lastWeekHardwarePowerSpendIncome;
                    string lifetimeHardwarePowerSpendIncome = HqDisplayQueriesHud.LifetimeHardwarePowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHardwarePowerSpendIncome))
                        _summary.text += " · " + lifetimeHardwarePowerSpendIncome;
                    string thisWeekHiringGreySpendIncome = HqDisplayQueriesHud.ThisWeekHiringGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekHiringGreySpendIncome))
                        _summary.text += " · " + thisWeekHiringGreySpendIncome;
                    string lastWeekHiringGreySpendIncome = HqDisplayQueriesHud.LastWeekHiringGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringGreySpendIncome))
                        _summary.text += " · " + lastWeekHiringGreySpendIncome;
                    string lifetimeHiringGreySpendIncome = HqDisplayQueriesHud.LifetimeHiringGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeHiringGreySpendIncome))
                        _summary.text += " · " + lifetimeHiringGreySpendIncome;
                    string thisWeekRevenueNetSpendIncome = HqDisplayQueriesHud.ThisWeekRevenueNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRevenueNetSpendIncome))
                        _summary.text += " · " + thisWeekRevenueNetSpendIncome;
                    string lastWeekRevenueNetSpendIncome = HqDisplayQueriesHud.LastWeekRevenueNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRevenueNetSpendIncome))
                        _summary.text += " · " + lastWeekRevenueNetSpendIncome;
                    string lifetimeRevenueNetSpendIncome = HqDisplayQueriesHud.LifetimeRevenueNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueNetSpendIncome))
                        _summary.text += " · " + lifetimeRevenueNetSpendIncome;
                    string thisWeekChipDevLandSpendIncome = HqDisplayQueriesHud.ThisWeekChipDevLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekChipDevLandSpendIncome))
                        _summary.text += " · " + thisWeekChipDevLandSpendIncome;
                    string lastWeekChipDevLandSpendIncome = HqDisplayQueriesHud.LastWeekChipDevLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekChipDevLandSpendIncome))
                        _summary.text += " · " + lastWeekChipDevLandSpendIncome;
                    string lifetimeChipDevLandSpendIncome = HqDisplayQueriesHud.LifetimeChipDevLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeChipDevLandSpendIncome))
                        _summary.text += " · " + lifetimeChipDevLandSpendIncome;
                    string thisWeekConstructionGrantsSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionGrantsSpendIncome))
                        _summary.text += " · " + thisWeekConstructionGrantsSpendIncome;
                    string lastWeekConstructionGrantsSpendIncome = HqDisplayQueriesHud.LastWeekConstructionGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionGrantsSpendIncome))
                        _summary.text += " · " + lastWeekConstructionGrantsSpendIncome;
                    string lifetimeConstructionGrantsSpendIncome = HqDisplayQueriesHud.LifetimeConstructionGrantsSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionGrantsSpendIncome))
                        _summary.text += " · " + lifetimeConstructionGrantsSpendIncome;
                    string thisWeekTrainingLaunderSpendIncome = HqDisplayQueriesHud.ThisWeekTrainingLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingLaunderSpendIncome))
                        _summary.text += " · " + thisWeekTrainingLaunderSpendIncome;
                    string lastWeekTrainingLaunderSpendIncome = HqDisplayQueriesHud.LastWeekTrainingLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekTrainingLaunderSpendIncome))
                        _summary.text += " · " + lastWeekTrainingLaunderSpendIncome;
                    string lifetimeTrainingLaunderSpendIncome = HqDisplayQueriesHud.LifetimeTrainingLaunderSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeTrainingLaunderSpendIncome))
                        _summary.text += " · " + lifetimeTrainingLaunderSpendIncome;
                    string thisWeekPowerChipSpendIncome = HqDisplayQueriesHud.ThisWeekPowerChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerChipSpendIncome))
                        _summary.text += " · " + thisWeekPowerChipSpendIncome;
                    string lastWeekPowerChipSpendIncome = HqDisplayQueriesHud.LastWeekPowerChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerChipSpendIncome))
                        _summary.text += " · " + lastWeekPowerChipSpendIncome;
                    string lifetimePowerChipSpendIncome = HqDisplayQueriesHud.LifetimePowerChipSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerChipSpendIncome))
                        _summary.text += " · " + lifetimePowerChipSpendIncome;
                    string thisWeekGrantHardwareSpendIncome = HqDisplayQueriesHud.ThisWeekGrantHardwareSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantHardwareSpendIncome))
                        _summary.text += " · " + thisWeekGrantHardwareSpendIncome;
                    string lastWeekGrantHardwareSpendIncome = HqDisplayQueriesHud.LastWeekGrantHardwareSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantHardwareSpendIncome))
                        _summary.text += " · " + lastWeekGrantHardwareSpendIncome;
                    string lifetimeGrantHardwareSpendIncome = HqDisplayQueriesHud.LifetimeGrantHardwareSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantHardwareSpendIncome))
                        _summary.text += " · " + lifetimeGrantHardwareSpendIncome;
                    string thisWeekPowerFinesSpendIncome = HqDisplayQueriesHud.ThisWeekPowerFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPowerFinesSpendIncome))
                        _summary.text += " · " + thisWeekPowerFinesSpendIncome;
                    string lastWeekPowerFinesSpendIncome = HqDisplayQueriesHud.LastWeekPowerFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPowerFinesSpendIncome))
                        _summary.text += " · " + lastWeekPowerFinesSpendIncome;
                    string lifetimePowerFinesSpendIncome = HqDisplayQueriesHud.LifetimePowerFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePowerFinesSpendIncome))
                        _summary.text += " · " + lifetimePowerFinesSpendIncome;
                    string thisWeekGrantsFinesSpendIncome = HqDisplayQueriesHud.ThisWeekGrantsFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantsFinesSpendIncome))
                        _summary.text += " · " + thisWeekGrantsFinesSpendIncome;
                    string lastWeekGrantsFinesSpendIncome = HqDisplayQueriesHud.LastWeekGrantsFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantsFinesSpendIncome))
                        _summary.text += " · " + lastWeekGrantsFinesSpendIncome;
                    string lifetimeGrantsFinesSpendIncome = HqDisplayQueriesHud.LifetimeGrantsFinesSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantsFinesSpendIncome))
                        _summary.text += " · " + lifetimeGrantsFinesSpendIncome;
                    string thisWeekRentGrantSpendIncome = HqDisplayQueriesHud.ThisWeekRentGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekRentGrantSpendIncome))
                        _summary.text += " · " + thisWeekRentGrantSpendIncome;
                    string lastWeekRentGrantSpendIncome = HqDisplayQueriesHud.LastWeekRentGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekRentGrantSpendIncome))
                        _summary.text += " · " + lastWeekRentGrantSpendIncome;
                    string lifetimeRentGrantSpendIncome = HqDisplayQueriesHud.LifetimeRentGrantSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRentGrantSpendIncome))
                        _summary.text += " · " + lifetimeRentGrantSpendIncome;
                    string thisWeekGrantsNetSpendIncome = HqDisplayQueriesHud.ThisWeekGrantsNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGrantsNetSpendIncome))
                        _summary.text += " · " + thisWeekGrantsNetSpendIncome;
                    string lastWeekGrantsNetSpendIncome = HqDisplayQueriesHud.LastWeekGrantsNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGrantsNetSpendIncome))
                        _summary.text += " · " + lastWeekGrantsNetSpendIncome;
                    string lifetimeGrantsNetSpendIncome = HqDisplayQueriesHud.LifetimeGrantsNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGrantsNetSpendIncome))
                        _summary.text += " · " + lifetimeGrantsNetSpendIncome;
                    string thisWeekPatentSiteSpendIncome = HqDisplayQueriesHud.ThisWeekPatentSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentSiteSpendIncome))
                        _summary.text += " · " + thisWeekPatentSiteSpendIncome;
                    string lastWeekPatentSiteSpendIncome = HqDisplayQueriesHud.LastWeekPatentSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentSiteSpendIncome))
                        _summary.text += " · " + lastWeekPatentSiteSpendIncome;
                    string lifetimePatentSiteSpendIncome = HqDisplayQueriesHud.LifetimePatentSiteSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentSiteSpendIncome))
                        _summary.text += " · " + lifetimePatentSiteSpendIncome;
                    string thisWeekGreyLandSpendIncome = HqDisplayQueriesHud.ThisWeekGreyLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGreyLandSpendIncome))
                        _summary.text += " · " + thisWeekGreyLandSpendIncome;
                    string lastWeekGreyLandSpendIncome = HqDisplayQueriesHud.LastWeekGreyLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGreyLandSpendIncome))
                        _summary.text += " · " + lastWeekGreyLandSpendIncome;
                    string lifetimeGreyLandSpendIncome = HqDisplayQueriesHud.LifetimeGreyLandSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGreyLandSpendIncome))
                        _summary.text += " · " + lifetimeGreyLandSpendIncome;
                    string thisWeekConstructionPatentSpendIncome = HqDisplayQueriesHud.ThisWeekConstructionPatentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekConstructionPatentSpendIncome))
                        _summary.text += " · " + thisWeekConstructionPatentSpendIncome;
                    string lastWeekConstructionPatentSpendIncome = HqDisplayQueriesHud.LastWeekConstructionPatentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekConstructionPatentSpendIncome))
                        _summary.text += " · " + lastWeekConstructionPatentSpendIncome;
                    string lifetimeConstructionPatentSpendIncome = HqDisplayQueriesHud.LifetimeConstructionPatentSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeConstructionPatentSpendIncome))
                        _summary.text += " · " + lifetimeConstructionPatentSpendIncome;
                    string thisWeekGpuNetSpendIncome = HqDisplayQueriesHud.ThisWeekGpuNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekGpuNetSpendIncome))
                        _summary.text += " · " + thisWeekGpuNetSpendIncome;
                    string lastWeekGpuNetSpendIncome = HqDisplayQueriesHud.LastWeekGpuNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekGpuNetSpendIncome))
                        _summary.text += " · " + lastWeekGpuNetSpendIncome;
                    string lifetimeGpuNetSpendIncome = HqDisplayQueriesHud.LifetimeGpuNetSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeGpuNetSpendIncome))
                        _summary.text += " · " + lifetimeGpuNetSpendIncome;
                    string thisWeekPatentPowerSpendIncome = HqDisplayQueriesHud.ThisWeekPatentPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekPatentPowerSpendIncome))
                        _summary.text += " · " + thisWeekPatentPowerSpendIncome;
                    string lastWeekPatentPowerSpendIncome = HqDisplayQueriesHud.LastWeekPatentPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekPatentPowerSpendIncome))
                        _summary.text += " · " + lastWeekPatentPowerSpendIncome;
                    string lifetimePatentPowerSpendIncome = HqDisplayQueriesHud.LifetimePatentPowerSpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimePatentPowerSpendIncome))
                        _summary.text += " · " + lifetimePatentPowerSpendIncome;
                    string thisWeekFinesGreySpendIncome = HqDisplayQueriesHud.ThisWeekFinesGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekFinesGreySpendIncome))
                        _summary.text += " · " + thisWeekFinesGreySpendIncome;
                    string lastWeekFinesGreySpendIncome = HqDisplayQueriesHud.LastWeekFinesGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekFinesGreySpendIncome))
                        _summary.text += " · " + lastWeekFinesGreySpendIncome;
                    string lifetimeFinesGreySpendIncome = HqDisplayQueriesHud.LifetimeFinesGreySpendIncomeSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeFinesGreySpendIncome))
                        _summary.text += " · " + lifetimeFinesGreySpendIncome;
                    string thisWeekTrainingPatent = HqDisplayQueriesHud.ThisWeekTrainingPatentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekTrainingPatent))
                        _summary.text += " · " + thisWeekTrainingPatent;
                    string lastWeekHiringRent = HqDisplayQueriesHud.LastWeekHiringRentSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lastWeekHiringRent))
                        _summary.text += " · " + lastWeekHiringRent;
                    string lifetimeRevenueGrant = HqDisplayQueriesHud.LifetimeRevenueGrantSummaryLine(lab);
                    if (!string.IsNullOrEmpty(lifetimeRevenueGrant))
                        _summary.text += " · " + lifetimeRevenueGrant;
                    string thisWeekIncomeChip = HqDisplayQueriesHud.ThisWeekIncomeChipSummaryLine(lab);
                    if (!string.IsNullOrEmpty(thisWeekIncomeChip))
                        _summary.text += " · " + thisWeekIncomeChip;
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