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