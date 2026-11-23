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
            _gpuSummary.text = "GPUs: " + rented + " rented, " + owned + " owned (" + gen.Name + ")" + lastWeekLandPart + lastWeekSiteCapitalPart + lifetimeGrantsNetPart + thisWeekChipRevenuePart + lastWeekConstructionPatentPart + lastWeekHiringPatentPart + thisWeekBuildPart + thisWeekFinesPowerPart + lastWeekSalariesRoomPart + lifetimeRevenueChipPart + lastWeekGpuUpkeepRentPart + lastWeekRevenueNetPart + thisWeekNetTrainingPart + lastWeekChipDevLandPart + thisWeekGrantsFinesPart + lifetimePowerFinesPart;

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